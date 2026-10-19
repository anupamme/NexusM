using System.Collections.Concurrent;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using NexusM.Data;
using NexusM.Models;

namespace NexusM.Services;

/// <summary>
/// Handles PIN-based authentication, PBKDF2 hashing, and brute-force protection.
/// Mirrors the security approach from the PowerShell NexusM version.
/// </summary>
public class PinSecurityService
{
    private const int Iterations = 100_000;
    private const int SaltSizeBytes = 32;
    private const int HashSizeBytes = 32;
    private const int MaxFailedAttempts = 5;
    private const int LockoutMinutes = 15;
    private const int MaxLockoutMinutes = 24 * 60;
    /// <summary>How long a key can go without a new failed attempt before its escalation
    /// history (LockoutCount) is forgiven - an honest typo every few months should not
    /// eventually earn a 24-hour lockout.</summary>
    private static readonly TimeSpan LockoutDecayWindow = TimeSpan.FromHours(24);

    private readonly IServiceProvider _serviceProvider;
    private readonly ConfigService _configService;
    private readonly ILogger<PinSecurityService> _logger;

    /// <summary>
    /// Tracks failed login attempts per "IP:username" key.
    /// </summary>
    private readonly ConcurrentDictionary<string, LoginAttempt> _loginAttempts = new();

    public PinSecurityService(
        IServiceProvider serviceProvider,
        ConfigService configService,
        ILogger<PinSecurityService> logger)
    {
        _serviceProvider = serviceProvider;
        _configService = configService;
        _logger = logger;
    }

    /// <summary>
    /// Hash a PIN using PBKDF2-HMAC-SHA256 with 100,000 iterations.
    /// If no salt is provided, generates a new 32-byte random salt.
    /// </summary>
    public (string Hash, string Salt) HashPin(string pin, string? existingSalt = null)
    {
        byte[] saltBytes;
        if (!string.IsNullOrEmpty(existingSalt))
        {
            saltBytes = Convert.FromBase64String(existingSalt);
        }
        else
        {
            saltBytes = new byte[SaltSizeBytes];
            RandomNumberGenerator.Fill(saltBytes);
        }

        using var pbkdf2 = new Rfc2898DeriveBytes(
            pin,
            saltBytes,
            Iterations,
            HashAlgorithmName.SHA256);

        var hashBytes = pbkdf2.GetBytes(HashSizeBytes);

        return (
            Hash: Convert.ToBase64String(hashBytes),
            Salt: Convert.ToBase64String(saltBytes)
        );
    }

    /// <summary>
    /// Verify a PIN against a stored hash and salt.
    /// </summary>
    public bool VerifyPin(string pin, string storedHash, string storedSalt)
    {
        var (computedHash, _) = HashPin(pin, storedSalt);
        return CryptographicOperations.FixedTimeEquals(
            Convert.FromBase64String(computedHash),
            Convert.FromBase64String(storedHash));
    }

    /// <summary>
    /// Creates the default admin user if no users exist in the database.
    /// Also ensures all users have their per-user .db files in users/ folder.
    /// Default PIN is 000000 (matching PowerShell version).
    /// </summary>
    public async Task EnsureDefaultAdminAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<UsersDbContext>();

        if (!await db.Users.AnyAsync())
        {
            var adminUsername = _configService.Config.Security.DefaultAdminUser;
            var (hash, salt) = HashPin("000000");

            var admin = new AppUser
            {
                Username = adminUsername,
                DisplayName = "Admin",
                PinHash = hash,
                PinSalt = salt,
                Role = "admin",
                IsActive = true,
                DateCreated = DateTime.UtcNow
            };

            db.Users.Add(admin);
            await db.SaveChangesAsync();
            AutoLoginCache.Invalidate();

            _logger.LogWarning("Default admin user '{Username}' created with PIN 000000 - change this immediately!", adminUsername);
        }

        // Ensure admin.db exists
        var adminName = _configService.Config.Security.DefaultAdminUser;
        CreateUserDatabase(adminName);
    }

    /// <summary>
    /// Creates a per-user SQLite database file in the users/ folder.
    /// </summary>
    public void CreateUserDatabase(string username)
    {
        if (!UserNames.IsPathSafe(username))
        {
            _logger.LogError("Refusing to create a user database for unsafe user name '{Username}'", username);
            return;
        }
        var usersDir = Path.Combine(AppContext.BaseDirectory, "users");
        if (!Directory.Exists(usersDir))
            Directory.CreateDirectory(usersDir);

        var userDbPath = UserNames.DbPath(username);
        if (File.Exists(userDbPath))
            return;

        // Create a valid SQLite database using Microsoft.Data.Sqlite
        using var conn = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={userDbPath}");
        conn.Open();
        SqliteWal.Apply(conn);
        // Full per-user schema from the start (it used to be Favourites only, with the rest
        // created lazily on the first open) - same statements, same tables.
        UserDbSchema.Ensure(conn);
        _logger.LogInformation("Created user database: {Path}", userDbPath);
    }

    /// <summary>
    /// Check if an IP+username combination is currently locked out due to brute-force protection.
    /// Returns the minutes remaining if locked, or 0 if not locked.
    /// </summary>
    public int GetLockoutMinutesRemaining(string clientIP, string username)
    {
        var key = $"{clientIP}:{username}";
        if (!_loginAttempts.TryGetValue(key, out var attempt)) return 0;

        lock (attempt)
        {
            if (attempt.LockoutUntil.HasValue)
            {
                if (DateTime.UtcNow < attempt.LockoutUntil.Value)
                {
                    return (int)Math.Ceiling((attempt.LockoutUntil.Value - DateTime.UtcNow).TotalMinutes);
                }
                // Lockout window passed - clear the lockout itself, but keep LockoutCount so a
                // repeat offender's next lockout still escalates, instead of resetting the
                // backoff to square one on every single retry (see GitHub issue discussion on
                // PR #1 - that version's LockoutCount was wiped right here, making the
                // escalation never actually take effect).
                attempt.LockoutUntil = null;
            }

            // Forgive old escalation history once the key has been quiet for a while, and drop
            // the entry entirely once there is nothing left worth keeping, so a one-off burst
            // from a source that never retries does not sit in memory forever.
            if (attempt.LastFailedAttemptUtc is { } last && DateTime.UtcNow - last > LockoutDecayWindow)
            {
                attempt.LockoutCount = 0;
                attempt.FailedAttempts = 0;
            }
            if (attempt is { FailedAttempts: 0, LockoutCount: 0, LockoutUntil: null })
            {
                _loginAttempts.TryRemove(key, out _);
            }
        }
        return 0;
    }

    /// <summary>
    /// Record a failed login attempt. Returns true if the account is now locked.
    /// </summary>
    public bool RecordFailedAttempt(string clientIP, string username)
    {
        var key = $"{clientIP}:{username}";
        var attempt = _loginAttempts.GetOrAdd(key, _ => new LoginAttempt());

        lock (attempt)
        {
            attempt.FailedAttempts++;
            attempt.LastFailedAttemptUtc = DateTime.UtcNow;

            if (attempt.FailedAttempts >= MaxFailedAttempts)
            {
                // Exponential backoff: 15, 30, 60, ... minutes, capped at 24 hours. LockoutCount
                // survives across individual lockout windows (see GetLockoutMinutesRemaining)
                // so a repeat offender keeps escalating instead of resetting every time they
                // wait out the current lockout.
                var lockoutMinutes = Math.Min(LockoutMinutes * Math.Pow(2, attempt.LockoutCount), MaxLockoutMinutes);
                attempt.LockoutUntil = DateTime.UtcNow.AddMinutes(lockoutMinutes);
                attempt.LockoutCount++;
                attempt.FailedAttempts = 0;
                _logger.LogWarning("Account locked: {Username} from {IP} (locked for {Minutes} minutes, escalation level {Level})",
                    username, clientIP, lockoutMinutes, attempt.LockoutCount);
                return true;
            }

            return false;
        }
    }

    /// <summary>
    /// Clear failed attempts after a successful login.
    /// </summary>
    public void ClearFailedAttempts(string clientIP, string username)
    {
        var key = $"{clientIP}:{username}";
        _loginAttempts.TryRemove(key, out _);
    }

    private class LoginAttempt
    {
        public int FailedAttempts { get; set; }
        public DateTime? LockoutUntil { get; set; }
        public int LockoutCount { get; set; }
        public DateTime? LastFailedAttemptUtc { get; set; }
    }
}

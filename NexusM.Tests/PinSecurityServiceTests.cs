using Microsoft.Extensions.Logging.Abstractions;
using NexusM.Services;
using Xunit;

namespace NexusM.Tests;

public class PinSecurityServiceTests
{
    private const string ClientIP = "127.0.0.1";
    private const string Username = "testuser";
    private const int MaxFailedAttempts = 5;

    private static (PinSecurityService Service, Action<DateTime> SetNow) CreateService(DateTime start)
    {
        var service = new PinSecurityService(null!, null!, NullLogger<PinSecurityService>.Instance);
        var now = start;
        service.SetClockForTesting(() => now);
        return (service, value => now = value);
    }

    private static void TriggerLockout(PinSecurityService service, string ip = ClientIP, string username = Username)
    {
        var locked = false;
        for (var i = 0; i < MaxFailedAttempts; i++)
        {
            locked = service.RecordFailedAttempt(ip, username);
        }
        Assert.True(locked, "Expected the final failed attempt to trigger a lockout");
    }

    [Fact]
    public void Escalation_SecondLockoutIsDoubleTheFirst()
    {
        var start = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var (service, setNow) = CreateService(start);

        TriggerLockout(service);
        Assert.Equal(15, service.GetLockoutMinutesRemaining(ClientIP, Username));

        setNow(start.AddMinutes(16)); // past the first lockout window

        TriggerLockout(service);
        Assert.Equal(30, service.GetLockoutMinutesRemaining(ClientIP, Username));
    }

    [Fact]
    public void Expiration_PreservesEscalationHistory()
    {
        var start = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var (service, setNow) = CreateService(start);

        TriggerLockout(service);

        setNow(start.AddMinutes(16)); // first lockout expired, well under the 24h decay window
        Assert.Equal(0, service.GetLockoutMinutesRemaining(ClientIP, Username));

        TriggerLockout(service);
        Assert.Equal(30, service.GetLockoutMinutesRemaining(ClientIP, Username));
    }

    [Fact]
    public void Decay_ResetsHistoryAfter24HoursOfInactivity()
    {
        var start = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var (service, setNow) = CreateService(start);

        TriggerLockout(service);

        setNow(start.AddHours(25)); // past the lockout AND past the 24h decay window
        Assert.Equal(0, service.GetLockoutMinutesRemaining(ClientIP, Username));

        TriggerLockout(service);
        Assert.Equal(15, service.GetLockoutMinutesRemaining(ClientIP, Username));
    }

    [Fact]
    public void Duration_NeverExceedsMaximum()
    {
        var start = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var (service, setNow) = CreateService(start);
        var current = start;
        var sawCap = false;

        for (var cycle = 0; cycle < 9; cycle++)
        {
            TriggerLockout(service);
            var minutes = service.GetLockoutMinutesRemaining(ClientIP, Username);
            Assert.True(minutes <= 1440, $"Lockout duration {minutes} exceeded the 1440-minute cap on cycle {cycle}");
            if (minutes == 1440)
                sawCap = true;

            current = current.AddMinutes(minutes + 1);
            setNow(current);
        }

        Assert.True(sawCap, "Expected repeated lockouts to eventually hit the 1440-minute cap");
    }

    [Fact]
    public async Task ConcurrentFailures_ProduceConsistentLockoutState()
    {
        var service = new PinSecurityService(null!, null!, NullLogger<PinSecurityService>.Instance);
        const string ip = "10.0.0.1";
        const string username = "concurrent-user";

        var tasks = Enumerable.Range(0, MaxFailedAttempts)
            .Select(_ => Task.Run(() => service.RecordFailedAttempt(ip, username)))
            .ToArray();

        var results = await Task.WhenAll(tasks);

        Assert.Equal(1, results.Count(locked => locked));
        Assert.Equal(15, service.GetLockoutMinutesRemaining(ip, username));
    }
}

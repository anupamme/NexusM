# NexusM: Free Self-Hosted Media Server

**NexusM** is a free, self-hosted media server and portable media library manager for Windows, Linux, and Docker. It is a direct alternative to Plex, Jellyfin, and Emby, turning a computer you already own into a private streaming service for movies, TV shows, music, photos, podcasts, audiobooks, and eBooks, with no cloud account and no subscription.

No cloud dependency. No subscription. No account required. Your media, your server, your rules.

---

## Overview

NexusM is a single-file executable (Windows and Linux, with an official Docker image) that serves a web-based UI for managing and playing every common type of personal media. It scans your local and network drives, extracts metadata, fetches rich information from online databases (TMDB, TVMaze, LRCLIB, Jikan/MyAnimeList, Watchmode, fanart.tv), and presents everything through a responsive interface accessible from any browser on your network, with native Android TV and Samsung TV apps for the living room.

Unlike Plex and Emby, hardware-accelerated transcoding (NVIDIA NVENC, Intel QSV/VAAPI, AMD AMF) is included at no cost and requires no paid tier. Unlike Jellyfin, most features ship built in rather than through third-party plugins.

---

## NexusM vs Plex vs Jellyfin vs Emby

| | NexusM | Plex | Jellyfin | Emby |
|---|---|---|---|---|
| Price | Free | Free, with paid Plex Pass tier | Free, open source | Free, with paid Emby Premiere tier |
| Account required | No | Yes (Plex account) | No | Yes for some features |
| Hardware transcoding | Included free | Requires Plex Pass | Included free | Requires Emby Premiere |
| License model | Source-available (PolyForm Noncommercial) | Proprietary | Open source (GPL) | Proprietary |
| Media types | Movies, TV, anime, music, music videos, photos, podcasts, audiobooks, eBooks, comics, radio, internet TV | Movies, TV, music, photos | Movies, TV, music, photos, books | Movies, TV, music, photos, books |
| Built-in game emulation | Yes (ROM and HTML5 web games) | No | No | No |
| Audiobook library | Yes, dedicated library type with chapters, speed control, sleep timer, and bookmarks | No, requires a third-party app such as Audiobookshelf | No native support | No confirmed native support |
| eBook and comic library | Yes, PDF/EPUB with in-browser reader plus CBZ/CBR comic reader | Via third-party plugins only | Basic Books library type | Basic Books library type |
| Photo library with map view | Yes, GPS clustering on a world map | "Places" map feature shipped in 2017, since removed | No | No |
| Custom music player interfaces | Yes, multiple built-in skins (retro hardware-style players, a cinema "Go Big" mode) | Single player UI | Single player UI | Single player UI |
| Natural-language / dialogue search | Yes (Smart Search, local AI model, searches subtitle dialogue to jump to a scene) | No | No | No |
| Built-in content request system | Yes, native Radarr/Sonarr requests, no separate app needed | No, requires Overseerr or Ombi | No, requires Jellyseerr or Ombi | No, requires Ombi |
| Built-in DLNA/UPnP server | Yes | Yes | Yes | Yes |
| Native Android TV / Samsung TV apps | Yes, both | Yes, both | Android TV yes, Samsung TV app rolling out | Android TV yes, Samsung TV limited |

NexusM does not just match Plex, Jellyfin, and Emby, it goes well beyond their core feature set in several areas. The music library is considerably more advanced: full-text search, ReplayGain normalisation, a 10-band equalizer, synced lyrics, Last.fm scrobbling, and several selectable custom player interfaces, including retro hardware-style skins and a fullscreen "Go Big" cinema mode, rather than the single fixed player found in the other three. Audiobooks and eBooks are first-class libraries in NexusM, with chapter navigation, variable playback speed, sleep timers, and bookmarks for audiobooks, and a proper in-browser reader for PDF, EPUB, CBZ, and CBR, where Plex, Jellyfin, and Emby either have no native audiobook support at all or only a basic Books library type with no comparable reading experience. The photo library also stands apart: NexusM plots your library on an interactive world map using embedded GPS data, a feature Plex briefly offered and discontinued, and one that Jellyfin and Emby do not currently offer. On top of all this, NexusM adds built-in retro game emulation and bundled HTML5 web games, none of which the other three provide, while keeping hardware transcoding free and not requiring a user account.

*Comparison accurate as of October 2026, based on each project's own published pricing and feature pages, official forums, and documentation.*

---

## Features

### Media Libraries
- **Music** - Browse by tracks, albums, artists, and genres. ID3/Vorbis tag extraction, album artwork, lyrics display with synced lyrics support (via LRCLIB). FTS5 full-text search, ReplayGain volume normalisation, and M3U playlist export.
- **Movies and TV Shows** - Poster grid with metadata from TMDB/TVMaze. Cast photos, descriptions, genres, watched status, and resume playback.
- **Anime** - Dedicated anime library powered by Jikan (MyAnimeList). Series and episode browsing with MAL ratings, synopsis, cover art, and character data.
- **Actors** - Browse actors from your video library. Biography, filmography, and "Known For" credits from TMDB, with clickable cast on movie and TV detail pages.
- **Music Videos** - Artist-based browsing with auto-generated thumbnails.
- **Pictures** - Photo gallery with category filtering, EXIF metadata, and thumbnail generation.
- **eBooks** - PDF and EPUB library with cover extraction and in-browser reader.
- **Comic Books** - CBZ and CBR archives with a dedicated page-by-page reader.
- **AudioBooks** - MP3 and M4B audiobook library with per-book progress tracking, chapter navigation, variable playback speed, sleep timer, bookmarks, and a "Continue Listening" shelf.
- **Podcasts** - RSS feed subscriptions with episode playback, OPML import, and a built-in Discover panel.
- **Radio** - Internet radio stations with country and genre filtering, live streaming, and real-time ICY/Icecast stream title updates.
- **Internet TV** - IPTV channel management via M3U/M3U8 playlist import. Includes an EPG programme guide (XMLTV) with a Plex-style timeline grid, now/next data, and automatic channel matching.

### Playback
- Built-in web audio player with shuffle, repeat, queue management, and lyrics overlay.
- 10-band graphic equalizer with presets, applied via Web Audio API. Works for music, radio, and audiobooks.
- Audio output device picker - route playback to a specific local output (speakers, monitor, Bluetooth headset) directly from the player bar, the browser equivalent of a native app's output selector.
- Built-in web video player with HLS streaming and a redesigned cinema-style interface.
- Google Cast - cast music, movies, TV, and music videos from the web interface to any Google TV or Chromecast on your network. Server-side casting (like DLNA) works over plain HTTP with no browser HTTPS requirement, with album art on screen, playlist auto-advance, and a device picker.
- Chapters - movies, TV, anime, and documentaries with embedded chapters show a chapter list and let you jump between them during playback.
- Server-side video transcoding with hardware acceleration: NVIDIA NVENC, Intel QSV/VAAPI, and AMD AMF.
- HDR video support - tone mapping pipeline with dual-track HLS (HDR passthrough + SDR fallback). Supports HDR10, HDR10+, HLG, and Dolby Vision via zscale and tonemapx.
- Automatic remuxing for format compatibility without re-encoding.
- HLS caching to avoid re-transcoding previously watched content.
- Subtitle support - automatic online search and download via OpenSubtitles.com and SubDL.com, plus automatic sidecar .srt detection (Jellyfin-compatible naming), manual SRT/VTT upload per title, embedded subtitle tracks, and a client-side sync-offset control to fix out-of-sync subtitles.
- Video resume - saves and restores playback position per user, per title.
- Where to Watch - displays streaming availability for movies and TV shows via Watchmode, showing which services currently carry the title in your region.
- Community Ratings - TMDB community scores shown on movie and TV detail pages.
- Trakt.tv integration - native support for Trakt scrobbling, syncing watched history, and accessing your Trakt watchlist and ratings directly from NexusM.
- Last.fm scrobbling - scrobble your music listening history to Last.fm.

### Discovery
- **Smart Search** - semantic, natural-language search across your library, powered by a local AI model. Finds titles by meaning rather than exact keywords, and includes dialogue/scene search for movies and TV: search for what a character says (for example "when he promises he will be back") and jump straight to that moment in the video, matched against subtitle text rather than on-screen action. Runs locally, opt-in, no cloud API calls.
- **What's New Online** - a discovery page showing recent and upcoming releases from TMDB across movies, TV shows, anime, documentaries, and cartoons. Filter by country, genre, and mode (recent or upcoming). Streaming provider badges are shown for each title where available.
- **fanart.tv Artwork** - pick alternative posters, backdrops, and artist images from fanart.tv in the metadata editor for movies, TV shows, and music artists.
- **Mood Explorer** - 8 mood-based music recommendation filters: Chill, Energy, Focus, Party, Love, Study, Discovery, and Nostalgia.
- **Go Big Mode** - a fullscreen TV and cinema presentation mode with large poster cards, full keyboard navigation (arrow keys, Enter, Escape, F for fullscreen, Space to pause), and a paired mobile remote. A phone or tablet on the same network can browse the library and send content to the main display while Go Big is running.

### Requests
- **Native Radarr/Sonarr Requests** - users can browse for movies and TV shows not yet in the library and request them directly from NexusM, with a per-request quality profile and season picker. Approved requests are sent straight to your own Radarr and Sonarr instances. No separate request app (such as Overseerr or Ombi) is needed.

### Playlists
- **Auto-Generated Playlists** - automatically builds smart playlists from your music library: by decade, top 100 most played, recently added, and all favourites. Configuration is saved server-side for consistency across all devices.
- Manual playlists with drag-and-drop track ordering and per-user storage.

### Analytics and Insights
- **Deep Dive Library Analysis** - detailed breakdowns of your library by file format, video codec, resolution, audio codec, genre, bitrate, and release year. Separate charts for music, movies, TV, and other media types with exportable data.
- Viewing insights: watch patterns over time, top genres, completion rates, comfort rewatch titles, and listening statistics.
- Smart Insights - highlights from your library such as hidden gems, most-rewatched titles, and recently discovered artists.
- **Server Resource Monitor** - real-time CPU and RAM usage graphs updated every 15 seconds, visible from the admin panel. Cross-platform: reads /proc/stat and /proc/meminfo on Linux, uses P/Invoke on Windows.
- **Library Scan Scheduler** - automated background scanning on a configurable schedule, so your library stays up to date without manual intervention.
- CSV and HTML export of full library data.

### Network Sharing
- **DLNA/UPnP Media Server** - built-in DLNA server with SSDP device discovery. Exposes your music, videos, music videos, and anime library to any DLNA-compatible renderer on the local network (smart TVs, VLC, Kodi, hardware media players). No third-party DLNA software required.
- Network share (SMB/CIFS) support with credential management and auto-mount on startup.

### Security and Multi-User
- PIN-based authentication with admin and guest roles.
- IP whitelist for network-level access control.
- Per-user favourites, playlists, watched history, play counts, and preferences.
- Configurable session timeout.
- HTTPS support with auto-generated self-signed certificate including all LAN IPs and mDNS hostname.
- Access logging - separate rotating logs for LAN (LocalAccess.log) and remote (RemoteAccess.log) media access, recording user, IP, timestamp, media, and status.

### Customisation
- 6 built-in visual themes: Dark, Blue, Purple, Emerald, Sky Grey, and Sky Blue.
- **Custom Template System** - load external CSS and JavaScript template files at runtime to completely restyle the interface without touching the core application. Templates are hot-loaded from the templates folder and selectable from Settings. Ships with 6 templates: Horizon (amber home-theatre layout), Amazing! (Amazon Prime style), Jellyfish (Jellyfin inspired), YablakoTV (Apple TV style), Plux! (Plex inspired), and Netflux (Netflix style, with hero video previews).
- Configurable sidebar with individual show/hide toggles for every section.
- UI preferences including default view modes, Go Big autostart, and per-section sort order.
- 26 interface languages: English, French, German, Spanish, Portuguese, Italian, Dutch, Polish, Romanian, Russian, Swedish, Ukrainian, Norwegian, Finnish, Estonian, Lithuanian, Slovenian, Albanian, Serbian, Chinese, Japanese, Korean, Hindi, Indonesian, Vietnamese, and Thai.

### Other
- Global search across all media types with instant results.
- Auto-update - optional, opt-in in-app updater that checks nexusm.org, downloads new versions, verifies checksums, backs up the current install, and applies with one click, with rollback support.
- System tray integration and optional Windows startup registration.
- Portable - no installation required, runs from a single folder on Windows or Linux.

---

## Technology Stack

| Component | Technology |
|---|---|
| Runtime | .NET 8 (ASP.NET Core, Kestrel) |
| Database | SQLite via Entity Framework Core |
| Frontend | Vanilla JavaScript, HTML5, CSS3 (no framework) |
| Video Streaming | HLS via hls.js, FFmpeg for transcoding and remuxing |
| HDR Tone Mapping | FFmpeg tonemapx, zscale (libzimg), VAAPI, D3D11VA, NVDEC |
| eBook and Comic Rendering | epub.js, PDF.js, SharpCompress (CBZ/CBR) |
| Metadata | TMDB API, TVMaze API, LRCLIB API, Jikan API (MyAnimeList), fanart.tv API |
| Streaming Providers | Watchmode API |
| Scrobbling | Trakt.tv API, Last.fm API |
| EPG | XMLTV parsing |
| Google Cast | Sharpcaster (CASTV2 protocol) |
| DLNA/UPnP | Custom SSDP implementation (System.Net.Sockets, no external package) |
| Audio Tags | TagLibSharp |
| Image Processing | SixLabors.ImageSharp |
| PDF Processing | PdfSharpCore, PDFtoImage |
| Logging | Serilog |
| Configuration | INI-based (ini-parser-netstandard) |

---

## Getting Started

### Requirements
- Windows 10/11 (64-bit) or Linux (x64)
- A modern web browser (Chrome, Firefox, Edge, Safari)
- FFmpeg (Custom version, built-in)
- TMDB API key (optional, free - for movie/TV metadata, actor data, and What's New Online)
- Watchmode API key (optional, free tier available - for streaming provider availability)

### Running NexusM

1. Extract the portable package to a folder of your choice.
2. Run `NexusM.exe` on Windows or `./NexusM` on Linux.
3. A browser window will open to `http://localhost:8182`.
4. Log in with the default username `admin` and PIN `000000` on first use.
5. Go to **Settings** to configure your media library folders, API keys, and other preferences.
6. Click **Scan Library** to index your media.

### Accessing from Other Devices

NexusM binds to all network interfaces by default (`0.0.0.0:8182`). Access it from any device on your network using the host machine's IP address (e.g., `http://192.168.1.100:8182`). Ensure your firewall allows connections on port 8182.

---

## Configuration

NexusM is configured via `NexusM.conf` (INI format) located next to the executable. Most settings can also be changed from the web-based Settings page.

Key configuration sections: Server, Security, Library, Playback, Transcoding, Metadata, Database, Logging, DLNA, and UI.

See the [User Guide](https://nexusm.org/d/nexusm_user_guide.pdf) for a full configuration reference.

---

## Frequently Asked Questions

**Is NexusM free?**
Yes. NexusM is free to download and use for personal, non-commercial purposes, including hardware-accelerated transcoding, which Plex and Emby charge for through Plex Pass and Emby Premiere.

**Is NexusM a Plex alternative?**
Yes. NexusM covers the same core use case, a private media server for movies, TV, and music, without a cloud account, a required internet connection, or a paid tier for hardware transcoding.

**Is NexusM a Jellyfin alternative?**
Yes. NexusM targets the same self-hosted, no-account model as Jellyfin, but ships more media types and features built in by default, including game emulation, photos, podcasts, audiobooks, eBooks, and internet TV with an EPG grid, rather than relying on a plugin ecosystem.

**Is NexusM open source?**
NexusM is source-available. The code on GitHub is public and can be read, built, and modified for personal, non-commercial use under the PolyForm Noncommercial License 1.0.0. Commercial use or redistribution requires a separate license from the author.

**Does NexusM require an account or internet connection?**
No. NexusM runs entirely on your own hardware and does not require a user account, a cloud relay, or an internet connection for local playback. An internet connection is only used optionally, for metadata lookups (TMDB, TVMaze) and remote access.

**What platforms does NexusM run on?**
Windows and Linux as a portable, single-folder install, plus an official Docker image. Client apps are available for Android TV and Samsung TV.

**Does NexusM support hardware transcoding?**
Yes, at no cost. NexusM supports NVIDIA NVENC, Intel Quick Sync (QSV/VAAPI), and AMD AMF hardware encoders, including HDR tone mapping for HDR10, HDR10+, HLG, and Dolby Vision content.

**Does NexusM have a more advanced music, photo, or search experience than Plex, Jellyfin, or Emby?**
In several areas, yes. The music library supports full-text search, ReplayGain normalisation, a 10-band equalizer, synced lyrics, and several custom player interfaces, including retro hardware-style skins, where the other three offer one fixed player. Smart Search adds semantic, natural-language search, including dialogue/scene search that finds a moment in a movie or TV episode by what a character says, something none of the three provide natively. The photo library plots images on an interactive world map using embedded GPS data, a feature Plex briefly shipped and discontinued and that Jellyfin and Emby do not currently offer. NexusM also includes native Radarr/Sonarr request support built in, where Plex, Jellyfin, and Emby all rely on a separate app such as Overseerr, Jellyseerr, or Ombi for the same functionality.

---

## License

This project is licensed under the **PolyForm Noncommercial License 1.0.0**.

You are free to use, copy, modify, and share NexusM for personal, non-commercial purposes, including sharing your own modified versions. Commercial use of NexusM, or of any modified version of it, requires a separate license from the author.

See [LICENSE](LICENSE) for full terms.

## Third-Party Libraries

NexusM uses open-source libraries under their respective licenses (MIT, Apache-2.0, LGPL-2.1, BSD-2-Clause, Unlicense). See [LICENSE](LICENSE) for the complete list.

---
<img width="1374" height="1339" alt="Image" src="https://github.com/user-attachments/assets/5b7ea842-0913-4f2b-bf8c-10450f69c2c0" />

<img width="1882" height="968" alt="Image" src="https://github.com/user-attachments/assets/f99138f3-dda2-4e84-ba03-62c8addcc945" />

<img width="2544" height="1334" alt="Image" src="https://github.com/user-attachments/assets/17953707-40d7-4541-96d8-b7461b3739b7" />

<img width="2392" height="1282" alt="Image" src="https://github.com/user-attachments/assets/97681e66-7421-4490-8e9d-510e6027014b" />

<img width="2545" height="1352" alt="Image" src="https://github.com/user-attachments/assets/b58525f5-04b2-4551-bed5-221ebc48faca" />

<img width="1907" height="1346" alt="Image" src="https://github.com/user-attachments/assets/7d369840-022f-4a0a-ae17-27800836af8a" />

<img width="2535" height="1339" alt="Image" src="https://github.com/user-attachments/assets/5a845c93-f221-4853-9c1f-96788755144b" />

<img width="1877" height="1031" alt="Image" src="https://github.com/user-attachments/assets/d9a22d4d-7681-4d85-86ee-b55744bc6c2b" />

<img width="1877" height="1024" alt="Image" src="https://github.com/user-attachments/assets/304a6148-7648-4115-aa30-9ba5727365c1" />

<img width="2500" height="1367" alt="Image" src="https://github.com/user-attachments/assets/32f94ee8-e281-425a-9c76-70504c0cdb83" />

<img width="1655" height="693" alt="Image" src="https://github.com/user-attachments/assets/991bd589-c5de-4801-a0ce-c99425f65993" />

<img width="1877" height="1386" alt="Image" src="https://github.com/user-attachments/assets/3617d976-121b-494b-aa50-daab4c7976e3" />

<img width="2506" height="1331" alt="Image" src="https://github.com/user-attachments/assets/e17367ac-7046-406b-a357-e0fc9aa65d73" />

---
Android client / Google TV

<img width="1280" height="800" alt="Image" src="https://github.com/user-attachments/assets/3776b2b9-0044-4e39-8f4d-2414af6625b9" />
<img width="1280" height="800" alt="Image" src="https://github.com/user-attachments/assets/6945ca1f-7731-4f5d-8373-bbae4ed9fb06" />


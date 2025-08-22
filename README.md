
# osu! Beatmaps from Twitch Chat

A C# .NET command-line tool that extracts osu! beatmap URLs from Twitch chat logs. This tool fetches all VODs (Video on Demand) for a specified Twitch streamer, downloads their chat logs, and parses them to find osu! beatmap links that were shared in the chat.

> **⚠️ WARNING: This tool uses undocumented Twitch API endpoints and may break at any time. Updates will be provided when possible.**

## Features

- 🎮 Extract osu! beatmap URLs from any Twitch streamer's chat history
- 📦 Smart caching system to avoid re-downloading processed VODs
- 📊 Progress tracking during chat download
- 📄 JSON output format for easy data processing
- 🔍 Automatic duplicate removal
- 💾 Persistent storage of processed VODs

## Prerequisites

- .NET 6.0 Runtime or SDK
- Internet connection for API access

## Installation

### From Source

1. Clone the repository:
```bash
git clone https://github.com/nayutalienx/osu-beatmaps-from-twitch-chat.git
cd osu-beatmaps-from-twitch-chat
```

2. Build the project:
```bash
dotnet build
```

3. Run the application:
```bash
dotnet run --project BeatmapDownloaderCLI <streamer_username>
```

### Pre-built Binary

Check the [releases page](https://github.com/nayutalienx/osu-beatmaps-from-twitch-chat/releases) for pre-built executables.

## Usage

### Basic Usage

```bash
# Using dotnet run
dotnet run --project BeatmapDownloaderCLI <streamer_username>

# Using compiled executable
./BeatmapDownloaderCLI.exe <streamer_username>
```

### Example

```bash
$ ./BeatmapDownloaderCLI.exe shigetora
Found 33 vods for shigetora
Founded vods, that already downloaded. Cache will be used.

Download chat for vod 1234567890
[STATUS] - Downloading chat...
[STATUS] - Downloading 45%
[STATUS] - Downloading 100%

Amount of beatmaps: 714
Beatmaps saved to output/output.json
```

## How It Works

1. **VOD Discovery**: Uses Twitch's GraphQL API to fetch all archived VODs for the specified streamer
2. **Chat Download**: Downloads chat logs for each VOD in JSON format using TwitchDownloaderCore
3. **Caching**: Stores processed VOD IDs to avoid re-downloading (stored in `archive/archive.json`)
4. **URL Extraction**: Parses chat messages using regex to find osu! beatmap URLs (`osu.ppy.sh/b/[number]`)
5. **Deduplication**: Removes duplicate URLs and saves unique results
6. **Output**: Saves all found beatmap URLs to `output/output.json`

## Output Format

The tool generates a JSON file containing an array of beatmap URLs:

```json
[
  "osu.ppy.sh/b/2889995",
  "osu.ppy.sh/b/2167822",
  "osu.ppy.sh/b/3092273",
  "osu.ppy.sh/b/1234567"
]
```

Each URL can be used to:
- Visit the beatmap page: `https://osu.ppy.sh/b/2889995`
- Direct download: `https://osu.ppy.sh/d/[beatmapset_id]` (requires additional API call)

## File Structure

After running the tool, the following directories and files will be created:

```
├── archive/
│   └── archive.json          # Cache of processed VOD IDs
├── chat/
│   ├── [vod_id_1].json      # Downloaded chat logs
│   ├── [vod_id_2].json
│   └── ...
└── output/
    └── output.json          # Final list of beatmap URLs
```

## Dependencies

The project uses the following key dependencies:

- **Newtonsoft.Json** (12.0.3) - JSON serialization and parsing
- **SkiaSharp** (2.80.2) - Graphics library (used by TwitchDownloaderCore)
- **TwitchDownloaderCore** - Core library for Twitch chat downloading

> **Note**: Some dependencies have known vulnerabilities. Consider updating to newer versions if available.

## Technical Details

### API Endpoints

The tool uses Twitch's undocumented GraphQL endpoint:
- **Base URL**: `https://gql.twitch.tv/gql`
- **Client ID**: `kimne78kx3ncx6brgo4mv6wki5h1ko` (anonymous client)
- **Operation**: `FilterableVideoTower_Videos` for VOD discovery

### URL Pattern Matching

The tool searches for URLs matching the pattern:
```regex
osu\.ppy\.sh/b/\d+
```

This captures both:
- Direct beatmap links: `osu.ppy.sh/b/123456`
- Full URLs in chat: `https://osu.ppy.sh/b/123456`

## Limitations

- ⚠️ **API Dependency**: Relies on undocumented Twitch APIs that may change or break
- 📅 **VOD Availability**: Can only process VODs that are still available on Twitch
- 🕐 **Rate Limiting**: May be subject to Twitch's rate limiting policies
- 💾 **Storage**: Large streamers with many VODs will require significant disk space for chat logs
- 🔒 **Subscriber-only VODs**: Cannot access subscriber-only or deleted content

## Troubleshooting

### Common Issues

**"Vods not found"**
- Verify the streamer username is correct
- Check if the streamer has any archived VODs
- Ensure the streamer's VODs are publicly accessible

**"Invalid ID, unable to parse"**
- This usually indicates an API response issue
- Try running the tool again after a few minutes
- Check your internet connection

**Build Errors**
- Ensure you have .NET 6.0 SDK installed
- Run `dotnet restore` to restore NuGet packages
- Check for any missing dependencies

**Runtime Errors**
- Make sure you have .NET 6.0 Runtime installed
- On Linux, you may need to install additional SkiaSharp dependencies

## Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/amazing-feature`
3. Commit your changes: `git commit -m 'Add amazing feature'`
4. Push to the branch: `git push origin feature/amazing-feature`
5. Open a Pull Request

### Development Setup

```bash
# Clone and build
git clone https://github.com/nayutalienx/osu-beatmaps-from-twitch-chat.git
cd osu-beatmaps-from-twitch-chat
dotnet restore
dotnet build

# Run tests (if available)
dotnet test

# Run the application
dotnet run --project BeatmapDownloaderCLI <test_streamer>
```

## License

This project is licensed under the MIT License. See the [LICENSE.txt](LICENSE.txt) file for details.

## Acknowledgments

- Built on top of [TwitchDownloader](https://github.com/lay295/TwitchDownloader) core library
- Uses the osu! community's beatmap URL format standards
- Thanks to the Twitch and osu! communities for making this tool possible

## Related Projects

- [TwitchDownloader](https://github.com/lay295/TwitchDownloader) - Full-featured Twitch VOD and chat downloader
- [osu!](https://osu.ppy.sh/) - The rhythm game this tool is designed for

---

**Disclaimer**: This tool is not affiliated with Twitch Interactive, Inc. or ppy Pty Ltd (osu!). Use responsibly and in accordance with Twitch's Terms of Service.

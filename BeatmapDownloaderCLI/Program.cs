// See https://aka.ms/new-console-template for more information

using BeatmapDownloaderCLI;
using SkiaSharp;
using TwitchDownloaderCore;
using TwitchDownloaderCore.Options;

namespace BeatmapDownloaderCLI
{
    class Program
    {
        static readonly string _vodArchivePath = "archive/archive.json";
        static readonly string _beatmapOutputPath = "output/output.json";
        static readonly string _chatDir = "chat/";
        static readonly string _jsonType = ".json";
        static bool _wasLastMessagePercent = false;
        static string previousStatus = "";
        private static bool skipMessagePrinted = false;

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Pass streamer nickname please.");
                Environment.Exit(1);
            }

            FileStore vodFileStore = new FileStore(_vodArchivePath);

            string[] vods = VodParser.ParseUserVodIds(args[0]);

            foreach (var vodId in vods)
            {
                if (vodFileStore.Items.Contains(vodId))
                {
                    if (!skipMessagePrinted)
                    {
                        Console.WriteLine("Founded vods, that already downloaded. Cache will be used.");
                        skipMessagePrinted = true;
                    }

                    continue;
                }

                Console.WriteLine("\nDownload chat for vod " + vodId);
                DownloadChat(vodId);
                vodFileStore.Add(vodId);
                vodFileStore.Store();
            }

            List<string> urls = new List<string>();

            foreach (var vod in vods)
            {
                using (StreamReader r = new StreamReader(getChatPath(vod)))
                {
                    string json = r.ReadToEnd();
                    urls.AddRange(BeatmapUrlParser.ParseUrlsFromString(json));
                }
            }

            urls = urls.Distinct().ToList();

            Console.WriteLine("Amount of beatmaps: " + urls.Count);
            FileStore output = new FileStore(_beatmapOutputPath);
            output.Items = urls;
            output.Store();
            Console.WriteLine("Beatmaps saved to " + _beatmapOutputPath);
        }

        private static void DownloadChat(string vodId)
        {
            ChatDownloadOptions downloadOptions = new ChatDownloadOptions();

            if (vodId == "")
            {
                Console.WriteLine("[ERROR] - Invalid ID, unable to parse.");
                Environment.Exit(1);
            }

            downloadOptions.IsJson = true;
            downloadOptions.Id = vodId;
            downloadOptions.Filename = getChatPath(vodId);
            FileUtils.PreparePath(downloadOptions.Filename);

            ChatDownloader chatDownloader = new ChatDownloader(downloadOptions);
            Progress<ProgressReport> progress = new Progress<ProgressReport>();
            progress.ProgressChanged += Progress_ProgressChanged;
            chatDownloader.DownloadAsync(progress, new CancellationToken()).Wait();
        }

        private static void Progress_ProgressChanged(object sender, ProgressReport e)
        {
            if (e.reportType == ReportType.Message)
            {
                if (_wasLastMessagePercent)
                {
                    _wasLastMessagePercent = false;
                    Console.WriteLine("");
                }

                string currentStatus = "[STATUS] - " + e.data;
                if (currentStatus != previousStatus)
                {
                    previousStatus = currentStatus;
                    Console.WriteLine(currentStatus);
                }
            }
            else if (e.reportType == ReportType.Log)
            {
                if (_wasLastMessagePercent)
                {
                    _wasLastMessagePercent = false;
                    Console.WriteLine("");
                }

                Console.WriteLine("[LOG] - " + e.data);
            }
            else if (e.reportType == ReportType.MessageInfo)
            {
                Console.Write("\r[STATUS] - " + e.data);
                _wasLastMessagePercent = true;
            }
        }

        private static string getChatPath(string vod)
        {
            return _chatDir + vod + _jsonType;
        }
    }
}
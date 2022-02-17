namespace BeatmapDownloaderCLI;

public class FileUtils
{
    public static void PreparePath(string file)
    {
        string[] combinedPath = file.Split("/");
        combinedPath[^1] = "";
        if (!Directory.Exists(file)) Directory.CreateDirectory(String.Join("/", combinedPath));
        if (!File.Exists(file)) File.AppendText(file);
    }
}
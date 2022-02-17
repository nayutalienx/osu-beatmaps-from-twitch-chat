using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BeatmapDownloaderCLI;

public class BeatmapUrlParser
{
    public static List<string> ParseUrlsFromString(string source)
    {
        dynamic parsedJson = JsonConvert.DeserializeObject(source);
        string rawFormattedJson = JsonConvert.SerializeObject(parsedJson, Formatting.Indented);

        List<string> linesWithUrls = new List<string>();

        StringReader strReader = new StringReader(rawFormattedJson);

        while (true)
        {
            string line = strReader.ReadLine();
            if (line != null)
            {
                if (line.Contains("osu.ppy.sh/b/"))
                {
                    string urlPart = line.Split("://")[1];
                    urlPart = urlPart.Replace("\",", "");
                    if (!Char.IsNumber(urlPart[urlPart.Length - 1]))
                    {
                        urlPart = urlPart.Split(" ")[0];
                    }

                    linesWithUrls.Add(urlPart);
                }
            }
            else
            {
                break;
            }
        }

        return linesWithUrls.Distinct().ToList();
    }
}
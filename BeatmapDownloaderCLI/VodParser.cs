using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Newtonsoft.Json;

namespace BeatmapDownloaderCLI;

public class VodParser
{
    // todo: generate clientId for anonymous user
    private static string clientId = "kimne78kx3ncx6brgo4mv6wki5h1ko";
    private static string url = "https://gql.twitch.tv/";
    private static string endpoint = "gql";

    class VodResponse
    {
        public VodData data { get; set; }
    }

    class VodData
    {
        public VodUser user { get; set; }
    }


    class VodUser
    {
        public string id { get; set; }

        public VodVideos videos { get; set; }
    }


    class VodVideos
    {
        public VodEdge[] edges { get; set; }
    }

    class VodEdge
    {
        public VodNode node { get; set; }
    }

    class VodNode
    {
        public string id { get; set; }
    }

    public static string[] ParseUserVodIds(string username)
    {
        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri(url);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, endpoint);
        request.Content = new StringContent(getJsonBodyForUser(username), Encoding.UTF8, "application/json");

        request.Headers.Add("Client-Id", clientId);

        var response = client.Send(request);

        List<VodResponse> result =
            JsonConvert.DeserializeObject<List<VodResponse>>(response.Content.ReadAsStringAsync().Result);

        List<string> ids = new List<string>();

        foreach (var vodResponse in result)
        {
            foreach (var videosEdge in vodResponse.data.user.videos.edges)
            {
                ids.Add(videosEdge.node.id);
            }
        }

        if (ids.Count == 0)
        {
            Console.WriteLine("Vods not found");
            Environment.Exit(1);
        }

        Console.WriteLine("Found " + ids.Count + " vods for " + username);

        return ids.ToArray();
    }

    private static string getJsonBodyForUser(string username)
    {
        return
            "[\n{\n\"operationName\": \"FilterableVideoTower_Videos\",\n\"variables\": {\n\"limit\": 100,\n\"channelOwnerLogin\": \"" +
            username +
            "\",\n\"broadcastType\": \"ARCHIVE\",\n\"videoSort\": \"TIME\"\n},\n\"extensions\": {\n\"persistedQuery\": {\n\"version\": 1,\n\"sha256Hash\": \"a937f1d22e269e39a03b509f65a7490f9fc247d7f83d6ac1421523e3b68042cb\"\n}\n}\n}\n]";
    }
}
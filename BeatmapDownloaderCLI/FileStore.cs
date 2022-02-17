using Newtonsoft.Json;

namespace BeatmapDownloaderCLI;

public class FileStore
{
    private List<string> items;

    public List<string> Items
    {
        get => new(items);
        set => items = value;
    }

    private string file;

    public FileStore(string file)
    {
        this.file = file;

        FileUtils.PreparePath(file);

        using (StreamReader r = new StreamReader(file))
        {
            string json = r.ReadToEnd();
            if (String.IsNullOrEmpty(json))
            {
                items = new List<string>();
            }
            else
            {
                items = JsonConvert.DeserializeObject<List<string>>(json);
            }
        }
    }

    public void Add(string val)
    {
        items.Add(val);
    }

    public void Store()
    {
        using (StreamWriter file = File.CreateText(this.file))
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Serialize(file, items);
        }
    }
}
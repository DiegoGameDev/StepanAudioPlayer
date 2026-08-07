using Newtonsoft.Json;
using Stepan.Song;

namespace Stepan.Service;

public class SongListService
{
    private readonly JsonSerializerSettings settings = new()
    {
        Formatting = Formatting.Indented,
        TypeNameHandling = TypeNameHandling.Auto
    };

    public SongList GetPlayList(string name)
    {
        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Stepan", "StepanPlayLists", name);
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);

            SongList? songList =  JsonConvert.DeserializeObject<SongList>(json, settings);
            return songList;
        }

        var newListJson = JsonConvert.SerializeObject(new SongList(), settings);

        File.WriteAllText(path, newListJson);
        return new SongList();
    }

    public SongList SaveSongList(SongList songList, string name)
    {
        if (songList == null) return default;

        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Stepan", "StepanPlayLists", name);

        var json = JsonConvert.SerializeObject(songList, settings);

        File.WriteAllText(path, json);

        return songList;
    }

    public void DeleteSongList(string name)
    {
        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Stepan", "StepanPlayLists", name);

        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    public IEnumerable<SongList> SongLists()
    {
        string extension = ".stpsl";
        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Stepan", "StepanPlayLists");
        var files = Directory.GetFiles(path).Where(p => extension.Contains(Path.GetExtension(p)));

        List<SongList> songLists = new();
        foreach (var i in files)
        {
            string json = File.ReadAllText(i);
            songLists.Add(JsonConvert.DeserializeObject<SongList>(json));
        }

        return songLists;
    }
}
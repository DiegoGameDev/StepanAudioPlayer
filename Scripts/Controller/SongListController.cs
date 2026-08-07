using Newtonsoft.Json;
using Stepan.Service;
using Stepan.Song;

namespace Stepan.Controller;

public class SongListController
{
    private readonly SongListService _service = new();

    private readonly JsonSerializerSettings settings = new()
    {
        TypeNameHandling = TypeNameHandling.Auto,
        Formatting= Formatting.Indented
    };

    public class MusicFolder{public List<string> folders = new List<string>();}
    private MusicFolder? musicFolder;

    /// <summary>
    /// load musics in default songList
    /// </summary>
    public SongList LoadDefaultMusics()
    {
        if (musicFolder == null)
            LoadMusicFolder();

        var extensoes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".wav", ".mp3"
        };

        List<string> files = new List<string>();

        for (int f = 0; f < musicFolder.folders.Count; f++)
            files.AddRange(Directory.EnumerateFiles(musicFolder.folders[f]).Where(p => extensoes.Contains(Path.GetExtension(p))).ToList());

        SongList songList = new SongList();
        songList.SongListName = "Default";

        for (int i = 0; i < files.Count; i++)
        {
            songList.filePath.Add(files[i]);
        }

        _service.SaveSongList(songList, "Default.stpsl");
        return songList;
    }

    public void Delete(string name)
    {
        _service.DeleteSongList(name);
    }
    
    public IEnumerable<SongList> GetSongList()
    {
        return _service.SongLists();
    }
    public SongList LoadSongList(string playlistName)
    {
        SongList songList = new();

        return _service.GetPlayList(playlistName);
    }

    public void LoadMusicFolder()
    {
        string basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StepanAudioPlayer", "MusicFolderPaths.stpf");

        if (File.Exists(basePath))
        {
            var read = File.ReadAllText(basePath);
            musicFolder = JsonConvert.DeserializeObject<MusicFolder>(read, settings);
            
            return;
        }

        musicFolder = new();
        musicFolder.folders.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), "StepanMusics"));

        var setJson = JsonConvert.SerializeObject(musicFolder);

        File.WriteAllText(basePath, setJson);
    }

    public void SetMusicFolder(string folder)
    {
        string basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StepanAudioPlayer", "MusicFolderPaths.stpf");

        musicFolder?.folders.Add(folder);

        var setJson = JsonConvert.SerializeObject(musicFolder);

        File.WriteAllText(basePath, setJson);
    }
}
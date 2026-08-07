using System.Text;
using Stepan.Controller;
using Stepan.Song;

namespace Stepan.Models;

public class SongListLayout : ILayout
{
    private readonly LayoutController _layoutController = new();
    private readonly SongListController _songListController = new();
    int currentOptionIndex = 0;

    public SongList SonglistForEdit;
    List<SongList> songLists= new();

    private List<string> options = new List<string>()
    {
        "CreatePlaylist"
    };
    public SongListLayout()
    {
        IEnumerable<string> playlists;
        songLists = _songListController.GetSongList().ToList();
        playlists = songLists.Select(x => x.SongListName);

        options.AddRange(playlists);
        SonglistForEdit = songLists[currentOptionIndex];
    }


    public string CompileLayout()
    {
        StringBuilder layout = new(_layoutController.ReadLayout("SongListLayout.STP"));
        
        layout.Replace("(Options)", OptionsCompiled());

        return layout.ToString();
    }

    private string OptionsCompiled()
    {
        // titulo e quebra de linha
        StringBuilder stringBuilder = new StringBuilder();
        
        for(int i = 0; i < options.Count; i++)
        {
            if (i == currentOptionIndex)
                stringBuilder.Append(string.Concat(options[i], " <<", Environment.NewLine));
            else
                stringBuilder.Append(string.Concat(options[i], Environment.NewLine));
        }

        return stringBuilder.ToString();
    }

    public void Selector(int nextSongIndex)
    {
        switch(nextSongIndex)
        {
            case 1:
            currentOptionIndex++;
            break;
            case -1:
            currentOptionIndex--;
            break;
        }

        currentOptionIndex =  Math.Clamp(currentOptionIndex, 0, options.Count - 1);

        if (currentOptionIndex == 0)
        {
            SonglistForEdit = _songListController.LoadSongList("Default.stpsl");
            return;
        }

        SonglistForEdit = songLists[currentOptionIndex - 1];
    }

    public void Confirm()
    {
        string optionSelected = string.Concat(options[currentOptionIndex], ".stpsl");

        if (currentOptionIndex == 0)
        {
            Program.CreatePlaylistRender?.Invoke();
            return;
        }

        Layout.CurrentLayout.ChangeSongList(optionSelected);
    }

    public void Delete()
    {
        _songListController.Delete(string.Concat(options[currentOptionIndex], ".stpsl"));
        options.Remove(options[currentOptionIndex]);
    }
}
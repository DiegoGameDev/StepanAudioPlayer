using System.Text;
using Stepan.Controller;
using Stepan.Service;
using Stepan.Song;

namespace Stepan.Models;

public class SongListEdit : ILayout
{
    List<string> songs = new();
    SongList songListBase = new();

    SongList newSongList = new();

    private readonly SongListService _songListService = new();
    private readonly LayoutController _layoutController = new();
    public int CurrentOptionIndex {get; private set;} = 0;
    List<int> indices = new();

    public void Init()
    {
        songListBase = _songListService.GetPlayList("Default.stpsl");
        songs = songListBase.SongNames();
    }

    //isso vai ficar extremamente pesado conforme muitas musicas estiverem no sistema
    //Depois eu resolvo
    public void Init(SongList songList)
    {
        indices = new();
        songListBase = songList;
        newSongList.filePath.AddRange(songListBase.filePath);
        SongList allSongsInSystem = _songListService.GetPlayList("Default.stpsl");
        allSongsInSystem.filePath = allSongsInSystem.filePath.Where(x => !songListBase.filePath.Contains(x)).ToList();

        for (int index = 0; index < songListBase.filePath.Count; index++)
        {
            indices.Add(index);
        }

        songListBase.filePath.AddRange(allSongsInSystem.filePath);
        songs = songListBase.SongNames();
        newSongList.SongListName = songListBase.SongListName;

        for (int i = 0; i < songs.Count; i++)
        {
            if (indices.Contains(i))
            {
                songs[i] = string.Concat("X ", songs[i]);
            }
        }
    }

    public void Add()
    {
        if (indices.Contains(CurrentOptionIndex))
        {
            songs[CurrentOptionIndex] = songs[CurrentOptionIndex].Remove(0, 2);

            indices.Remove(CurrentOptionIndex);
            newSongList.filePath.Remove(songListBase.filePath[CurrentOptionIndex]);
            //newSongList.songName.Remove(songListBase.songName[CurrentOptionIndex]);
            return;
        }

        indices.Add(CurrentOptionIndex);
        newSongList.filePath.Add(songListBase.filePath[CurrentOptionIndex]);
        //newSongList.songName.Add(songListBase.songName[CurrentOptionIndex]);

        songs[CurrentOptionIndex] = string.Concat("X ", songs[CurrentOptionIndex]);
    }

    public string CompileLayout()
    {
        StringBuilder layout = new(_layoutController.ReadLayout("SongListEditLayout.STP"));

        layout.Replace("(Options)", CompileOptions());

        return layout.ToString();
    }

    string CompileOptions()
    {
        StringBuilder songsBuilder = new();

        for (int i = 0; i < songs.Count; i++)
        {
            if (i == CurrentOptionIndex)
            {
                songsBuilder.Append(string.Concat(songs[i], " < < <")).Append(Environment.NewLine);
            }
            else
            {
                songsBuilder.Append(songs[i]).Append(Environment.NewLine);
            }
        }

        return songsBuilder.ToString();
    }

    public void Selector(int nextSongIndex)
    {
        switch(nextSongIndex)
        {
            case 1:
            CurrentOptionIndex++;
            break;
            case -1:
            CurrentOptionIndex--;
            break;
        }

        CurrentOptionIndex =  Math.Clamp(CurrentOptionIndex, 0, songs.Count - 1);
    }

    public void SetName(string playListName)
    {
        newSongList.SongListName = playListName;
    }

    public void Confirm()
    {
        _songListService.SaveSongList(newSongList, string.Concat(newSongList.SongListName, ".stpsl"));
    }
}
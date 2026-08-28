using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using NAudio.Wave;
using Stepan.Controller;
using Stepan.Song;

namespace Stepan.Models;

//layout contem o header, footer e um objeto que armazena musicas
public class Layout : IDisposable, ILayout
{
    private SongList songListBase = new();
    public SongList currentPlayList = new();

    public ReproductionMode reproductionMode = ReproductionMode.RepeatPlaylist;
    int reproductionModeIndex = 0;
    public ReproductionOrder reproductionOrder = ReproductionOrder.ByOrder;

    private string currentPlayListCompiled = "";
    public int currentSongIndex {private set; get;} = 0;
    public string CurrentSongPlaying {get; private set;}
    private int currentSongPlayingindex = 0;

    private readonly LayoutController _layoutController = new();
    private readonly SongListController _songListController = new();

    public Player getPlayer => player;
    Player player;
    public PlayerState playerState = PlayerState.Playing;
    public enum PlayerState{Playing, Paused}
    public bool Played {get; private set;}

    public static Layout CurrentLayout {get; set;}


    public void SetLayout()
    {
        CurrentLayout = this;
    }

    public void ChangeSongList(string name)
    {
        SongList PlayList = _songListController.LoadSongList(name);
        if (PlayList != null)
        {
            songListBase = new(PlayList);
            currentPlayList = new(songListBase);
        }

        currentSongIndex = 0;
    }

    public void LoadPlayList(string name)
    {
        _songListController.LoadMusicFolder();
        songListBase =  _songListController.LoadDefaultMusics();
        currentPlayList = new(songListBase);
        
        if (player == null)
            player = new(currentPlayList.filePath[currentSongIndex]);
    }

    public string CompileLayout()
    {
        StringBuilder stringBuilder = new();

        stringBuilder.Append(_layoutController.ReadDefaultLayout());
        
        stringBuilder.Replace("(Musicas)", PlayListCompiled());

        return stringBuilder.ToString();
    }

    private string PlayListCompiled()
    {
        // titulo e quebra de linha
        StringBuilder stringBuilder = new StringBuilder($" {currentPlayList.SongListName}").Append(Environment.NewLine).Append(Environment.NewLine);

        var names = currentPlayList.SongNames();
        
        for(int i = 0; i < currentPlayList.SongNames().Count; i++)
        {
            if (i == currentSongIndex)
                stringBuilder.Append(string.Concat(">> : ", names[i], Environment.NewLine));
            else
                stringBuilder.Append(string.Concat(names[i], Environment.NewLine));
        }

        return stringBuilder.ToString();
    }

    /// <summary>
    /// Down arrow increments +1
    /// Up arrow incremeants -1
    /// </summary>
    /// <param name="nextSongIndex"></param>
    public void Selector(int nextSongIndex)
    {
        switch(nextSongIndex)
        {
            case 1:
            currentSongIndex++;
            break;
            case -1:
            currentSongIndex--;
            break;
        }

        currentSongIndex =  Math.Clamp(currentSongIndex, 0, currentPlayList.SongNames().Count - 1);
    }

    public void OnMusicFinished()
    {
        switch(reproductionMode)
        {
            case ReproductionMode.RepeatTrack:
                Play(CurrentSongPlaying);
            break;
            case ReproductionMode.RepeatPlaylist:
                currentSongIndex++;
                if (currentSongIndex > currentPlayList.SongNames().Count - 1)
                    currentSongIndex = 0;
                Play();
            break;
            case ReproductionMode.NoRepeat: 
                currentSongIndex++;
                if (currentSongIndex > currentPlayList.SongNames().Count - 1)
                    currentSongIndex = 0;
                player.Stop();
            break;
        }
    }
    #region  Reproductions Behaviour
    public void ChangeReproducionMode()
    {
        reproductionModeIndex++;
        if (reproductionModeIndex > 2)
            reproductionModeIndex = 0;

        reproductionMode = (ReproductionMode)reproductionModeIndex;
    }

    public void ChangeReproductionOrder()
    {
        switch (reproductionOrder)
        {
            case ReproductionOrder.ByOrder:
                Randomizer();
            break;
            case ReproductionOrder.Random:
                DefaultOrder();
            break;
        }
    }

    private void DefaultOrder()
    {
        currentPlayList = new(songListBase);
        reproductionOrder = ReproductionOrder.ByOrder;
        Program.CallRender.Invoke();
    }

    private void Randomizer()
    {
        SongList songListRandomized = new(songListBase);

        Random.Shared.Shuffle<string>(CollectionsMarshal.AsSpan(songListRandomized.filePath));
        currentPlayList = new(songListRandomized);

        reproductionOrder = ReproductionOrder.Random;
        Program.CallRender.Invoke();
    }
    #endregion

    public void Play(string path = "")
    {
        if (!string.IsNullOrEmpty(path))
        {
            Played = true;
            playerState = PlayerState.Playing;
            player.Stop();
            CurrentSongPlaying = path;
            player = new Player(CurrentSongPlaying);
            player.Play();
            Program.CallRender.Invoke();
        }

        Played = true;
        playerState = PlayerState.Playing;
        player.Stop();
        CurrentSongPlaying = new(currentPlayList.filePath[currentSongIndex]);
        player = new Player(currentPlayList.filePath[currentSongIndex]);
        player.Play();
        currentSongPlayingindex = currentSongIndex;
        Program.CallRender.Invoke();
    }
    public void Pause()
    {
        PlayerState state;
        if (playerState == PlayerState.Playing)
        {
            state = PlayerState.Paused;
            player.Pause();
        }
        else
        {
            state = PlayerState.Playing;
            player.Play();
        }

        playerState = state;
    }
    public void ChangeVolume(bool sum)
    {
        player.IncrementVolume(sum);
    }

    public void Dispose()
    {
        player.Dispose();
    }
}
public enum ReproductionMode {RepeatPlaylist = 0, RepeatTrack = 1, NoRepeat = 2}
public enum ReproductionOrder {ByOrder, Random}
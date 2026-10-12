using System.Text;
using Stepan.Models;

public class Program
{
    public static void Main(string[] args)
    {
        AppLauncher.Launch();
        RenderLayout();
    }

    internal static Action CallRender = DrawCall;
    internal static Action CreatePlaylistRender = CreatePlaylistDrawCall;
    
    static bool needsRender = true;
    internal static bool isPlaying = false;

    static void DrawCall(){Console.Clear(); Render(currentLayout);}
    static void CreatePlaylistDrawCall() => CreatePlaylist();
    static bool creatingPlaylist = false;

    static Layout musicSelector_layout;
    static SongListEdit editSonglist_layout;
    static ILayout currentLayout;

    private static void RenderLayout()
    {
        musicSelector_layout = new ();
        musicSelector_layout.SetLayout();
        musicSelector_layout.LoadPlayList("default.stpsl");

        isPlaying = true;

        object s = new();
        EventArgs e = new EventArgs();
        AppDomain.CurrentDomain.ProcessExit += END;

        while(isPlaying)
        {
            if (creatingPlaylist)
                break;
                
            if (Console.KeyAvailable)
            {
                switch(Console.ReadKey().Key)
                {
                    case ConsoleKey.Enter:
                    musicSelector_layout.PlayedByKeyboard();
                    Console.Clear();
                    needsRender = true;
                    break;

                    case ConsoleKey.UpArrow:
                    musicSelector_layout.Selector(-1);
                    Console.Clear();
                    needsRender = true;
                    break;

                    case ConsoleKey.DownArrow:
                    musicSelector_layout.Selector(1);
                    Console.Clear();
                    needsRender = true;
                    break;

                    case ConsoleKey.P:
                    musicSelector_layout.Pause();
                    Console.Clear();
                    needsRender = true;
                    break;

                    case ConsoleKey.S:
                        SongListsLayout();
                    break;

                    case ConsoleKey.F5:
                        musicSelector_layout.LoadPlayList("default.stpsl");
                        Console.Clear();
                    needsRender = true;
                    break;

                    case ConsoleKey.O:
                        musicSelector_layout.ChangeReproductionOrder();
                        break;

                    case ConsoleKey.R:
                        musicSelector_layout.ChangeReproducionMode();
                        break;

                    default:

                    case ConsoleKey.Add:
                        musicSelector_layout.ChangeVolume(true);
                    break;
                    case ConsoleKey.Subtract:
                        musicSelector_layout.ChangeVolume(false);
                    break;
                    
                    break;
                }
                
            }

            if (needsRender)
            {
                needsRender = false;
                Console.Clear();
                currentLayout = musicSelector_layout;
                Render(musicSelector_layout);
            }

            Thread.Sleep(16);
        }
    }

    public static void Render(ILayout layout)
    {
        Console.WriteLine(layout.CompileLayout());

        string message;

        if (musicSelector_layout.Played){
            if (musicSelector_layout.playerState == Layout.PlayerState.Playing)
            {
                message = string.Concat("Playing: ", Path.GetFileNameWithoutExtension(musicSelector_layout.CurrentSongPlaying));
                WriteColor(message, ConsoleColor.Green);
            }
            else
            {
                message = string.Concat("Playing: ", Path.GetFileNameWithoutExtension(musicSelector_layout.CurrentSongPlaying), " - Paused");
                WriteColor(message, ConsoleColor.Red);
            }
        }
        StringBuilder reproductionInfo = new StringBuilder().Append(Environment.NewLine).Append($"Volume: {musicSelector_layout.getPlayer.Volume}").Append(Environment.NewLine)
                                                            .Append(Environment.NewLine).Append($"Reproduction Order: {musicSelector_layout.reproductionOrder.ToString()}")
                                                            .Append(Environment.NewLine).Append($"Reproduction Mode: {musicSelector_layout.reproductionMode.ToString()}");

        WriteColor(reproductionInfo.ToString(), ConsoleColor.Cyan);

    }

    static void WriteColor(string texto, ConsoleColor cor)
    {
        var corAtual = Console.ForegroundColor;

        Console.ForegroundColor = cor;
        Console.WriteLine(texto);

        Console.ForegroundColor = corAtual;
    }

    static void CreatePlaylist()
    {
        
        if (editSonglist_layout == null)
        {
            editSonglist_layout = new();
            editSonglist_layout.Init();
        }

        creatingPlaylist = true;

        while(creatingPlaylist)
        {
            Console.Clear();
            currentLayout = editSonglist_layout;
            Render(editSonglist_layout);
            switch(Console.ReadKey().Key)
                {
                    case ConsoleKey.Enter:
                    editSonglist_layout.Confirm();
                    //finish the playlist creation and render the defaultLayout
                    creatingPlaylist = false;
                    needsRender = true;
                    Console.Clear();
                    break;

                    case ConsoleKey.Escape:
                        creatingPlaylist = false;
                        needsRender = true;
                        Console.Clear();
                    break;

                    case ConsoleKey.Spacebar:
                    editSonglist_layout.Add();
                    Console.Clear();
                    break;

                    case ConsoleKey.UpArrow:
                    editSonglist_layout.Selector(-1);
                    Console.Clear();
                    break;

                    case ConsoleKey.DownArrow:
                    editSonglist_layout.Selector(1);
                    Console.Clear();
                    break;

                    case ConsoleKey.P:
                    musicSelector_layout.Pause();
                    Console.Clear();
                    break;

                    case ConsoleKey.S:
                        Console.Clear();
                        Console.WriteLine("Playlist Name");
                        string playlistname = Console.ReadLine();
                        editSonglist_layout.SetName(playlistname);
                        Console.Clear();
                    break;

                    default:
                        Console.Clear();
                    break;
                }
        }
        editSonglist_layout = null;
    }

    private static void SongListsLayout()

    {
        SongListLayout songListLayout = new();
        bool PlaylistLayout = true;

        Layout defaultLayout = Layout.CurrentLayout;

        int timesBackspacePressed = 0;

        while(PlaylistLayout)
        {
            Console.Clear();
            currentLayout = songListLayout;
            Render(songListLayout);

            switch(Console.ReadKey().Key)
            {
                case ConsoleKey.Enter:
                songListLayout.Confirm();
                PlaylistLayout = false;
                needsRender = true;
                break;

                case ConsoleKey.Backspace:
                    timesBackspacePressed++;
                    if (timesBackspacePressed >= 2)
                    {
                        songListLayout.Delete();
                        Render(songListLayout);
                        Console.Clear();
                    }
                break;

                case ConsoleKey.E:
                    editSonglist_layout = new();
                    editSonglist_layout.Init(songListLayout.SonglistForEdit);
                    CreatePlaylistRender.Invoke();
                    Console.Clear();
                break;

                case ConsoleKey.Escape:
                PlaylistLayout = false;
                needsRender = true;
                Console.Clear();
                break;

                case ConsoleKey.UpArrow:
                songListLayout.Selector(-1);
                Console.Clear();
                break;

                case ConsoleKey.DownArrow:
                songListLayout.Selector(1);
                Console.Clear();
                break;

                case ConsoleKey.P:
                defaultLayout.Pause();
                Console.Clear();
                break;
            }
        }
    }

    private static void END(object sender, EventArgs e)
    {
        musicSelector_layout.Dispose();
    }
}
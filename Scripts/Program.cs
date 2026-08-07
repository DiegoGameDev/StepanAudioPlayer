using Stepan.Controller;
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

    static void DrawCall() => needsRender = true;
    static void CreatePlaylistDrawCall() => CreatePlaylist();
    static bool creatingPlaylist = false;

    static Layout defaultLayout;
    static SongListEdit editSonglistLayout;

    private static void RenderLayout()
    {
        defaultLayout = new ();
        defaultLayout.SetLayout();
        defaultLayout.LoadPlayList("default.stpsl");

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
                    defaultLayout.Play();
                    Console.Clear();
                    needsRender = true;
                    break;

                    case ConsoleKey.UpArrow:
                    defaultLayout.Selector(-1);
                    Console.Clear();
                    needsRender = true;
                    break;

                    case ConsoleKey.DownArrow:
                    defaultLayout.Selector(1);
                    Console.Clear();
                    needsRender = true;
                    break;

                    case ConsoleKey.P:
                    defaultLayout.Pause();
                    Console.Clear();
                    needsRender = true;
                    break;

                    case ConsoleKey.S:
                        SongListsLayout();
                    break;

                    case ConsoleKey.F5:
                        defaultLayout.LoadPlayList("default.stpsl");
                        Console.Clear();
                    needsRender = true;
                    break;

                    default:
                    
                    break;
                }
                
            }

            if (needsRender)
            {
                needsRender = false;
                Console.Clear();
                Render(defaultLayout);
            }

            Thread.Sleep(16);
        }
    }

    static void Render(ILayout layout)
    {
        Console.WriteLine(layout.CompileLayout());

        string message;

        if (defaultLayout.Played){
            if (defaultLayout.playerState == Layout.PlayerState.Playing)
            {
                message = string.Concat("Playing: ", defaultLayout.CurrentSongPlaying);
                WriteColor(message, ConsoleColor.Green);
            }
            else
            {
                message = string.Concat("Playing: ", defaultLayout.CurrentSongPlaying, " - Paused");
                WriteColor(message, ConsoleColor.Red);
            }
        }

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
        
        if (editSonglistLayout == null)
        {
            editSonglistLayout = new();
            editSonglistLayout.Init();
        }

        creatingPlaylist = true;

        while(creatingPlaylist)
        {
            Console.Clear();
            Render(editSonglistLayout);
            switch(Console.ReadKey().Key)
                {
                    case ConsoleKey.Enter:
                    editSonglistLayout.Confirm();
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
                    editSonglistLayout.Add();
                    Console.Clear();
                    break;

                    case ConsoleKey.UpArrow:
                    editSonglistLayout.Selector(-1);
                    Console.Clear();
                    break;

                    case ConsoleKey.DownArrow:
                    editSonglistLayout.Selector(1);
                    Console.Clear();
                    break;

                    case ConsoleKey.P:
                    defaultLayout.Pause();
                    Console.Clear();
                    break;

                    case ConsoleKey.S:
                        Console.Clear();
                        Console.WriteLine("Playlist Name");
                        string playlistname = Console.ReadLine();
                        editSonglistLayout.SetName(playlistname);
                        Console.Clear();
                    break;

                    default:
                        Console.Clear();
                    break;
                }
        }
        editSonglistLayout = null;
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
                    editSonglistLayout = new();
                    editSonglistLayout.Init(songListLayout.SonglistForEdit);
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
        defaultLayout.Dispose();
    }
}
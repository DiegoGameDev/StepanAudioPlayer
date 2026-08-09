using System;
using System.IO;

public static class AppLauncher
{
    public static void Launch()
    {
        string roaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string basePath = Path.Combine(roaming, "Stepan");

        // Pastas
        string folderMusic = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), "StepanMusics");
        string folderPattern = Path.Combine(basePath, "StepanPattern");
        string folderPlayer = Path.Combine(basePath, "StepanAudioPlayer");
        string folderPlaylists = Path.Combine(basePath, "StepanPlayLists");

        // Arquivos + conteúdo padrão
        string configPath = Path.Combine(folderPlayer, "config.stpc");
        string configContent = "{ \"volume\": 100 }";

        // ===== StepanPattern (dependências) =====
        string musicScreenPath = Path.Combine(folderPattern, "defaultLayout.stp");
        string musicScreenContent =
    @"Your Songs
    -

(Musicas)

Comands:
Pause/Resume (P) - Play (ENTER) - Selection(Up_ARROW, Down_ARROW) - Change Playlist(S) - Search Songs(F5)

StepanPlayer  -version 1.1  2026
Diego Santana
";

        string addMusicScreenPath = Path.Combine(folderPattern, "SongListEditLayout.stp");
        string addMusicScreenContent =
    @"Stepan Musics

(Options)

Commands:
Add Song(SPACE) - Set PlayList Name(S) - Confirm(ENTER)

StepanPlayer  -version 1.1  2026
";

        string playlistsScreenPath = Path.Combine(folderPattern, "SongListLayout.stp");
        string playlistsScreenContent =
    @"Stepan Playlists

(Options)

Commands:
Select(ENTER) - Edit Playlist(E) - Remove( 2 * BACKSPACE)

StepanPlayer  -version 1.1  2026
";

        bool needsSetup = false;

        // Criar pastas
        if (!Directory.Exists(folderMusic))
        {
            Directory.CreateDirectory(folderMusic);
            needsSetup = true;    
        }

        if (!Directory.Exists(folderPattern))
        {
            Directory.CreateDirectory(folderPattern);
            needsSetup = true;
        }

        if (!Directory.Exists(folderPlayer))
        {
            Directory.CreateDirectory(folderPlayer);
            needsSetup = true;
        }

        if (!Directory.Exists(folderPlaylists))
        {
            Directory.CreateDirectory(folderPlaylists);
            needsSetup = true;
        }

        // Arquivos principais
        if (!File.Exists(configPath))
        {
            File.WriteAllText(configPath, configContent);
            needsSetup = true;
        }

        // ===== Verificação de integridade StepanPattern =====
        if (!File.Exists(musicScreenPath))
        {
            File.WriteAllText(musicScreenPath, musicScreenContent);
            needsSetup = true;
        }

        if (!File.Exists(addMusicScreenPath))
        {
            File.WriteAllText(addMusicScreenPath, addMusicScreenContent);
            needsSetup = true;
        }

        if (!File.Exists(playlistsScreenPath))
        {
            File.WriteAllText(playlistsScreenPath, playlistsScreenContent);
            needsSetup = true;
        }

        string line = Environment.NewLine + Environment.NewLine;

        // Feedback
        if (needsSetup)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(string.Concat("Enviroment fixed.", line));
        }

        Console.ResetColor();
    }
}
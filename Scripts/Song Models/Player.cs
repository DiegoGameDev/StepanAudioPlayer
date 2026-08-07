using NAudio.Wave;
using NAudio;
using Stepan.Models;
using Newtonsoft.Json;

namespace Stepan.Song;

public class Player : IDisposable
{
    public static Player instance;

    private readonly AudioFileReader audio;
    private readonly WaveOutEvent outPutDevice;
    private bool isManualStop = false;

    PlayerConfig playerConfig;

    public Player(string audioPath)
    {
        instance?.audio?.Dispose();
        instance?.outPutDevice?.Stop();
        instance?.outPutDevice?.Dispose();

        audio = new AudioFileReader(audioPath);
        using(outPutDevice = new WaveOutEvent());

        outPutDevice.PlaybackStopped += FinishedMusic;
        outPutDevice.Init(audio);
        instance = this;

        string roaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string path = Path.Combine(roaming, "Stepan", "StepanAudioPlayer", "config.stpc");

        var json = File.ReadAllText(path);

        playerConfig = JsonConvert.DeserializeObject<PlayerConfig>(json);
        playerConfig.volume = Math.Clamp(playerConfig.volume, 0, 100);

        outPutDevice.Volume = playerConfig.volume / 100;
    }

    public void Play()
    {
        outPutDevice.Play();
    }

    public void Pause()
    {
        outPutDevice.Pause();
    }

    public void Stop()
    {
        isManualStop = true;
        outPutDevice.Stop();
    }

    private void FinishedMusic(object? sender, StoppedEventArgs e)
    {
        if (e.Exception != null)
        {
            Console.Write("erro");
            return;
        }

        if (isManualStop)
        {
            isManualStop = false;
            return;
        }

        if (instance.audio.CurrentTime - instance.audio.TotalTime < TimeSpan.FromMilliseconds(200))
        {
            Layout.CurrentLayout.OnMusicFinished();
        }
    }

    public void Dispose()
    {
        Program.isPlaying = false;

        audio?.Dispose();
        outPutDevice?.Stop();
        outPutDevice?.Dispose();
    }
}
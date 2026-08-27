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

    public float Volume => playerConfig.volume;
    PlayerConfig playerConfig;

    public Player(string audioPath)
    {
        instance?.audio?.Dispose();
        instance?.outPutDevice?.Stop();
        instance?.outPutDevice?.Dispose();

        audio = new AudioFileReader(audioPath);
        using(outPutDevice = new WaveOutEvent());

        outPutDevice.PlaybackStopped += FinishedMusic;
        
        string roaming = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
        
        #if DEBUG
        roaming = Path.Combine(Directory.GetCurrentDirectory());
            #endif
        string path = Path.Combine(roaming, "Stepan", "StepanAudioPlayer", "config.stpc");

        var json = File.ReadAllText(path);

        playerConfig = JsonConvert.DeserializeObject<PlayerConfig>(json);
        playerConfig.volume = Math.Clamp(playerConfig.volume, 0, 100);
        audio.Volume = playerConfig.volume / 100;

        outPutDevice.Init(audio);
        instance = this;
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

    public void IncrementVolume(bool increment)
    {
        float value = increment ? playerConfig.IncrementVolumeValue : playerConfig.IncrementVolumeValue * -1; 
        playerConfig.volume = Math.Clamp(playerConfig.volume + value, 0, 100);
        outPutDevice.Volume = playerConfig.volume / 100;

        string roaming = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
        
        #if DEBUG
        roaming = Path.Combine(Directory.GetCurrentDirectory());
            #endif
        string path = Path.Combine(roaming, "Stepan", "StepanAudioPlayer", "config.stpc");

        string json = JsonConvert.SerializeObject(playerConfig);
        File.WriteAllText(path, json);
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
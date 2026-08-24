using System.Collections.Generic;
using System.IO;
using Godot;

public partial class SoundManager : Node
{

    const int MAX_ADUIO_PLAYERS = 10;

    #region Singleton

    public static SoundManager Instance {private set; get;}
    public void MakeThisInstance()
    {
        Logger.Success("Ready");
        Instance = this;
    }

    #endregion

    private List<AudioStreamPlayer> _audioStreamPlayerBuffer = new();
    private Dictionary<string, AudioStream> _registeredStreams = new();
    private Dictionary<AudioStreamPlayer, bool> _loopingPlayers = new();

    public override void _Ready()
    {
        MakeThisInstance();

        // Fill buffer
        for(int i=0; i<MAX_ADUIO_PLAYERS; i++)
        {
            var audioStreamPlayer = new AudioStreamPlayer();
            AddChild(audioStreamPlayer);
            audioStreamPlayer.Finished += () => OnPlayerFinished(audioStreamPlayer);
            _audioStreamPlayerBuffer.Add(audioStreamPlayer);
        }
    }

    private void OnPlayerFinished(AudioStreamPlayer player)
    {
        if(_loopingPlayers.TryGetValue(player, out var looping) && looping)
        {
            player.Play();
        }
    }

    #region Public API

    public static void RegisterStream(string key, string streamPath)
    {
        var stream = GD.Load<AudioStream>(streamPath);
        Instance._registeredStreams[key] = stream;
        Logger.Info($"Registered AudioStream: {key} from {streamPath}");
    }

    public static void RegisterStreams(Dictionary<string, string> streams)
    {
        foreach((string id, string path) in streams)
            RegisterStream(id, path);
    }

    public static void PlaySound(string key, bool loop = false, float speed = 1f)
    {
        if(!Instance._registeredStreams.TryGetValue(key, out var stream))
        {
            Logger.Warn($"No stream registered for key '{key}'");
            return;
        }

        var player = Instance._audioStreamPlayerBuffer.Find(p => !p.Playing);
        if(player == null)
        {
            Logger.Warn($"All audio players in use, discarding sound '{key}'");
            return;
        }

        Instance._loopingPlayers[player] = loop;
        player.Stream = stream;
        player.PitchScale = speed;
        player.Play();

        Logger.Info($"Playing sound: {key} looping: {loop} speed: {speed}");
    }

    public static void StopAllSounds()
    {
        foreach(var player in Instance._audioStreamPlayerBuffer)
        {
            Instance._loopingPlayers[player] = false;
            player.Stop();
        }

        Logger.Info("Stopped all sounds");
    }

    #endregion


}

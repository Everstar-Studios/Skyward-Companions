using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class AudioInstance
{
    private EventInstance instance;

    public AudioInstance(AudioAsset audioAsset, GameObject gameObject)
    {
        instance = RuntimeManager.CreateInstance(audioAsset.SoundRef);
        RuntimeManager.AttachInstanceToGameObject(instance, gameObject);
    }

    public bool IsPlaying()
    {
        instance.getPlaybackState(out PLAYBACK_STATE state);
        return state == PLAYBACK_STATE.PLAYING;
    }

    public void Play()
    {
        instance.start();
    }

    public static AudioInstance Create(AudioAsset audioAsset, GameObject gameObject)
    {
        return new AudioInstance(audioAsset, gameObject);
    }

    public void Stop()
    {
        instance.stop(STOP_MODE.IMMEDIATE);
    }

    public void Release()
    {
        instance.release();
    }
}
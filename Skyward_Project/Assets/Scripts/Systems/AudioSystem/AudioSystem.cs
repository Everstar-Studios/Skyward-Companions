using UnityEngine;
using System.Collections.Generic;
using Skyward.Core;
using FMOD.Studio;
using FMODUnity;

[RequiredSystem]
public class AudioSystem : BaseSystem<AudioSystem>, ISkywardComponent
{
    private List<AudioInstance> audioInstances = new();

    private Bus sfxBus;
    private Bus musicBus;
    private Bus cutsceneBus;
    
    protected override void Initialize(GameContext context)
    {
        base.Initialize(context);
        
        sfxBus = RuntimeManager.GetBus("bus:/SFX");
        musicBus = RuntimeManager.GetBus("bus:/Music");
        cutsceneBus = RuntimeManager.GetBus("bus:/Cutscene");

        float savedSFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        float savedMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);

        SetSoundEffectsVolume(savedSFXVolume);
        SetMusicVolume(savedMusicVolume);
    }

    protected override void Cleanup()
    {
        base.Cleanup();
        
        foreach (var instance in audioInstances)
        {
            instance.Stop();
            instance.Release();
        }
        
        audioInstances.Clear();
    }
    
    public static void PlayOneShot(AudioAsset sound, Vector3 worldPosition = default)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            EditorUtils.LoadPreviewBanks();
            EditorUtils.System.getEventByID(sound.SoundRef.Guid, out var eventDescription);
            eventDescription.createInstance(out var eventInstance);
        
            eventInstance.start();
            return;
        }

#endif
        
        RuntimeManager.PlayOneShot(sound.SoundRef, worldPosition);
    }
    
    public static AudioInstance CreateAudioInstance(AudioAsset audioAsset, GameObject gameObject)
    {
        if (audioAsset == null)
            return null;
        
        Instance.audioInstances ??= new List<AudioInstance>();
        
        var instance = AudioInstance.Create(audioAsset, gameObject);
        Instance.audioInstances.Add(instance);
        return instance;
    }

    public static bool TryCreateAudioInstance(AudioAsset audioAsset, GameObject gameObject,
        out AudioInstance audioInstance)
    {
        audioInstance = CreateAudioInstance(audioAsset, gameObject);
        return audioInstance != null;
    }

    public static void ReleaseInstance(ref AudioInstance audioInstance)
    {
        audioInstance.Release();
        Instance.audioInstances.Remove(audioInstance);
        audioInstance = null;
    }

    public static void PauseMusic()
    {
        Instance.musicBus.setPaused(true);
    }
    
    public static void UnpauseMusic()
    {
        Instance.musicBus.setPaused(false);
    }
    
    private void AdjustVolume(Bus bus, float volume)
    {
        float newVolume = Mathf.Clamp01(volume);
        bus.setVolume(newVolume);
    }

    public static void SetMusicVolume(float value)
    {
        Instance.AdjustVolume(Instance.musicBus, value);
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public static void SetSoundEffectsVolume(float value)
    {
        Instance.AdjustVolume(Instance.sfxBus, value);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }
    
    public static void SetCutsceneVolume(float value)
    {
        Instance.AdjustVolume(Instance.cutsceneBus, value);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    public static float GetMusicVolume()
    {
        Instance.musicBus.getVolume(out float volume);
        return volume;
    }
}

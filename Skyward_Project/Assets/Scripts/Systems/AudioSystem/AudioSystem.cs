using System;
using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using Skyward.Core;
using FMOD.Studio;
using FMODUnity;
using Random = UnityEngine.Random;

[Serializable]
public struct AudioData
{
    public AudioClip clip;
    public AudioMixer mixer;
}

public enum SoundType
{
    Music,
    SFX
}

[RequiredSystem]
public class AudioSystem : BaseSystem<AudioSystem>, ISkywardComponent
{
    private List<AudioInstance> audioInstances;
    [Header("Mixer")]
    public AudioMixer audioMixer;
    
    private Bus sfxBus;
    private Bus ambienceBus;

    public static float SoundEffectsVolume { get; private set; } = 1f;
    
    protected override void Initialize(GameContext context)
    {
        base.Initialize(context);
        
        sfxBus = RuntimeManager.GetBus("bus:/SFX");
        ambienceBus = RuntimeManager.GetBus("bus:/Music");

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

    public static void Play(AudioAsset audioAsset)
    {
        
    }
    
    public static void PlayRandomSFX(AudioClip[] clips)
    {
        var clip = clips[Random.Range(0, clips.Length)];
        //Play(clip, SoundType.SFX);
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
        Instance.audioInstances ??= new List<AudioInstance>();
        
        var instance = AudioInstance.Create(audioAsset, gameObject);
        Instance.audioInstances.Add(instance);
        return instance;
    }

    public static void ReleaseInstance(ref AudioInstance audioInstance)
    {
        audioInstance.Release();
        Instance.audioInstances.Remove(audioInstance);
        audioInstance = null;
    }

    // public static void Pause()
    // {
    //     if (Instance == null)
    //         return;
    //
    //     if (Instance.musicSource != null)
    //         Instance.musicSource.Pause();
    //
    //     if (Instance.sfxSource != null)
    //         Instance.sfxSource.Pause();
    //
    //     foreach (var ambient in ComponentSystem.GetAllComponents<AmbientSoundPlayer>())
    //         ambient.Pause();
    // }
    //
    // public static void Unpause()
    // {
    //     if (Instance == null)
    //         return;
    //
    //     if (Instance.musicSource != null)
    //         Instance.musicSource.UnPause();
    //
    //     if (Instance.sfxSource != null)
    //         Instance.sfxSource.UnPause();
    //
    //     foreach (var ambient in ComponentSystem.GetAllComponents<AmbientSoundPlayer>())
    //         ambient.Unpause();
    // }
    //
    // public void StopMusic()
    // {
    //     if (musicSource.isPlaying)
    //         musicSource.Stop();
    // }

    private void AdjustVolume(Bus bus, float volume)
    {
        float newVolume = Mathf.Clamp01(volume);
        bus.setVolume(newVolume);
    }

    public void SetMusicVolume(float value)
    {
        AdjustVolume(ambienceBus, value);
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetSoundEffectsVolume(float value)
    {
        SoundEffectsVolume = value;
        float db = value <= 0.0001f ? -80f : Mathf.Log10(value) * 20f;
        audioMixer.SetFloat("SFXVolume", db);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }
    
    public AudioMixerGroup GetMusicMixerGroup()
    {
        return audioMixer.FindMatchingGroups("Music")[0];
    }
}

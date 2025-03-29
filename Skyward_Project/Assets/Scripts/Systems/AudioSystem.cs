using System;
using System.Collections.Generic;
using Skyward.Core;
using UnityEngine;

[RequiredSystem]
public class AudioSystem : BaseSystem<AudioSystem>, ISkywardComponent
{
    private AudioSource audioSource;

    protected override void Awake()
    {
        base.Awake();

        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public static void Play(AudioClip clip, float volume = 1f)
    {
        Instance.audioSource.PlayOneShot(clip, volume);
    }

    public static void PlayRandom(AudioClip[] clips)
    {
        var clip = clips[UnityEngine.Random.Range(0, clips.Length)];
        Play(clip, 1f);
    }

    public static void Stop()
    {
        Instance.audioSource.Stop();
    }

    public static void Pause()
    {
        Instance.audioSource.Pause();
        foreach (var ambient in ComponentSystem.GetAllComponents<AmbientSoundPlayer>())
            ambient.Pause();
    }
    
    public static void Unpause()
    {
        Instance.audioSource.UnPause();
        foreach (var ambient in ComponentSystem.GetAllComponents<AmbientSoundPlayer>())
            ambient.Unpause();
    }
}

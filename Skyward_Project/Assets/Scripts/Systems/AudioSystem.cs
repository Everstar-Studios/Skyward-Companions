using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using Skyward.Core;

[RequiredSystem]
public class AudioSystem : BaseSystem<AudioSystem>, ISkywardComponent
{
    [Header("Mixer")]
    public AudioMixer audioMixer;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Sounds")]
    public List<Sound> sounds; // Inspector'dan doldurulacak

    public static float SoundEffectsVolume { get; private set; } = 1f;

    private void Start()
    {
        Play("MainMenu");
    }

    protected override void Awake()
    {
        base.Awake();

        // if (musicSource == null || sfxSource == null)
        // {
        //     Debug.LogError("AudioSources not assigned in inspector!");
        // }
    }

    // Yeni sistem: isme göre oynat
    public void Play(string name, float volume = 1f)
    {
        Sound sound = sounds.Find(s => s.name == name);
        if (sound == null)
        {
            Debug.LogWarning("Sound not found: " + name);
            return;
        }

        switch (sound.type)
        {
            case SoundType.Music:
                musicSource.clip = sound.clip;
                musicSource.loop = true;
                musicSource.Play();
                break;

            case SoundType.SFX:
                sfxSource.PlayOneShot(sound.clip, volume * SoundEffectsVolume);
                break;
        }
    }

    // Geriye dönük destek: doğrudan AudioClip oynat (SFX olarak)
    public static void Play(AudioClip clip, float volume = 1f)
    {
        if (clip == null || Instance == null || Instance.sfxSource == null) return;
        Instance.sfxSource.PlayOneShot(clip, volume * SoundEffectsVolume);
    }

    // Geriye dönük destek: rastgele ses çal (SFX olarak)
    public static void PlayRandom(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return;
        var clip = clips[Random.Range(0, clips.Length)];
        Play(clip);
    }

    public static void Pause()
    {
        Instance.musicSource?.Pause();
        Instance.sfxSource?.Pause();
    }

    public static void Unpause()
    {
        Instance.musicSource?.UnPause();
        Instance.sfxSource?.UnPause();
    }

    public void StopMusic()
    {
        if (musicSource.isPlaying)
            musicSource.Stop();
    }

    public void SetMusicVolume(float value)
    {
        float db = value <= 0.0001f ? -80f : Mathf.Log10(value) * 20f;
        audioMixer.SetFloat("MusicVolume", db);
    }

    public void SetSoundEffectsVolume(float value)
    {
        SoundEffectsVolume = value;
        float db = value <= 0.0001f ? -80f : Mathf.Log10(value) * 20f;
        audioMixer.SetFloat("SFXVolume", db);
    }
}

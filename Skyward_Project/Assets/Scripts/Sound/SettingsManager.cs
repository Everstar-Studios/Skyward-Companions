using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider cutsceneVolumeSlider;
    [SerializeField] private Slider sensitivitySlider;

    private IEnumerator Start()
    {
        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 1f);
        float cutsceneVol = PlayerPrefs.GetFloat("CutsceneVolume", 1f);

        musicVolumeSlider.value = musicVol;
        sfxVolumeSlider.value = sfxVol;
        cutsceneVolumeSlider.value = cutsceneVol;

        yield return new WaitUntil(() => AudioSystem.Instance != null);
        AudioSystem.SetMusicVolume(musicVol);
        AudioSystem.SetSoundEffectsVolume(sfxVol);
        AudioSystem.SetCutsceneVolume(cutsceneVol);

        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        cutsceneVolumeSlider.onValueChanged.AddListener(OnCutsceneVolumeChanged);
        sensitivitySlider.onValueChanged.AddListener(OnCameraSensitivityChanged);
    }

    private void OnEnable()
    {
        sensitivitySlider.value = Settings.Sensitivity;
    }

    private void OnCameraSensitivityChanged(float value)
    {
        Settings.Sensitivity = value;
    }

    private void OnMusicVolumeChanged(float value)
    {
        AudioSystem.SetMusicVolume(value);
    }

    private void OnSFXVolumeChanged(float value)
    {
        AudioSystem.SetSoundEffectsVolume(value);
    }
    
    private void OnCutsceneVolumeChanged(float value)
    {
        AudioSystem.SetCutsceneVolume(value);
    }
}

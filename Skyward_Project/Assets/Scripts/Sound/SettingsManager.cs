using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider sensitivitySlider;

    private IEnumerator Start()
    {
        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 1f);

        musicVolumeSlider.value = musicVol;
        sfxVolumeSlider.value = sfxVol;

        yield return new WaitUntil(() => AudioSystem.Instance != null);
        ApplyMusicVolume(musicVol);
        ApplySFXVolume(sfxVol);

        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
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
        ApplyMusicVolume(value);
    }

    private void OnSFXVolumeChanged(float value)
    {
        ApplySFXVolume(value);
    }

    private void ApplyMusicVolume(float value)
    {
        AudioSystem.Instance.SetMusicVolume(value);
    }

    private void ApplySFXVolume(float value)
    {
        AudioSystem.Instance.SetSoundEffectsVolume(value);
    }
}

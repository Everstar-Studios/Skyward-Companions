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
    [SerializeField] private Dropdown frameRateDropdown;

    private IEnumerator Start()
    {
        int frameRate = PlayerPrefs.GetInt("FrameRate", 60);
        Application.targetFrameRate = frameRate;
        frameRateDropdown.value = GetDropdownOrderFromFPS(frameRate);

        musicVolumeSlider.value = AudioSystem.GetMusicVolume();
        sfxVolumeSlider.value = AudioSystem.GetSfxVolume();
        cutsceneVolumeSlider.value = AudioSystem.GetCutsceneVolume();

        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        cutsceneVolumeSlider.onValueChanged.AddListener(OnCutsceneVolumeChanged);
        sensitivitySlider.onValueChanged.AddListener(OnCameraSensitivityChanged);
        frameRateDropdown.onValueChanged.AddListener(OnFrameRateChanged);
        
        yield return new WaitUntil(() => AudioSystem.Instance != null);
    }

    private void OnDestroy()
    {
        musicVolumeSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
        sfxVolumeSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);
        cutsceneVolumeSlider.onValueChanged.RemoveListener(OnCutsceneVolumeChanged);
        sensitivitySlider.onValueChanged.RemoveListener(OnCameraSensitivityChanged);
        frameRateDropdown.onValueChanged.RemoveListener(OnFrameRateChanged);
    }

    private void OnFrameRateChanged(int order)
    {
        int frameRate = 60;
        if (order == 0)
            frameRate = 30;
        else if (order == 1)
            frameRate = 60;
        else if (order == 2)
            frameRate = 120;

        Application.targetFrameRate = frameRate;

        PlayerPrefs.SetInt("FrameRate", frameRate);
    }

    private int GetDropdownOrderFromFPS(int frameRate)
    {
        if (frameRate == 30)
            return 0;
        if (frameRate == 60)
            return 1;
        if (frameRate == 120)
            return 2;

        return 2;
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

using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    private void Start()
    {
        // Kaydedilen ayarları yükle
        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 1f);

        musicVolumeSlider.value = musicVol;
        sfxVolumeSlider.value = sfxVol;

        ApplyMusicVolume(musicVol);
        ApplySFXVolume(sfxVol);

        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }

    private void OnMusicVolumeChanged(float value)
    {
        ApplyMusicVolume(value);
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    private void OnSFXVolumeChanged(float value)
    {
        ApplySFXVolume(value);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    private void ApplyMusicVolume(float value)
    {
        AudioSystem.Instance?.SetMusicVolume(value);
    }

    private void ApplySFXVolume(float value)
    {
        AudioSystem.Instance?.SetSoundEffectsVolume(value);
    }
}

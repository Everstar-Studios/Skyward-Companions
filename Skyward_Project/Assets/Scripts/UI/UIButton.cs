using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButton : MonoBehaviour
{
    public AudioClip clickSound;
    protected Button button;
    protected virtual void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnClick);
    }

    public virtual void OnClick()
    {
        AudioClip clickSoundSFX = clickSound != null ? clickSound : Configs.UIConfig.defaultButtonClickSound;
        AudioSystem.Play(clickSoundSFX);
    }
}

using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButton : MonoBehaviour
{
    public AudioAsset audioAsset;
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
        AudioAsset clickSoundSFX = audioAsset != null ? audioAsset : ConfigSystem.GetConfig<UIConfig>().defaultButtonClickSound;
        clickSoundSFX.Play();
    }
}

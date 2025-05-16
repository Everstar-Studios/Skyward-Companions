using System;
using Skyward.Core;
using TMPro;
using UnityEngine;

public class UIHeightIndicator : MonoBehaviour, ISkywardComponent
{
    [SerializeField]
    private TMP_Text textComponent;

    void ISkywardComponent.WorldLoaded(GameContext context)
    {
        if (HeightSystem.Instance == null)
            gameObject.SetActive(false);
    }

    private void Update()
    {
        int height = Mathf.CeilToInt(HeightSystem.Height);
        textComponent.text = $"{height} M";
    }
}

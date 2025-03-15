using System;
using Skyward.Core;
using TMPro;
using UnityEngine;

public class UIHeightIndicator : MonoBehaviour, ISkywardComponent
{
    [SerializeField]
    private TMP_Text textComponent;

    private void Update()
    {
        int height = Mathf.CeilToInt(HeightSystem.Height);
        textComponent.text = $"{height} M";
    }
}

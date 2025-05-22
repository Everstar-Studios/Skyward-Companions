using System;
using Skyward.Core;
using Skyward.Systems;
using TMPro;
using UnityEngine;

public class UILevelEnd : MonoBehaviour
{
    [SerializeField] private TMP_Text timeText;
    private void OnEnable()
    {
        TimeSpan timeSpan = TimeSystem.TimeSpan;
        string timeFormatted = $"Time: {timeSpan.Minutes}:{timeSpan.Seconds:D2}";
        timeText.text = timeFormatted;
    }
}

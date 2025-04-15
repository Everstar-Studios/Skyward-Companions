using System;
using Skyward.Systems;
using TMPro;
using UnityEngine;

namespace Skyward.UI
{
    public class UITimeManager : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text timeText;

        private void Update()
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(TimeSystem.LevelTimer);
            string timeFormatted = $"{timeSpan.Minutes}:{timeSpan.Seconds:D2}";
            timeText.text = timeFormatted;
        }
    }
}

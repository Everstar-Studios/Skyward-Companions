using System;
using System.Globalization;
using TMPro;
using Unity.Services.Leaderboards.Models;
using UnityEngine;

public class LeaderboardPlayerItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI timeText;
    
    private LeaderboardEntry player = null;

    public void Initialize(LeaderboardEntry entry)
    {
        player = entry;
        rankText.text = (player.Rank + 1).ToString();
        nameText.text = entry.PlayerName.Split('#')[0];
        var timeSpan = TimeSpan.FromSeconds(player.Score);
        string timeFormatted = $"{timeSpan.Minutes}:{timeSpan.Seconds:D2}";
        timeText.text = timeFormatted;
    }
}

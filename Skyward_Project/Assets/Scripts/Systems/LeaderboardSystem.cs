using System;
using Newtonsoft.Json;
using Skyward.Core;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using UnityEngine;

[Serializable]
public class ScoreMetadata
{
    public string levelName;
    public float height;
}

[RequiredSystem]
public class LeaderboardSystem : BaseSystem<LeaderboardSystem>
{
    private async void Start()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        
        var scoreResponse = await LeaderboardsService.Instance.AddPlayerScoreAsync("Skyward-Leaderboard", 102);
    }

    public async void AddScoreWithMetadata(string leaderboardId, float score)
    {
        // var scoreMetadata = new ScoreMetadata { levelName = "LEVEL_01", height = 120 };
        // var playerEntry = await LeaderboardsService.Instance
        //     .AddPlayerScoreAsync(
        //         leaderboardId,
        //         score,
        //         new AddPlayerScoreOptions { Metadata = scoreMetadata }
        //     );
        // Debug.Log(JsonConvert.SerializeObject(playerEntry));
    }
}

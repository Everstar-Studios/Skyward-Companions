using System;
using Newtonsoft.Json;
using Skyward.Core;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using UnityEngine;
using Random = System.Random;

[Serializable]
public class ScoreMetadata
{
    public string levelName;
    public float timeTaken;
}

[RequiredSystem]
public class LeaderboardSystem : BaseSystem<LeaderboardSystem>
{
    private async void Start()
    {
        if (UnityServices.State == ServicesInitializationState.Initialized)
            return;
        
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    protected override void Preload(GameContext context)
    {
        base.Preload(context);
        
        
    }

    protected override void WorldLoading(GameContext context)
    {
        base.WorldLoading(context);

        GameSystem.LevelCompleted += OnLevelCompleted;
    }

    protected override void Cleanup()
    {
        base.Cleanup();
        
        GameSystem.LevelCompleted-= OnLevelCompleted;
    }

    private async void OnLevelCompleted(object sender, EventArgs args)
    {
        var options = new AddPlayerScoreOptions()
        {
            Metadata = new ScoreMetadata { levelName = GameSystem.GetCurrentLevelName() + UnityEngine.Random.Range(0f, 5f), timeTaken = Time.timeSinceLevelLoad}
        };
        await LeaderboardsService.Instance.AddPlayerScoreAsync("Skyward-Leaderboard", Time.timeSinceLevelLoad, options);
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

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
    public string playerName;
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

    protected override void WorldLoading(GameContext context)
    {
        base.WorldLoading(context);

        GameSystem.LevelCompleted += OnLevelCompleted;
    }

    protected override void Cleanup()
    {
        base.Cleanup();
        
        GameSystem.LevelCompleted -= OnLevelCompleted;
    }

    private async void OnLevelCompleted(object sender, GameSystem.LevelEndEventArgs args)
    {
        string levelName = GameSystem.GetCurrentLevelName();
        string leaderboardId = $"Skyward-{levelName}";
        await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardId, args.time);
    }
}

using System;
using Skyward.Core;
using Skyward.Systems;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequiredSystem]
public class GameSystem : BaseSystem<GameSystem>
{
    private SkywardGame gameInstance;
    
    public static event EventHandler LevelCompleted
    {
        add => Instance.levelCompleted += value;
        remove => Instance.levelCompleted -= value;
    }

    private event EventHandler levelCompleted;
    protected override void Initialize(GameContext context)
    {
        base.Initialize(context);

        gameInstance = Instance.GameContext.game;
    }

    public static void LaunchLevel(int sceneIndex)
    {
        Instance.GameContext.game.LaunchLevel(sceneIndex);
    }

    public static void OnLevelCompleted()
    {
        GameInputSystem.DisableInput();
        Instance.levelCompleted?.Invoke(Instance, EventArgs.Empty);
        Instance.gameInstance.OnLevelCompleted();
    }
}

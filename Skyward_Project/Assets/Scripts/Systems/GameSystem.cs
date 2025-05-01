using System;
using System.Collections.Generic;
using System.IO;
using Skyward.Core;
using Skyward.Systems;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequiredSystem]
public class GameSystem : BaseSystem<GameSystem>
{
    private SkywardGame gameInstance;

    private SceneInfo sceneInfo = new();
    // TODO Omer: Send time
    public static event EventHandler LevelCompleted
    {
        add => Instance.levelCompleted += value;
        remove => Instance.levelCompleted -= value;
    }

    private event EventHandler levelCompleted;

    protected override void Initialize(GameContext context)
    {
        base.Initialize(context);

        context.Store(sceneInfo);
    }

    protected override void WorldLoading(GameContext context)
    {
        base.WorldLoading(context);

        gameInstance = Instance.GameContext.game;
    }

    public static void LaunchLevel(string sceneName)
    {
        Instance.GameContext.game.LaunchLevel(sceneName);
    }

    public static void OnLevelCompleted()
    {
        Instance.sceneInfo.MarkComplete();
        GameInputSystem.DisableInput();
        Instance.levelCompleted?.Invoke(Instance, EventArgs.Empty);
        MainMenu();
    }

    public static void Quit()
    {
        Instance.GameContext.game.Quit();
    }
    
    public static void MainMenu()
    {
        Quit();
        SceneManager.LoadScene("SCN_Lobby");
    }

    public static string GetCurrentLevelName()
    {
        return SceneManager.GetActiveScene().name;
    }

    private class SceneInfo : ISkywardSerializable
    {
        private int maxUnlockedLevelIndex;
        public void Serialize()
        {
            PlayerPrefs.SetInt("MaxUnlockedLevelIndex", maxUnlockedLevelIndex);
        }

        public void Deserialize()
        {
            maxUnlockedLevelIndex = PlayerPrefs.GetInt("MaxUnlockedLevelIndex");
        }
        
        public void MarkComplete()
        {
            if (TryGetNextSceneName(out int maxUnlockedIndex))
            {
                maxUnlockedLevelIndex = maxUnlockedIndex;
                PlayerPrefs.SetInt("MaxUnlockedLevelIndex", maxUnlockedLevelIndex);
            }
        }
        
        private bool TryGetNextSceneName(out int nextSceneIndex)
        {
            int currentIndex = SceneManager.GetActiveScene().buildIndex;
            nextSceneIndex = currentIndex + 1;
            return nextSceneIndex < SceneManager.sceneCountInBuildSettings;
        }
        
        public bool IsUnlocked(int index) => index <= maxUnlockedLevelIndex;
    }

    public static bool IsLevelUnlocked(int index)
    {
        return Instance.sceneInfo.IsUnlocked(index);
    }
}

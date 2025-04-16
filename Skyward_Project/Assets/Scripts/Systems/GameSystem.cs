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
        
        context.Store("Scene", sceneInfo);
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
        private HashSet<string> unlockedLevels = new();
        public void Serialize(BinaryWriter writer)
        {
            writer.Write(unlockedLevels.Count);
            foreach (var level in unlockedLevels)
                writer.Write(level);
        }

        public void Deserialize(BinaryReader reader)
        {
            int completedLevelCount = reader.ReadInt32();
            for (int i = 0; i < completedLevelCount; i++)
            {
                string completedLevelName = reader.ReadString();
                unlockedLevels.Add(completedLevelName);
            }
        }
        
        public void MarkComplete()
        {
            if (TryGetNextSceneName(out string nextSceneName))
                unlockedLevels.Add(nextSceneName);
            
            LeaderboardSystem.Instance.AddScoreWithMetadata("Skyward-Leaderboard", TimeSystem.TimeInLevel);
        }
        
        private bool TryGetNextSceneName(out string nextSceneName)
        {
            int currentIndex = SceneManager.GetActiveScene().buildIndex;
            int nextIndex = currentIndex + 1;
            nextSceneName = null;

            if (nextIndex < SceneManager.sceneCountInBuildSettings)
            {
                string path = SceneUtility.GetScenePathByBuildIndex(nextIndex);
                nextSceneName = Path.GetFileNameWithoutExtension(path);
                return true;
            }

            return false;
        }
        
        public bool IsUnlocked(string sceneName) => unlockedLevels.Contains(sceneName);
    }

    public static bool IsLevelUnlocked(string sceneName)
    {
        return Instance.sceneInfo.IsUnlocked(sceneName);
    }
}

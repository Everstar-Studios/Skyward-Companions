using System;
using System.Collections;
using Skyward.Core;
using Skyward.Systems;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

[RequiredSystem]
public class GameSystem : BaseSystem<GameSystem>
{
    private SkywardGame gameInstance;

    private SceneInfo sceneInfo = new();
    
    public static event EventHandler LevelDownloadFailed
    {
        add => Instance.levelDownloadFailed += value;
        remove => Instance.levelDownloadFailed -= value;
    }

    private event EventHandler levelDownloadFailed;
    
    public static event EventHandler<float> LevelDownloading
    {
        add => Instance.levelDownloading += value;
        remove => Instance.levelDownloading -= value;
    }

    private event EventHandler<float> levelDownloading;
    
    public static event EventHandler LevelDownloaded
    {
        add => Instance.levelDownloaded += value;
        remove => Instance.levelDownloaded -= value;
    }

    private event EventHandler levelDownloaded;
    
    public static event EventHandler PreLevelLoad
    {
        add => Instance.preLevelLoad += value;
        remove => Instance.preLevelLoad -= value;
    }

    private event EventHandler preLevelLoad;
    
    public static event EventHandler<float> LevelLoading
    {
        add => Instance.levelLoading += value;
        remove => Instance.levelLoading -= value;
    }

    private event EventHandler<float> levelLoading;
    
    public static event EventHandler LevelLoaded
    {
        add => Instance.levelLoaded += value;
        remove => Instance.levelLoaded -= value;
    }

    private event EventHandler levelLoaded;
    
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

    public static void RequestLevelLaunch(string levelKey)
    {
        Instance.StartCoroutine(Instance.RequestLevelLaunchCoroutine(levelKey));
    }

    private IEnumerator RequestLevelLaunchCoroutine(string levelKey)
    {
        preLevelLoad?.Invoke(this, EventArgs.Empty);
        yield return new WaitForEndOfFrame();
        
        var sizeHandle = Addressables.GetDownloadSizeAsync(levelKey);
        yield return sizeHandle;
        
        if (sizeHandle.Status != AsyncOperationStatus.Succeeded)
        {
            levelDownloadFailed?.Invoke(this, EventArgs.Empty);
            Addressables.Release(sizeHandle);
            yield break;
        }

        long bytes = sizeHandle.Result;
        Addressables.Release(sizeHandle);
        
        if (bytes == 0)
        {
            yield return LaunchLevel(levelKey);
            yield break;
        }
        
        var downloadHandle = Addressables.DownloadDependenciesAsync(levelKey);
        while (!downloadHandle.IsDone)
        {
            var status = downloadHandle.GetDownloadStatus();
            levelDownloading?.Invoke(this, status.Percent);
            yield return null;
        }
        
        if (downloadHandle.Status != AsyncOperationStatus.Succeeded)
        {
            levelDownloadFailed?.Invoke(this, EventArgs.Empty);
            Addressables.Release(downloadHandle);
            yield break;
        }
        
        Addressables.Release(downloadHandle);
        levelDownloaded?.Invoke(this, EventArgs.Empty);
        
        yield return LaunchLevel(levelKey);
    }

    private IEnumerator LaunchLevel(string levelKey)
    {
        var levelHandle = Addressables.LoadSceneAsync(levelKey, LoadSceneMode.Additive);
        gamecontext.game.NotifyLevelLoading();
        while (!levelHandle.IsDone)
        {
            levelLoading?.Invoke(this, levelHandle.PercentComplete);
            yield return null;
        }
        
        levelLoaded?.Invoke(this, EventArgs.Empty);
        gamecontext.game.LevelLoadCompleted();
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

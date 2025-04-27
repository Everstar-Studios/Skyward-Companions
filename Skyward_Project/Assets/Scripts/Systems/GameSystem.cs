using System;
using System.Collections;
using Skyward.Core;
using Skyward.Systems;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

[RequiredSystem]
public class GameSystem : BaseSystem<GameSystem>
{
    private SceneInfo sceneInfo = new();
    private SceneInstance levelInstance;
    private bool isLoading;

    public static int LastUnlockedLevel => Instance.sceneInfo.LastUnlocked;
    
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
    
    private event EventHandler quitting;
    
    public static event EventHandler Quitting
    {
        add => Instance.quitting += value;
        remove => Instance.quitting -= value;
    }
    
    protected override void Initialize(GameContext context)
    {
        base.Initialize(context);

        context.Store(sceneInfo);
    }

    public static void RequestLevelLaunch(string levelKey)
    {
        if (Instance.isLoading)
            return;
        
        Instance.StartCoroutine(Instance.RequestLevelLaunchCoroutine(levelKey));
    }
    
    private IEnumerator RequestLevelLaunchCoroutine(string levelKey)
    {
        isLoading = true;
        preLevelLoad?.Invoke(this, EventArgs.Empty);
        yield return new WaitForEndOfFrame();
        
        var sizeHandle = Addressables.GetDownloadSizeAsync(levelKey);
        yield return sizeHandle;
        
        if (sizeHandle.Status != AsyncOperationStatus.Succeeded)
        {
            levelDownloadFailed?.Invoke(this, EventArgs.Empty);
            Addressables.Release(sizeHandle);
            isLoading = false;
            yield break;
        }

        long bytes = sizeHandle.Result;
        Addressables.Release(sizeHandle);
        
        float mb = bytes / (1024f * 1024f);
        Debug.Log($"Download size for '{levelKey}': {mb:0.#} MB");
        
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
            isLoading = false;
            yield break;
        }
        
        Addressables.Release(downloadHandle);
        levelDownloaded?.Invoke(this, EventArgs.Empty);
        
        yield return LaunchLevel(levelKey);
    }
    
    private IEnumerator LaunchLevel(string levelKey)
    {
        yield return new WaitForSeconds(1f);
        
        var levelHandle = Addressables.LoadSceneAsync(levelKey, LoadSceneMode.Additive);
        gamecontext.game.NotifyLevelLoading();
        while (!levelHandle.IsDone)
        {
            levelLoading?.Invoke(this, levelHandle.PercentComplete);
            yield return null;
        }

        isLoading = false;
        if (levelHandle.Status != AsyncOperationStatus.Succeeded)
            throw levelHandle.OperationException;

        levelInstance = levelHandle.Result;
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
        Instance.quitting?.Invoke(Instance, EventArgs.Empty);
    }
    
    public static void MainMenu()
    {
        Quit();
        Addressables.UnloadSceneAsync(Instance.levelInstance);
    }

    public static string GetCurrentLevelName()
    {
        return SceneManager.GetActiveScene().name;
    }

    private class SceneInfo : ISkywardSerializable
    {
        private int maxUnlockedLevelIndex;
        private int currentLevelIndex;
        
        private const string Key = "LastUnlockedLevel";
        public void Serialize()
        {
            if (maxUnlockedLevelIndex > 0)
                PlayerPrefs.SetInt(Key, maxUnlockedLevelIndex);
        }

        public void Deserialize()
        {
            maxUnlockedLevelIndex = PlayerPrefs.GetInt(Key, 1);
        }
        
        public int LastUnlocked
        {
            get => PlayerPrefs.GetInt(Key, 1);
            private set { PlayerPrefs.SetInt(Key, value); PlayerPrefs.Save(); }
        }

        public void Unlock(int levelIndex)
        {
            if (levelIndex > LastUnlocked) LastUnlocked = levelIndex;
        }
        
        private static int FindIndex(string activeSceneName)
        {
            var levels = ConfigSystem.GetConfig<LevelConfig>().levels;
            for (int i = 0; i < levels.Length; i++)
                if (levels[i].sceneLabel.labelString == activeSceneName)
                    return i;
            Debug.LogWarning("LevelComplete: active scene not found in LevelList.");
            return 0;
        }
        
        public bool IsUnlocked(int index) => index <= LastUnlocked;

        public void MarkComplete()
        {
            var sceneName = UnityEngine.SceneManagement.SceneManager
                .GetActiveScene().name;
            currentLevelIndex = FindIndex(sceneName);
            Unlock(currentLevelIndex + 2);
        }
    }

    public static bool IsLevelUnlocked(int index)
    {
        return Instance.sceneInfo.IsUnlocked(index);
    }
}

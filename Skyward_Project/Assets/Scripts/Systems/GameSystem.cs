using System;
using System.Collections;
using Skyward.Core;
using Skyward.Systems;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

[RequiredSystem]
public class GameSystem : BaseSystem<GameSystem>
{
    private SceneInfo sceneInfo = new();
    private AsyncOperationHandle<SceneInstance> levelHandle;
    private bool isLoading;

    public static int LastCompletedLevelIndex => Instance.sceneInfo.LastCompleted;
    
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
    
    private event EventHandler backToMainMenu;
    
    public static event EventHandler BackToMainMenu
    {
        add => Instance.backToMainMenu += value;
        remove => Instance.backToMainMenu -= value;
    }

    private bool isCatalogLoaded;
    
    protected override void Initialize(GameContext context)
    {
        base.Initialize(context);

        context.Store(sceneInfo);

        TryLoadNextUncompletedScene();
    }

    private void TryLoadNextUncompletedScene()
    {
        string nextUncompletedSceneLabel = sceneInfo.GetNextUncompletedScene();
        if (!string.IsNullOrEmpty(nextUncompletedSceneLabel))
            RequestLevelLoad(nextUncompletedSceneLabel);
    }

    public static void RequestLevelLoad(string levelKey)
    {
        Instance.StartCoroutine(Instance.LevelDownloadRequested(levelKey));
    }

    private IEnumerator LevelDownloadRequested(string levelKey)
    {
        levelAlreadyLoaded = false;
        yield return LoadLevelInternal(levelKey);

        if (levelAlreadyLoaded)
            yield break;

        yield return DownloadLevel(levelKey);
    }

    private IEnumerator LoadLevelInternal(string levelKey)
    {
        if (!isCatalogLoaded)
        {
            string catalogUrl = DeliveryBucketManager.GetContentCatalogURL(BucketEnvironment.Development);
            var catalogHandle = Addressables.LoadContentCatalogAsync(catalogUrl);
            yield return catalogHandle;

            if (catalogHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError("Failed to load content catalog: " + catalogHandle.OperationException);
                levelDownloadFailed?.Invoke(this, EventArgs.Empty);
                Addressables.Release(catalogHandle);
                isLoading = false;
                yield break;
            }

            Addressables.Release(catalogHandle);
            Debug.Log($"Remote catalog loaded: {catalogUrl}");
            isCatalogLoaded = true;
        }
//#endif
        

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

        levelAlreadyLoaded = bytes == 0;
    }

    public static void RequestLevelLaunch(string levelKey)
    {
        if (Instance.isLoading)
            return;
        
        Instance.StartCoroutine(Instance.RequestLevelLaunchCoroutine(levelKey));
    }

    private bool levelAlreadyLoaded;
    private IEnumerator RequestLevelLaunchCoroutine(string levelKey)
    {
        isLoading = true;
        preLevelLoad?.Invoke(this, EventArgs.Empty);
        yield return new WaitForEndOfFrame();

        levelAlreadyLoaded = false;
        yield return LoadLevelInternal(levelKey);
        yield return null;
        
        if (levelAlreadyLoaded)
        {
            yield return LaunchLevel(levelKey);
            yield break;
        }

        yield return DownloadLevel(levelKey);
        
        yield return null;
        yield return LaunchLevel(levelKey);
    }

    private Scene currentGameScene;
    private IEnumerator LaunchLevel(string levelKey)
    {
        levelHandle = Addressables.LoadSceneAsync(levelKey, LoadSceneMode.Additive);
        gamecontext.game.NotifyLevelLoading();
        while (!levelHandle.IsDone)
        {
            levelLoading?.Invoke(this, levelHandle.PercentComplete);
            yield return null;
        }

        isLoading = false;
        if (levelHandle.Status != AsyncOperationStatus.Succeeded)
            throw levelHandle.OperationException;

        currentGameScene = levelHandle.Result.Scene;
        levelLoaded?.Invoke(this, EventArgs.Empty);
        gamecontext.game.LevelLoadCompleted();
    }

    private IEnumerator DownloadLevel(string levelKey)
    {
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
    }

    public static void OnLevelCompleted()
    {
        Instance.sceneInfo.MarkComplete();
        GameInputSystem.DisableInput();
        Instance.levelCompleted?.Invoke(Instance, EventArgs.Empty);
        Instance.TryLoadNextUncompletedScene();
        MainMenu();
    }

    private void OnApplicationQuit()
    {
        Quit();
        if (Instance.levelHandle.IsValid())
            Addressables.Release(Instance.levelHandle);
    }

    public static void MainMenu()
    {
        Instance.StartCoroutine(MainMenuInternal());
        
    }

    private static void Quit(bool mainMenu = false)
    {
        Instance.currentGameScene = default;
        Instance.GameContext.game.Quit();
        if (mainMenu)
            Instance.backToMainMenu?.Invoke(Instance, EventArgs.Empty);
        else
            Instance.quitting?.Invoke(Instance, EventArgs.Empty);
    }

    private static IEnumerator MainMenuInternal()
    {
        Quit(mainMenu: true);
        yield return Addressables.UnloadSceneAsync(Instance.levelHandle.Result);
    }

    public static string GetCurrentLevelName()
    {
        return Instance.sceneInfo.GetCurrentSceneName();
    }

    private class SceneInfo : ISkywardSerializable
    {
        public Scene currentGameScene;
        private int maxCompletedLevelIndex;
        private int currentLevelIndex;
        
        private const string Key = "LastUnlockedLevel";
        public void Serialize()
        {
            if (maxCompletedLevelIndex > 0)
                PlayerPrefs.SetInt(Key, maxCompletedLevelIndex);
        }

        public void Deserialize()
        {
            maxCompletedLevelIndex = PlayerPrefs.GetInt(Key, -1);
        }
        
        public int LastCompleted
        {
            get => PlayerPrefs.GetInt(Key, -1);
            private set { PlayerPrefs.SetInt(Key, value); PlayerPrefs.Save(); }
        }
        
        private static int FindIndex(string activeSceneName)
        {
            var levels = ConfigSystem.GetConfig<LevelConfig>().levels;
            for (int i = 0; i < levels.Length; i++)
                if (levels[i].sceneLabel.labelString == activeSceneName)
                    return i;
            
            throw new Exception($"{activeSceneName} does not exist in LevelConfig.LevelList.");
        }
        
        public string GetCurrentSceneName()
        {
            if (!Instance.currentGameScene.IsValid())
                return SceneManager.GetActiveScene().name;
        
            return Instance.currentGameScene.name;
        }
        
        public bool IsUnlocked(int index) => index <= LastCompleted + 1;

        public void MarkComplete()
        {
            int levelIndex = FindIndex(currentGameScene.name);
            if (levelIndex > LastCompleted) 
                LastCompleted = levelIndex;
        }

        public string GetNextUncompletedScene()
        {
            int nextUncompletedSceneIndex = LastCompleted + 1;
            var levels = ConfigSystem.GetConfig<LevelConfig>().levels;
            for (int i = 0; i < levels.Length; i++)
                if (i == nextUncompletedSceneIndex)
                    return levels[i].sceneLabel.labelString;
            
            return String.Empty;
        }
    }

    public static bool IsLevelUnlocked(int index)
    {
        return Instance.sceneInfo.IsUnlocked(index);
    }
}

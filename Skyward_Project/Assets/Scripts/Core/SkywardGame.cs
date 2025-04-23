using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Skyward.Core;
using Skyward.Utils;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class SkywardGame : MonoBehaviour
{
    public static SkywardGame Instance { get; private set; }
    
    public GameObject gameManagerPrefab;
    public bool inLobby = false;
    private GameObject GameManager { get; set; }
        
    private GameObject systemsGameObject;
    private GameContext context;

    public GameFactory Factory => factory;
    private GameFactory factory;
    
    private List<ISystem> systems = new();

    public event Action preLevelLoading;
    public event Action<AsyncOperation> levelLoading;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);

        Instance = this;
        
        if (!inLobby)
        {
            StartCoroutine(LaunchedFromLevel());
        }
    }
    
    public IEnumerator Initialize()
    {
        context = new GameContext(this);

        Configs.Init();
        //CreateFactory();
        CreateSystems();
        CreateGameManager();
        context.Load();
        yield break;
    }

    private IEnumerator LaunchedFromLevel()
    {
        yield return Initialize();
        TrackPrespawnedObjects();
        NotifyLevelLoading();

        yield return new WaitForEndOfFrame();
        OnLevelLoaded();
    }

    public void LaunchLevel(AssetReference levelRef)
    {
        StartCoroutine(LaunchLevelInternal(levelRef));

    }

    private IEnumerator LaunchLevelInternal(AssetReference levelRef)
    {
        preLevelLoading?.Invoke();
        yield return new WaitForEndOfFrame();
        
        // var async = SceneManager.LoadSceneAsync(sceneName);
        // levelLoading?.Invoke(async);
        // async.completed += LevelLoadCompleted;
        var asnyc = Addressables.LoadAssetAsync<UnityEngine.Object>(levelRef);
        asnyc.Completed += Completed;
        NotifyLevelLoading();
    }

    private void Completed(AsyncOperationHandle<Object> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
            Instantiate(obj.Result);
    }

    private void LevelLoadCompleted(AsyncOperation async)
    {
        async.completed -= LevelLoadCompleted;
        TrackPrespawnedObjects();
        OnLevelLoaded();
    }

    private void OnLevelLoaded()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        NotifyWorldLoaded();
    }

    private void NotifyWorldLoaded()
    {
        foreach (var comp in ComponentSystem.GetAllComponents<ISkywardComponent>())
            comp.WorldLoaded(context);
    }
    
    void CreateFactory()
    {
        factory = new GameFactory(context);
        factory.ObjectCreated += SetupObject;
    }

    private void SetupObject(object sender, GameObject go)
    {
        IEnumerable<ICoreComponent> components = go.GetComponentsInChildren<ICoreComponent>(true);
        foreach(ICoreComponent component in components)
        {
            ComponentSystem.TrackComponent(component);
        }
    }

    private void CreateGameManager()
    {
        GameManager = Instantiate(gameManagerPrefab);
    }
    
    private void TrackPrespawnedObjects()
    {
        foreach (var obj in FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None).Where(obj => obj.transform.parent == null))
        {
            foreach (ICoreComponent coreComponent in obj.GetComponentsInChildren<ICoreComponent>(true))
            {
                ComponentSystem.TrackComponent(coreComponent);
            }
        }
    }
    
    private void CleanupAllComponents()
    {
        foreach (ISkywardComponent component in ComponentSystem.GetAllComponents<ISkywardComponent>(true))
        {
            component.Cleanup();
        }
        
        foreach (ISystem system in ComponentSystem.GetAllComponents<ISystem>(true))
        {
            system.Cleanup();
        }
    }

    void CreateSystems()
    {
        if (systemsGameObject == null)
            systemsGameObject = new GameObject("Systems");
        
        foreach (Type systemType in AllRequiredSystems())
        {
            if (systemsGameObject.TryGetComponent(systemType, out _))
                continue;

            systemsGameObject.AddComponent(systemType);
            Component systemComponent = systemsGameObject.GetComponent(systemType);
            if (systemComponent is BaseSystem baseSystem)
            {
                baseSystem.gamecontext = context;
                systems.Add(baseSystem);
            }
        }
        
        foreach (ISystem system in systems)
            system.Initialize(context);

        
        DontDestroyOnLoad(systemsGameObject);
    }
    
    void NotifyLevelLoading()
    {
        foreach (var system in systems)
            system.OnWorldLoading(context);
    }
    
    private static IEnumerable<Type> AllRequiredSystems()
    {
        foreach((Type systemType, RequiredSystemAttribute attribute) in ReflectionHelper.AllTypesWithAttribute<RequiredSystemAttribute>())
        {
            yield return systemType;
        }
    }

    public void Quit()
    {
        context.Save();
        DestroyAll();
    }

    private void DestroyAll()
    {
        CleanupAllComponents();
        ComponentSystem.UntrackAll();
        DestroyImmediate(GameManager.gameObject);
        Destroy(systemsGameObject);
    }

    private void OnApplicationQuit()
    {
        Quit();
    }
}

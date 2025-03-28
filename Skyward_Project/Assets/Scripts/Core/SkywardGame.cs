using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Skyward.Core;
using Skyward.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkywardGame : MonoBehaviour
{
    public static SkywardGame Instance { get; private set; }
    
    public GameObject gameManagerPrefab;
    private GameObject GameManager { get; set; }
        
    private GameObject systemsGameObject;
    private GameContext context;

    public GameFactory Factory => factory;
    private GameFactory factory;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);

        Instance = this;
    }
    
    public void LaunchLevel(string sceneName)
    {
        InitializeSystems();
        var async = SceneManager.LoadSceneAsync(sceneName);
        async.completed += OnLevelLoaded;
    }

    private void OnLevelLoaded(AsyncOperation operation)
    {
        operation.completed -= OnLevelLoaded;
        OnLevelLoaded();
    }

    private void OnLevelLoaded()
    {
        #if UNITY_EDITOR
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        #endif
        
        TrackPrespawnedObjects();
        NotifyWorldLoaded();
    }

    private void NotifyWorldLoaded()
    {
        foreach (var comp in ComponentSystem.GetAllComponents<ISkywardComponent>())
            comp.WorldLoaded(context);
    }

    public IEnumerator Initialize(GameSettings settings)
    {
        context = new GameContext(this);

        Configs.Init();
        CreateFactory();
        CreateSystems();
        CreateGameManager();
        context.Load();
        yield break;
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

        List<ISystem> iSystems = new List<ISystem>();
        foreach (Type systemType in AllRequiredSystems())
        {
            if (systemsGameObject.TryGetComponent(systemType, out _))
                continue;

            systemsGameObject.AddComponent(systemType);
            
            Component systemComponent = (Component)FindAnyObjectByType(systemType);
            if (systemComponent is BaseSystem baseSystem)
            {
                baseSystem.gamecontext = context;
            }
            
            if (systemComponent is ISystem system)
                iSystems.Add(system);
        }
        foreach (BaseSystem system in systemsGameObject.GetComponents<BaseSystem>())
            ComponentSystem.TrackComponent(system);
        
        foreach (ISystem system in iSystems)
            system.Preload(context);

        
        DontDestroyOnLoad(systemsGameObject);
    }
    
    void InitializeSystems()
    {
        foreach (var system in ComponentSystem<ISystem>.Components)
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

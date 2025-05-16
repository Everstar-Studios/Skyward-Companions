using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Skyward.Core;
using Skyward.Utils;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

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

        Instance = this;
        
        if (!inLobby)
        {
            StartCoroutine(LaunchedFromLevel());
        }
    }
    
    public IEnumerator Initialize(bool reinitialization = false)
    {
        context = new GameContext(this);
        
        //CreateFactory();
        if (!reinitialization)
            CreateSystems();
        
        CreateGameManager();
        context.Load();
        
        yield return ConfigSystem.AllConfigurationsLoaded;
        
        NotifyPostInitialize();
        
        yield return new WaitForFixedUpdate();
    }

    private void NotifyPostInitialize()
    {
        foreach (ISystem system in systems)
            system.PostInitialize(context);
    }
    
    private IEnumerator LaunchedFromLevel()
    {
        yield return Initialize();
        TrackPrespawnedObjects();
        NotifyLevelLoading();

        yield return new WaitForEndOfFrame();
        OnLevelLoaded();
    }
    
    private void Completed(AsyncOperationHandle<SceneInstance> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
            obj.Result.ActivateAsync();
    }

    public void LevelLoadCompleted()
    {
        TrackPrespawnedObjects();
        OnLevelLoaded();
    }

    private void OnLevelLoaded()
    {
        #if UNITY_EDITOR
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        #endif
        
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
        ComponentSystem.UntrackAll();
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
            /*
            if (systemsGameObject.TryGetComponent(systemType, out _))
                continue;

            systemsGameObject.AddComponent(systemType);
            Component systemComponent = systemsGameObject.GetComponent(systemType);
            if (systemComponent is BaseSystem baseSystem)
            {
                baseSystem.gamecontext = context;
                systems.Add(baseSystem);
            }*/

            /////////////////////
            Component existingComponent = (Component)UnityEngine.Object.FindAnyObjectByType(systemType);

            if (systemsGameObject.GetComponent(systemType) == null && existingComponent == null)
            {
                systemsGameObject.AddComponent(systemType);
            }

            Component systemComponent = existingComponent ?? systemsGameObject.GetComponent(systemType);

            if (systemComponent is BaseSystem baseSystem)
            {
                baseSystem.gamecontext = context;
                systems.Add(baseSystem);
            }
        }
        
        foreach (ISystem system in systems)
            system.Initialize(context);
    }
    
    internal void NotifyLevelLoading()
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
    }
}

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

    private IEnumerator Start()
    {
        if (FindAnyObjectByType<Lobby>() == null)
        {
            yield return Initialize(new GameSettings());
            OnLevelLoaded();
        }
    }
    
    public void LaunchLevel(int sceneIndex)
    {
        InitializeSystems();
        
        var async = SceneManager.LoadSceneAsync(sceneIndex);
        async.completed += OnLevelLoaded;
    }

    private void OnLevelLoaded(AsyncOperation operation)
    {
        operation.completed -= OnLevelLoaded;
        OnLevelLoaded();
    }

    private void OnLevelLoaded()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        TrackPrespawnedObjects();
        NotifyWorldLoaded();
    }

    private void NotifyWorldLoaded()
    {
        foreach (var comp in ComponentSystem.GetAllComponents<ISkywardComponent>())
            comp.WorldLoaded();
    }

    public IEnumerator Initialize(GameSettings settings)
    {
        context = new GameContext(this);
        
        CreateSystems();
        CreateGameManager();
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
        if (GameManager == null)
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

    void CreateSystems()
    {
        if (systemsGameObject == null)
            systemsGameObject = new GameObject("Systems");

        foreach (Type systemType in AllRequiredSystems())
        {
            if (systemsGameObject.TryGetComponent(systemType, out _))
                continue;

            systemsGameObject.AddComponent(systemType);
            
            Component systemComponent = (Component)FindAnyObjectByType(systemType);
            if (systemComponent is BaseSystem system)
            {
                system.gamecontext = context;
            }
        }
        
        DontDestroyOnLoad(systemsGameObject);
    }
    
    void InitializeSystems()
    {
        foreach (var system in ComponentSystem<ISystem>.Components)
            system.Initialize(context);
    }
    
    private static IEnumerable<Type> AllRequiredSystems()
    {
        foreach((Type systemType, RequiredSystemAttribute attribute) in ReflectionHelper.AllTypesWithAttribute<RequiredSystemAttribute>())
        {
            yield return systemType;
        }
    }

    private void OnApplicationQuit()
    {
        foreach (var skywardComponent in ComponentSystem.GetAllComponents<ISkywardComponent>())
        {
            skywardComponent.Cleanup();
        }
    }
}

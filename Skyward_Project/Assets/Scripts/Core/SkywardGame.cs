using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Skyward.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkywardGame : MonoBehaviour
{
    public GameObject gameManagerPrefab;
    private GameObject GameManager { get; set; }
        
    private GameObject systemsGameObject;
    private GameContext context;

    public GameFactory Factory => factory;
    private GameFactory factory;

    public IEnumerator Initialize(GameSettings settings)
    {
        context = new GameContext()
        {
        };

        CreateFactory();
        CreateSystems();
        CreateGameManager();
        TrackPrespawnedObjects();
        InitializeSystems();
        
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
        }
        
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
        foreach (var system in systemsGameObject.GetComponents<ISystem>())
        {
            system.Cleanup();
        }
    }

    public void LevelRequested(string levelName)
    {
        GameSystem.OnLevelRequested(levelName);
    }
}

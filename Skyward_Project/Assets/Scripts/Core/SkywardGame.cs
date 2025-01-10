using System;
using System.Collections;
using System.Collections.Generic;
using Skyward.Core;
using UnityEngine;

public class SkywardGame : MonoBehaviour
{
    public GameObject gameManagerPrefab;
    private GameObject GameManager { get; set; }
        
    private GameObject systemsGameObject;
    private GameContext context;

    public IEnumerator Initialize(GameSettings settings)
    {
        context = new GameContext()
        {
        };

        CreateSystems();
        CreateGameManager();
        yield break;
    }

    private void CreateGameManager()
    {
        if (GameManager == null)
            GameManager = Instantiate(gameManagerPrefab);
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
}

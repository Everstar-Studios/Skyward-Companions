using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using Skyward.Core;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[RequiredSystem]
public class ConfigSystem : BaseSystem<ConfigSystem>
{
    private ConfigComponent configComponent;
    private Dictionary<Type, ScriptableObject> allConfigs = new();
    
    private List<AsyncOperationHandle> loadingConfigs = new ();
    
    public static IEnumerator AllConfigurationsLoaded
    {
        get
        {
            while (Instance == null)
                yield return new WaitForEndOfFrame();
    
            // while(Instance.loadingConfigs.Any())
            //     yield return new WaitForFixedUpdate();
        }
    }
    
    protected override void Initialize(GameContext context)
    {
        base.Initialize(context);

        configComponent = FindAnyObjectByType<ConfigComponent>();
        foreach (var config in configComponent.Configs)
            LoadConfig(config);
    }

    public static T GetConfig<T>() where T : ScriptableObject
    {
        Type type = typeof(T);
        var allConfigs = Instance.allConfigs;

        return (T)allConfigs[type];
    }
    
    public void LoadConfig(BaseConfig config)
    {
        allConfigs.Add(config.GetType(), config);
    }
}

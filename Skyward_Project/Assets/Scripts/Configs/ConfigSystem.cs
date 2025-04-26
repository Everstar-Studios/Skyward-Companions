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
    
            while(Instance.loadingConfigs.Any())
                yield return new WaitForFixedUpdate();
        }
    }
    
    protected override void Initialize(GameContext context)
    {
        base.Initialize(context);

        configComponent = FindAnyObjectByType<ConfigComponent>();
        foreach (var config in configComponent.Configs)
        {
            StartCoroutine(LoadConfig(config));
        }
    }

    public static T GetConfig<T>() where T : ScriptableObject
    {
        Type type = typeof(T);
        var allConfigs = Instance.allConfigs;

        return (T)allConfigs[type];
    }
    
    public IEnumerator LoadConfig(AssetReference assetRef)
    {
        AsyncOperationHandle<ScriptableObject> handle;
        handle = assetRef.LoadAssetAsync<ScriptableObject>();
        loadingConfigs.Add(handle);
        yield return handle;
        loadingConfigs.Remove(handle);

        if (allConfigs.ContainsKey(handle.Result.GetType()))
            Debug.LogError($"[Core] Configuration of type {handle.Result.GetType().GetNiceName()} already exists.");
        else
            allConfigs[handle.Result.GetType()] = Instantiate(handle.Result);

        yield return new WaitUntil(() => !allConfigs.ContainsKey(handle.Result.GetType()));
            
        Addressables.Release(handle);
    }
}

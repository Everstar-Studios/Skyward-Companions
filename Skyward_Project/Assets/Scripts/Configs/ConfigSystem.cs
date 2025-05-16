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
    
    private event EventHandler configsLoaded;
    
    public static event EventHandler ConfigsLoaded
    {
        add => Instance.configsLoaded += value;
        remove => Instance.configsLoaded -= value;
    }
    
    public static IEnumerator AllConfigurationsLoaded
    {
        get
        {
            while(!Instance.allConfigs.Any())
                yield return new WaitForEndOfFrame();
    
            while(Instance.loadingConfigs.Any())
                yield return new WaitForFixedUpdate();
        }
    }
    
    protected override void Initialize(GameContext context)
    {
        base.Initialize(context);

        GameSystem.CatalogLoaded += OnCatalogLoaded;
    }

    protected override void Cleanup()
    {
        base.Cleanup();
        
        GameSystem.CatalogLoaded -= OnCatalogLoaded;
    }

    private void OnCatalogLoaded(object sender, string catalogUrl)
    {
        StartCoroutine(LoadConfigs());
    }

    private IEnumerator LoadConfigs()
    {
        configComponent = FindAnyObjectByType<ConfigComponent>();
        
        var sizeHandle = Addressables.GetDownloadSizeAsync("Config");
        yield return sizeHandle;
        
        if (sizeHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Addressables.Release(sizeHandle);
            yield break;
        }
        
        long bytes = sizeHandle.Result;
        Addressables.Release(sizeHandle);
        
        float mb = bytes / (1024f * 1024f);
        Debug.Log($"Download size for Configs: {mb:0.#} MB");

        if (bytes > 0)
        {
            var downloadHandle = Addressables.DownloadDependenciesAsync("Config");
            yield return downloadHandle;

            if (downloadHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError("Failed to download configs.");
                Addressables.Release(downloadHandle);
                yield break;
            }

            Addressables.Release(downloadHandle);
            Debug.Log("Configs downloaded.");
        }
        else
        {
            Debug.Log("Configs already cached locally.");
        }
        
        foreach (var configRef in configComponent.Configs)
            yield return LoadConfig(configRef);
        
        configsLoaded?.Invoke(this, EventArgs.Empty);
    }

    public static T GetConfig<T>() where T : ScriptableObject
    {
        Type type = typeof(T);
        return (T)Instance.allConfigs[type];
    }
    
    private IEnumerator LoadConfig(AssetReference assetRef)
    {
        AsyncOperationHandle<ScriptableObject> handle;
        handle = assetRef.LoadAssetAsync<ScriptableObject>();
        loadingConfigs.Add(handle);
        yield return handle;
        loadingConfigs.Remove(handle);
        
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            var loadedConfig = handle.Result;
            Debug.Log($"Loaded config: {loadedConfig.name}");
        }
        else
        {
            Debug.LogError($"Failed to load config from reference: {handle.Result}");
            Addressables.Release(handle);
            yield break;
        }

        if (allConfigs.ContainsKey(handle.Result.GetType()))
            Debug.LogError($"[Core] Configuration of type {handle.Result.GetType().GetNiceName()} already exists.");
        else
            allConfigs[handle.Result.GetType()] = Instantiate(handle.Result);
            
        Addressables.Release(handle);
    }
}

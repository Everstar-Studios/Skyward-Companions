using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlatformBuckets
{
    public string Windows;
    public string iOS;
    public string Android;
    public string MacOS;
}

[Serializable]
public class BucketIdConfig
{
    public PlatformBuckets development;
    public PlatformBuckets production;
}

public enum BucketEnvironment
{
    Development,
    Production
}

public static class DeliveryBucketManager
{
    private static BucketIdConfig cachedConfig;

    public static string GetBucketId(BucketEnvironment environment)
    {
        if (cachedConfig == null)
        {
            TextAsset json = Resources.Load<TextAsset>("bucket_ids");
            if (json == null)
                throw new Exception("bucket_ids.json could not be found in Resources.");
            cachedConfig = JsonUtility.FromJson<BucketIdConfig>(json.text);
        }

        PlatformBuckets envBuckets = environment switch
        {
            BucketEnvironment.Development => cachedConfig.development,
            BucketEnvironment.Production  => cachedConfig.production,
            _ => throw new ArgumentException($"Unknown environment: {environment}")
        };

        return Application.platform switch
        {
            RuntimePlatform.WindowsEditor or RuntimePlatform.WindowsPlayer => envBuckets.Windows,
            RuntimePlatform.IPhonePlayer => envBuckets.iOS,
            RuntimePlatform.Android => envBuckets.Android,
            RuntimePlatform.OSXEditor or RuntimePlatform.OSXPlayer => envBuckets.MacOS,
            _ => throw new NotSupportedException($"Platform not supported: {Application.platform}")
        };
    }

    public static string GetContentCatalogURL(BucketEnvironment environment)
    {
        string bucketId = GetBucketId(environment);
        string environmentStr = environment == BucketEnvironment.Development ? "development" : "production";
        string catalogUrl = $"https://5020019f-5188-4075-84eb-a2113dd902e6.client-api.unity3dusercontent.com/client_api/v1/environments/{environmentStr}/buckets/{bucketId}/release_by_badge/latest/entry_by_path/content/?path=catalog_1.0.0.bin";
        return catalogUrl;
    }
}
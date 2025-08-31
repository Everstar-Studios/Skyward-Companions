using System;
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

        return ResolvePlatformBucket(envBuckets);
    }

    public static string GetContentCatalogURL(BucketEnvironment environment)
    {
        string bucketId = GetBucketId(environment);
        string environmentStr = environment == BucketEnvironment.Development ? "development" : "production";
        string catalogUrl = $"https://32fc0e12-ca2c-49f1-8c2e-f26741f6f6f9.client-api.unity3dusercontent.com/client_api/v1/environments/{environmentStr}/buckets/{bucketId}/release_by_badge/latest/entry_by_path/content/?path=catalog_1.0.0.bin";
        return catalogUrl;
    }

    private static string ResolvePlatformBucket(PlatformBuckets envBuckets)
    {
        // In the Editor, prefer the active build target (so iOS/Android targets don't look like macOS)
#if UNITY_EDITOR
        switch (UnityEditor.EditorUserBuildSettings.activeBuildTarget)
        {
            case UnityEditor.BuildTarget.iOS:
                return envBuckets.iOS;
            case UnityEditor.BuildTarget.Android:
                return envBuckets.Android;
            case UnityEditor.BuildTarget.StandaloneWindows:
            case UnityEditor.BuildTarget.StandaloneWindows64:
                return envBuckets.Windows;
            case UnityEditor.BuildTarget.StandaloneOSX:
                return envBuckets.MacOS;
            // fall through to runtime check for any other targets
        }
#endif

        // Runtime (players) and general fallback
        return Application.platform switch
        {
            RuntimePlatform.IPhonePlayer => envBuckets.iOS,
            RuntimePlatform.Android      => envBuckets.Android,
            RuntimePlatform.WindowsPlayer or RuntimePlatform.WindowsEditor => envBuckets.Windows,
            RuntimePlatform.OSXPlayer    or RuntimePlatform.OSXEditor     => envBuckets.MacOS,
            _ => throw new NotSupportedException($"Platform not supported: {Application.platform}")
        };
    }
}

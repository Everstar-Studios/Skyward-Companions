using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.Build;
using UnityEngine;

public static class BuildModeSwitcher
{
    private const string DEVELOPMENT_SYMBOL = "SKYWARD_DEVELOPMENT";

    [MenuItem("Skyward/Toggle Development Mode")]
    public static void ToggleDevelopmentMode()
    {
        NamedBuildTarget buildTarget = NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        string defines = PlayerSettings.GetScriptingDefineSymbols(buildTarget);
        bool isDevelopment = defines.Contains(DEVELOPMENT_SYMBOL);

        if (isDevelopment)
        {
            defines = defines.Replace(DEVELOPMENT_SYMBOL, "").Replace(";;", ";").Trim(';');
            Debug.Log("Switched to Release Mode");
            SetAddressablesPlayMode(false);
        }
        else
        {
            if (!string.IsNullOrEmpty(defines))
                defines += ";";
            defines += DEVELOPMENT_SYMBOL;
            Debug.Log("Switched to Development Mode");
            SetAddressablesPlayMode(true);
        }

        PlayerSettings.SetScriptingDefineSymbols(buildTarget, defines);
    }

    [MenuItem("Skyward/Toggle Development Mode", true)]
    public static bool ToggleDevelopmentModeValidation()
    {
        NamedBuildTarget buildTarget = NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        string defines = PlayerSettings.GetScriptingDefineSymbols(buildTarget);

        Menu.SetChecked("Skyward/Toggle Development Mode", defines.Contains(DEVELOPMENT_SYMBOL));
        return true;
    }
    
    private static void SetAddressablesPlayMode(bool useAssetDatabase)
    {
        if (!AddressableAssetSettingsDefaultObject.SettingsExists)
        {
            Debug.LogWarning("Addressables Settings not found.");
            return;
        }

        var settings = AddressableAssetSettingsDefaultObject.Settings;

        string targetBuilderName = useAssetDatabase
            ? "BuildScriptFastMode" // AssetDatabase
            : "BuildScriptPackedPlayMode"; // Use Existing Build

        int index = settings.DataBuilders.FindIndex(builder => builder.GetType().Name == targetBuilderName);

        if (index == -1)
        {
            Debug.LogError($"Could not find Addressables play mode script: {targetBuilderName}");
            return;
        }

        settings.ActivePlayModeDataBuilderIndex = index;
        EditorUtility.SetDirty(settings);
        AssetDatabase.SaveAssets();

        Debug.Log($"Set Addressables Play Mode Script to {(useAssetDatabase ? "Asset Database" : "Use Existing Build")}");
    }
}
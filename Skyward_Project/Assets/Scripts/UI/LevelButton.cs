using System;
using System.IO;
using Skyward.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LevelButton : UIButton, ISkywardComponent
{
    public bool unlockedByDefault = false;
    public GameObject lockIcon;
    
    public AssetLabelReference levelLabel;

    private int sceneIndex;

    protected override void Awake()
    {
        base.Awake();

        // int sceneCount = SceneManager.sceneCountInBuildSettings;
        // for (int i = 0; i < sceneCount; i++)
        // {
        //     string path = SceneUtility.GetScenePathByBuildIndex(i);
        //     string name = Path.GetFileNameWithoutExtension(path);
        //     if (!name.Equals(sceneName, StringComparison.OrdinalIgnoreCase)) 
        //         continue;
        //     
        //     sceneIndex = i;
        //     break;
        // }
        
        Unlock();
    }

    public override void OnClick()
    {

        if (!unlockedByDefault && !GameSystem.IsLevelUnlocked(sceneIndex))
            return;
        
        base.OnClick();

        foreach (var levelButton in FindObjectsByType<LevelButton>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            levelButton.Disable();
        
        GameSystem.RequestLevelLaunch(levelLabel.labelString);
    }

    private void Disable()
    {
        button.onClick.RemoveListener(OnClick);
    }

    public void Unlock()
    {
        bool isUnlocked = unlockedByDefault || GameSystem.IsLevelUnlocked(sceneIndex);
        button.interactable = isUnlocked;
        lockIcon.SetActive(!isUnlocked);
    }

    public void ForceUnlock()
    {
        if (button == null)
            button = GetComponent<Button>();
        
        unlockedByDefault = true;
        Unlock();
    }
}

// #if UNITY_EDITOR
//
// [CustomEditor(typeof(LevelButton))]
// public class LevelButtonEditor : Editor
// {
//     private string[] sceneNames;
//
//     private void OnEnable()
//     {
//         int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
//         sceneNames = new string[sceneCount];
//
//         for (int i = 0; i < sceneCount; i++)
//         {
//             string path = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i);
//             sceneNames[i] = System.IO.Path.GetFileNameWithoutExtension(path);
//         }
//     }
//
//     public override void OnInspectorGUI()
//     {
//         serializedObject.Update();
//
//         LevelButton levelButton = (LevelButton)target;
//         SerializedProperty sceneNameProp = serializedObject.FindProperty("sceneName");
//         SerializedProperty clickSoundProp = serializedObject.FindProperty("clickSound");
//         EditorGUILayout.PropertyField(clickSoundProp);
//
//         int currentIndex = System.Array.IndexOf(sceneNames, sceneNameProp.stringValue);
//         if (currentIndex < 0) currentIndex = 0;
//
//         int selectedIndex = EditorGUILayout.Popup("Scene Name", currentIndex, sceneNames);
//         sceneNameProp.stringValue = sceneNames[selectedIndex];
//
//         EditorGUILayout.PropertyField(serializedObject.FindProperty("unlockedByDefault"));
//         EditorGUILayout.PropertyField(serializedObject.FindProperty("lockIcon"));
//
//         serializedObject.ApplyModifiedProperties();
//     }
// }
// #endif
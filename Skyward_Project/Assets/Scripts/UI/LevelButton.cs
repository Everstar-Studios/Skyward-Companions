using System;
using Skyward.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LevelButton : MonoBehaviour, ISkywardComponent
{
    public string sceneName;
    public bool unlockedByDefault = false;
    private Button button;
    public GameObject lockIcon;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
        
        bool isUnlocked = unlockedByDefault || GameSystem.IsLevelUnlocked(sceneName);
        button.interactable = isUnlocked;
        lockIcon.SetActive(!isUnlocked);
    }

    private void OnClick()
    {
        if (!unlockedByDefault && !GameSystem.IsLevelUnlocked(sceneName))
            return;

        GameSystem.LaunchLevel(sceneName);
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(LevelButton))]
public class LevelButtonEditor : Editor
{
    private string[] sceneNames;

    private void OnEnable()
    {
        int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
        sceneNames = new string[sceneCount];

        for (int i = 0; i < sceneCount; i++)
        {
            string path = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i);
            sceneNames[i] = System.IO.Path.GetFileNameWithoutExtension(path);
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        LevelButton levelButton = (LevelButton)target;
        SerializedProperty sceneNameProp = serializedObject.FindProperty("sceneName");

        int currentIndex = System.Array.IndexOf(sceneNames, sceneNameProp.stringValue);
        if (currentIndex < 0) currentIndex = 0;

        int selectedIndex = EditorGUILayout.Popup("Scene Name", currentIndex, sceneNames);
        sceneNameProp.stringValue = sceneNames[selectedIndex];

        EditorGUILayout.PropertyField(serializedObject.FindProperty("unlockedByDefault"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("lockIcon"));

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
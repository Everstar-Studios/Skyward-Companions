using Skyward.Characters;
using UnityEditor;
using Skyward.Core;
using UnityEngine;

namespace Companions.Editor
{
    public class SetupSceneDependencies : EditorWindow
    {
        [MenuItem("Skyward/Setup Scene Dependencies")]
        public static void Method()
        {
            AddSceneDependencies();
        }

        private static void AddSceneDependencies()
        {
            var camera = FindAnyObjectByType<Camera>();
            var player = FindAnyObjectByType<PlayerController>();

            if (camera == null)
            {
                GameObject cameraPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Core/GO_MainCamera.prefab");
                PrefabUtility.InstantiatePrefab(cameraPrefab);
            }
            
            if (player == null)
            {
                GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Characters/GO_Player.prefab");
                PrefabUtility.InstantiatePrefab(playerPrefab);
            }
        }
    }
}
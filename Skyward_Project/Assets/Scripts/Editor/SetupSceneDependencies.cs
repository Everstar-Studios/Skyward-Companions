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
            var game = FindAnyObjectByType<SkywardGame>();
            var hud = FindAnyObjectByType<GameHUDComponent>();
            var player = FindAnyObjectByType<Player>();

            if (game == null)
            {
                GameObject skywardGamePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Core/GO_SkywardGame.prefab");
                PrefabUtility.InstantiatePrefab(skywardGamePrefab);
            }
            if (hud == null)
            {
                GameObject hudPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Core/GO_GameUI.prefab");
                PrefabUtility.InstantiatePrefab(hudPrefab);
            }
            if (player == null)
            {
                GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Characters/GO_Player.prefab");
                PrefabUtility.InstantiatePrefab(playerPrefab);
            }
        }
    }
}
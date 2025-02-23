using Skyward.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequiredSystem]
public class GameSystem : BaseSystem<GameSystem>
{
    public static void LaunchLevel(int sceneIndex)
    {
        Instance.GameContext.game.LaunchLevel(sceneIndex);
    }
}

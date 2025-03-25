using Skyward.UI;
using UnityEngine;

[CreateAssetMenu(fileName = "Launch Game Button", menuName = "Skyward/UI/LaunchGameButton")]
public class LaunchGameButtonAsset : ButtonAsset
{
    public string levelName;
    public override void Action()
    {
        base.Action();
        GameSystem.LaunchLevel(levelName);
    }
}

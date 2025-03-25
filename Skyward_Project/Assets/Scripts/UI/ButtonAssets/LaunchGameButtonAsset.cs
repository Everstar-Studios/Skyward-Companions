using UnityEngine;

[CreateAssetMenu(fileName = "Launch Game Button", menuName = "Skyward/Button Assets/LaunchGameButton")]
public class LaunchGameButtonAsset : ButtonAsset
{
    public string levelName;
    public override void Action()
    {
        base.Action();
        GameSystem.LaunchLevel(levelName);
    }
}

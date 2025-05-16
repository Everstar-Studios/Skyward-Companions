using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "Level Config", menuName = "Skyward/Configs/LevelConfig")]
public class LevelConfig : BaseConfig
{
    [System.Serializable]
    public struct LevelInfo
    {
        public AssetLabelReference sceneLabel;
    }
    
    public LevelInfo[] levels;
}

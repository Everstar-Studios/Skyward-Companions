using UnityEngine;

public static class Configs
{
    public static PlayerConfig PlayerConfig { get; private set; }
    
    public static void Init()
    {
        PlayerConfig = Resources.Load<PlayerConfig>("Configs/PlayerConfig");
    }
}

using UnityEngine;

public static class Configs
{
    public static PlayerConfig PlayerConfig => ConfigSystem.GetConfig<PlayerConfig>();
    public static UIConfig UIConfig => ConfigSystem.GetConfig<UIConfig>();
}

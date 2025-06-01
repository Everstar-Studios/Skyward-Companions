using System;
using System.IO;
using Skyward.Characters;
using Skyward.Core;
using UnityEngine;

[RequiredSystem]
public class PlayerSystem : BaseSystem<PlayerSystem>, ISkywardSerializable
{
    public static PlayerController Player => Instance.player;

    public static string PlayerName
    {
        get
        {
            int index = PlayerFullName.IndexOf('#');
            return index >= 0 ? PlayerFullName.Substring(0, index) : PlayerFullName;
        }
        
    }
    public static string PlayerFullName { get; set; }

    private PlayerController player;
    private GameContext gameContext;
    
    public static event EventHandler<PlayerController> PlayerFound
    {
        add => Instance.playerFound += value;
        remove => Instance.playerFound -= value;
    }

    private event EventHandler<PlayerController> playerFound;

    protected override void Initialize(GameContext context)
    {
        base.Initialize(context);
        
        context.Store(this);
    }

    protected override void WorldLoaded(GameContext context)
    {
        base.WorldLoaded(context);
        
        player = FindAnyObjectByType<PlayerController>();
        playerFound?.Invoke(this, player);
    }

    public void Serialize()
    {
        PlayerPrefs.SetString("PlayerName", PlayerName);
    }

    public void Deserialize()
    {
        PlayerFullName = PlayerPrefs.GetString("PlayerName");
    }
}

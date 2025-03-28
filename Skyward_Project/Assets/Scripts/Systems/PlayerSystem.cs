using System;
using System.IO;
using Skyward.Characters;
using Skyward.Core;
using UnityEngine;

[RequiredSystem]
public class PlayerSystem : BaseSystem<PlayerSystem>, ISkywardComponent, ISkywardSerializable
{
    public static PlayerController Player => Instance.player;
    
    public static Vector3 PlayerColliderCenter => Player.player.Collider.bounds.center;
    public static string PlayerName { get; set; }

    private PlayerController player;
    private GameContext gameContext;
    
    public static event EventHandler<PlayerController> PlayerFound
    {
        add => Instance.playerFound += value;
        remove => Instance.playerFound -= value;
    }

    private event EventHandler<PlayerController> playerFound;

    protected override void Preload(GameContext context)
    {
        base.Preload(context);
        
        context.Store("PLAYER_NAME", this);
    }

    void ISkywardComponent.WorldLoaded(GameContext context)
    {
        player = FindAnyObjectByType<PlayerController>();
        playerFound?.Invoke(this, player);
    }

    public void Serialize(BinaryWriter writer)
    {
        writer.Write(PlayerName);
    }

    public void Deserialize(BinaryReader reader)
    {
        PlayerName = reader.ReadString();
    }
}

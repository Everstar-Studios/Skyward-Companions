using System;
using Skyward.Characters;
using Skyward.Core;
using UnityEngine;

[RequiredSystem]
public class PlayerSystem : BaseSystem<PlayerSystem>, ISkywardComponent
{
    public static PlayerController Player => Instance.player;
    
    public static Vector3 PlayerColliderCenter => Player.player.Collider.bounds.center;
    
    private PlayerController player;
    private GameContext gameContext;
    
    public static event EventHandler<PlayerController> PlayerFound
    {
        add => Instance.playerFound += value;
        remove => Instance.playerFound -= value;
    }

    private event EventHandler<PlayerController> playerFound;

    void ISkywardComponent.WorldLoaded()
    {
        player = FindAnyObjectByType<PlayerController>();
        playerFound?.Invoke(this, player);
    }

    protected override void Cleanup()
    {
        base.Cleanup();
        
        Destroy(player.gameObject);
    }
}

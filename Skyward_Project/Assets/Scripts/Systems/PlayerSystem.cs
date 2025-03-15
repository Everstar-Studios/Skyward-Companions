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

    protected override void Awake()
    {
        base.Awake();
        
        player = FindAnyObjectByType<PlayerController>();
    }

    protected override void Cleanup()
    {
        base.Cleanup();
        
        Destroy(player.gameObject);
    }
}

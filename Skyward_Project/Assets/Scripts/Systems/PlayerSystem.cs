using Skyward.Characters;
using Skyward.Core;
using UnityEngine;

[RequiredSystem]
public class PlayerSystem : BaseSystem<PlayerSystem>, ISkywardComponent
{
    public static PlayerController Player => Instance.player;
    private PlayerController player;
    private GameContext gameContext;
    
    void ISkywardComponent.WorldLoaded()
    {
        player = FindAnyObjectByType<PlayerController>();
    }
}

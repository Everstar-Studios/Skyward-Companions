using System;
using Skyward.Characters;
using Skyward.Core;
using Skyward.Systems;
using UnityEngine;

public class DeathZoneComponent : MonoBehaviour, ISkywardComponent
{
    [SerializeField] public Collider trigger;
    
    void ISkywardComponent.WorldLoaded()
    {
        CheckpointSystem.AddDeathZone(this);
    }
}

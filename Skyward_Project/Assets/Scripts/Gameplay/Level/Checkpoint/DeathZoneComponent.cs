using System;
using Skyward.Characters;
using Skyward.Core;
using Skyward.Systems;
using UnityEngine;

public class DeathZoneComponent : MonoBehaviour, ISkywardComponent
{
    [SerializeField] public Collider trigger;

    private void Awake()
    {
        trigger.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        CheckpointSystem.EnteredDeathZone();
    }
}

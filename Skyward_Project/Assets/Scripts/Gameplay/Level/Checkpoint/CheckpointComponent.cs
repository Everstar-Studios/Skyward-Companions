using System;
using Skyward.Characters;
using Skyward.Core;
using Skyward.Systems;
using UnityEngine;
using UnityEngine.Events;

public class CheckpointComponent : MonoBehaviour, ISkywardComponent
{
    public Transform checkpointPositionOverride;
    public Collider trigger;
    public UnityEvent checkpointReachedEvent;
    
    public DeathZoneComponent DeathZone { get; private set; }
    public Vector3 Position => checkpointPositionOverride != null ? checkpointPositionOverride.position : transform.position;

    private bool activated;

    private void Awake()
    {
        DeathZone = GetComponentInChildren<DeathZoneComponent>();
        trigger.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (activated)
            return;
        if (!other.TryGetComponent(out PlayerController player))
            return;

        CheckpointSystem.OnCheckpointReached(this, player);
        checkpointReachedEvent.Invoke();
        activated = true;
    }

    public void ActivateDeathZone() => DeathZone.gameObject.SetActive(true);
}

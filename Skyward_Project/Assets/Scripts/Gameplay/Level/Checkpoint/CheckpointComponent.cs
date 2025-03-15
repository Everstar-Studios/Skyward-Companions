using System;
using Skyward.Characters;
using Skyward.Core;
using Skyward.Systems;
using UnityEngine;
using UnityEngine.Events;

public class CheckpointComponent : MonoBehaviour, ISkywardComponent
{
    public DeathZoneComponent deathZone;
    public Transform checkpointPositionOverride;
    public Collider trigger;
    public UnityEvent checkpointReachedEvent;
    
    public Vector3 Position => checkpointPositionOverride != null ? checkpointPositionOverride.position : transform.position;

    private bool activated;

    private void Awake()
    {
        if (deathZone == null)
            Debug.LogException(new Exception($"{gameObject.name} has no death zone attached!! You need to set this"));
        
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

    public void ActivateDeathZone() => deathZone.gameObject.SetActive(true);
}

using System;
using Skyward.Characters;
using Skyward.Systems;
using UnityEngine;
using UnityEngine.Events;

public class CheckpointComponent : MonoBehaviour
{
    public Transform checkpointPositionOverride;
    public Collider trigger;
    public UnityEvent checkpointReachedEvent;

    private bool activated;

    private void Awake()
    {
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
}

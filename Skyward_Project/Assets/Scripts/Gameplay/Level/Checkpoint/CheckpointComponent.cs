using System;
using Skyward.Characters;
using Skyward.Systems;
using UnityEngine;

public class CheckpointComponent : MonoBehaviour
{
    public Collider trigger;

    private void Awake()
    {
        trigger.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out PlayerController player))
            return;

        CheckpointSystem.OnCheckpointReached(player);
    }
}

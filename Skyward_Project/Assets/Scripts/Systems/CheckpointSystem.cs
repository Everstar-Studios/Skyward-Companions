using System;
using System.Collections;
using System.Collections.Generic;
using Skyward.Characters;
using Skyward.Core;
using UnityEngine;

namespace Skyward.Systems
{
    public class DeathZoneReachedEventArgs : EventArgs
    {
        public float waitTime = 0f;
    }
    
    [RequiredSystem]
    public class CheckpointSystem : BaseSystem<CheckpointSystem>, ISkywardComponent
    {
        private List<DeathZoneComponent> deathZoneComponents = new();
        
        private Vector3 lastCheckpointPosition;
        public static Vector3 LastCheckpointPosition => Instance.lastCheckpointPosition;

        private static Transform player;

        private bool worldLoaded = false;
        private bool respawningInProgress = false;
        
        public static event EventHandler<DeathZoneReachedEventArgs> DeathZoneReached
        {
            add => Instance.deathZoneReached += value;
            remove => Instance.deathZoneReached -= value;
        }

        private event EventHandler<DeathZoneReachedEventArgs> deathZoneReached;
        
        void ISkywardComponent.WorldLoaded()
        {
            worldLoaded = true;
            player = PlayerSystem.Player.transform;
            lastCheckpointPosition = player.position;
        }
        
        public static void OnCheckpointReached(PlayerController player)
        {
            Instance.lastCheckpointPosition = player.transform.position;
        }

        private void RespawnFromLastCheckpoint()
        {
            Instance.StartCoroutine(RespawnFlow());
        }

        private IEnumerator RespawnFlow()
        {
            respawningInProgress = true;
            var args = new DeathZoneReachedEventArgs();
            deathZoneReached?.Invoke(this, args);
            if (args.waitTime > float.Epsilon)
                yield return new WaitForSeconds(args.waitTime);
            
            PlayerSystem.Player.player.Teleport(lastCheckpointPosition);
            respawningInProgress = false;
        }

        public static void AddDeathZone(DeathZoneComponent deathZoneComponent)
        {
            Instance.deathZoneComponents.Add(deathZoneComponent);
        }

        private void Update()
        {
            if (respawningInProgress || !worldLoaded)
                return;

            foreach (DeathZoneComponent deathZone in deathZoneComponents)
            {
                bool enteredDeathZone = deathZone.trigger.bounds.Contains(player.position);
                if (enteredDeathZone)
                {
                    RespawnFromLastCheckpoint();
                    break;
                }
            }
        }
    }
}

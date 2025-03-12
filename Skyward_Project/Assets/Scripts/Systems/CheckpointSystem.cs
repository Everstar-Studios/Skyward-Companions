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

        private Vector3 PlayerColliderCenter => PlayerSystem.Player.player.Collider.bounds.center;
        
        public static event EventHandler<DeathZoneReachedEventArgs> DeathZoneReached
        {
            add => Instance.deathZoneReached += value;
            remove => Instance.deathZoneReached -= value;
        }

        private event EventHandler<DeathZoneReachedEventArgs> deathZoneReached;
        
        public static event EventHandler CheckpointReached
        {
            add => Instance.checkPointReached += value;
            remove => Instance.checkPointReached -= value;
        }

        private event EventHandler checkPointReached;
        
        void ISkywardComponent.WorldLoaded()
        {
            worldLoaded = true;
            player = PlayerSystem.Player.transform;
            lastCheckpointPosition = player.position;
        }
        
        public static void OnCheckpointReached(CheckpointComponent checkpoint, PlayerController player)
        {
            Instance.lastCheckpointPosition = checkpoint.checkpointPositionOverride != null ? checkpoint.checkpointPositionOverride.position : player.transform.position;
            Instance.checkPointReached?.Invoke(Instance, EventArgs.Empty);
        }

        public static void RespawnFromLastCheckpoint()
        {
            Instance.StartCoroutine(Instance.RespawnFlow());
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

            Vector3 playerPosition = player.position;
            foreach (DeathZoneComponent deathZone in deathZoneComponents)
            {
                Vector3 closestPoint = deathZone.trigger.ClosestPoint(playerPosition);
                bool enteredDeathZone = Vector3.Distance(closestPoint, PlayerColliderCenter) < 0.5f;
                if (enteredDeathZone)
                {
                    RespawnFromLastCheckpoint();
                    break;
                }
            }
        }
    }
}

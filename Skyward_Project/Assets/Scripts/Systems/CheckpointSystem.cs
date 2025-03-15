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
        public float timeToTeleportPlayer = 0f;
        public float timeToReEnableInput = 0f;
    }
    
    [RequiredSystem]
    public class CheckpointSystem : BaseSystem<CheckpointSystem>, ISkywardComponent
    {
        private List<DeathZoneComponent> deathZoneComponents = new();

        private Vector3 playerSpawnPosition;
        
        public Vector3 LastCheckpointPosition => activeCheckpoint != null ? activeCheckpoint.Position : playerSpawnPosition;

        private static Transform player;

        private bool canUpdate = false;
        private bool respawningInProgress = false;

        private Vector3 PlayerColliderCenter => PlayerSystem.Player.player.Collider.bounds.center;
        
        public static event EventHandler<DeathZoneReachedEventArgs> DeathZoneReached
        {
            add => Instance.deathZoneReached += value;
            remove => Instance.deathZoneReached -= value;
        }

        private event EventHandler<DeathZoneReachedEventArgs> deathZoneReached;

        private List<CheckpointComponent> checkpoints = new();
        private CheckpointComponent activeCheckpoint;
        private DeathZoneComponent activeDeathZone;
        public static CheckpointComponent ActiveCheckpoint => Instance.activeCheckpoint;
        
        public static event EventHandler CheckpointReached
        {
            add => Instance.checkPointReached += value;
            remove => Instance.checkPointReached -= value;
        }

        private event EventHandler checkPointReached;
        
        void ISkywardComponent.WorldLoaded()
        {
            player = PlayerSystem.Player.transform;
            playerSpawnPosition = player.position;
            foreach (var checkpoint in ComponentSystem.GetAllComponents<CheckpointComponent>())
            {
                checkpoints.Add(checkpoint);
                checkpoint.DeathZone.gameObject.SetActive(false);
            }

            int defaultCounter = 0;
            foreach (var deathZone in ComponentSystem.GetAllComponents<DeathZoneComponent>())
            {
                deathZoneComponents.Add(deathZone);
                if (deathZone.transform.parent == null)
                {
                    activeDeathZone = deathZone;
                    defaultCounter++;
                }
            }

            if (defaultCounter != 1)
            {
                string part = defaultCounter == 0 ? "no death zones" : "more than one death zones";
                Debug.LogError($"There are {part} with 'isDefault' boolean set to true in the scene!! Ensure there is one");
            }
            
            canUpdate = activeDeathZone != null;
        }
        
        public static void OnCheckpointReached(CheckpointComponent checkpoint, PlayerController player)
        {
            Instance.checkPointReached?.Invoke(Instance, EventArgs.Empty);

            Instance.deathZoneComponents.ForEach(d => d.gameObject.SetActive(false));
            
            Instance.activeCheckpoint = checkpoint;
            Instance.activeDeathZone = checkpoint.DeathZone;
            checkpoint.ActivateDeathZone();
        }

        private static void RespawnFromLastCheckpoint()
        {
            Instance.StartCoroutine(Instance.RespawnFlow());
        }

        private IEnumerator RespawnFlow()
        {
            GameInputSystem.DisableInput();
            respawningInProgress = true;
            var args = new DeathZoneReachedEventArgs();
            deathZoneReached?.Invoke(this, args);
            if (args.timeToTeleportPlayer > float.Epsilon)
                yield return new WaitForSeconds(args.timeToTeleportPlayer);
            
            PlayerSystem.Player.player.Teleport(LastCheckpointPosition);
            
            if (args.timeToReEnableInput > float.Epsilon)
                yield return new WaitForSeconds(args.timeToReEnableInput);
            
            GameInputSystem.EnableInput();
            respawningInProgress = false;
        }

        private void Update()
        {
            if (!canUpdate || respawningInProgress)
                return;
            
            Vector3 closestPoint = activeDeathZone.trigger.ClosestPoint(player.position);
            bool enteredDeathZone = Vector3.Distance(closestPoint, PlayerColliderCenter) < 0.5f;
            if (enteredDeathZone)
                RespawnFromLastCheckpoint();
        }
    }
}

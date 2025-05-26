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
    public class CheckpointSystem : BaseSystem<CheckpointSystem>
    {
        private List<DeathZoneComponent> deathZoneComponents = new();

        private Vector3 playerSpawnPosition;
        
        public Vector3 LastCheckpointPosition => activeCheckpoint != null ? activeCheckpoint.Position : playerSpawnPosition;

        private Transform player;

        private bool canUpdate = false;
        private bool respawningInProgress = false;
        
        public static bool IsReady => Instance.canUpdate;
        
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

        protected override void WorldLoading(GameContext context)
        {
            base.WorldLoading(context);

            PlayerSystem.PlayerFound += OnPlayerSpawned;
        }
        
        protected override void Cleanup()
        {
            deathZoneComponents.Clear();
            checkpoints.Clear();
            activeDeathZone = null;
            activeCheckpoint = null;
            
            PlayerSystem.PlayerFound -= OnPlayerSpawned;
            canUpdate = false;
        }

        private void OnPlayerSpawned(object sender, PlayerController player)
        {
            this.player = player.transform;
            playerSpawnPosition = this.player.position;
            foreach (var checkpoint in ComponentSystem.GetAllComponents<CheckpointComponent>())
            {
                checkpoints.Add(checkpoint);
                checkpoint.DeathZone.gameObject.SetActive(false);
            }

            int defaultCounter = 0;
            foreach (var deathZone in ComponentSystem.GetAllComponents<DeathZoneComponent>())
            {
                deathZoneComponents.Add(deathZone);
                Transform parent = deathZone.transform.parent;
                if (parent == null || !parent.TryGetComponent(out CheckpointComponent _))
                {
                    // Default death zone
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

        public static void RespawnFromLastCheckpoint()
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

        public static void EnteredDeathZone(DeathZoneComponent deathZone, PlayerController player)
        {
            ConfigSystem.GetConfig<PlayerConfig>().deathSound.Play(player.transform.position);
            RespawnFromLastCheckpoint();
        }
    }
}

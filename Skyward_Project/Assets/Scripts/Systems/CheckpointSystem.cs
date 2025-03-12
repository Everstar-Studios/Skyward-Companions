using Skyward.Characters;
using Skyward.Core;
using UnityEngine;

namespace Skyward.Systems
{
    [RequiredSystem]
    public class CheckpointSystem : BaseSystem<CheckpointSystem>
    {
        private Vector3 lastCheckpointPosition;
        public static Vector3 LastCheckpointPosition => Instance.lastCheckpointPosition;
        
        public static void OnCheckpointReached(PlayerController player)
        {
            Instance.lastCheckpointPosition = player.transform.position;
        }

        public static void AddDeathZone(DeathZoneComponent deathZoneComponent)
        {
            Instance.deathZoneComponents.Add(deathZoneComponent);
        }
    }
}

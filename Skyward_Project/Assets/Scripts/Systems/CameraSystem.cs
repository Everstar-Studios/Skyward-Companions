using FMODUnity;
using Skyward.Characters;
using Skyward.Core;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

namespace Skyward.Systems
{
    [RequiredSystem]
    public class CameraSystem : BaseSystem<CameraSystem>
    {
        private Camera mainCamera;
        public static Camera Camera => Instance.mainCamera;

        public static CinemachineBrain Brain => CinemachineCore.FindPotentialTargetBrain(MainVirtualCamera);
        public static bool HasBrain => Brain != null;

        private CinemachineCamera mainVirtualCamera;
        public static CinemachineCamera MainVirtualCamera => Instance.mainVirtualCamera;

        private StudioListener fmodStudioListener;

        protected override void Awake()
        {
            base.Awake();

            mainCamera = GetComponent<Camera>();
            fmodStudioListener = GetComponent<StudioListener>();
        }

        protected override void Initialize(GameContext context)
        {
            base.Initialize(context);
            
            PlayerSystem.PlayerFound += PlayerFound;
        }

        private void PlayerFound(object sender, PlayerController player)
        {
            fmodStudioListener.AttenuationObject = player.gameObject;
        }

        protected override void Cleanup()
        {
            base.Cleanup();
            
            PlayerSystem.PlayerFound -= PlayerFound;
        }

        protected override void WorldLoaded(GameContext context)
        {
            base.WorldLoaded(context);
            
        }

        public static void SetCamera(CinemachineCamera cinemachineCamera)
        {
            Instance.mainVirtualCamera = cinemachineCamera;
        }

        public static void EnableCamera()
        {
            Instance.mainCamera.enabled = true;
        }

        public static void DisableCamera()
        {
            Instance.mainCamera.enabled = false;
        }

        public static void SetupFollowTarget(Transform follow)
        {
            MainVirtualCamera.Follow = follow;
        }
    }
}


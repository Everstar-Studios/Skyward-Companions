using Skyward.Core;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

namespace Skyward.Systems
{
    // [RequiredSystem]
    public class CameraSystem : BaseSystem<CameraSystem>
    {
        private Camera mainCamera;
        public static Camera Camera => Instance.mainCamera;

        public static CinemachineBrain Brain => CinemachineCore.FindPotentialTargetBrain(MainVirtualCamera);
        public static bool HasBrain => Brain != null;

        private CinemachineCamera mainVirtualCamera;
        public static CinemachineCamera MainVirtualCamera => Instance.mainVirtualCamera;

        protected override void Awake()
        {
            base.Awake();
            
            mainCamera = GetComponent<Camera>();
            mainVirtualCamera = mainCamera.GetComponentInChildren<CinemachineCamera>();
        }

        protected override void Cleanup()
        {
            base.Cleanup();
            
            DestroyImmediate(mainCamera.gameObject);
        }

        public static void EnableCamera()
        {
            MainVirtualCamera.gameObject.SetActive(true);
        }

        public static void DisableCamera()
        {
            MainVirtualCamera.gameObject.SetActive(false);
        }

        public static void SetupFollowTarget(Transform follow)
        {
            MainVirtualCamera.Follow = follow;
        }
    }
}


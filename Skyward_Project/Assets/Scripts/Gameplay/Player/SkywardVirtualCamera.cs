using System;
using Skyward.Core;
using Skyward.Systems;
using Unity.Cinemachine;
using UnityEngine;

public class SkywardVirtualCamera : MonoBehaviour, ISkywardComponent
{
    private Vector2 lastInput;
    private Vector2 look;
    public float topClamp = 70;
    public float bottomClamp = -30;
    
    private CinemachineCamera cinemachineCamera;
    
    private float cinemachineTargetYaw;
    private float cinemachineTargetPitch;
    private CinemachineOrbitalFollow orbitalFollowComponent;

    void ISkywardComponent.WorldLoaded(GameContext context)
    {
        GameInputSystem.OnLook += OnLook;
    }

    void ISkywardComponent.Cleanup()
    {
        GameInputSystem.OnLook -= OnLook;
    }

    private void Awake()
    {
        orbitalFollowComponent = GetComponent<CinemachineOrbitalFollow>();
    }

    private void OnLook(object sender, Vector2 value)
    {
        look = value;
    }
    
    private void Start()
    {
        cinemachineCamera = gameObject.GetComponent<CinemachineCamera>();
        CameraSystem.SetCamera(cinemachineCamera);
    }

    void LateUpdate()
    {
        CameraRotation();
    }
    
    private void CameraRotation()
    {
        if (look.sqrMagnitude >= 0.001f)
        {
            cinemachineTargetYaw += look.x * Time.deltaTime;
            cinemachineTargetPitch += look.y * Time.deltaTime;
        }
        
        cinemachineTargetYaw = ClampAngle(cinemachineTargetYaw, float.MinValue, float.MaxValue);
        cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, bottomClamp, topClamp);

        orbitalFollowComponent.HorizontalAxis.Value = cinemachineTargetYaw;
        orbitalFollowComponent.VerticalAxis.Value = cinemachineTargetPitch;
    }
    
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}

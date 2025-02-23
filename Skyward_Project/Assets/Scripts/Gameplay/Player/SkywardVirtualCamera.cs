using System;
using Skyward.Systems;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkywardVirtualCamera : MonoBehaviour
{
    public InputActionReference inputActionReference;
    
    private Vector2 lastInput;
    private float lastX;
    private float lastY;
    
    private CinemachineCamera cinemachineCamera;
    private CinemachineOrbitalFollow orbitalFollowComponent;

    private void Awake()
    {
        orbitalFollowComponent = GetComponent<CinemachineOrbitalFollow>();
        lastX = orbitalFollowComponent.HorizontalAxis.Value;
        lastY = orbitalFollowComponent.VerticalAxis.Value;
    }

    private void Start()
    {
        cinemachineCamera = gameObject.GetComponent<CinemachineCamera>();
        CameraSystem.SetCamera(cinemachineCamera);
    }

    void Update()
    {
        UpdateTouchInput();
    }

    private void UpdateTouchInput()
    {
        bool isUsingTouch = Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed;
        if (isUsingTouch == false)
            return;
        
        Vector2 input = inputActionReference.action.ReadValue<Vector2>();
        
        Vector2 movementDelta = input - lastInput;
        lastInput = input;

        lastX = orbitalFollowComponent.HorizontalAxis.Value;
        lastY = orbitalFollowComponent.VerticalAxis.Value;
        if (movementDelta.magnitude > float.Epsilon)
            return;

        orbitalFollowComponent.HorizontalAxis.Value = lastX;
        orbitalFollowComponent.VerticalAxis.Value = lastY;
    }
}

using System;
using Skyward.Core;
using Skyward.Systems;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkywardVirtualCamera : MonoBehaviour, ISkywardComponent
{
    private Vector2 lastInput;
    private float lastX;
    private float lastY;
    
    private CinemachineCamera cinemachineCamera;
    private CinemachineOrbitalFollow orbitalFollowComponent;
    private CinemachineInputAxisController controller;
    
    private bool IsTouchingScreen => Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed;

    void ISkywardComponent.WorldLoaded(GameContext context)
    {
        gameObject.SetActive(true);
    }

    private void Awake()
    {
        orbitalFollowComponent = GetComponent<CinemachineOrbitalFollow>();
        controller = GetComponent<CinemachineInputAxisController>();
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
        Vector2 input = new Vector2(controller.Controllers[0].InputValue, controller.Controllers[1].InputValue);
        
        Vector2 movementDelta = input - lastInput;
        lastInput = input;
        
        bool isStop = movementDelta.magnitude <= float.Epsilon;
        if (!isStop)
        {
            lastX = orbitalFollowComponent.HorizontalAxis.Value;
            lastY = orbitalFollowComponent.VerticalAxis.Value;
            return;
        }
        
        if (!IsTouchingScreen)
            return;

        orbitalFollowComponent.HorizontalAxis.Value = lastX;
        orbitalFollowComponent.VerticalAxis.Value = lastY;
    }
}

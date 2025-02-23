using System;
using Skyward.Systems;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputComponent : MonoBehaviour
{
    private PlayerInput playerInput;
    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool SprintInput { get; private set; }
    public bool JumpTriggered { get; private set; }
    
    private void OnMove(Vector2 value)
    {
        MoveInput = value;
    }

    private void OnLook(Vector2 value)
    {
        LookInput = value;
    }

    private void OnSprint(bool active)
    {
        SprintInput = active;
    }

    private void OnJump()
    {
        JumpTriggered = true;
    }

    private void LateUpdate()
    {
        JumpTriggered = false;
    }
}

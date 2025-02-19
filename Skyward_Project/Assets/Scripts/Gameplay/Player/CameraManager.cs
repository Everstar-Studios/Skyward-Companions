using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    public CinemachineInputAxisController inputController;
    public InputActionReference inputActionReference;
    public CinemachineOrbitalFollow cam;
    
    private Vector2 lastInput;

    void Update()
    {
        Vector2 input = inputActionReference.action.ReadValue<Vector2>();
        
        bool isUsingTouch = Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed;
        Vector2 movementDelta = input - lastInput;
        if (isUsingTouch)
            inputController.enabled = movementDelta.magnitude > float.Epsilon;

        lastInput = input;
    }
}

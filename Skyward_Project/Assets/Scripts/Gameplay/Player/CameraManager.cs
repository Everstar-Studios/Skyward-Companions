using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    public CinemachineInputAxisController inputController;
    public InputActionReference inputActionReference;
    public CinemachineOrbitalFollow cam;
    
    private Vector2 lastInput;
    private float lastX;
    private float lastY;

    void Update()
    {
        bool isUsingTouch = Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed;
        if (isUsingTouch == false)
            return;
        
        Vector2 input = inputActionReference.action.ReadValue<Vector2>();
        
        Vector2 movementDelta = input - lastInput;
        lastInput = input;
        
        bool isStop = movementDelta.magnitude <= float.Epsilon;
        if (!isStop)
        {
            lastX = cam.HorizontalAxis.Value;
            lastY = cam.VerticalAxis.Value;
            return;
        }

        cam.HorizontalAxis.Value = lastX;
        cam.VerticalAxis.Value = lastY;
    }
}

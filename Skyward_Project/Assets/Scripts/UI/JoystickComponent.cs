using System;
using Skyward.Core;
using Skyward.Systems;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.OnScreen;

public class JoystickComponent : OnScreenStick, ISkywardComponent
{
    private PointerEventData currentPointerEventData;
    
    private void Start() 
    {
        GameInputSystem.InputDisabled += InputDisabled;
    }

    void ISkywardComponent.Cleanup()
    {
        GameInputSystem.InputDisabled -= InputDisabled;
    }

    private void InputDisabled(object sender, EventArgs e)
    {
        if (TimeSystem.TimeInLevel >= float.Epsilon)
            OnPointerUp(null);
    }
}

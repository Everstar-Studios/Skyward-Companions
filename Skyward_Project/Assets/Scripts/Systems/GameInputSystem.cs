using System;
using System.Collections.Generic;
using Skyward.Core;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Skyward.Systems
{
    [RequiredSystem]
    public class GameInputSystem : BaseSystem<GameInputSystem>
    {
        public static List<IInputActionCollection2> Inputs => Instance.inputs;
        private List<IInputActionCollection2> inputs = new();
        
        public static event EventHandler OnMove
        {
            add => Instance.onMove += value;
            remove => Instance.onMove -= value;
        }

        private event EventHandler onMove;
        
        public static event EventHandler InputDisabled
        {
            add => Instance.inputDisabled += value;
            remove => Instance.inputDisabled -= value;
        }

        private event EventHandler inputDisabled;
        
        public static void EnableInput()
        {
            Inputs.ForEach((i) => i.Enable());
        }
        
        public static void DisableInput()
        {
            Inputs.ForEach((i) => i.Disable());
            Instance.inputDisabled?.Invoke(Instance, EventArgs.Empty);
        }

        public static void AddInputAction(IInputActionCollection2 inputAction)
        {
            Inputs.Add(inputAction);
        }

        protected override void Cleanup()
        {
            base.Cleanup();
            
            Instance.inputs.Clear();
        }

        public static void OnMoved(Vector2 value)
        {
            Instance.onMove?.Invoke(Instance, EventArgs.Empty);
        }
    }
}

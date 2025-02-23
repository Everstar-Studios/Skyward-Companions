using System;
using System.Collections.Generic;
using Skyward.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Skyward.Systems
{
    [RequiredSystem]
    public class GameInputSystem : BaseSystem<GameInputSystem>
    {
        public static List<IInputActionCollection2> Inputs => Instance.inputs;
        private List<IInputActionCollection2> inputs = new();
        public static void EnableInput()
        {
            Inputs.ForEach((i) => i.Enable());
        }
        
        public static void DisableInput()
        {
            Inputs.ForEach((i) => i.Disable());
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
    }
}

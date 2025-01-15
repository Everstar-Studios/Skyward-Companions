using System;
using Skyward.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Skyward.Systems
{
    public class GameInputSystem : BaseSystem<GameInputSystem>
    {
        private PlayerInput playerInput;

        public static PlayerInput PlayerInput => Instance.playerInput;
        
        // TODO OK: Implement generic commands for input actions
        public static event Action<Vector2> onMove;
        public static event Action<Vector2> onLook;
        public static event Action onInteract;
        public static event Action onJump;
        public static event Action<bool> onSprint;
        public static event Action onTogglePOV;

        protected override void Awake()
        {
            base.Awake();

            playerInput = GetComponent<PlayerInput>();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            onMove?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            Debug.Log(UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId));
            onLook?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnInteract(InputValue value)
        {
            onInteract?.Invoke();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            onJump?.Invoke();
        }

        public void OnSprint(InputValue value)
        {
            onSprint?.Invoke(value.isPressed);
        }

        public void OnTogglePOV(InputValue value)
        {
            onTogglePOV?.Invoke();
        }
    }
}

#define inputsystem
using System;
using System.Collections;
using System.Collections.Generic;
using Skyward.Core;
using Skyward.Systems;
using UnityEngine;


namespace Skyward.Characters
{
    public class LocomotionInputManager : MonoBehaviour, ISkywardComponent
    {
        [Header("Keys")]
        [SerializeField] KeyCode jumpKey = KeyCode.Space;
        [SerializeField] KeyCode dropKey = KeyCode.E;
        [SerializeField] KeyCode moveType = KeyCode.Tab;
        [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;
        [SerializeField] KeyCode interactionKey = KeyCode.E;


        [Header("Buttons")]
        [SerializeField] string jumpButton;
        [SerializeField] string dropButton;
        [SerializeField] string moveTypeButton;
        [SerializeField] string sprintButton;
        [SerializeField] string interactionButton;

        public bool JumpKeyDown { get; set; }
        public bool Drop { get; set; }
        public Vector2 DirectionInput { get; set; }
        public Vector2 CameraInput { get; set; }
        public bool ToggleRun { get; set; }
        public bool SprintKey { get; set; }
        public bool Interaction { get; set; }

        public event Action OnInteractionPressed;
        public event Action<float> OnInteractionReleased;

        public float InteractionButtonHoldTime { get; set; } = 0f;
        bool interactionButtonDown;
        
        LocomotionInputAction input;

        void Start()
        {
            input = new LocomotionInputAction();
            input.Enable();
            GameInputSystem.AddInputAction(input);
        }

        private void OnDisable()
        {
            input.Disable();
        }


        private void Update()
        {
            //Horizontal and Vertical Movement
            HandleDirectionalInput();

            //Camera Movement
            HandlecameraInput();

            //JumpKeyDown
            HandleJumpKeyDown();

            //Drop
            HandleDrop();

            //Walk or Run 
            HandleToggleRun();

            //Sprint
            HandleSprint();

            //Interaction
            HandleInteraction();
        }

        void HandleDirectionalInput()
        {
            DirectionInput = input.Locomotion.MoveInput.ReadValue<Vector2>();
            if (DirectionInput.magnitude > float.Epsilon)
                GameInputSystem.OnMoved(DirectionInput);
        }

        void HandlecameraInput()
        {
            CameraInput = input.Locomotion.CameraInput.ReadValue<Vector2>();
        }

        void HandleJumpKeyDown()
        {
            JumpKeyDown = input.Locomotion.Jump.WasPressedThisFrame();
        }

        void HandleDrop()
        {
            Drop = input.Locomotion.Drop.inProgress;
        }

        void HandleToggleRun()
        {
            ToggleRun = input.Locomotion.MoveType.WasPressedThisFrame();
        }

        void HandleSprint()
        {
            SprintKey = input.Locomotion.SprintKey.inProgress;
        }

        void HandleInteraction()
        {
            if (input.Locomotion.Interaction.WasPressedThisFrame())
            {
                interactionButtonDown = true;
                Interaction = true;
            }
            else
            {
                Interaction = false;
            }

            if (interactionButtonDown)
            {
                if (input.Locomotion.Interaction.WasReleasedThisFrame())
                {
                    interactionButtonDown = false;
                    InteractionButtonHoldTime = 0f;
                }

                InteractionButtonHoldTime += Time.deltaTime;
            }
        }
    }
}
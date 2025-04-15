#define inputsystem
using System;
using Skyward.Core;
using Skyward.Systems;
using UnityEngine;

namespace FS_ParkourSystem
{
    public partial class ParkourInputManager : MonoBehaviour, ISkywardComponent
    {
        [Header("Keys")]
        [SerializeField] KeyCode jumpKey = KeyCode.Space;
        [SerializeField] KeyCode dropKey = KeyCode.E;
        [SerializeField] KeyCode jumpFromHangKey = KeyCode.Q;


        [Header("Buttons")]
        [SerializeField] string jumpButton;
        [SerializeField] string dropButton;
        [SerializeField] string jumpFromHangButton;

        public bool Jump { get; set; }
        public bool JumpKeyDown { get; set; }
        public bool Drop { get; set; }
        public bool JumpFromHang { get; set; }

        ParkourInputAction input;
        
        void Start()
        {
            input = new ParkourInputAction();
            input.Enable();
            GameInputSystem.AddInputAction(input);
        }
        
        private void OnDisable()
        {
            input.Disable();
        }

        private void Update()
        {
            //Jump
            HandleJump();

            //JumpKeyDown
            HandleJumpKeyDown();

            //Drop
            HandleDrop();

            //JumpFromHang
            HandleJumpFromHang();
        }

        void HandleJump()
        {
            Jump = input.Parkour.Jump.inProgress;

        }

        void HandleJumpKeyDown()
        {
            JumpKeyDown = input.Parkour.Jump.WasPressedThisFrame();
        }

        void HandleDrop()
        {
            Drop = input.Parkour.Drop.inProgress;
        }

        void HandleJumpFromHang()
        {
            JumpFromHang = input.Parkour.JumpFromHang.inProgress;
        }
    }
}

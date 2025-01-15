using Skyward.Systems;
using UnityEngine;
using Skyward.Movement;

public partial class Player
{
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int Grounded = Animator.StringToHash("Grounded");

    public class WalkingMoveState : MoveState
    {
        private Player player;
        private Animator animator;
        public WalkingMoveState(MovementComponent owner) : base(owner)
        {
            player = owner.GetComponent<Player>();
            animator = owner.GetComponentInChildren<Animator>();
        }

        protected override bool CanEnter(MovementContext context) => player.inputComponent.MoveInput != Vector2.zero;
        public override bool CanExit(MovementContext context) => player.inputComponent.MoveInput == Vector2.zero;

        protected override void Update(MovementContext context)
        {
            base.Update(context);
            
            Transform cameraTransform = CameraSystem.Camera.transform;
            Vector2 moveInput = player.inputComponent.MoveInput;
            
            Vector3 desiredMovement = (cameraTransform.right * moveInput.x + cameraTransform.forward * moveInput.y) * context.speed;
            desiredMovement.y = 0;

            if (context.IsGrounded)
            {
                context.velocity = new Vector3(desiredMovement.x, context.velocity.y, desiredMovement.z);
            }
            else
            {
                // Implement air control
                Vector3 horizontalVelocity = new Vector3(context.velocity.x, 0, context.velocity.z);
                Vector3 desiredHorizontalMovement = new Vector3(desiredMovement.x, 0, desiredMovement.z);

                // Blend current velocity with desired movement based on controlPercentage
                Vector3 airControlMovement = Vector3.Lerp(horizontalVelocity, desiredHorizontalMovement, player.GetControlPercentage());

                // Apply the air control adjustment while preserving vertical velocity
                context.velocity = new Vector3(airControlMovement.x, context.velocity.y, airControlMovement.z);
            }
            
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z));
            context.transform.rotation = Quaternion.Slerp(context.transform.rotation, targetRotation, Time.deltaTime * 10);
        }
    }

}

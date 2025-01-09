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
            
            Vector3 direction = CameraSystem.Camera.transform.forward;
            Vector2 moveInput = player.inputComponent.MoveInput;
            Vector3 rightDirection = -Vector3.Cross(direction, Vector3.up);
            
            Vector3 desiredMovement = (rightDirection * moveInput.x + direction * moveInput.y) * context.speed;

            if (context.IsGrounded)
            {
                // Normal walking behavior
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
            
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            context.transform.rotation = Quaternion.Slerp(context.transform.rotation, targetRotation, Time.deltaTime * 10);
        }

        protected override void OnExit(MovementContext context)
        {
            base.OnExit(context);

            // animator.SetFloat(Speed, 0f);
        }
    }

}

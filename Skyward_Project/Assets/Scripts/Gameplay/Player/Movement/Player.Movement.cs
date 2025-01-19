using System.Collections.Generic;
using Skyward.Systems;
using Skyward.Movement;
using UnityEngine;

public partial class Player : ISpeedProvider
{
    private MovementComponent movementComponent;

    void SetupMovement()
    {
        movementComponent = GetComponent<MovementComponent>();
        List<State> states = new List<State>
        {
            new JumpingMoveState(movementComponent),
            new WalkingMoveState(movementComponent),
            new IdleMoveState(movementComponent),
        };

        movementComponent.Initialize(states);
    }

    float ISpeedProvider.GetSpeed()
    {
        return movementComponent.Speed;
    }

    public float GetControlPercentage()
    {
        if (movementComponent.InAir)
            return controlPercentageDuringJump;
        
        return 1;
    }
}

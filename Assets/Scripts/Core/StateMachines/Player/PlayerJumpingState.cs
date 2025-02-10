using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpingState : PlayerBaseState
{
    private Vector3 momentum;
    private bool isFalling = false;
    public PlayerJumpingState(PlayerStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        momentum = stateMachine.Momentum;
        momentum.y = 0f;

        stateMachine.ForceReceiver.ResetVerticalVelocity();

        stateMachine.ForceReceiver.Jump(stateMachine.JumpForce);
    }

    public override void Tick(float deltaTime)
    {
        if (stateMachine.ForceReceiver.VerticalVelocity <= 0f)
        {
            isFalling = true;
        }

        Move(momentum, deltaTime);

        if (isFalling && stateMachine.Controller.isGrounded)
        {
            ReturnToLocomotion();
        }

        Vector3 movement = CalculateMovement();

        FaceMovementDirection(movement, deltaTime);
        
    }

    public override void Exit()
    {
        // stateMachine.Animator.ResetTrigger("Jump");
    }
}

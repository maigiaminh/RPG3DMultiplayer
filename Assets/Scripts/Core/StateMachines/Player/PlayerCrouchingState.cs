using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrouchingState : PlayerBaseState
{
    private const float CrossFadeDuration = 0.1f;
    private bool keyReleased;
    private int crouchHash;
    public PlayerCrouchingState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.InputReader.CrouchEvent += OnCrouch;
        stateMachine.InputReader.OpenMouseModeEvent += EnterInteractState;


        if (stateMachine.InputReader.MovementValue != Vector2.zero) keyReleased = false;

        crouchHash = stateMachine.AnimationController.GetCrouch((int) stateMachine.WeaponController.weaponLogic.weaponType);

        stateMachine.Animator.CrossFadeInFixedTime(crouchHash, CrossFadeDuration);
    }

    public override void Tick(float deltaTime)
    {
        Move(deltaTime);

        if(stateMachine.InputReader.MovementValue == Vector2.zero) {
            if (!keyReleased){
                keyReleased = true;
                return;
            }
            
            return; 
        }
        
        if(!keyReleased) { return; }

        stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
    }

    public override void Exit()
    {
        stateMachine.InputReader.CrouchEvent -= OnCrouch;
        stateMachine.InputReader.OpenMouseModeEvent -= EnterInteractState;
    }

    private void OnCrouch()
    {        
        stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
    }

    private void EnterInteractState(){
        stateMachine.SwitchState(new PlayerInteractingState(stateMachine, null));
    }    
}

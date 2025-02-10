using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerAimingState : PlayerBaseState
{
    public PlayerAimingState(PlayerStateMachine stateMachine) : base(stateMachine) { }
    private readonly int FreeLookSpeedHash = Animator.StringToHash("FreeLookSpeed");
    
    private const float AnimatorDampTime = 0.1f;
    private const float CrossFadeDuration = 0.1f;
    private int aimingHash;

    public override void Enter()
    {
        aimingHash = stateMachine.AnimationController.GetAiming((int)stateMachine.WeaponController.weaponLogic.weaponType);

        stateMachine.Animator.CrossFadeInFixedTime(aimingHash, CrossFadeDuration);
        stateMachine.AnimationController.StartAiming();
        
        stateMachine.PlayerCameraController.SwitchToAiming();        
        stateMachine.WeaponController.projectilePool.SetDamage(25 + Mathf.CeilToInt(PlayerStatManager.Instance.Strength / 10));

    }

    public override void Tick(float deltaTime)
    {
        if(!stateMachine.InputReader.IsSecondaryPerforming) {
            ReturnToLocomotion();
        }

        RotatePlayerToCamera();
        if(stateMachine.InputReader.IsAttacking)
        {
            stateMachine.AnimationController.GetWeapon((int)stateMachine.WeaponController.weaponLogic.weaponType);
            float normalizedTime = GetNormalizedTime(stateMachine.Animator, "Attack", 1);
            if(normalizedTime < 0.7f){
                stateMachine.AnimationController.EnableRunningAttack();
            }
        }

        Vector3 movement = CalculateMovement();
        Move(movement * stateMachine.FreeLookMovementSpeed, deltaTime);

        if (stateMachine.InputReader.MovementValue == Vector2.zero)
        {
            stateMachine.Animator.SetFloat(FreeLookSpeedHash, 0, AnimatorDampTime, deltaTime);
            return;
        }

        stateMachine.Animator.SetFloat(FreeLookSpeedHash, 0.5f, AnimatorDampTime, deltaTime);
        
    }

    public override void Exit()
    {
        stateMachine.AnimationController.DoneAiming();
    }
}

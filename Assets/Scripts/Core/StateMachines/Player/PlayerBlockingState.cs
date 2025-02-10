using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlockingState : PlayerBaseState
{
    public PlayerBlockingState(PlayerStateMachine stateMachine) : base(stateMachine) { }
    private const float CrossFadeDuration = 0.1f;
    private int blockHash;
    public override bool CanTakeDamage() => false;

    public override void Enter()
    {
        blockHash = stateMachine.AnimationController.GetBlock((int)stateMachine.WeaponController.weaponLogic.weaponType);

        stateMachine.Animator.CrossFadeInFixedTime(blockHash, CrossFadeDuration);
    }

    public override void Tick(float deltaTime)
    {
        Move(deltaTime);
        FaceTarget();
        if(!stateMachine.InputReader.IsSecondaryPerforming) {
            ReturnToLocomotion();
        }
    }

    public override void Exit()
    {

    }

}

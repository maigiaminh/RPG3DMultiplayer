using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerResuscitatingState : PlayerBaseState
{
    public PlayerResuscitatingState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    private const float AnimatorDampTime = 0.1f;
    private int tiredHash;

    public override void Enter()
    {
        tiredHash = stateMachine.AnimationController.GetTired((int) stateMachine.WeaponController.weaponLogic.weaponType);

        stateMachine.Animator.CrossFadeInFixedTime(tiredHash, AnimatorDampTime);

        GameEventManager.Instance.PlayerEvents.PlayerTired();
    }

    public override void Tick(float deltaTime)
    {
        if(stateMachine.Stats.CurrentStamina >= (stateMachine.Stats.MaxStamina / 10f)) ReturnToLocomotion();
    }

    public override void Exit()
    {
        GameEventManager.Instance.PlayerEvents.PlayerNotTired();
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRollingState : PlayerBaseState
{
    private readonly int RollForwardHash = Animator.StringToHash("RollForward");
    private readonly int RollRightHash = Animator.StringToHash("RollRight");

    private float previousFrameTime;
    private Vector3 rollingDirectionInput;

    private const float CrossFadeDuration = 0.1f;
    private int blendTreeHash;
    public override bool CanTakeDamage() => false;

    public PlayerRollingState(PlayerStateMachine stateMachine, Vector3 rollingDirectionInput) : base(stateMachine)
    {
        this.rollingDirectionInput = rollingDirectionInput;
    }

    public override void Enter()
    {
        if(rollingDirectionInput.x > 0)  rollingDirectionInput = new Vector3(1, 0);

        else if(rollingDirectionInput.x < 0)  rollingDirectionInput = new Vector3(-1, 0);

        else if(rollingDirectionInput.y > 0) rollingDirectionInput = new Vector3(0, 1);

        else rollingDirectionInput = new Vector3(0, -1);

        blendTreeHash = stateMachine.AnimationController.GetRollBlendTree((int) stateMachine.WeaponController.weaponLogic.weaponType);
        stateMachine.Animator.SetFloat(RollForwardHash, rollingDirectionInput.y);
        stateMachine.Animator.SetFloat(RollRightHash, rollingDirectionInput.x);
        stateMachine.Animator.CrossFadeInFixedTime(blendTreeHash, CrossFadeDuration);

        SoundManager.PlaySound(SoundType.ROLL, stateMachine.Chest);
        
        stateMachine.Stats.SetInvulnerable(true);
    }

    public override void Tick(float deltaTime)
    {
        Vector3 movement = new Vector3();

        movement += stateMachine.transform.right * rollingDirectionInput.x * stateMachine.RollLength;
        movement += stateMachine.transform.forward * rollingDirectionInput.y * stateMachine.RollLength;

        float normalizedTime = GetNormalizedTime(stateMachine.Animator, "Roll", 0);

        Move(movement, deltaTime);

        FaceTarget();

        previousFrameTime = normalizedTime;

        if (normalizedTime >= previousFrameTime && normalizedTime < 0.6f) { return; }
  
        if (stateMachine.Targeter.CurrentTarget != null)
        {
            FaceTarget();
            stateMachine.SwitchState(new PlayerTargetingState(stateMachine));
        }
        else
        {
            stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
        }
    }

    public override void Exit()
    {
        stateMachine.Stats.SetInvulnerable(false);
    }
}

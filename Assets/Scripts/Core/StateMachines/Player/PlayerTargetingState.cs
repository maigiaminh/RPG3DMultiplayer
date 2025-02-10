using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargetingState : PlayerBaseState
{
    private readonly int TargetingForwardHash = Animator.StringToHash("TargetingForward");
    private readonly int TargetingRightHash = Animator.StringToHash("TargetingRight");

    private const float CrossFadeDuration = 0.1f;
    private int blendTreeHash;


    public PlayerTargetingState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.InputReader.TargetEvent += OnTarget;
        stateMachine.InputReader.DodgeEvent += OnDodge;
        stateMachine.SkillManager.PerformSkill += OnSkillPerform;

        // stateMachine.InputReader.JumpEvent += OnJump;
        stateMachine.InputReader.RollEvent += OnRoll;

        stateMachine.HeadAimRig.weight = 1;

        blendTreeHash = stateMachine.AnimationController.GetTargetingBlendTree((int) stateMachine.WeaponController.weaponLogic.weaponType);

        stateMachine.Animator.CrossFadeInFixedTime(blendTreeHash, CrossFadeDuration);

        stateMachine.PlayerCameraController.SwitchToTarget();
    }

    public override void Tick(float deltaTime)
    {
        if(stateMachine.Targeter.CurrentTarget != null)
            stateMachine.HeadAimTarget.transform.position = stateMachine.Targeter.CurrentTarget.transform.position;
        
        if(stateMachine.InputReader.IsAttacking)
        {
            stateMachine.SwitchState(new PlayerAttackingState(stateMachine, 0));
            return;
        }

        if(stateMachine.InputReader.IsSecondaryPerforming){
            stateMachine.SwitchState(new PlayerBlockingState(stateMachine));
            return;
        }

        if (stateMachine.Targeter.CurrentTarget == null)
        {
            stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
            return;
        }
        Vector3 movement = CalculateMovement(deltaTime);
        
        float boost = 0;
        
        if(PlayerStatManager.Instance.Agility > 0) boost = stateMachine.FreeLookMovementSpeed * PlayerStatManager.Instance.Agility / 1000;

        Move(movement * (stateMachine.TargetingMovementSpeed + boost), deltaTime);

        UpdateAnimator(deltaTime);

        FaceTarget();
    }

    public override void Exit()
    {
        stateMachine.InputReader.TargetEvent -= OnTarget;
        stateMachine.InputReader.DodgeEvent -= OnDodge;
        stateMachine.SkillManager.PerformSkill -= OnSkillPerform;

        // stateMachine.InputReader.JumpEvent -= OnJump;
        stateMachine.InputReader.RollEvent -= OnRoll;

        stateMachine.HeadAimRig.weight = 0;
    }

    private void OnTarget()
    {
        stateMachine.Targeter.Cancel();
        
        stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
    }

    private Vector3 CalculateMovement(float deltaTime)
    {
        Vector3 movement = new Vector3();

        movement += stateMachine.transform.right * stateMachine.InputReader.MovementValue.x;
        movement += stateMachine.transform.forward * stateMachine.InputReader.MovementValue.y;

        return movement;
    }

    private void UpdateAnimator(float deltaTime)
    {
        if (stateMachine.InputReader.MovementValue.y == 0)
        {
            stateMachine.Animator.SetFloat(TargetingForwardHash, 0, 0.1f, deltaTime);
        }
        else
        {
            float value = stateMachine.InputReader.MovementValue.y > 0 ? 1f : -1f;
            stateMachine.Animator.SetFloat(TargetingForwardHash, value, 0.1f, deltaTime);
        }

        if (stateMachine.InputReader.MovementValue.x == 0)
        {
            stateMachine.Animator.SetFloat(TargetingRightHash, 0, 0.1f, deltaTime);
        }
        else
        {
            float value = stateMachine.InputReader.MovementValue.x > 0 ? 1f : -1f;
            stateMachine.Animator.SetFloat(TargetingRightHash, value, 0.1f, deltaTime);
        }
    }

    private void OnDodge()
    {
        if (stateMachine.InputReader.MovementValue == Vector2.zero) { return; }

        if (stateMachine.InputReader.MovementValue.x == 0) { return; }
        
        stateMachine.SwitchState(new PlayerDodgingState(stateMachine, stateMachine.InputReader.MovementValue));
    }

    private void OnJump()
    {
        // if(stateMachine.Controller.isGrounded) 
        stateMachine.SwitchState(new PlayerJumpingState(stateMachine));
    }

    private void OnRoll(){
        if (stateMachine.InputReader.MovementValue == Vector2.zero) { return; }

        stateMachine.SwitchState(new PlayerRollingState(stateMachine, stateMachine.InputReader.MovementValue));
    }

    private void OnSkillPerform(CharacterSkillScriptableObject skill, int skillIndex){
        if(stateMachine.WeaponController.weaponLogic.weaponType == 0) return; 

        stateMachine.SwitchState(new PlayerPerformSkillState(stateMachine, skill, skillIndex));
    }
}

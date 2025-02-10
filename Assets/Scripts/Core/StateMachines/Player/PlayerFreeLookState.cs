using System;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class PlayerFreeLookState : PlayerBaseState
{
    private readonly int FreeLookSpeedHash = Animator.StringToHash("FreeLookSpeed");
    
    private const float AnimatorDampTime = 0.1f;
    private float runSpeed;
    private bool isCastSkill = false;
    private int skillPostion;
    // private float speed = 0.5f;
    private int blendTreeHash;
    private int changeWeaponHash;
    public PlayerFreeLookState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.InputReader.TargetEvent += OnTarget;
        stateMachine.InputReader.JumpEvent += OnJump;
        stateMachine.InputReader.CrouchEvent += OnCrouch;
        stateMachine.InputReader.TestEvent += OnTest;
        stateMachine.InputReader.ChangeWeaponEvent += OnChangeWeapon;
        stateMachine.InputReader.OpenMouseModeEvent += EnterInteractState;
        stateMachine.SkillManager.PerformSkill += OnSkillPerform;
        GameEventManager.Instance.PlayerEvents.OnPlayerInteractStateEnter += EnterInteractState;

        if(stateMachine.WeaponController.weaponLogic != null){
            blendTreeHash = stateMachine.AnimationController.GetFreeLookBlendTree((int) stateMachine.WeaponController.weaponLogic.weaponType);
        }
        else{
            blendTreeHash = stateMachine.AnimationController.GetFreeLookBlendTree(0);
        }

        stateMachine.Animator.CrossFadeInFixedTime(blendTreeHash, AnimatorDampTime);

        stateMachine.PlayerCameraController.SwitchToFreeLook();
    }
 
    public override void Tick(float deltaTime)
    {   
        float changeWeaponTime = GetNormalizedTime(stateMachine.Animator, "ChangeWeapon", 1);
        if(changeWeaponTime > 0.9f || changeWeaponTime == 0f){
            if(stateMachine.InputReader.IsAttacking)
            {
                if(stateMachine.InputReader.MovementValue != Vector2.zero){
                    stateMachine.AnimationController.GetWeapon((int)stateMachine.WeaponController.weaponLogic.weaponType);

                    if((int)stateMachine.WeaponController.weaponLogic.weaponType == 2) {
                        stateMachine.SwitchState(new PlayerAttackingState(stateMachine, 0));
                        return;
                    }

                    float normalizedTime = GetNormalizedTime(stateMachine.Animator, "Attack", 1);
                    if(normalizedTime < 0.7f){
                        stateMachine.AnimationController.EnableRunningAttack();
                    }
                }
                else{
                    stateMachine.SwitchState(new PlayerAttackingState(stateMachine, 0));
                    return;
                }
            }

            /*
            else if(isCastSkill){
                stateMachine.AnimationController.GetWeapon((int)stateMachine.WeaponController.weaponLogic.weaponType);
                stateMachine.AnimationController.SetCastingSkillPosition(skillPostion);
                float normalizedTime = GetNormalizedTime(stateMachine.Animator, "Skill", 1);
                if(normalizedTime < 0.8f){
                    stateMachine.AnimationController.EnableRunningCastSkill();
                }
                else{
                    isCastSkill = false;
                }
            }
            */

            else if(stateMachine.InputReader.IsSecondaryPerforming){
                if(stateMachine.WeaponController.weaponLogic.weaponType != WeaponType.Bow){
                    stateMachine.SwitchState(new PlayerBlockingState(stateMachine));
                }
                else{
                    stateMachine.SwitchState(new PlayerAimingState(stateMachine));
                }
                return;
            }
        }
        

        Vector3 movement = CalculateMovement();
        
        if(stateMachine.InputReader.IsSprinting && stateMachine.Stats.CurrentStamina > 0){
            stateMachine.Stats.ReduceStamina(1);
            runSpeed = 1f;
        }
        else{
            runSpeed = 0.5f;
        }
        
        if(stateMachine.Stats.CurrentStamina <= 0f) stateMachine.SwitchState(new PlayerResuscitatingState(stateMachine));
        
        float boost = 0;
        
        if(PlayerStatManager.Instance.Agility > 0) boost = stateMachine.FreeLookMovementSpeed * PlayerStatManager.Instance.Agility / 1000;
        
        Move(movement * (stateMachine.FreeLookMovementSpeed + boost) * runSpeed, deltaTime);

        if (stateMachine.InputReader.MovementValue == Vector2.zero)
        {
            stateMachine.Animator.SetFloat(FreeLookSpeedHash, 0, AnimatorDampTime, deltaTime);
            return;
        }

        stateMachine.Animator.SetFloat(FreeLookSpeedHash, runSpeed, AnimatorDampTime, deltaTime);

        FaceMovementDirection(movement, deltaTime);
    }

    public override void Exit()
    {
        stateMachine.InputReader.TargetEvent -= OnTarget;
        stateMachine.InputReader.JumpEvent -= OnJump;
        stateMachine.InputReader.CrouchEvent -= OnCrouch;
        stateMachine.InputReader.ChangeWeaponEvent -= OnChangeWeapon;
        stateMachine.InputReader.TestEvent -= OnTest;
        stateMachine.InputReader.OpenMouseModeEvent -= EnterInteractState;
        stateMachine.SkillManager.PerformSkill -= OnSkillPerform;
        GameEventManager.Instance.PlayerEvents.OnPlayerInteractStateEnter -= EnterInteractState;
    }

    private void OnTarget()
    {
        if (!stateMachine.Targeter.SelectTarget()) { return; }

        stateMachine.SwitchState(new PlayerTargetingState(stateMachine));
    }

    private void OnJump()
    {
        stateMachine.SwitchState(new PlayerJumpingState(stateMachine));
    }

    private void OnCrouch(){
        stateMachine.SwitchState(new PlayerCrouchingState(stateMachine));
    }

    private void OnChangeWeapon(){
        if(stateMachine.WeaponController.primaryWeapon == 0) return;
        // if(!stateMachine.AnimationController.animator.GetBool("Done-Anim")) return;
        
        //Unsealth
        if((int)stateMachine.WeaponController.weaponLogic.weaponType != stateMachine.WeaponController.primaryWeapon){
            changeWeaponHash = stateMachine.AnimationController.GetUnSheath(stateMachine.WeaponController.primaryWeapon);
            stateMachine.Animator.CrossFadeInFixedTime(changeWeaponHash, AnimatorDampTime, 1);
            stateMachine.WeaponController.ChangeWeapon(stateMachine.WeaponController.primaryWeapon, stateMachine.WeaponController.weaponName);
        }

        //Sealth
        else {
            changeWeaponHash = stateMachine.AnimationController.GetSheath(stateMachine.WeaponController.primaryWeapon);
            stateMachine.Animator.CrossFadeInFixedTime(changeWeaponHash, AnimatorDampTime, 1);
            stateMachine.WeaponController.ChangeWeapon(0, "Unarmed");
        }

        blendTreeHash = stateMachine.AnimationController.GetFreeLookBlendTree((int) stateMachine.WeaponController.weaponLogic.weaponType);
        stateMachine.Animator.CrossFadeInFixedTime(blendTreeHash, AnimatorDampTime);
    
    }
    
    private void OnTest(){
        stateMachine.SwitchState(new PlayerFormalState(stateMachine));
    }

    private void OnSkillPerform(CharacterSkillScriptableObject skill, int skillIndex){
        if(stateMachine.WeaponController.weaponLogic.weaponType == 0) return; 

        stateMachine.SwitchState(new PlayerPerformSkillState(stateMachine, skill, skillIndex));
    }
    
    private void EnterInteractState(Transform trans){
        stateMachine.SwitchState(new PlayerInteractingState(stateMachine, trans));
    }

    private void EnterInteractState(){
        stateMachine.SwitchState(new PlayerInteractingState(stateMachine, null));
    } 
}

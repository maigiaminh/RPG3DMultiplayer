using UnityEngine;

public class PlayerAttackingState : PlayerBaseState
{
    private Attack attack;
    private float previousFrameTime;
    private bool alreadyAppliedForce;

    public override bool CanTakeDamage() => false;

    public PlayerAttackingState(PlayerStateMachine stateMachine, int attackIndex) : base(stateMachine)
    {
        attack = stateMachine.WeaponController.weaponLogic.comboAttacks[attackIndex];
    }

    public override void Enter()
    {
        if(stateMachine.WeaponController.weaponLogic.weaponType != WeaponType.Bow && stateMachine.WeaponController.weaponLogic.weaponType != WeaponType.WizardStaff){
            if(stateMachine.WeaponController.weaponDamageLeftHand != null){
                stateMachine.WeaponController.weaponDamageLeftHand.SetAttack(attack.Damage + Mathf.CeilToInt(PlayerStatManager.Instance.Strength / 10), attack.Knockback, (int)stateMachine.WeaponController.weaponLogic.weaponType);
            }

            if(stateMachine.WeaponController.weaponDamageRightHand != null){
                stateMachine.WeaponController.weaponDamageRightHand.SetAttack(attack.Damage + Mathf.CeilToInt(PlayerStatManager.Instance.Strength / 10), attack.Knockback, (int)stateMachine.WeaponController.weaponLogic.weaponType);
            }
        }
        else{
            if(stateMachine.WeaponController.weaponLogic.weaponType == WeaponType.Bow){
                stateMachine.WeaponController.projectilePool.SetDamage(attack.Damage + Mathf.CeilToInt(PlayerStatManager.Instance.Strength / 10));
            }
            else{
                stateMachine.WeaponController.projectilePool.SetDamage(attack.Damage + Mathf.CeilToInt(PlayerStatManager.Instance.Intelligence / 10));
            }
        }

        stateMachine.Animator.CrossFadeInFixedTime(attack.AnimationName, 0.1f);
    }


    public override void Tick(float deltaTime)
    {
        Move(deltaTime);
        
        float normalizedTime = GetNormalizedTime(stateMachine.Animator, "Attack", 0);

        if(normalizedTime < 0.2f){
            FaceMovementDirection(CalculateDirection(), deltaTime);
        }
        
        if (normalizedTime >= previousFrameTime && normalizedTime < 1f)
        {
            if (normalizedTime >= attack.ForceTime)
            {
                TryApplyForce();
            }


            if (stateMachine.InputReader.IsAttacking)
            {
                TryComboAttack(normalizedTime);
            }
        }
        else
        {
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

        previousFrameTime = normalizedTime;

    }

    public override void Exit()
    {

    }

    private void TryComboAttack(float normalizedTime)
    {
        if (attack.ComboStateIndex == -1) { return; }

        if (normalizedTime < attack.ComboAttackTime) { return; }

        stateMachine.SwitchState
        (
            new PlayerAttackingState
            (
                stateMachine,
                attack.ComboStateIndex
            )
        );
    }

    private void TryApplyForce()
    {
        if (alreadyAppliedForce) { return; }

        stateMachine.ForceReceiver.AddForce(stateMachine.transform.forward * attack.Force);

        alreadyAppliedForce = true;
    }

    private Vector3 CalculateDirection(){

        Vector3 forward = stateMachine.MainCameraTransform.forward;
        Vector3 right = stateMachine.MainCameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        return forward;
    }
}


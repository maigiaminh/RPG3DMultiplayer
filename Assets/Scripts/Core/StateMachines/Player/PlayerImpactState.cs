using UnityEngine;

public class PlayerImpactState : PlayerBaseState
{
    private const float CrossFadeDuration = 0.1f;
    private int impactHash;
    public PlayerImpactState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override bool CanTakeDamage() => false;
    
    public override void Enter()
    {
        int currentWeapon = (int)stateMachine.WeaponController.weaponLogic.weaponType;
        if(stateMachine.WeaponController.weaponLogic.weaponType != WeaponType.Bow 
        && stateMachine.InputReader.IsSecondaryPerforming){
            impactHash = stateMachine.AnimationController.GetBlockHit(currentWeapon);
            var sound = SoundManager.GetBlockSound(currentWeapon);
            SoundManager.PlaySound(sound, stateMachine.Chest);
        }
        else{
            impactHash = stateMachine.AnimationController.GetHit(currentWeapon);
            SoundManager.PlaySound(SoundType.IMPACT, stateMachine.Chest);
        }

        stateMachine.Animator.CrossFadeInFixedTime(impactHash, CrossFadeDuration);
    }

    public override void Tick(float deltaTime)
    {
        Move(deltaTime);

        float normalizedTime = GetNormalizedTime(stateMachine.Animator, "Impact", 0);

        if (normalizedTime >=  0.8f)
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
    }

    public override void Exit() { }
}


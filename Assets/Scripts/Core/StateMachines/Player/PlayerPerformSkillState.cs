using UnityEngine;

public class PlayerPerformSkillState : PlayerBaseState
{
    private CharacterSkillScriptableObject _skill;
    public override bool CanTakeDamage() => false;

    public PlayerPerformSkillState(PlayerStateMachine stateMachine, CharacterSkillScriptableObject skill, int skillIndex) : base(stateMachine) 
    { 
        _skill = skill;
    }
    private const float CrossFadeDuration = 0.1f;
    private int castHash;

    public override void Enter()
    {

        castHash = stateMachine.AnimationController.GetCastHash(
            (int) stateMachine.WeaponController.weaponLogic.weaponType,
            (int) _skill.skillSpawnType);

        stateMachine.Animator.CrossFadeInFixedTime(castHash, CrossFadeDuration);
    }

    public override void Tick(float deltaTime)
    {
        Move(deltaTime);

        float normalizedTime = GetNormalizedTime(stateMachine.Animator, "Skill", 0);

        if(normalizedTime < 0.2f){
            FaceMovementDirection(CalculateDirection(), deltaTime);
        }
        
        
        if (normalizedTime >=  0.9f)
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

    public override void Exit()
    {

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

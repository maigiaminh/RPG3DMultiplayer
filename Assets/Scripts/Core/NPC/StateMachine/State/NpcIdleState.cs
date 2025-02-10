using UnityEngine;

public class NpcIdleState : NpcBaseState
{
    private const float _crossFadeDuration = 0.3f;
    private int _hashIdle;

    public NpcIdleState(NpcStateMachine stateMachine) : base(stateMachine)
    {
        _hashIdle = _stateMachine.AnimationController.IdleHashKey;
    }
    public override void Enter()
    {
        base.Enter();
        _stateMachine.HeadAimTarget.transform.position = _stateMachine.initialHeadPos;
        _stateMachine.animator.CrossFadeInFixedTime(_hashIdle, _crossFadeDuration);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Tick(float deltaTime)
    {
        base.Tick(deltaTime);
    }

}

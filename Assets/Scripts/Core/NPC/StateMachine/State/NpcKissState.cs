using UnityEngine;

public class NpcKissState : NpcBaseState
{
    private int _kissHashKey = 0;
    private const float CrossFadeDuration = 0.1f;

    public NpcKissState(NpcStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        _kissHashKey = _stateMachine.AnimationController.BlowKissKey;
        _stateMachine.animator.CrossFadeInFixedTime(_kissHashKey, CrossFadeDuration);

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

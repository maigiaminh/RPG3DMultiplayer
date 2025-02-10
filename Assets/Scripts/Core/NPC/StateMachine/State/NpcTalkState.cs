using UnityEngine;

public class NpcTalkState : NpcBaseState
{
    private const float _crossFadeDuration = 0.3f;
    private int _hashTalk;
    public NpcTalkState(NpcStateMachine stateMachine) : base(stateMachine)
    {
        _hashTalk = Animator.StringToHash("Talk");
    }

    public override void Enter()
    {
        base.Enter();
        _stateMachine.animator.CrossFadeInFixedTime(_hashTalk, _crossFadeDuration);
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

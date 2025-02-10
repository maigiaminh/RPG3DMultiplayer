using UnityEngine;

public class NpcBaseState : State
{
    protected readonly NpcStateMachine _stateMachine;
    protected GameObject _headAimTarget;
    protected Vector3 offset = new Vector3(0, 1.5f, 0);
    public NpcBaseState(NpcStateMachine stateMachine) => _stateMachine = stateMachine;
    public override void Enter()
    {
        _headAimTarget = _stateMachine.HeadAimTarget;
    }

    public override void Exit()
    {
        _headAimTarget.transform.position = _stateMachine.initialHeadPos;   
        _headAimTarget = null;
    }

    public override void Tick(float deltaTime)
    {
        if(_stateMachine.PlayerDetector.player != null)
        {
            _headAimTarget.transform.position = _stateMachine.PlayerDetector.player.position + offset;
        }
    }
}

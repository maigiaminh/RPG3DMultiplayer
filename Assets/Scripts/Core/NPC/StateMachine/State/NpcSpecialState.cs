using UnityEngine;

public class NpcSpecialState : State
{
    protected NpcStateMachine _npcStateMachine;
    protected int _SpecialHashKey = 0;
    protected int _specialType = 1;
    protected const float CrossFadeDuration = 0.2f;

    public NpcSpecialState(NpcStateMachine stateMachine, int specialType = 1){
        _npcStateMachine = stateMachine;
        _specialType = specialType;
    }
    public override void Enter()
    {
        _npcStateMachine.animator.CrossFadeInFixedTime(_SpecialHashKey, CrossFadeDuration);
    }

    public override void Exit()
    {
    }

    public override void Tick(float deltaTime)
    {
    }
}

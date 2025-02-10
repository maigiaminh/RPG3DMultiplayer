using UnityEngine;

public class NpcDanceState : NpcSpecialState
{
    public NpcDanceState(NpcStateMachine stateMachine, int danceType) : base(stateMachine, danceType)
    {
    }

    public override void Enter()
    {
        _SpecialHashKey = _npcStateMachine.AnimationController.GetSpecialType(this, _specialType);
        base.Enter();
    }

}

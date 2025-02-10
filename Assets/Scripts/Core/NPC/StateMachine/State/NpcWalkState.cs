using UnityEngine;

public class NpcWalkState : State
{
    private NpcStateMachine _stateMachine;
    private const float _crossFadeDuration = 0.1f;
    private int _hashKey;
    public float deltaTimeToChangeState = 1f;

    private float _currentBlendValue = 0;
    public NpcWalkState(NpcStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }
    public override void Enter()
    {
        _hashKey = _stateMachine.AnimationController.WalkHashKey;
        _stateMachine.HeadAimTarget.transform.position = _stateMachine.initialHeadPos;
        _stateMachine.animator.CrossFadeInFixedTime(_hashKey, _crossFadeDuration);
        _currentBlendValue = 0f; // Initialize blend to start from 0
    }

    public override void Exit()
    {
        _currentBlendValue = 0f;
    }

    public override void Tick(float deltaTime)
    {
        _currentBlendValue += deltaTime / deltaTimeToChangeState;
        _currentBlendValue = Mathf.Clamp01(_currentBlendValue); // Ensure blend stays between 0 and 1
        _stateMachine.animator.SetFloat("Blend", _currentBlendValue);
    }


}

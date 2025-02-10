using UnityEngine;

public class NpcWaveHandState : NpcBaseState
{
    private int _waveHandsHashKey;
    private float _crossFadeDuration = 0.2f;
    private float _rotateTime;
    private GameObject _npc;
    private GameObject _player;

    public NpcWaveHandState(NpcStateMachine stateMachine, GameObject npc, GameObject player) : base(stateMachine)
    {
        _npc = npc;
        _player = player;
    }

    public override void Enter()
    {
        base.Enter();

        _rotateTime = _stateMachine.TimeToRotate;
        _waveHandsHashKey = _stateMachine.AnimationController.WaveHandsHashKey;
        _stateMachine.animator.CrossFadeInFixedTime(_waveHandsHashKey, _crossFadeDuration);
    }

    public override void Exit()
    {
        base.Exit();

    }

    public override void Tick(float deltaTime)
    {
        base.Tick(deltaTime);
        if (_rotateTime <= 0)
        {
            return;
        }
        Vector3 direction = (_player.transform.position - _npc.transform.position).normalized;

        // Calculate the target rotation
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // Only rotate around the Y-axis
        targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);

        // Smoothly rotate towards the player
        _npc.transform.rotation = Quaternion.Slerp(_npc.transform.rotation, targetRotation, deltaTime / _rotateTime);
        _rotateTime -= deltaTime;
    }

}

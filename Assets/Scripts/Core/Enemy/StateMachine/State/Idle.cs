

using System;
using UnityEngine;
using UnityEngine.AI;

public class Idle : IState
{
    private readonly IEnemy _enemy;
    private readonly Animator _animator;
    private readonly NavMeshAgent _navMeshAgent;
    private static readonly int Speed = Animator.StringToHash("Speed");
    public float idleTime = 7f;
    public float timer = 0f;
    private float _deltaTime = 2f;
    public Idle(IEnemy enemy, NavMeshAgent navMeshAgent, Animator animator, float idleTime)
    {
        _enemy = enemy;
        _animator = animator;
        _navMeshAgent = navMeshAgent;
        this.idleTime = Mathf.Clamp(UnityEngine.Random.Range(1, idleTime), 1, idleTime + _deltaTime);

    }
    public void Tick()
    {
        timer += Time.deltaTime;

    }
    public void OnEnter()
    {
        timer = 0f;

        _animator.SetInteger(Speed, 0);
    }

    public void OnExit()
    {
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Chase : IState
{
    private readonly IEnemy _enemy;
    private readonly NavMeshAgent _navMeshAgent;
    private readonly Animator _animator;
    private readonly RangeDetector _targetDetector;
    private float _chaseSpeed;
    private float _initialSpeed;
    public Chase(IEnemy enemy, NavMeshAgent navMeshAgent, Animator animator, RangeDetector targetDetector, float chaseSpeed)
    {
        _enemy = enemy;
        _navMeshAgent = navMeshAgent;
        _animator = animator;
        _targetDetector = targetDetector;
        _chaseSpeed = chaseSpeed;
    }
    public void Tick()
    {
        _navMeshAgent.SetDestination(_targetDetector.detectTarget.position);

    }

    public void OnEnter()
    {
        _initialSpeed = _navMeshAgent.speed;
        _navMeshAgent.enabled = true;
        _navMeshAgent.isStopped = false;
        _navMeshAgent.speed = _chaseSpeed;
        _animator.SetTrigger("Chase");
        _animator.SetInteger("Speed", 2);

    }

    public void OnExit()
    {
        _navMeshAgent.ResetPath();

        _navMeshAgent.speed = _initialSpeed;

        _animator.ResetTrigger("Chase");
        _animator.SetInteger("Speed", -1);
    }



}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyDie : IState
{
    private readonly Enemy _enemy;
    private readonly Animator _animator;
    private readonly NavMeshAgent _navMeshAgent;
    public float deathTimer = 0;
    public float timeToDissolve;
    
    public EnemyDie(Enemy enemy, NavMeshAgent navMeshAgent, Animator animator, float timeToDissolve)
    {
        deathTimer = 0;
        _enemy = enemy;
        _animator = animator;
        _navMeshAgent = navMeshAgent;
        this.timeToDissolve = timeToDissolve;
    }
    public void Tick() {
        deathTimer += Time.deltaTime;
        if(deathTimer >= timeToDissolve) _enemy.isDissolve = true;
    }

    public void OnEnter()
    {
        deathTimer = 0;
        _animator.SetInteger("Speed", -1);
        _navMeshAgent.ResetPath();
        _animator.SetTrigger("Die");
    }

    public void OnExit()
    {
    }


}

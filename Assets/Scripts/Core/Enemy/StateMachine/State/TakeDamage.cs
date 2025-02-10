using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TakeDamage : IState
{
    private Enemy _enemy;
    private readonly Animator _animator;
    private readonly NavMeshAgent _navMeshAgent;
    private readonly List<float> _TakeDamageAnimTimes;
    public float animTimer = 0;
    
    public TakeDamage(Enemy enemy, NavMeshAgent navMeshAgent, Animator animator, List<float> TakeDamageAnimTimes)
    {
        _enemy = enemy;
        _animator = animator;
        _navMeshAgent = navMeshAgent;
        _TakeDamageAnimTimes = TakeDamageAnimTimes;
    }

    public void Tick(){
        animTimer += Time.deltaTime;
        if(animTimer >= _TakeDamageAnimTimes[_enemy.currentTakeDamageAnimIndex % _TakeDamageAnimTimes.Count])
        {
            _enemy.enemyTakeDamage = false;
        }
    }
    public void OnEnter()
    {
        Debug.Log("------------------" + _TakeDamageAnimTimes.Count);
        
        animTimer = 0;
        _enemy.enemyTakeDamage = true;
        _navMeshAgent.ResetPath();
        _animator.SetTrigger("TakeDamageTrigger");
        _animator.SetInteger("TakeDamage", _enemy.currentTakeDamageAnimIndex % _TakeDamageAnimTimes.Count);
        _enemy.currentTakeDamageAnimIndex++;
    }

    public void OnExit()
    {
        Debug.Log("-------- Take Damage Exit --------");
        _animator.ResetTrigger("TakeDamageTrigger");
        _animator.SetInteger("TakeDamage", -1);
    }


}

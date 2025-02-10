

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttack : IState
{
    private readonly IEnemy _enemy;
    private readonly NavMeshAgent _navMeshAgent;
    private readonly Animator _animator;
    // private List<> 
    public int currentAtkAnimIndex = 0;
    private int _allAtkAnimCount = 0;

    private float _initialAnimatorSpeed;
    public float timerNextATk = 0f;
    private float _attackSpeed;
    private List<float> _attackAnimTimes = new List<float>();
    public bool isAttacking = false;




    // private static readonly int Atk = Animator.StringToHash("Attack");

    public EnemyAttack(IEnemy enemy, NavMeshAgent navMeshAgent, Animator animator, float speed, List<float> attackAnimTimes)
    {
        _enemy = enemy;
        _navMeshAgent = navMeshAgent;
        _animator = animator;
        _attackSpeed = speed;
        _attackAnimTimes = attackAnimTimes;
        _allAtkAnimCount = attackAnimTimes.Count;
    }


    public void Tick()
    {
        timerNextATk += Time.deltaTime;
        if (timerNextATk >= _attackAnimTimes[currentAtkAnimIndex % _allAtkAnimCount])
        {
            isAttacking = true;
            currentAtkAnimIndex++;
            _animator.SetInteger("Attack", (currentAtkAnimIndex) % _allAtkAnimCount);
            timerNextATk = 0f;


            // _enemy.Attack();
        } else {
            isAttacking = false;
        }

    }
    public void OnEnter()
    {
        _navMeshAgent.ResetPath();
        currentAtkAnimIndex = 0;
        timerNextATk = 0;
        _initialAnimatorSpeed = _animator.speed;
        _animator.speed = _attackSpeed;

        // _allAtkAnimCount = GetTotalAttackAnim();
        _animator.SetInteger("Attack", (currentAtkAnimIndex) % _allAtkAnimCount);
        // _enemy.Attack();
    }

    public void OnExit()
    {
        _animator.speed = _initialAnimatorSpeed;

        _animator.SetInteger("Attack", -1);
        timerNextATk = 0; 
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CastAbility : IState
{
    private readonly Enemy _enemy;
    private readonly Animator _animator;
    private readonly NavMeshAgent _navMeshAgent;
    private readonly List<SkillData> _skillData;  


    public CastAbility(Enemy enemy, NavMeshAgent navMeshAgent, Animator animator, List<SkillData> skillData)
    {
        _enemy = enemy;
        _navMeshAgent = navMeshAgent;
        _animator = animator;
        _skillData = skillData;
    }
    public void Tick()
    {

    }
    public void OnEnter()
    {
        _animator.SetTrigger("CastAbility");
        EnemyAuraManager.Instance.ActivateAura(_enemy.enemyData.AuraType, _enemy);
        // foreach(SkillData skill in _skillData)
        // {

        // }
        // ((Enemy)_enemy).CurrentSkill 
    }

    public void OnExit()
    {
        _animator.ResetTrigger("CastAbility");
        _animator.SetInteger("Speed", 0);
        EnemyAuraManager.Instance.DeactivateAura(_enemy.enemyData.AuraType, _enemy);
    }


}

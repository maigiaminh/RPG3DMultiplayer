using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.PlayerLoop;


[RequireComponent(typeof(SphereCollider))]
public class AttackRadius : MonoBehaviour
{
    public SphereCollider Collider;
    public int damage = 10;
    public float attackDelay = 0.5f;

    public delegate void AttackEvent(IDamageable Target);
    public AttackEvent OnAttack;

    protected RangeDetector _attackDetector;

    protected List<IDamageable> _damageablesList = new List<IDamageable>();

    public List<float> attackAnimTimes = new List<float>();

    private Enemy _enemy;

    protected virtual void Awake()
    {
        Collider = GetComponent<SphereCollider>();
        _attackDetector = GetComponent<RangeDetector>();
    }


    protected void OnEnable()
    {
        _attackDetector.OnTargetChanged += UpdateDamageablesList;
        // if(_enemyAttack != null)
        //     _enemyAttack.OnAttack += Attack;


    }
    protected void OnDisable()
    {
        _attackDetector.OnTargetChanged -= UpdateDamageablesList;
        if (_enemy != null)
            _enemy.EnemyOnAttack -= Attack;
    }


    public void Initialize(Enemy enemy)
    {
        _enemy = enemy;
        if (_enemy != null)
            _enemy.EnemyOnAttack += Attack;
    }

    protected void UpdateDamageablesList(IDamageable damageable)
    {
        _damageablesList = _attackDetector.targetsInRange;
    }


    protected virtual void Attack()
    {
        if (_damageablesList.Count == 0) return;

        IDamageable closestDamageable = null;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < _damageablesList.Count; i++)
        {
            Transform damageableTransform = _damageablesList[i].GetTransform();
            float distance = Vector3.Distance(transform.position, damageableTransform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestDamageable = _damageablesList[i];
            }
        }

        if (closestDamageable != null)
        {
            OnAttack?.Invoke(closestDamageable);
            closestDamageable.TakeDamage(null, damage);
        }

        closestDamageable = null;
        closestDistance = float.MaxValue;

        _damageablesList.RemoveAll(DisabledDamageables);

    }


    protected bool DisabledDamageables(IDamageable Damageable)
    {
        return Damageable != null && !Damageable.GetTransform().gameObject.activeSelf;
    }


    public void ConfigAttackAttribute()
    {

    }
}

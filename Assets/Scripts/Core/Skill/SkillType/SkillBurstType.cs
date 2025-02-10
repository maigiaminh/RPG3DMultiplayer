using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBurstType : CharacterSkillBase
{
    private float _damagerate = 0;
    private List<IDamageable> _targets = new List<IDamageable>();

    protected override void ConfigSkillBase(CharacterSkillScriptableObject data)
    {
        base.ConfigSkillBase(data);
        _damagerate = data.DamageRateBurst;
    }

    public override void Execute(Transform spawnPos, Vector3 direct, CharacterSkillScriptableObject data)
    {
        base.Execute(spawnPos, direct, data);

        Debug.Log("Burst");
        _timerDamageRate = 0;

        if (data.HaveDelay)
        {
            StartCoroutine(DelayForFirstDamage());
        }
    }

    private void Update()
    {
        _timerDamageRate += Time.deltaTime;
        if (_timerDamageRate >= 1 / _damagerate)
        {
            _timerDamageRate = 0;
            ApplyDamageToTargets();
        }
    }

    private IEnumerator DelayForFirstDamage()
    {
        yield return new WaitForSeconds(_skillData.DelayTime);
        ApplyDamageToTargets();
    }

    private void ApplyDamageToTargets()
    {
        foreach (var target in _targets)
        {
            Debug.Log("Apply Damage");
            target.TakeDamage(transform, _damage);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<IDamageable>(out var damageable)) return;
        if (other.GetComponent<SkillManager>() == _skillManager) return;
        Debug.Log("Add Target 1");

        if (!other.CompareTag("Enemy")) return;

        Debug.Log("Add Target 2");


        if (!_targets.Contains(damageable))
        {
            _targets.Add(damageable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent<IDamageable>(out var damageable)) return;
        if (other.GetComponent<SkillManager>() == _skillManager) return;
        if (!other.CompareTag("Enemy")) return;

        _targets.Remove(damageable);
    }
}

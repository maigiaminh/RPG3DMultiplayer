using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillGroundAreaType : CharacterSkillBase
{
    private float _damageRate = 0;
    private List<IDamageable> _targets = new List<IDamageable>();

    protected override void ConfigSkillBase(CharacterSkillScriptableObject data)
    {
        base.ConfigSkillBase(data);
        _damageRate = data.GroundDamageRate;
    }
    public override void Execute(Transform spawnPos, Vector3 direct, CharacterSkillScriptableObject data)
    {
        base.Execute(spawnPos, direct, data);
        transform.forward = direct;
        StartCoroutine(DelayForFirstDamage());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<IDamageable>(out var damageable)) return;
        if (other.GetComponent<SkillManager>() == _skillManager) return;
        if (!other.CompareTag("Enemy")) return;

        _targets.Add(damageable);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent<IDamageable>(out var damageable)) return;
        if (other.GetComponent<SkillManager>() == _skillManager) return;
        if (!other.CompareTag("Enemy")) return;
        if (!_targets.Contains(damageable)) return;

        _targets.Remove(damageable);
    }

    private void Update()
    {
        _timerDamageRate += Time.deltaTime;
        if(_timerDamageRate >= 1 / _damageRate)
        {
            _timerDamageRate = 0;
            ApplyDamageToTargets(_targets);
        }
    }
    private IEnumerator DelayForFirstDamage()
    {
        yield return new WaitForSeconds(_skillData.DelayTime);
        ApplyDamageToTargets(_targets);
    }

    private void ApplyDamageToTargets(List<IDamageable> targets)
    {
        if (_targets.Count == 0) return;

        foreach (var target in targets)
        {
            target.TakeDamage(null, _damage);
        }
    }

}

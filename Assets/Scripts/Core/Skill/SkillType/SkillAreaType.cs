using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAreaType : CharacterSkillBase
{
    private float _radius = 0;
    private float _damagerate = 0;
    private List<IDamageable> _targets = new List<IDamageable>();

    protected override void ConfigSkillBase(CharacterSkillScriptableObject data)
    {
        base.ConfigSkillBase(data);
        _radius = data.AreaRadius;
        _damagerate = data.DamageRateArea;
        _targets.Clear();
    }

    public override void Execute(Transform spawnPos, Vector3 direct, CharacterSkillScriptableObject data)
    {
        base.Execute(spawnPos, direct, data);

        _timerDamageRate = 0;
        ConfigAreaRange();

        StartCoroutine(DelayForFirstDamage());
    }


    private void Update()
    {
        _timerDamageRate += Time.deltaTime;
        if (_timerDamageRate >= 1 / _damagerate)
        {
            _timerDamageRate = 0;
            ApplyDamageToTargets(_targets);
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<IDamageable>(out var damageable)) return;
        if (other.GetComponent<SkillManager>() == _skillManager) return;
        if (!other.CompareTag("Enemy")) return;

        if (!_targets.Contains(damageable))
        {
            _targets.Add(damageable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent<IDamageable>(out var damageable)) return;
        if (other.GetComponent<SkillManager>() == _skillManager) return;

        if (_targets.Contains(damageable))
        {
            _targets.Remove(damageable);
        }
    }

    private IEnumerator DelayForFirstDamage()
    {
        yield return new WaitForSeconds(_skillData.DelayTime);
        ApplyDamageToTargets(_targets);
    }

    private void ApplyDamageToTargets(List<IDamageable> targets)
    {
        foreach (var target in targets)
        {
            target.TakeDamage(transform, _damage);
        }
    }
    private void ConfigAreaRange()
    {
        transform.localScale = transform.localScale * _radius;
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCCAOEConstantType : CharacterSkillBase
{
    private float _radius;
    private float _ccDuration;
    private CCType _ccType;
    private float _damageRate;
    private float _delayTime;
    private List<IDamageable> _targets = new List<IDamageable>();

    private float _counter = 0;
    protected override void ConfigSkillBase(CharacterSkillScriptableObject data)
    {
        base.ConfigSkillBase(data);
        _ccType = data.CCStickPlayerType;
        _radius = data.CCRadius;
        _ccDuration = data.Duration;
        _damageRate = data.CCDamageRate;
        _delayTime = data.DelayTime;
    }

    public override void Execute(Transform spawnPos, Vector3 direct, CharacterSkillScriptableObject data)
    {
        base.Execute(spawnPos, direct, data);
        StartCoroutine(DelayForFirstDamage());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;
        if (!other.TryGetComponent<IDamageable>(out var damageable)) return;
        _targets.Add(damageable);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;
        if (!other.TryGetComponent<IDamageable>(out var damageable)) return;
        if (!_targets.Contains(damageable)) return;
        _targets.Remove(damageable);
    }

    private void Update() {
        _counter += Time.deltaTime;
        if (_counter >= 1 / _damageRate)
        {
            _counter = 0;
            ApplyCC();
        }
    }

    public void ApplyCC()
    {
        if (_targets.Count == 0) return;
        foreach (var target in _targets)
        {
            target.ApplyCC(_ccType, _ccDuration);
            target.TakeDamage(null, _damage);
        }
    }
    private IEnumerator DelayForFirstDamage()
    {
        yield return new WaitForSeconds(_delayTime);
        ApplyCC();
    }
}

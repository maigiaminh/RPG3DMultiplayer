using System.Collections.Generic;
using UnityEngine;

public class SkillConstantType : CharacterSkillBase
{
    private List<IDamageable> _targets = new List<IDamageable>();
    protected override void ConfigSkillBase(CharacterSkillScriptableObject data)
    {
        base.ConfigSkillBase(data);
    }

    public override void Execute(Transform spawnPos, Vector3 direct, CharacterSkillScriptableObject data)
    {
        base.Execute(spawnPos, direct, data);
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

    public void ApplyDamage()
    {
        if(_targets.Count == 0) return;
        foreach (var target in _targets)
        {
            target.TakeDamage(transform, _damage);
        }
    }

}

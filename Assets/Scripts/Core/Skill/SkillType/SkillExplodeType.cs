using System;
using UnityEngine;

public class SkillExplodeType : CharacterSkillBase
{
    private float _radius = 0;
    

    protected override void ConfigSkillBase(CharacterSkillScriptableObject data)
    {
        base.ConfigSkillBase(data);
        _radius = data.ExplodeRadius;

    }

    public override void Execute(Transform spawnPos, Vector3 direct, CharacterSkillScriptableObject data)
    {
        base.Execute(spawnPos, direct, data);
    }

    public void ApplyDamageToTargets()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _radius);
        foreach (var collider in colliders)
        {
            if (!collider.TryGetComponent<IDamageable>(out var damageable)) continue;
            if (collider.GetComponent<SkillManager>() == _skillManager) continue;
            if (!collider.CompareTag("Enemy")) continue;

            damageable.TakeDamage(transform, _damage);
        }
    }


}

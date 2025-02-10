using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DetectDamageableInRange : MonoBehaviour
{
    public List<IDamageable> damageables = new List<IDamageable>();
    private SkillManager _skillManager;

    protected virtual void OnDisable()
    {
        damageables.Clear();
    }


    public void Config(SkillManager skillManager)
    {
        _skillManager = skillManager;
    }
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<IDamageable>(out var damageable)) return;
        if (other.TryGetComponent<SkillManager>(out var skillManager)) return;
        if (skillManager == _skillManager) return;
        damageables.Add(damageable);
    }
    protected virtual void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent<IDamageable>(out var damageable)) return;
        if (!damageables.Contains(damageable)) return;

        damageables.Remove(damageable);
    }



}

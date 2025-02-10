using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyAuraManager : Singleton<EnemyAuraManager>
{
    public List<AuraSkill> auraSkills = new List<AuraSkill>();

    private Dictionary<AuraType, ObjectPool<AuraSkill>> _auraSkillPools;


    private void Start()
    {
        _auraSkillPools = new Dictionary<AuraType, ObjectPool<AuraSkill>>();
        foreach (var skill in auraSkills)
        {
            if (skill.auraType == AuraType.None) continue;
            if (_auraSkillPools.ContainsKey(skill.auraType))
            {
                Debug.LogError("AuraSkill with the same type already exists: " + skill.auraType);
                continue;
            }
            _auraSkillPools.Add(skill.auraType, new ObjectPool<AuraSkill>(
                createFunc: () => Instantiate(skill),
                actionOnGet: (aura) => aura.gameObject.SetActive(true),
                actionOnRelease: (aura) => aura.gameObject.SetActive(false),
                actionOnDestroy: (aura) => Destroy(aura.gameObject),
                collectionCheck: true
            ));
        }
    }

    public void ActivateAura(AuraType auraType, IEnemy enemy)
    {
        if (!_auraSkillPools.ContainsKey(auraType)) return;

        AuraSkill aura = _auraSkillPools[auraType].Get();
        RaycastHit hit;
        aura.transform.position = Physics.Raycast(enemy.GameObject.transform.position, Vector3.down, out hit)
                                    ? hit.point
                                    : enemy.GameObject.transform.position;

        aura.transform.SetParent(enemy.GameObject.transform);
    }

    public void DeactivateAura(AuraType auraType, Enemy enemy)
    {
        if (!_auraSkillPools.ContainsKey(auraType)) return;
        AuraSkill aura = enemy.GameObject.GetComponentInChildren<AuraSkill>();
        if (aura == null) return;
        _auraSkillPools[auraType].Release(aura);
    }


}

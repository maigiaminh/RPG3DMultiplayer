using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[CreateAssetMenu(fileName = "BeamSkill", menuName = "Enemy/Skill/BeamSkill", order = 1)]
public class BeamSkill : SkillData
{
    public ConstantDamage skillPrefab;
    public float skillRange = 10f;
    public float damageRate = 1;
    public float delayTime = 1;

    public override SkillData ScaleUpForLevel(ScalingEnemyData scaling, int level)
    {
        BeamSkill Instance = CreateInstance<BeamSkill>();

        ScaleUpBaseValueForLevel(Instance, scaling, level);
        Instance.skillRange = skillRange;

        return Instance;
    }

    public override bool CanUseSkill(Enemy enemy, IDamageable damageable, int level)
    {
        if (!base.CanUseSkill(enemy, damageable, level)) return false;
        if (damageable == null) return false;
        float distance = Vector3.Distance(enemy.transform.position, damageable.GetTransform().position);


        return
            enemy.skillActivatingDict[this] == false &&
            distance <= skillRange;
    }

    public override void UseSkill(Enemy enemy, IDamageable damageable, float enemyCooldown)
    {
        base.UseSkill(enemy, damageable, enemyCooldown);

        Debug.Log("BeamSkill: UseSkill");

        enemy.StartCoroutine(ActivateSkill(enemy, damageable));
    }

    IEnumerator ActivateSkill(Enemy enemy, IDamageable damageable)
    {

        Vector3 damageablePosition = damageable.GetTransform().position;


        // GameObject skill = _skillPool.Get();
        // AreaDamage areaDamage = skill.GetComponentInChildren<AreaDamage>();

        ConstantDamage skill = Instantiate(skillPrefab, enemy.GetProjectileSpawnpoint(), Quaternion.identity);

        skill.transform.forward = damageable.GetTransform().position - skill.transform.position;

        ConfigConstantDamage(skill);


        yield return new WaitForSeconds(duration);

        enemy.isSkillActivating = false;

        Destroy(skill);
    }

    private void ConfigConstantDamage(ConstantDamage skill)
    {
        skill.Damage = damage;
        skill.DamageRate = damageRate;
        skill.DelayTime = delayTime;
    }
}

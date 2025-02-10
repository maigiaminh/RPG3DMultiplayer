using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[CreateAssetMenu(fileName = "StrikeSkill", menuName = "Enemy/Skill/StrikeSkill", order = 1)]
public class StrikeSkill : SkillData
{
    public GameObject skillPrefab;
    public float skillRange;
    public float delayTime;
    public float radius;

    public bool IsDynamic = false;


    public override SkillData ScaleUpForLevel(ScalingEnemyData scaling, int level)
    {
        StrikeSkill Instance = CreateInstance<StrikeSkill>();

        ScaleUpBaseValueForLevel(Instance, scaling, level);
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

        Debug.Log("StrikeSkill: UseSkill");

        enemy.StartCoroutine(ActivateSkill(enemy, damageable));
    }

    IEnumerator ActivateSkill(Enemy enemy, IDamageable damageable)
    {

        Vector3 damageablePosition = IsDynamic ? damageable.GetTransform().position : enemy.GetTransform().position;

        // Vector2 randomPoint = UnityEngine.Random.insideUnitCircle * 2;


        RaycastHit hit;
        LayerMask mask = LayerMask.GetMask("Default");
        mask &= ~(1 << LayerMask.NameToLayer("Enemy"));
        if (Physics.Raycast(damageablePosition, Vector3.down, out hit, 100f, mask))
        {
            damageablePosition = hit.point;
            Debug.Log("StrikeSkill: Ground found + hit name: " + hit.collider.name);
        }
        else
        {
            Debug.LogError("StrikeSkill: No ground found");
        }

        // GameObject skill = _skillPool.Get();
        // AreaDamage areaDamage = skill.GetComponentInChildren<AreaDamage>();

        GameObject skill = Instantiate(skillPrefab, damageablePosition, Quaternion.identity);
        ExplodeDamage areaDamage = skill.GetComponent<ExplodeDamage>();
        ConfigAreaDamage(areaDamage);



        yield return new WaitForSeconds(duration);

        enemy.isSkillActivating = false;

        Destroy(skill);
    }


    private void ConfigAreaDamage(ExplodeDamage areaDamage)
    {
        areaDamage.Damage = damage;
        areaDamage.DelayTime = delayTime;
        areaDamage.Radius = radius;
    }


}

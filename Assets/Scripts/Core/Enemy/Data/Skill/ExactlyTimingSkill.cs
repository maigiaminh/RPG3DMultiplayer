using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[CreateAssetMenu(fileName = "ExactlyTimingSkill", menuName = "Enemy/Skill/ExactlyTimingSkill", order = 1)]
public class ExactlyTimingSkill : SkillData
{
    public GameObject skillPrefab;
    public float skillRange = 10f;
    public float radius = 5f;
    public List<float> timeToApplyDamage = new List<float>();
    
    public override SkillData ScaleUpForLevel(ScalingEnemyData scaling, int level)
    {
        ExactlyTimingSkill Instance = CreateInstance<ExactlyTimingSkill>();

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

        Debug.Log("ExactlyTimingSkill: UseSkill");

        enemy.StartCoroutine(ActivateSkill(enemy, damageable));
    }

    IEnumerator ActivateSkill(Enemy enemy, IDamageable damageable)
    {

        Vector3 damageablePosition = damageable.GetTransform().position;

        RaycastHit hit;
        if (Physics.Raycast(damageablePosition, Vector3.down, out hit, 100f, LayerMask.GetMask("Default")))
        {
            damageablePosition = hit.point;
        }
        else
        {
            Debug.LogError("ExactlyTimingSkill: No ground found");
        }


        GameObject skill = Instantiate(skillPrefab, damageablePosition, Quaternion.identity);
        TimingDamage timingDamage = skill.GetComponent<TimingDamage>();

        ConfigTimimgDamage(timingDamage);



        yield return new WaitForSeconds(duration);

        enemy.isSkillActivating = false;

        Destroy(skill);
    }


    private void ConfigTimimgDamage(TimingDamage timingDamage)
    {
        timingDamage.Damage = damage;
        timingDamage.Radius = radius;
        timingDamage.timeToApplyDamage = timeToApplyDamage;
    }


}

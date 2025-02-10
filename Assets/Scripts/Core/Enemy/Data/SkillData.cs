using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "Enemy/SkillData", order = 1)]
public class SkillData : ScriptableObject
{
    public float cooldown;
    public float duration;
    public int damage;
    public int unlockSkillLevel;

    // public bool isActivating;


    public virtual SkillData ScaleUpForLevel(ScalingEnemyData scaling, int level)
    {
        SkillData scaledUpSkill = CreateInstance<SkillData>();
        
        ScaleUpBaseValueForLevel(scaledUpSkill, scaling, level);



        return scaledUpSkill;
    }

    protected void ScaleUpBaseValueForLevel(SkillData scaledUpSkill, ScalingEnemyData scaling, int level)
    {
        scaledUpSkill.cooldown = cooldown;
        scaledUpSkill.duration = duration;
        scaledUpSkill.damage = damage * Mathf.FloorToInt(scaling.AttackValueCurve.Evaluate(level));
        scaledUpSkill.unlockSkillLevel = unlockSkillLevel;
    }

    public virtual void UseSkill(Enemy enemy, IDamageable damageable, float enemyCooldown){
        enemy.skillActivatingDict[this] = true;
        enemy.isSkillActivating = true;
        enemy.StartCoroutine(CounterCooldown(enemy ,enemyCooldown));
    }

    IEnumerator CounterCooldown(Enemy enemy ,float enemyCooldown)
    {
        yield return new WaitForSeconds(enemyCooldown + duration);
        enemy.skillActivatingDict[this] = false;
    }

    public virtual bool CanUseSkill(Enemy enemy, IDamageable damageable, int level){
        return level >= unlockSkillLevel;
    }
}

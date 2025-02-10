using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[CreateAssetMenu(fileName = "Meteor", menuName = "Enemy/Skill/Meteor", order = 1)]
public class MeteorSkill : SkillData
{
    public GameObject skillPrefab;
    public float skillRange = 10f;

    public override SkillData ScaleUpForLevel(ScalingEnemyData scaling, int level)
    {
        MeteorSkill Instance = CreateInstance<MeteorSkill>();

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

        Debug.Log("Meteor: UseSkill");

        enemy.StartCoroutine(ActivateSkill(enemy, damageable));
    }

    IEnumerator ActivateSkill(Enemy enemy, IDamageable damageable)
    {

        Vector3 damageablePosition = damageable.GetTransform().position;

        Vector2 randomPoint = UnityEngine.Random.insideUnitCircle;

        Vector3 potentialPoint = new Vector3(
            damageablePosition.x + randomPoint.x,
            damageablePosition.y,
            damageablePosition.z + randomPoint.y
        );

        RaycastHit hit;
        if (Physics.Raycast(potentialPoint, Vector3.down, out hit, 100f, LayerMask.GetMask("Default")))
        {
            potentialPoint = hit.point;
        }
        else
        {
            Debug.LogError("Meteor: No ground found");
        }

        // GameObject skill = _skillPool.Get();
        // AreaDamage areaDamage = skill.GetComponentInChildren<AreaDamage>();

        GameObject skill = Instantiate(skillPrefab, potentialPoint, Quaternion.identity);
        ConfigMeteor(skill);

        yield return new WaitForSeconds(duration);

        enemy.isSkillActivating = false;

        Destroy(skill);
    }


    private void ConfigMeteor(GameObject skill)
    {
        var meteor = skill.GetComponent<HS_ParticleCollisionInstance>();
        if(meteor == null) meteor = skill.GetComponentInChildren<HS_ParticleCollisionInstance>();
        var hit = meteor.EffectsOnCollision[0].GetComponent<MeteorHit>();
        hit.Damage = damage;
    }
}

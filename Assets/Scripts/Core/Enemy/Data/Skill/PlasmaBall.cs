using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[CreateAssetMenu(fileName = "PlasmaBall", menuName = "Enemy/Skill/PlasmaBall", order = 1)]
public class PlasmaBall : SkillData
{
    public GameObject skillPrefab;
    public GameObject explosionEffect;
    public float skillRange = 10f;
    public float speed = 10f;
    public float explodeRadius = 4f;
    private ObjectPool<GameObject> _skillPool;
    private bool _poolInitialize = false;

    public void InitializePool()
    {
        if (!_poolInitialize)
        {
            _skillPool = new ObjectPool<GameObject>(
                CreateSkill,
                OnTakeSkillFromPool,
                OnReturnSkillFromPool,
                OnDestroySkill,
                true,
                100,  // Số lượng đối tượng ban đầu trong pool
                2000  // Số lượng tối đa của đối tượng trong pool
            );
            _poolInitialize = true;
        }
    }


    public override SkillData ScaleUpForLevel(ScalingEnemyData scaling, int level)
    {
        PlasmaBall Instance = CreateInstance<PlasmaBall>();

        ScaleUpBaseValueForLevel(Instance, scaling, level);
        Instance.skillRange = skillRange;
        // Instance.InitializePool();

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

        Debug.Log("PlasmaBall: UseSkill");

        enemy.StartCoroutine(ActivateSkill(enemy, damageable));
    }

    IEnumerator ActivateSkill(Enemy enemy, IDamageable damageable)
    {

        Vector3 damageablePosition = damageable.GetTransform().position;


        // RaycastHit hit;
        // IDamageable damageableHit = null;
        // if (Physics.Raycast(enemy.transform.position, damageablePosition, out hit, 100f, LayerMask.GetMask("Player")))
        // {
        //     if (hit.collider.TryGetComponent(out IDamageable beDamage))
        //     {
        //         damageableHit = beDamage;
        //     }
        // }

        Debug.Log("PlasmaBall: ActivateSkill");


        // GameObject skill = _skillPool.Get();
        // AreaDamage areaDamage = skill.GetComponentInChildren<AreaDamage>();
        Vector3 spawnPoint = enemy.GetProjectileSpawnpoint();
        GameObject skill = Instantiate(skillPrefab, spawnPoint, Quaternion.identity);
        BurstDamage burstDamage = skill.GetComponent<BurstDamage>();
        ForceToTarget forceToTarget = skill.GetComponent<ForceToTarget>();
        forceToTarget.SetTarget(damageable.GetTransform(), speed);
        ConfigAreaDamage(burstDamage, damageablePosition);


        yield return new WaitForSeconds(duration);

        enemy.isSkillActivating = false;

    }

    private void DestroySkill(BurstDamage skill)
    {
        var explodeObject = Instantiate(explosionEffect, skill.transform.position, Quaternion.identity);
        explodeObject.transform.SetParent(null);

        Destroy(skill.gameObject);


    }


    private void ConfigAreaDamage(BurstDamage burstDamage, Vector3 damageablePosition)
    {
        burstDamage.onExplode += OnSkillExplode;
        burstDamage.damage = damage;
        burstDamage.targetPoint = damageablePosition;
        burstDamage.explodeRadius = explodeRadius;
        burstDamage.timeExisting = duration;
    }

    private void OnSkillExplode(BurstDamage burstDamageObject)
    {
        DestroySkill(burstDamageObject);

    }




    #region Pooling Methods
    private GameObject CreateSkill()
    {
        GameObject skill = Instantiate(skillPrefab);
        return skill;
    }

    private void OnReturnSkillFromPool(GameObject skill)
    {
        skill.SetActive(false);
    }
    private void OnTakeSkillFromPool(GameObject skill)
    {
        skill.SetActive(true);
    }
    private void OnDestroySkill(GameObject skill)
    {
        Destroy(skill);
    }
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;


[CreateAssetMenu(fileName = "PoisonPlace", menuName = "Enemy/Skill/PoisonPlace", order = 1)]
public class PoisonPlace : SkillData
{
    public GameObject skillPrefab;
    public float skillRange = 10f;
    public float tickRate = 2f;
    public float insideUnitCircleRadius = 4f;
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
        PoisonPlace Instance = CreateInstance<PoisonPlace>();

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

        Debug.Log("PoisonPlace: UseSkill");

        enemy.StartCoroutine(ActivateSkill(enemy, damageable));
    }

    IEnumerator ActivateSkill(Enemy enemy, IDamageable damageable)
    {

        Vector3 damageablePosition = damageable.GetTransform().position;

        Vector2 randomPoint = UnityEngine.Random.insideUnitCircle * insideUnitCircleRadius;

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
            Debug.LogError("PoisonPlace: No ground found");
        }

        // GameObject skill = _skillPool.Get();
        // AreaDamage areaDamage = skill.GetComponentInChildren<AreaDamage>();

        GameObject skill = Instantiate(skillPrefab, potentialPoint, Quaternion.identity);
        // AreaDamage areaDamage = skill.GetComponent<AreaDamage>();
        // ConfigAreaDamage(areaDamage);



        yield return new WaitForSeconds(duration);

        enemy.isSkillActivating = false;

        Destroy(skill);
    }


    // private void ConfigAreaDamage(AreaDamage areaDamage)
    // {
    //     areaDamage.damage = damage;
    //     areaDamage.tickRate = tickRate;
    // }


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

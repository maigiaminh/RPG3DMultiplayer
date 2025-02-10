using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackConfiguration", menuName = "Enemy/Attack Configuration")]
public class AttackConfiguration : ScriptableObject
{
    [Header("Stats Configs")]
    public bool isRanged = false;
    public int damage = 5;
    public float attackRange = 5f;
    public float attackSpeed = 1f;
    public float attackDelay = 1.5f;
    public float attackCooldown = 3;

    // Ranged Configs
    [Space]
    [Header("Ranged Configs")]
    public Projectile projectilePrefab;
    public LayerMask LineOfSightLayers;

    // Animator config
    [Space]
    [Header("Animator Configs")]
    private List<float> _attackAnimTimes = new List<float>();
    private List<float> _takeDamageAnimTimes = new List<float>();


    public AttackConfiguration ScaleUpForLevel(ScalingEnemyData Scaling, int Level)
    {
        AttackConfiguration scaledUpConfiguration = CreateInstance<AttackConfiguration>();

        scaledUpConfiguration.isRanged = isRanged;
        scaledUpConfiguration.damage = Mathf.FloorToInt(damage * Scaling.AttackValueCurve.Evaluate(Level));
        scaledUpConfiguration.attackRange = attackRange;
        scaledUpConfiguration.attackDelay = attackDelay;

        scaledUpConfiguration.projectilePrefab = projectilePrefab;
        scaledUpConfiguration.LineOfSightLayers = LineOfSightLayers;

        return scaledUpConfiguration;

    }

    public async void SetUpEnemy(Enemy enemy)
    {
        enemy.AttackDelay = attackDelay;
        enemy.Damage = damage;
        enemy.AttackSpeed = attackSpeed;
        enemy.AttackCooldown = attackCooldown;
        enemy.AttackRange = attackRange;
        _attackAnimTimes = GetAttackAnimTime(enemy.animator);
        enemy.AttackAnimTimes = _attackAnimTimes;
        _takeDamageAnimTimes = GetTakeDamageAnimTime(enemy.animator);
        enemy.TakeDamageAnimTimes = _takeDamageAnimTimes;


        if (isRanged)
        {
            await WaitForSettingUpRangedAttack(enemy);
        }


    }



    private async Task WaitForSettingUpRangedAttack(Enemy enemy)
    {

        while (enemy.rangedAttackRadius == null)
        {
            await Task.Delay(100);
        }
        RangedAttackRadius rangedAttackRadius = enemy.rangedAttackRadius.GetComponent<RangedAttackRadius>();
        if (rangedAttackRadius == null)
        {
            Debug.LogError("Ranged Attack Radius is null");
            return;
        }
        rangedAttackRadius.projectilePrefab = projectilePrefab;
        rangedAttackRadius.Mask = LineOfSightLayers;

        rangedAttackRadius.CreateBulletPool();
    }


    private List<float> GetAttackAnimTime(Animator animator)
    {
        List<float> times = new List<float>();
        int index = 1;
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (attackSpeed == 0) attackSpeed = 1;
            if (clip.name.Equals("Attack " + index))
            {
                times.Add(clip.length / attackSpeed);
                index++;
            }
        }

        if (attackSpeed == 0) attackSpeed = 1;

        for (int i = 0; i < times.Count; i++)
        {
            times[i] = times[i] / attackSpeed;
        }

        return times ?? new List<float>();
    }
    private List<float> GetTakeDamageAnimTime(Animator animator)
    {
        List<float> times = new List<float>();
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name.StartsWith("TakeDamage"))
            {
                times.Add(clip.length);
            }
        }
        Debug.Log("Take Damage Anim Times: " + times.Count);
        foreach(float time in times)
        {
            Debug.Log("Time: " + time);
        }
        return times ?? new List<float>();
    }


}

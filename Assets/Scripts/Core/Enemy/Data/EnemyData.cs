using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/EnemyData", order = 0)]
public class 
EnemyData : ScriptableObject
{
    [Header("Token")]
    public int Token;
    [Header("Enemy Type")]
    public EnemyType EnemyType;
    public DifficultyType DifficultyType;
    [Header("Rewards")]
    public int XP;
    public int Gold;
    public int Misc;
    [Header("Item Drops")]
    public List<ItemDrop> ItemDrops;
    [Range(0f, 1f)]
    public float dropRate = 1f;     // Tỉ lệ rơi chung
    [Header("Data & Prefab")]
    public IEnemy Prefab;
    public AttackConfiguration AttackConfiguration;
    public SkillData[] Skills;
    public AuraType AuraType;

    [Header("Outfit")]
    [SerializeField]
    public EnemyOutfitData OutfitData;
    public Sprite Portrait;


    // Enemy Stats
    [Header("Enemy Attributes")]

    // Enemy Stats
    public int Health;
    public float DissolveTime = 7f;
    public float TimeToDissolve = 3f;
    public GameObject DissolveEffect;

    // Movement Configs
    public float ChaseSpeed = 2;
    public float ChaseRange = 5;
    public float IdleTime = 7f;
    public float PatrolSpeed = 1.5f;
    public float PatrolTime = 7f;
    public float RotationTime = 1.5f;

    // Enemy Type
    public bool isRanger = false;
    public bool isMelee = false;
    public bool isMage = false;
    public bool isTank = false;

    // NavMeshAgent Configs
    [Header("NavMeshAgent Attributes")]
    public float AIUpdateInterval = 0.1f;

    public float Acceleration = 8;
    public float AngularSpeed = 120;

    // -1 means everything
    public int AreaMask = -1;
    public int AvoidancePriority = 50;
    public float BaseOffset = 0;
    public float Height = 2;
    public ObstacleAvoidanceType ObstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
    public float Radius = 0.5f;
    public float MoveSpeed = 1f;
    public float StoppingDistance = 0.5f;
    public EnemyData ScaleUpForLevel(ScalingEnemyData Scaling, int Level)
    {

        EnemyData scaledUpEnemy = CreateInstance<EnemyData>();
        // Config Enemy XP
        scaledUpEnemy.XP = Mathf.FloorToInt(XP * Scaling.XPCurve.Evaluate(Level));
        scaledUpEnemy.Gold = Mathf.FloorToInt(Gold * Scaling.GoldCurve.Evaluate(Level));

        // Config Enemy Stats
        scaledUpEnemy.AttackConfiguration = AttackConfiguration.ScaleUpForLevel(Scaling, Level);
        scaledUpEnemy.MoveSpeed = MoveSpeed * Scaling.SpeedCurve.Evaluate(Level);
        scaledUpEnemy.Health = Mathf.FloorToInt(Health * Scaling.HealthCurve.Evaluate(Level));

        scaledUpEnemy.name = name;
        scaledUpEnemy.Prefab = Prefab;

        // Scale up Skills
        scaledUpEnemy.Skills = new SkillData[Skills.Length];
        if (Skills.Length != 0)
        {
            for (int i = 0; i < Skills.Length; i++)
            {
                scaledUpEnemy.Skills[i] = Skills[i].ScaleUpForLevel(Scaling, Level);
            }
        }


        // NavMesh Configs
        scaledUpEnemy.AIUpdateInterval = AIUpdateInterval;
        scaledUpEnemy.Acceleration = Acceleration;
        scaledUpEnemy.AngularSpeed = AngularSpeed;
        scaledUpEnemy.AreaMask = AreaMask;
        scaledUpEnemy.AvoidancePriority = AvoidancePriority;
        scaledUpEnemy.BaseOffset = BaseOffset;
        scaledUpEnemy.Height = Height;
        scaledUpEnemy.ObstacleAvoidanceType = ObstacleAvoidanceType;
        scaledUpEnemy.Radius = Radius;
        scaledUpEnemy.StoppingDistance = StoppingDistance;

        return scaledUpEnemy;
    }



    public void SetUpEnemy(Enemy enemy)
    {
        // Reward
        enemy.XP = XP;
        enemy.Gold = Gold;
        enemy.Misc = Misc;
        // Agent
        enemy.Agent.acceleration = Acceleration;
        enemy.Agent.angularSpeed = AngularSpeed;
        enemy.Agent.areaMask = AreaMask;
        enemy.Agent.avoidancePriority = AvoidancePriority;
        enemy.Agent.baseOffset = BaseOffset;
        enemy.Agent.height = Height;
        enemy.Agent.obstacleAvoidanceType = ObstacleAvoidanceType;
        enemy.Agent.radius = Radius;
        enemy.Agent.speed = MoveSpeed;
        enemy.Agent.stoppingDistance = StoppingDistance;
        


        // Config Enemy Stats
        enemy.DissolveTime = DissolveTime;
        enemy.TimeToDissolve = TimeToDissolve;
        enemy.DissolveEffect = DissolveEffect;
        enemy.MaxHealth = Health;
        enemy.MoveSpeed = MoveSpeed;
        enemy.ChaseSpeed = ChaseSpeed;
        enemy.ChaseRange = ChaseRange;
        enemy.IdleTime = IdleTime;
        enemy.PatrolSpeed = PatrolSpeed;
        enemy.PatrolTime = PatrolTime;
        enemy.RotationTime = RotationTime;
        enemy.IsRanged = isRanger;



        AttackConfiguration.SetUpEnemy(enemy);
    }



}

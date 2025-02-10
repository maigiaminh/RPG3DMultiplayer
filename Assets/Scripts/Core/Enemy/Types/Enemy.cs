using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BrainFailProductions.PolyFew;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;
using UnityEngine.UIElements;
using UnityEngine.VFX;

public class Enemy : MonoBehaviour, IEnemy, IDamageable, IHealth
{

    [Header("Enemy Data")]
    public EnemyData enemyData;
    public WeightedSpawnData weightedSpawnData;
    [SerializeField]
    private List<TargetType> _targetTypes = new List<TargetType>();
    [SerializeField]
    protected LayerMask _attackLayerMask;
    [SerializeField]
    protected LayerMask _chaseLayerMask;
    [SerializeField]
    protected SpawnArea _spawnArea;
    [SerializeField]
    private Transform _projectileSpawnpoint;

    // Skill
    public Enemy(IDamageable damageable, SkillData currentSkill, int health, float dissolveRate, GameObject dissolveEffect, int xP, int misc, float moveSpeed, float chaseRange, float patrolSpeed, float rotationTime, int damage, float attackSpeed, float attackCooldown, NavMeshAgent agent)
    {
        this.Damageable = damageable;
        this.CurrentSkill = currentSkill;
        this.Health = health;
        this.DissolveTime = dissolveRate;
        this.DissolveEffect = dissolveEffect;
        this.XP = xP;
        this.Misc = misc;
        this.MoveSpeed = moveSpeed;
        this.ChaseRange = chaseRange;
        this.PatrolSpeed = patrolSpeed;
        this.RotationTime = rotationTime;
        this.Damage = damage;
        this.AttackSpeed = attackSpeed;
        this.AttackCooldown = attackCooldown;
        this.Agent = agent;
        // ConfigSkill();
    }
    public IDamageable Damageable { get; set; }
    public int Level { get; set; } = 10;
    private List<SkillData> _skills = new List<SkillData>();
    // private float[] _skillCooldowns;
    public SkillData CurrentSkill { get; set; }
    public Dictionary<SkillData, bool> skillActivatingDict = new Dictionary<SkillData, bool>();

    [HideInInspector]
    public bool isSkillActivating = false;


    // Side Effect State Prefab
    [Header("Side Effect State Prefab")]
    public GameObject iceEffectPrefab;


    // Health Stats
    private GameEventManager _gameEventManager;


    enum HealthbarsMode
    {
        UI,
        Sprite
    }


    public GameObject GetGameObject() => gameObject;
    public float GetHealth() => Health;
    public float GetMaxHealth() => MaxHealth;


    [SerializeField] HealthbarsMode mode = HealthbarsMode.UI;
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    [HideInInspector] public bool enemyTakeDamage = false;
    [HideInInspector] public bool enemyDie = false;
    [HideInInspector] public int currentTakeDamageAnimIndex = 0;
    public float DissolveTime { get; set; }
    public float TimeToDissolve { get; set; }
    public bool isDissolve = false;
    public GameObject DissolveEffect { get; set; }


    // Rewards
    public int XP { get; set; }
    public int Gold { get; set; }
    public int Misc { get; set; }


    // Movement Stats
    public float MoveSpeed { get; set; }
    public float ChaseSpeed { get; set; }
    public float ChaseRange { get; set; }
    public float IdleTime { get; set; }
    public float PatrolSpeed { get; set; }
    public float PatrolTime { get; set; }
    public float RotationTime { get; set; }
    public bool IsRanged { get; set; }


    // Attack Stats
    public int Damage { get; set; }
    public float AttackRange { get; set; }
    public float AttackSpeed { get; set; } = 1;
    public float AttackDelay { get; set; }
    public float AttackCooldown { get; set; }


    // Animator Configs
    public List<float> AttackAnimTimes = new List<float>();
    public List<float> TakeDamageAnimTimes = new List<float>();



    public GameObject GameObject => gameObject;


    public NavMeshAgent Agent { get; set; }
    public SkinnedMeshRenderer SkinMeshRenderer { get; set; }




    public Transform GetTransform() => transform;



    protected EnemyStateMachine stateMachine;
    protected ObjectPool<IEnemy> pool;


    public Animator animator;
    protected RangeDetector _attackDetector;
    protected RangeDetector _chaseDetector;

    public AttackRadius attackRadius;
    public RangedAttackRadius rangedAttackRadius;
    // public EnemyLineOfSightChecker lineOfSightChecker;

    // EVENTTT
    public delegate void AttackEvent();
    public event AttackEvent EnemyOnAttack;

    public delegate void DeathEvent(IEnemy enemy);
    public event DeathEvent OnDeath;

    public UIHealthbars.HealthChangedAction HealthWasChanged;
    public event Action<int, int> EnemyHealthChanged;



    // CC Attribute
    [HideInInspector]
    public bool isFreeze = false;
    private float _freezeDuration = 0;
    [HideInInspector]
    public bool isFasten = false;
    private float _fastenDuration = 0;
    [HideInInspector]
    public bool isStun = false;
    private float _stunDuration = 0;
    [HideInInspector]
    public bool isSlow = false;
    private float _slowDuration = 0;

    // Release Control
    public Coroutine dissolveCoroutine = null;



    protected virtual void Awake()
    {

        _gameEventManager = GameEventManager.Instance;

        ConfigSkill();

        Agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        SkinMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        SkinMeshRenderer.material = PickRandomMaterial(enemyData.OutfitData.Materials);

        enemyData.SetUpEnemy(this);

        _attackDetector = GetAttackRangeDetector("AttackRangeDetector", IsRanged, AttackRange, _attackLayerMask);
        _chaseDetector = GetChaseRangeDetector("ChaseRangeDetector", ChaseRange, _chaseLayerMask);


        _chaseDetector.OnTargetChanged += (target) => Damageable = target;

        // Create Dissolve Effect
        CreateDissolve();


        // Setting up the state machine
        stateMachine = new EnemyStateMachine();
        var dissolveController = new DissolvingController(this, DissolveTime, DissolveEffect);


        var idle = new Idle(this, Agent, animator, IdleTime);
        var patrol = new Patrol(this, Agent, animator, PatrolSpeed, PatrolTime, _spawnArea);
        var chase = new Chase(this, Agent, animator, _chaseDetector, ChaseSpeed);
        var attack = new EnemyAttack(this, Agent, animator, AttackSpeed, AttackAnimTimes);
        var castAbility = new CastAbility(this, Agent, animator, _skills);
        var takeDamage = new TakeDamage(this, Agent, animator, TakeDamageAnimTimes);
        var die = new EnemyDie(this, Agent, animator, TimeToDissolve);
        var dissolve = new Dissolve(dissolveController);
        var freeze = new Freeze(this, Agent, animator, iceEffectPrefab);
        var fasten = new Fasten(this, Agent, animator);



        // Add At transitions
        At(idle, patrol, IsIdleTimerComplete());
        At(patrol, idle, IsPatrolTimerComplete());
        At(chase, idle, IsTargetNotInChaseRange());
        At(chase, attack, IsTargetInAttackRange());
        At(attack, idle, IsTargetNotInAttackRange());
        At(castAbility, idle, IsTargetNotInChaseRange());
        At(castAbility, attack, IsNotCastingSkillAndInAttackRange());
        At(takeDamage, idle, IsTakeDamageAnimComplete());
        At(freeze, idle, IsEnemyNotFreeze());
        At(fasten, idle, IsEnemyNotFasten());
        // At(die, dissolve, IsDissolve());

        // Add Any transitions 
        Any(chase, IsEnemyChaseAndActionFinish());
        Any(castAbility, IsCastingSkill());
        Any(takeDamage, EnemyTakeDamage());
        Any(die, IsEnemyDie());
        Any(dissolve, IsDissolve());
        Any(freeze, IsEnemyFreeze());
        Any(fasten, IsEnemyFasten()); ;


        // Set Initial State
        stateMachine.SetState(idle);


        // Settings
        void At(IState to, IState from, Func<bool> condition) => stateMachine.AddTransition(to, from, condition);
        void Any(IState state, Func<bool> condition) => stateMachine.AddAnyTransition(state, condition);


        // Conditions
        Func<bool> IsIdleTimerComplete() => () => idle.timer >= idle.idleTime;
        Func<bool> IsPatrolTimerComplete() => () => patrol.timer >= patrol.patrolTime;
        Func<bool> IsTargetNotInChaseRange() => () => !_chaseDetector.TargetInRange;
        Func<bool> IsTargetInAttackRange() => () => _attackDetector.TargetInRange;
        Func<bool> IsTargetNotInAttackRange() => () => !_attackDetector.TargetInRange;
        Func<bool> IsEnemyChaseAndActionFinish() => () => _chaseDetector.TargetInRange
                                                        && !_attackDetector.TargetInRange
                                                        && !isSkillActivating
                                                        && !enemyTakeDamage
                                                        && !enemyDie
                                                        && !isDissolve
                                                        && !isFreeze
                                                        && !isFasten;
        Func<bool> IsCastingSkill() => () => isSkillActivating
                                            && !enemyDie
                                            && !isDissolve
                                            && !isFreeze
                                            && !isFasten
                                            && !isStun
                                            && !isSlow
                                            && !enemyTakeDamage
                                            && _attackDetector.TargetInRange;
        Func<bool> IsNotCastingSkillAndInAttackRange() => () => !isSkillActivating
                                                        && _attackDetector.TargetInRange;
        Func<bool> EnemyTakeDamage() => () => enemyTakeDamage && !enemyDie && !isDissolve && !isFreeze;
        Func<bool> IsTakeDamageAnimComplete() => () => takeDamage.animTimer >=
                                                        TakeDamageAnimTimes[currentTakeDamageAnimIndex % TakeDamageAnimTimes.Count];
        Func<bool> IsEnemyDie() => () => enemyDie && !isDissolve;
        Func<bool> IsDissolve() => () => isDissolve;
        Func<bool> IsEnemyFreeze() => () => isFreeze;
        Func<bool> IsEnemyNotFreeze() => () => freeze.counterFreeze >= _freezeDuration;
        Func<bool> IsEnemyFasten() => () => isFasten;
        Func<bool> IsEnemyNotFasten() => () => fasten.counterFasten >= _fastenDuration;
    }
    private void OnEnable() => Restart();
    private void OnDisable() => HealthWasChanged = null;
    private void Start() => Invoke(nameof(SetupHealth), 1f);
    protected virtual void Update()
    {
        stateMachine.Tick();
        SkillCheck();
        TakeDamageCheck();
    }
    private void TakeDamageCheck() { }
    private void ConfigSkill()
    {
        if (enemyData.Skills.Length == 0) return;

        _skills = enemyData.Skills.ToList();
        CurrentSkill = _skills[0];

        if (skillActivatingDict == null)
        {
            skillActivatingDict = new Dictionary<SkillData, bool>();
        }


        for (int i = 0; i < _skills.Count; i++)
        {
            skillActivatingDict.Add(_skills[i], false);
        }
    }



    protected void SkillCheck()
    {
        if (enemyDie) return;
        if (IsInCC()) return;

        if (enemyData.Skills.Length == 0) return;
        for (int i = 0; i < _skills.Count; i++)
        {
            if (!_skills[i].CanUseSkill(this, Damageable, Level)) continue;
            Debug.Log("Can Use Skill________________");
            CurrentSkill = _skills[i];
            CurrentSkill.UseSkill(this, Damageable, _skills[i].cooldown);
            return;
        }
    }


    protected RangeDetector GetChaseRangeDetector(string name, float radius, LayerMask detectLayerMask)
    {
        GameObject detectRadiusGameObject = new GameObject(name);
        // GameObject detectRadiusGameObject = new GameObject(name);
        detectRadiusGameObject.layer = Mathf.RoundToInt(Mathf.Log(detectLayerMask.value, 2));
        // add sphere before adding AttackRadius script
        detectRadiusGameObject.AddComponent<SphereCollider>().isTrigger = true;
        RangeDetector detectRadius = detectRadiusGameObject.AddComponent<RangeDetector>();
        detectRadiusGameObject.transform.SetParent(transform);
        detectRadiusGameObject.transform.localPosition = Vector3.zero;
        // Setting attributes
        detectRadiusGameObject.GetComponent<SphereCollider>().radius = radius;
        // Draw Gizmo
        return detectRadius;
    }

    protected RangeDetector GetAttackRangeDetector(string name, bool isRange, float radius, LayerMask detectLayerMask)
    {
        GameObject detectRadiusGameObject = new GameObject(name);

        // GameObject detectRadiusGameObject = new GameObject(name);
        detectRadiusGameObject.layer = Mathf.RoundToInt(Mathf.Log(detectLayerMask.value, 2));

        // add sphere before adding AttackRadius script
        detectRadiusGameObject.AddComponent<SphereCollider>().isTrigger = true;
        RangeDetector detectRadius = detectRadiusGameObject.AddComponent<RangeDetector>();
        detectRadiusGameObject.transform.SetParent(transform);
        detectRadiusGameObject.transform.localPosition = Vector3.zero;

        // Setting attributes
        detectRadiusGameObject.GetComponent<SphereCollider>().radius = radius;
        // Draw Gizmo
        // DrawGizmo(detectRadiusGameObject, radius, UnityEngine.Random.ColorHSV());

        if (isRange == false)
        {
            attackRadius = detectRadiusGameObject.AddComponent<AttackRadius>();
            attackRadius.Initialize(this);
        }
        else
        {
            attackRadius = detectRadiusGameObject.AddComponent<RangedAttackRadius>();
            rangedAttackRadius = (RangedAttackRadius)attackRadius;
            rangedAttackRadius.Initialize(_projectileSpawnpoint, _attackLayerMask);

            attackRadius.Initialize(this);
            rangedAttackRadius.Initialize(this);
            // lineOfSightChecker = detectRadiusGameObject.AddComponent<EnemyLineOfSightChecker>();
        }

        return detectRadius;
    }


    protected void HandleEnemyAttack() => StartCoroutine(LookAtTarget());

    protected IEnumerator LookAtTarget()
    {
        if (_attackDetector.detectTarget == null) yield break;
        Quaternion lookRotation = Quaternion.LookRotation(_attackDetector.detectTarget.position - transform.position);
        float timer = 0;
        while (timer < RotationTime)
        {
            timer += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, timer);
            yield return null;
        }
        transform.rotation = lookRotation;
    }

    public void SetPool(ObjectPool<IEnemy> pool) => this.pool = pool;


    #region Health
    private void SetupHealth()
    {
        Health = MaxHealth;
        OnEnemyHealthChanged();
        AddHealthbarToThisObject();
    }

    void AddHealthbarToThisObject()
    {
        if (mode == HealthbarsMode.UI)
        {
            var healthBar = UIHealthbars.AddHealthbar(gameObject, MaxHealth);
            HealthWasChanged += healthBar.OnHealthChanged;
        }

        OnHealthChanged();
    }

    void OnHealthChanged() => HealthWasChanged?.Invoke(Health);
    void OnEnemyHealthChanged() => EnemyHealthChanged?.Invoke(Health, MaxHealth);
    public void Attack()
    {
        EnemyOnAttack?.Invoke();
        HandleEnemyAttack();
    }


    public void TakeDamage(Transform trans, int damage)
    {
        if (enemyDie) return;


        if (_gameEventManager) _gameEventManager.DamageEvent.NotifyDamage(transform, damage);

        enemyTakeDamage = true;
        Health = Mathf.Clamp(Health - damage, 0, MaxHealth);

        OnEnemyHealthChanged();
        OnHealthChanged();

        if (Health == 0)
        {
            OnDeath?.Invoke(this);
            if (_gameEventManager) _gameEventManager.PlayerEvents.PlayerDefeatEnemy(this, 1);
            Die();
        }

    }

    private void Die()
    {
        Debug.Log("Enemy Die");
        enemyDie = true;

        if (enemyData.ItemDrops.Count > 0) EnemyDropItems();

        if (Gold != 0) if (_gameEventManager) _gameEventManager.ResourceEvents.PlayerGainGold(Gold);
        if (Misc != 0) if (_gameEventManager) _gameEventManager.ResourceEvents.PlayerGainMisc(Misc);
        if (XP != 0) if (_gameEventManager) _gameEventManager.PlayerEvents.PlayerGainExperience(XP);
        // this.enabled = false;
        // Destroy(gameObject, 2f);
    }

    #region CC

    public void ApplyCC(CCType ccType, float ccDuration)
    {
        if (enemyDie) return;
        Debug.Log("Enemy Apply CC + " + ccType + " for " + ccDuration + " seconds");
        switch (ccType)
        {
            case CCType.Stun:
                Stun(ccDuration);
                break;
            case CCType.Slow:
                Slow(ccDuration);
                break;
            case CCType.Freeze:
                Freeze(ccDuration);
                break;
            case CCType.Fasten:
                Fasten(ccDuration);
                break;
        }
    }

    private void Fasten(float ccDuration)
    {
        _fastenDuration = ccDuration;
        isFasten = true;
        Invoke(nameof(UnFasten), ccDuration);
    }

    private void UnFasten() => isFasten = false;

    private void Freeze(float ccDuration)
    {
        _freezeDuration = ccDuration;
        isFreeze = true;
        Invoke(nameof(UnFreeze), ccDuration);
    }
    private void UnFreeze() => isFreeze = false;

    private void Slow(float ccDuration) => throw new NotImplementedException();

    private void Stun(float ccDuration) => throw new NotImplementedException();


    private bool IsInCC() => isFreeze || isFasten || isStun || isSlow;

    #endregion


    private void CreateDissolve()
    {
        Debug.Log($"Attempting to create dissolve effect for {name}");

        if (SkinMeshRenderer == null)
        {
            SkinMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
            if (SkinMeshRenderer == null)
            {
                Debug.LogError($"{name} does not have a Skinned Mesh Renderer.");
                return;
            }
        }

        DissolveEffect = Instantiate(DissolveEffect, transform.position, Quaternion.identity);
        var vfx = DissolveEffect.GetComponent<VisualEffect>();
        Debug.Log($"Instantiated DissolveEffect for {name}");

        vfx.gameObject.SetActive(true);

        if (vfx.HasSkinnedMeshRenderer("Skin"))
        {
            vfx.SetSkinnedMeshRenderer("Skin", SkinMeshRenderer);
            Debug.Log("Successfully set Skinned Mesh Renderer for dissolve effect.");
        }
        else
        {
            Debug.LogError("Dissolve Effect does not have Skinned Mesh Renderer property 'Skin'.");
        }

        DissolveEffect.SetActive(false);
        DissolveEffect.transform.SetParent(transform);
    }

    private void EnemyDropItems()
    {
        if (enemyData.ItemDrops != null && enemyData.ItemDrops.Count > 0)
        {
            // Kiểm tra tỉ lệ rơi chung
            float dropRate = enemyData.dropRate + PlayerStatManager.Instance.Luck * 0.01f;
            if (UnityEngine.Random.value <= dropRate)
            {
                foreach (ItemDrop itemDrop in enemyData.ItemDrops)
                {
                    // Tính toán số lượng vật phẩm rơi ra theo tỉ lệ với dropChance của từng ItemDrop
                    float adjustedDropChance = itemDrop.dropChance * dropRate;

                    if (UnityEngine.Random.value <= adjustedDropChance)
                    {
                        int quantity = itemDrop.GetRandomQuantity();
                        for (int i = 0; i < quantity; i++)
                        {
                            var item = Instantiate(itemDrop, transform.position, Quaternion.identity);
                            item.transform.position = FindProperSpawnPoint();
                        }
                    }
                }
            }
        }
    }


    private Vector3 FindProperSpawnPoint()
    {

        Vector3 spawnPoint = transform.position + UnityEngine.Random.insideUnitSphere * 5f;
        RaycastHit hit;
        Physics.Raycast(spawnPoint, Vector3.down, out hit, 100f);
        spawnPoint = hit.point;
        return spawnPoint;
    }

    public void EnemyDestroy()
    {
        if (dissolveCoroutine != null)
            StopCoroutine(dissolveCoroutine);
        if (pool != null)
            pool.Release(this);
        dissolveCoroutine = null;
    }

    private void Restart()
    {
        Health = MaxHealth;
        OnHealthChanged();
        enemyTakeDamage = false;
        enemyDie = false;
        isDissolve = false;
        isFreeze = false;
        isFasten = false;
        isStun = false;
        isSlow = false;
        dissolveCoroutine = null;
        DissolveEffect.SetActive(false);
        Material material = new Material(SkinMeshRenderer.material);
        material.SetFloat("_DissolveAmount", 0);
        SkinMeshRenderer.material = material;

    }

    public void SetState(IState state) => stateMachine.SetState(state);
    private Material PickRandomMaterial(List<Material> materials)
    {
        return materials[UnityEngine.Random.Range(0, materials.Count)];
    }



    public Vector3 GetProjectileSpawnpoint() => _projectileSpawnpoint.position;


    #endregion

    public enum TargetType
    {
        Player,
        Boat
    }

    public enum SpawnArea
    {
        Enemy
    }
}


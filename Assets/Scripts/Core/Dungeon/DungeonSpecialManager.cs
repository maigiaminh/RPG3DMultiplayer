using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

public class DungeonSpecialManager : MonoBehaviour
{

    public static DungeonSpecialManager Instance;
    public List<DungeonSpecialMap> DungeonSpecialMaps = new List<DungeonSpecialMap>();



    private List<Enemy> _bosses = new List<Enemy>();
    private TankPlayer _player;
    private DungeonDescriptScriptableObject.DungeonType _currentDungeonType;
    public DungeonDescriptScriptableObject.DungeonType CurrentDungeonType => _currentDungeonType;
    public DungeonData CurrentDungeonData { get; set; }
    private DungeonSpecialMap _currentDungeonSpecialMap;
    private List<Enemy> _aliveBosses = new List<Enemy>();
    private ObjectPool<IEnemy> _enemyPool;
    private int _aliveBossCount = 0;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;


    }

    private void Start()
    {
        if (DungeonDescriptManager.Instance == null)
        {
            Debug.LogError("Dungeon Descript Manager is not assigned");
            return;
        }
        Initialize();
        InitializePool();
        ConfigDungeonMap();
        SpawnEnemy();
        DungeonSpecialUIManager.Instance.DungeonSpecialUIPanel.UpdateBossStateItemUI(_aliveBosses);
        SpawnPlayerRandomly();
    }


    public void SpawnPlayerRandomly()
    {
        StartCoroutine(SpawnPlayer());
    }
    private void Initialize()
    {
        var specialDescriptPanel = DungeonDescriptManager.Instance.SpecialDescriptPanel;
        _currentDungeonType = specialDescriptPanel.CurrentDungeonDifficultyItem.DungeonType;
        CurrentDungeonData = specialDescriptPanel.CurrentDungeonDifficultyItem.DungeonDescript.DungeonData;
        _bosses = specialDescriptPanel.CurrentDungeonDifficultyItem.DungeonDescript.DungeonData.Bosses;
    }

    IEnumerator SpawnPlayer()
    {
        int retries = 20;
        WaitForSeconds wait = new WaitForSeconds(0.1f);
        while (_player == null && retries > 0)
        {
            retries--;
            _player = FindAnyObjectByType<TankPlayer>();
            yield return wait;
        }
        if (_player == null)
        {
            Debug.LogError("Player not found in Dungeon Special");
            yield break;
        }
        _player.transform.position = _currentDungeonSpecialMap.GetRandomPlayerSpawnpoint();
    }

    private void InitializePool()
    {
        _enemyPool = new ObjectPool<IEnemy>(
            createFunc: () => Instantiate(_bosses[UnityEngine.Random.Range(0, _bosses.Count)]),
            actionOnGet: (enemy) => enemy.GameObject.SetActive(true),
            actionOnRelease: (enemy) => enemy.GameObject.SetActive(false),
            actionOnDestroy: (enemy) => Destroy(enemy.GameObject),
            defaultCapacity: 5,
            maxSize: 10
        );
    }


    private void ConfigDungeonMap()
    {
        foreach (DungeonSpecialMap dungeonSpecialMap in DungeonSpecialMaps)
        {
            if (CurrentDungeonData != dungeonSpecialMap.DungeonData)
            {
                Destroy(dungeonSpecialMap);
                continue;
            }
            dungeonSpecialMap.gameObject.SetActive(true);
            if (CurrentDungeonData == dungeonSpecialMap.DungeonData) _currentDungeonSpecialMap = dungeonSpecialMap;
        }



    }

    public void SpawnEnemyRandomly(Enemy enemy)
    {
        if (_currentDungeonSpecialMap == null) return;
        enemy.Agent.Warp(_currentDungeonSpecialMap.GetRandomEnemySpawnpoint());
    }


    private void SpawnEnemy()
    {
        int amountBosses = GetAmountBossByType(_currentDungeonType);
        SpawnBossesByRandom(_bosses, amountBosses);
    }



    #region Spawn Enemy

    private void SpawnBossesByRandom(List<Enemy> enemies, int amountBosses)
    {
        _aliveBossCount = amountBosses;
        for (int i = 0; i < amountBosses; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, enemies.Count);
            Enemy enemy = (Enemy)_enemyPool.Get();
            enemy.SetPool(_enemyPool);

            enemy.OnDeath += (enemy) =>
            {
                --_aliveBossCount;
                if (_aliveBossCount <= 0)
                {
                    DungeonEventManager.Instance.dungeonEvents.DungeonFinished();
                }
            };

            enemy.EnemyHealthChanged += (currentHealth, maxHealth) =>
            {
                Debug.Log("Health was changed");
                DungeonSpecialUIManager.Instance.DungeonSpecialUIPanel.UpdateBossStateItemUI(_aliveBosses);
            };

            _aliveBosses.Add(enemy);
            SpawnEnemyRandomly(enemy);
        }
    }


    private int GetAmountBossByType(DungeonDescriptScriptableObject.DungeonType type)
    {
        switch (type)
        {
            case DungeonDescriptScriptableObject.DungeonType.Whispers:
                return 1;
            case DungeonDescriptScriptableObject.DungeonType.Coils:
                return 2;
            case DungeonDescriptScriptableObject.DungeonType.Tempest:
                return 3;
            case DungeonDescriptScriptableObject.DungeonType.Abyss:
                return 4;
        }
        return 1;
    }
    #endregion



}

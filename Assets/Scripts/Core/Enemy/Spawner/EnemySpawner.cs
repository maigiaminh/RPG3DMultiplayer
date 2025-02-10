using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public class EnemySpawner : MonoBehaviour
{

    [Header("Settings")]
    [SerializeField]
    private List<Enemy> _enemyPrefabs = new List<Enemy>();

    [SerializeField]
    private int _maxEnemies = 5;
    [SerializeField]
    private int _enemiesToNextSpawn = 5;
    [SerializeField]
    private int _spawnRate = 1;
    [SerializeField]
    private ScalingEnemyData _scalingEnemyData;
    [SerializeField]
    SpawnMethod _spawnMethod = SpawnMethod.RoundRobin;
    [SerializeField]
    private int _maxStage = 10;
    [SerializeField]
    Enemy.SpawnArea _spawnArea;
    [SerializeField]
    private bool _continuousSpawning = false;

    public ObjectPool<IEnemy> enemyPool;




    [Space]
    [Header("Read At Runtime")]
    [SerializeField] private int Level = 0;
    [SerializeField] private List<EnemyData> ScaledEnemies = new List<EnemyData>();






    // Spawn Weighted Data
    private List<WeightedSpawnData> _weightedSpawnEnemyData = new List<WeightedSpawnData>();
    private float[] _spawnWeights;


    private int _spawnedEnemy = 0;
    private int _aliveEnemies = 0;
    private NavMeshTriangulation _triangulation;
    private int _currentEnemyPrefabIndex = 0;
    private GameObject _parent;

    private void Awake()
    {
        _triangulation = NavMesh.CalculateTriangulation();
        Debug.Log("Triangulation Vertices Count: " + _triangulation.vertices.Length);

        _parent = new GameObject(gameObject.name + " Pool");
        ConfigEnemyPool();

        for (int i = 0; i < _enemyPrefabs.Count; i++)
        {
            ScaledEnemies.Add(_enemyPrefabs[i].enemyData.ScaleUpForLevel(_scalingEnemyData, Level));
            _weightedSpawnEnemyData.Add(_enemyPrefabs[i].weightedSpawnData);
        }
        _spawnWeights = new float[_weightedSpawnEnemyData.Count];
    }

    private void Start()
    {
        StartCoroutine(SpawnEnemies());
    }





    #region Spawning

    IEnumerator SpawnEnemies()
    {
        _spawnedEnemy = 0;
        _aliveEnemies = 0;

        for (int i = 0; i < _enemyPrefabs.Count; i++)
        {
            ScaledEnemies[i] = _enemyPrefabs[i].enemyData.ScaleUpForLevel(_scalingEnemyData, Level);
        }

        ResetSpawnWeight();


        WaitForSeconds rate = new WaitForSeconds(1 / _spawnRate);

        while (_spawnedEnemy < _maxEnemies)
        {
            IEnemy enemy = enemyPool.Get();
            Enemy enemyInstance = enemy as Enemy;
            NavMeshAgent agent = enemyInstance.Agent;

            enemyInstance.OnDeath += (e) => HandleEnemyDeath((Enemy)e);

            agent.Warp(GetSpawnPosition());

            enemy.GameObject.transform.SetParent(_parent.transform); // Set enemy to temporary object

            _spawnedEnemy++;

            yield return rate;
        }

        if (_continuousSpawning)
        {
            // ScaleUpSpawnRate();
            StartCoroutine(SpawnEnemies());
        }
    }



    private void ResetSpawnWeight()
    {
        float totalWeight = 0;
        for (int i = 0; i < _weightedSpawnEnemyData.Count; i++)
        {
            totalWeight += _weightedSpawnEnemyData[i].GetWeight();
        }
        for (int i = 0; i < _weightedSpawnEnemyData.Count; i++)
        {
            _spawnWeights[i] = _weightedSpawnEnemyData[i].GetWeight() / totalWeight;
        }
    }


    #endregion




    #region Utility Methods
    private Vector3 GetSpawnPosition()
    {
        int vertexIndex = UnityEngine.Random.Range(0, _triangulation.vertices.Length);
        int enemyArea = 1 << NavMesh.GetAreaFromName(_spawnArea.ToString());
        Debug.Log("Vertex Index: " + vertexIndex);
        NavMeshHit hit;
        if (!NavMesh.SamplePosition(_triangulation.vertices[vertexIndex], out hit, 1000, NavMesh.AllAreas))
        {
            vertexIndex = UnityEngine.Random.Range(0, _triangulation.vertices.Length);
        }
        return hit.position;
    }
    private void HandleEnemyDeath(Enemy enemy)
    {
        _aliveEnemies--;

        if (_aliveEnemies == _enemiesToNextSpawn)
        {
            StartCoroutine(SpawnEnemies());
        }
    }

    #endregion

    #region  Pooling Methods

    private void ConfigEnemyPool()
    {
        enemyPool = new ObjectPool<IEnemy>(
                CreateEnemy,
                OnTakeEnemyFromPool,
                OnReturnEnemyFromPool,
                OnDestroyEnemy,
                true,
                100,
                2000
            );
    }
    private void OnTakeEnemyFromPool(IEnemy enemy)
    {
        enemy.GameObject.SetActive(true);
    }

    private void OnReturnEnemyFromPool(IEnemy enemy)
    {
        enemy.GameObject.SetActive(false);
    }

    private void OnDestroyEnemy(IEnemy enemy)
    {
        Destroy(enemy.GameObject);
    }

    #endregion


    private IEnemy CreateEnemy()
    {
        IEnemy enemyToBeReturned = null;
        switch (_spawnMethod)
        {
            case SpawnMethod.RoundRobin:
                enemyToBeReturned = SpawnEnemyByRoundRobin();
                break;
            case SpawnMethod.Random:
                enemyToBeReturned = SpawnEnemyByRandom();
                break;
            case SpawnMethod.WeightedRandom:
                enemyToBeReturned = SpawnWeightRandomEnemy(UnityEngine.Random.value);
                break;
        }
        return enemyToBeReturned;
    }

    private IEnemy SpawnEnemyByRoundRobin()
    {
        Enemy enemyInstance;
        if (_enemyPrefabs.Count == 0)
        {
            Debug.LogError("No Enemy Prefabs Found");
            return null;
        }
        if (_enemyPrefabs.Count == 1)
        {
            enemyInstance = Instantiate(_enemyPrefabs[0]);
            enemyInstance.SetPool(enemyPool);
            return enemyInstance;
        }
        var enemyIndex = (_currentEnemyPrefabIndex++) % _enemyPrefabs.Count;
        enemyInstance = Instantiate(_enemyPrefabs[enemyIndex]);
        enemyInstance.SetPool(enemyPool);

        return enemyInstance;
    }

    private IEnemy SpawnEnemyByRandom()
    {
        Enemy enemyInstance;

        var enemyIndex = UnityEngine.Random.Range(0, _enemyPrefabs.Count);
        enemyInstance = Instantiate(_enemyPrefabs[enemyIndex]);
        enemyInstance.SetPool(enemyPool);

        return enemyInstance;
    }

    private IEnemy SpawnWeightRandomEnemy(float value)
    {
        for (int i = 0; i < _spawnWeights.Length; i++)
        {
            if (value <= _spawnWeights[i])
            {
                Enemy enemyInstance = Instantiate(_enemyPrefabs[i]);
                enemyInstance.SetPool(enemyPool);
                return enemyInstance;
            }
            value -= _spawnWeights[i];
        }
        Debug.Log("No Enemy Found");
        return null;
    }

}





public enum SpawnMethod
{
    RoundRobin,
    Random,
    WeightedRandom
}
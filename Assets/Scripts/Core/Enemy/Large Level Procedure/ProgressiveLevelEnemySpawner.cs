// using System;
// using System.Collections.Generic;
// using Unity.Mathematics;
// using UnityEngine;
// using UnityEngine.AI;
// using UnityEngine.Pool;

// public class ProgressiveLevelEnemySpawner : MonoBehaviour
// {
//     public Transform Player;
//     public float SpawnDensityPerTile = 0.5f;
//     public float tileCheckDelay = 0.5f;
//     public int MaxEnemies = 5;
//     public Vector3Int NavMeshSize = new Vector3Int(40, 10, 40);
//     public const int TileSize = 10;
//     public List<GameObject> _enemyPrefabs = new List<GameObject>();
//     public SpawnMethod EnemySpawnMethod = SpawnMethod.RoundRobin;
//     [SerializeField]
//     private AreaFloorBaker FloorBaker;
//     [SerializeField]
//     private LayerMask EnemyMask;
//     private int SpawnedEnemies = 0;

//     private ObjectPool<IEnemy> EnemyObjectPools;
//     private HashSet<Vector3> SpawnedTiles = new HashSet<Vector3>();
//     public HashSet<Enemy> AliveEnemies = new HashSet<Enemy>();
//     public Dictionary<Vector3, int> SpawnedTilesToEnemiesMap = new Dictionary<Vector3, int>();

//     private Collider[] EnemyColliders;


//     private void Awake()
//     {

//         EnemyObjectPools = new ObjectPool<IEnemy>(
//             CreateEnemy,
//             OnTakeEnemyFromPool,
//             OnReturnEnemyFromPool,
//             OnDestroyEnemy,
//             true,
//             100,
//             2000
//         );
//         FloorBaker.OnNavMeshUpdate += HandleNavMeshUpdate;
//     }

//     private void Start()
//     {
//         EnemyColliders = new Collider[MaxEnemies];
//     }



//     private void SpawnEnemyOnNewTiles(Vector3 currentTilePosition)
//     {
//         if (SpawnedEnemies >= MaxEnemies)
//         {
//             return;
//         }

//         for (int x = -1 * NavMeshSize.x / TileSize / 2; x < NavMeshSize.x / TileSize / 2; x++)
//         {
//             for (int z = -1 * NavMeshSize.z / TileSize / 2; z < NavMeshSize.z / TileSize / 2; z++)
//             {
//                 Vector3 tilePosition = new Vector3(currentTilePosition.x + x, currentTilePosition.y, currentTilePosition.z + z);
//                 int enemiesSpawnedForTile = 0;

//                 if (!SpawnedTiles.Contains(tilePosition))
//                 {
//                     SpawnedTiles.Add(tilePosition);
//                     SpawnedTilesToEnemiesMap.Add(tilePosition, enemiesSpawnedForTile);

//                     while (enemiesSpawnedForTile + UnityEngine.Random.value < SpawnDensityPerTile && SpawnedEnemies < MaxEnemies)
//                     {
//                         SpawnEnemyOnTile(tilePosition);
//                         enemiesSpawnedForTile++;
//                         SpawnedEnemies++;
//                     }
//                 }
//             }
//         }
//     }
//     private void SpawnEnemyOnTile(Vector3 tilePosition)
//     {
//         if (EnemySpawnMethod == SpawnMethod.RoundRobin)
//         {
//             SpawnEnemyByRoundRobin(SpawnedEnemies, tilePosition);
//         }
//         else if (EnemySpawnMethod == SpawnMethod.Random)
//         {
//             SpawnEnemyByRandom(tilePosition);
//         }
//     }

//     private void SpawnEnemyByRoundRobin(int enemyIndex, Vector3 tilePosition)
//     {
//         int spawnIndex = enemyIndex % _enemyPrefabs.Count;

//         // DoSpawnEnemy(spawnIndex, tilePosition);
//     }

//     private void SpawnEnemyByRandom(Vector3 tilePosition)
//     {
//         int spawnIndex = UnityEngine.Random.Range(0, _enemyPrefabs.Count);

//         // DoSpawnEnemy(spawnIndex, tilePosition);
//     }

//     private void DoSpawnEnemy(Vector3 tilePosition)
//     {
//         IEnemy enemy = EnemyObjectPools.Get();

//         if (enemy == null)
//         {
//             Debug.LogError("No Enemy Prefabs Found");
//             SpawnedEnemies--;
//             SpawnedTiles.Remove(tilePosition);
//             return;
//         }

//         NavMeshHit hit;
//         Vector3 SamplePosition = tilePosition * TileSize +
//                new Vector3(UnityEngine.Random.Range(-TileSize / 2, TileSize / 2), 0, UnityEngine.Random.Range(-TileSize / 2, TileSize / 2));
//         if (NavMesh.SamplePosition(SamplePosition, out hit, 10f, enemy.Agent.areaMask))
//         {
//             enemy.Agent.Warp(hit.position);
//             enemy.Agent.enabled = true;
//             AliveEnemies.Add(enemy as Enemy);
//             enemy.OnDeath += HandleEnemyDeath;
//         }
//         else
//         {
//             Debug.LogError("Could not find a valid position for enemy");
//             SpawnedEnemies--;
//             SpawnedTiles.Remove(tilePosition);
//             enemy.GameObject.SetActive(false);
//         }
//     }

//     private void HandleEnemyDeath(IEnemy enemy)
//     {
//         AliveEnemies.Remove(enemy as Enemy);
//     }


//     private void HandleNavMeshUpdate(Bounds bounds)
//     {
//         int Hits = Physics.OverlapBoxNonAlloc(bounds.center, bounds.extents, EnemyColliders, Quaternion.identity, EnemyMask.value);

//         IEnemy enemyComponent;

//         IEnemy[] enemyArray = new IEnemy[Hits];
//         for (int i = 0; i < Hits; i++)
//         {
//             if (EnemyColliders[i].gameObject.TryGetComponent<IEnemy>(out enemyComponent))
//             {
//                 enemyArray[i] = enemyComponent;
//                 enemyComponent.Agent.enabled = true;
//             }
//         }

//         HashSet<IEnemy> outOfBoundsEnemies = new HashSet<IEnemy>(AliveEnemies);

//         outOfBoundsEnemies.ExceptWith(enemyArray);

//         foreach (IEnemy enemy in enemyArray)
//         {
//             enemy.Agent.enabled = false;
//         }

//         Vector3 currentTilePosition = new Vector3(
//             Mathf.FloorToInt(Player.position.x / TileSize),
//             Mathf.FloorToInt(Player.position.y / TileSize),
//             Mathf.FloorToInt(Player.position.z / TileSize)
//         );

//         if (!SpawnedTiles.Contains(currentTilePosition))
//         {
//             SpawnedTiles.Add(currentTilePosition);
//         }

//         SpawnEnemyOnNewTiles(currentTilePosition);

//     }



//     #region Spawning

//     private IEnemy CreateEnemy()
//     {
//         IEnemy enemyToBeReturned = null;
//         switch (EnemySpawnMethod)
//         {
//             case SpawnMethod.RoundRobin:
//                 // enemyToBeReturned = SpawnEnemyByRoundRobin();
//                 break;
//             case SpawnMethod.Random:
//                 // enemyToBeReturned = SpawnEnemyByRandom();
//                 break;
//         }
//         return enemyToBeReturned;
//     }




//     private void OnTakeEnemyFromPool(IEnemy enemy)
//     {
//         enemy.GameObject.SetActive(true);
//     }

//     private void OnReturnEnemyFromPool(IEnemy enemy)
//     {
//         enemy.GameObject.SetActive(false);
//     }

//     private void OnDestroyEnemy(IEnemy enemy)
//     {
//         Destroy(enemy.GameObject);
//     }

//     #endregion
// }

using System.Collections.Generic;
using UnityEngine;

public class DungeonMap : MonoBehaviour
{
    public DungeonData DungeonData;
    public Transform enemySpawnpointParent;
    public Transform playerSpawnpointParent;

    protected List<Transform> _enemySpawnpoints = new List<Transform>();
    protected List<Transform> _playerSpawnpoints = new List<Transform>();
    protected void OnEnable()
    {
        if (enemySpawnpointParent != null)
        {
            foreach (Transform child in enemySpawnpointParent)
            {
                _enemySpawnpoints.Add(child);
            }
        }
        if (playerSpawnpointParent != null)
        {
            foreach (Transform child in playerSpawnpointParent)
            {
                _playerSpawnpoints.Add(child);
            }
        }

    }

    protected void OnDisable()
    {
        _enemySpawnpoints.Clear();
        _playerSpawnpoints.Clear();
    }

    public Vector3 GetRandomEnemySpawnpoint() => _enemySpawnpoints[Random.Range(0, _enemySpawnpoints.Count)].position;

    public Vector3 GetRandomPlayerSpawnpoint() => _playerSpawnpoints[Random.Range(0, _playerSpawnpoints.Count)].position;

}

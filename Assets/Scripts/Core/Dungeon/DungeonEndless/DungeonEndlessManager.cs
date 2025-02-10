using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonEndlessManager : MonoBehaviour
{
    public List<DungeonEndlessMap> _dungeonEndlessMaps = new List<DungeonEndlessMap>();
    private TankPlayer _player;
    private DungeonEndlessMap _currentDungeonEndlessMap;
    private DungeonData _currentDungeonData;
    private void Start()
    {
        Initialize();
        ConfigDungeonMap();
        StartCoroutine(SpawnPlayer());
    }
    private void Initialize()
    {
        _currentDungeonData = WorldMapUIManager.Instance.CurrentMap.dungeonData;
    }
    private void ConfigDungeonMap()
    {
        foreach (DungeonEndlessMap dungeonEndlessMap in _dungeonEndlessMaps)
        {
            if (dungeonEndlessMap.DungeonData != _currentDungeonData) Destroy(dungeonEndlessMap.gameObject);
            else _currentDungeonEndlessMap = dungeonEndlessMap;
        }
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
        _player.transform.position = _currentDungeonEndlessMap.GetRandomPlayerSpawnpoint();
    }





}

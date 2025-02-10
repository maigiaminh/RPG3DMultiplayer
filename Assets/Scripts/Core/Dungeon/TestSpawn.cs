using System.Collections.Generic;
using UnityEngine;

public class TestSpawn : MonoBehaviour
{
    // public GameObject player;

    // private void Start()
    // {
    //     Invoke("SetPlayerPos", 3f);
    // }

    // public void SetPlayerPos()
    // {
    //     Debug.Log("Setting player position");
    //     List<DungeonRoom> dungeonRooms = DungeonManager.Instance.GetDungeonRooms();
    //     Vector3 spawnpoint = dungeonRooms[UnityEngine.Random.Range(0, dungeonRooms.Count)].transform.position;

    //     RaycastHit hit;
    //     spawnpoint += UnityEngine.Random.insideUnitSphere * 5;
    //     while (Physics.Raycast(spawnpoint, Vector3.down, out hit, 100f))
    //     {
    //         spawnpoint = hit.point;
    //     }

    //     player.transform.position = spawnpoint;
    // }
}

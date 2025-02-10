using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkSpawnManager : NetworkBehaviour
{
    public static NetworkSpawnManager Instance;
    public List<Transform> spawnPoints = new List<Transform>();
    // public List<GameObject> spawnablePrefabs = new List<GameObject>();
    public NetworkList<ulong> SpawnableNetworkObjectIds = new NetworkList<ulong>();


    private bool IsServerSpawn = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one Network Spawn Manager in the scene.");
        }

        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log("IsHost: " + IsHost);
        Debug.Log("IsServer: " + IsServer);
        Debug.Log("IsClient: " + IsClient);
    }

    public override void OnNetworkDespawn()
    {
    }



    public Transform GetRandomSpawnpoint()
    {
        Debug.Log("GetRandomSpawnpoint");
        if (spawnPoints.Count == 0)
        {
            Debug.LogError("No spawn points available.");
            return null;
        }
        return spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)];
    }


    public NetworkObject GetNetworkObjectById(ulong networkObjectId)
    {
        NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out var networkObject);
        return networkObject;
    }

}

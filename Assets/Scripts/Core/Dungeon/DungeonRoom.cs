using Unity.AI.Navigation;
using UnityEngine;
using System.Collections.Generic;

public class DungeonRoom : MonoBehaviour
{
    [Header("NavMesh")]
    public NavMeshSurface navMeshSurface;
    [Header("Barriers")]
    public GameObject barrier;
    private int _roomToken;
    public int GetToken() => _roomToken;
    private bool _isEntered = false;
    private bool _isCleared = false;
    DungeonEventManager dungeonEventManager;
    private Collider _collider;
    public Transform PlayerSpawnpoint;

    public void Initialize(int tokens)
    {
        _roomToken = tokens;
    }
    private void Start()
    {
        _collider = GetComponent<Collider>();
        _collider.enabled = true;
        BakeNavMesh();
        dungeonEventManager = DungeonEventManager.Instance;
        // SpawnEnemy();
    }

    private void BakeNavMesh()
    {
        navMeshSurface = GetComponent<NavMeshSurface>();
        if (navMeshSurface == null)
        {
            Debug.LogError("NavMeshSurface is missing on the object!");
            return;
        }

        navMeshSurface.BuildNavMesh();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isEntered) return;
        if (!other.CompareTag("Player")) return;


        barrier.SetActive(true);
        dungeonEventManager.dungeonEvents.RoomEntered(this);
        dungeonEventManager.dungeonTokenEvents.AddToken(_roomToken);
        Debug.Log("Player Entered Room");

        _isEntered = true;
    }





}

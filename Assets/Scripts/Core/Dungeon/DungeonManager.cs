using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class DungeonManager : Singleton<DungeonManager>
{

  [Header("Dungeon Generation Settings")]
  #region Dungeon Generation Settings

  public int boardRows, boardColumns;
  public int minRoomSize, maxRoomSize;
  private List<DungeonRoom> _floorTiles = new List<DungeonRoom>();
  private List<GameObject> _wallTiles = new List<GameObject>();
  private List<GameObject> _ceilTiles = new List<GameObject>();
  private GameObject _corridorTile;
  private GameObject[,] boardPositionsFloor;
  private int _roomCount = 0;
  private List<DungeonRoom> _dungeonRooms = new List<DungeonRoom>();
  public List<DungeonRoom> GetDungeonRooms() => _dungeonRooms;
  #endregion


  private Enemy _boss;
  public int TotalTokens;
  public DungeonDescriptScriptableObject.DungeonType DungeonType;

  private Transform _playerTrans;

  public DungeonData CurrentDungeonData;

  #region Enemy Pooling

  [HideInInspector] public List<WeightedSpawnData> _weightedSpawnEnemyData = new List<WeightedSpawnData>();
  [HideInInspector] public float[] _spawnWeights;
  private ObjectPool<IEnemy> _enemyPool;

  // control the number of enemies in the room
  private List<Enemy> _currentEnemies = new List<Enemy>();
  private DungeonRoom _currentRoom;


  private bool _isFinalRoom = false;

  #endregion

  private GameObject _container;



  private void OnEnable()
  {
    SceneManager.sceneLoaded += OnSceneLoaded;
  }

  private void OnDisable()
  {
    SceneManager.sceneLoaded -= OnSceneLoaded;
  }

  public void SetDungeonData(DungeonData dungeonData)
  {
    CurrentDungeonData = dungeonData.GetNewDungeonData();
    minRoomSize = CurrentDungeonData.MinRoom;
    maxRoomSize = CurrentDungeonData.MaxRoom;
    _floorTiles = CurrentDungeonData.dungeonRooms;
    _wallTiles = CurrentDungeonData.wallTiles;
    _ceilTiles = CurrentDungeonData.ceilTiles;
    _corridorTile = CurrentDungeonData.corridorTile;
  }



  private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
  {
    if (!PlayerSpawnManager.Instance) return;
    if (!scene.name.Equals(PlayerSpawnManager.Instance.DUNGEON_SCENE_NAME)) return;
    EnterDungeon();
  }

  public void EnterDungeon()
  {
    ResetValue();
    SetDungeonData(DungeonDescriptManager.Instance.DungeonDescriptPanel.CurrentDungeonDifficultyItem.DungeonDescript.DungeonData);
    MakeTraditionalDungeon();
    SpawnPlayer();
  }




  #region Traditional Dungeon Handling

  public void MakeTraditionalDungeon()
  {
    DungeonType = DungeonDescriptManager.Instance.DungeonDescriptPanel.CurrentDungeonDifficultyItem.DungeonType;
    ConfigDungeon();
    _container = new GameObject("Dungeon");
    ConfigEnemyPool();
    SubDungeon rootSubDungeon = new SubDungeon(new Rect(0, 0, boardRows, boardColumns));
    CreateBSP(rootSubDungeon);
    rootSubDungeon.CreateRoom();
    boardPositionsFloor = new GameObject[boardRows, boardColumns];
    DrawRooms(rootSubDungeon);
    DrawCorridors(rootSubDungeon);
    Debug.Log("Room Count: " + _roomCount);
    Debug.Log("Dungeon Rooms Count: " + _dungeonRooms.Count);
    InitializeDungeonRoom();
    InitializeSpawnWeight();
    StartCoroutine(CheckEnemiesExistInRoom());


  }

  private void ConfigDungeon()
  {
    ConfigTotalTokens();
    DungeonEventManager.Instance.dungeonEvents.OnRoomEntered += OnRoomEntered;
    DungeonEventManager.Instance.dungeonEvents.OnDungeonFinished += HandleDungeonFinished;
    GameEventManager.Instance.PlayerEvents.OnPlayerDefeatEnemy += HandlePlayerDefeatBoss;
  }

  private void HandleDungeonFinished()
  {
    Debug.Log("Dungeon Finished");
    DungeonEventManager.Instance.dungeonEvents.OnRoomEntered -= OnRoomEntered;
    DungeonEventManager.Instance.dungeonEvents.OnDungeonFinished -= HandleDungeonFinished;
    GameEventManager.Instance.PlayerEvents.OnPlayerDefeatEnemy -= HandlePlayerDefeatBoss;
  }



  private void ConfigTotalTokens()
  {
    TotalTokens = CurrentDungeonData.GetTokenBaseOnDifficulty(DungeonType);

    Debug.Log("Total Tokens: " + TotalTokens);
  }


  public class SubDungeon
  {
    public SubDungeon left, right;
    public Rect rect;
    public Rect room = new(-1, -1, 0, 0);
    public int debugId;
    public List<Rect> corridors = new List<Rect>();

    private static int debugCounter = 0;

    public SubDungeon(Rect mrect)
    {
      rect = mrect;
      debugId = debugCounter;
      debugCounter++;
    }

    public bool IAmLeaf()
    {
      return left == null && right == null;
    }

    public bool Split(int minRoomSize, int maxRoomSize)
    {
      if (!IAmLeaf())
      {
        return false;
      }

      // choose a vertical or horizontal split depending on the proportions
      // i.e. if too wide split vertically, or too long horizontally, 
      // or if nearly square choose vertical or horizontal at random
      bool splitHorizontallly;
      if (rect.width / rect.height >= 1.25)
      {
        splitHorizontallly = false;
      }
      else if (rect.height / rect.width >= 1.25)
      {
        splitHorizontallly = true;
      }
      else
      {
        splitHorizontallly = Random.Range(0.0f, 1.0f) > 0.5;
      }

      if (Mathf.Min(rect.height, rect.width) / 2 < minRoomSize)
      {
        // Debug.Log("Sub-dungeon " + debugId + " will be a leaf");
        return false;
      }

      if (splitHorizontallly)
      {
        // split so that the resulting sub-dungeons widths are not too small
        // (since we are splitting horizontally) 
        int split = Random.Range(minRoomSize, (int)(rect.width - minRoomSize));

        left = new SubDungeon(new Rect(rect.x, rect.y, rect.width, split));
        right = new SubDungeon(
          new Rect(rect.x, rect.y + split, rect.width, rect.height - split));
      }
      else
      {
        int split = Random.Range(minRoomSize, (int)(rect.height - minRoomSize));

        left = new SubDungeon(new Rect(rect.x, rect.y, split, rect.height));
        right = new SubDungeon(
          new Rect(rect.x + split, rect.y, rect.width - split, rect.height));
      }

      return true;
    }

    public void CreateRoom()
    {
      left?.CreateRoom();
      right?.CreateRoom();
      if (left != null && right != null)
      {
        CreateCorridorBetween(left, right);
      }
      if (IAmLeaf())
      {
        int roomWidth = 4;
        int roomHeight = 4;
        int roomX = (roomWidth / 2) - 2;
        int roomY = (roomHeight / 2) - 2;

        // room position will be absolute in the board, not relative to the sub-dungeon
        room = new Rect(rect.x + roomX, rect.y + roomY, roomWidth, roomHeight);
        // Debug.Log("Created room " + room + " in sub-dungeon " + debugId + " " + rect);
      }
    }


    public void CreateCorridorBetween(SubDungeon left, SubDungeon right)
    {
      Rect lroom = left.GetRoom();
      Rect rroom = right.GetRoom();

      // Debug.Log("Creating corridor(s) between " + left.debugId + "(" + lroom + ") and " + right.debugId + " (" + rroom + ")");

      // attach the corridor to a random point in each room
      Vector2 lpoint = new Vector2((int)Random.Range(lroom.x + 1, lroom.xMax - 1), (int)Random.Range(lroom.y + 1, lroom.yMax - 1));
      Vector2 rpoint = new Vector2((int)Random.Range(rroom.x + 1, rroom.xMax - 1), (int)Random.Range(rroom.y + 1, rroom.yMax - 1));

      // always be sure that left point is on the left to simplify the code
      if (lpoint.x > rpoint.x)
      {
        Vector3 temp = lpoint;
        lpoint = rpoint;
        rpoint = temp;
      }

      int w = (int)(lpoint.x - rpoint.x);
      int h = (int)(lpoint.y - rpoint.y);

      // Debug.Log("lpoint: " + lpoint + ", rpoint: " + rpoint + ", w: " + w + ", h: " + h);

      // if the points are not aligned horizontally
      if (w != 0)
      {
        // choose at random to go horizontal then vertical or the opposite
        if (Random.Range(0, 1) > 2)
        {
          // add a corridor to the right
          corridors.Add(new Rect(lpoint.x, lpoint.y, Mathf.Abs(w) + 1, 1));

          // if left point is below right point go up
          // otherwise go down
          if (h < 0)
          {
            corridors.Add(new Rect(rpoint.x, lpoint.y, 1, Mathf.Abs(h)));
          }
          else
          {
            corridors.Add(new Rect(rpoint.x, lpoint.y, 1, -Mathf.Abs(h)));
          }
        }
        else
        {
          // go up or down
          if (h < 0)
          {
            corridors.Add(new Rect(lpoint.x, lpoint.y, 1, Mathf.Abs(h)));
          }
          else
          {
            corridors.Add(new Rect(lpoint.x, rpoint.y, 1, Mathf.Abs(h)));
          }

          // then go right
          corridors.Add(new Rect(lpoint.x, rpoint.y, Mathf.Abs(w) + 1, 1));
        }
      }
      else
      {
        // if the points are aligned horizontally
        // go up or down depending on the positions
        if (h < 0)
        {
          corridors.Add(new Rect((int)lpoint.x, (int)lpoint.y, 1, Mathf.Abs(h)));
        }
        else
        {
          corridors.Add(new Rect((int)rpoint.x, (int)rpoint.y, 1, Mathf.Abs(h)));
        }
      }

      // Debug.Log("Corridors: ");
      // foreach (Rect corridor in corridors)
      // {
      //   Debug.Log("corridor: " + corridor);
      // }
    }

    public Rect GetRoom()
    {
      if (IAmLeaf())
      {
        return room;
      }
      if (left != null)
      {
        Rect lroom = left.GetRoom();
        if (lroom.x != -1)
        {
          return lroom;
        }
      }
      if (right != null)
      {
        Rect rroom = right.GetRoom();
        if (rroom.x != -1)
        {
          return rroom;
        }
      }

      // workaround non nullable structs
      return new Rect(-1, -1, 0, 0);
    }
  }

  public void CreateBSP(SubDungeon subDungeon)
  {
    // Debug.Log("Splitting sub-dungeon " + subDungeon.debugId + ": " + subDungeon.rect);
    if (subDungeon.IAmLeaf())
    {
      // if the sub-dungeon is too large split it
      if (subDungeon.rect.width > maxRoomSize
        || subDungeon.rect.height > maxRoomSize
        || Random.Range(0.0f, 1.0f) > 0.25)
      {

        if (subDungeon.Split(minRoomSize, maxRoomSize))
        {
          CreateBSP(subDungeon.left);
          CreateBSP(subDungeon.right);
        }
      }
    }
  }

  public void DrawRooms(SubDungeon subDungeon)
  {
    if (subDungeon == null)
    {
      return;
    }
    if (subDungeon.IAmLeaf())
    {
      _roomCount++;
      GameObject instance = Instantiate(
        GetRandomGameObject(DungeonEnum.RandomTileType.Floor),
        new Vector3(
          (float)((subDungeon.room.xMax + subDungeon.room.x) / 2f * 5f),
          0f,
          (float)((subDungeon.room.yMax + subDungeon.room.y) / 2f * 5f)
        ),
        Quaternion.identity
      );
      // 
      if (instance.TryGetComponent(out DungeonRoom room)) _dungeonRooms.Add(room);
      else Debug.LogError("DungeonRoom component not found");
      //
      instance.transform.SetParent(_container.transform);

      instance = Instantiate(
        GetRandomGameObject(DungeonEnum.RandomTileType.Ceil),
        new Vector3(
          (float)((subDungeon.room.xMax + subDungeon.room.x) / 2f * 5f),
          10f,
          (float)((subDungeon.room.yMax + subDungeon.room.y) / 2f * 5f)
        ),
        Quaternion.identity
      );

      instance.transform.SetParent(_container.transform);

      for (int i = (int)subDungeon.room.x; i < subDungeon.room.xMax; i++)
      {
        for (int j = (int)subDungeon.room.y; j < subDungeon.room.yMax; j++)
        {
          if (i >= boardRows || j >= boardColumns) continue;
          boardPositionsFloor[i, j] = instance;
        }
      }

      for (int i = (int)subDungeon.room.x; i < subDungeon.room.xMax; i++)
      {
        //West Wall

        instance = Instantiate(
          GetRandomGameObject(DungeonEnum.RandomTileType.Wall),
          new Vector3(
            5f * i + 2.5f,
            0f,
            5f * subDungeon.room.y - 2.5f
          ),
          Quaternion.identity
        );
        instance.transform.SetParent(_container.transform);

        //East Wall

        instance = Instantiate(
          GetRandomGameObject(DungeonEnum.RandomTileType.Wall),
          new Vector3(
            5f * i + 2.5f,
            0f,
            5f * subDungeon.room.yMax - 2.5f
          ),
          Quaternion.identity
        );
        instance.transform.SetParent(_container.transform);
      }

      for (int i = (int)subDungeon.room.y; i < subDungeon.room.yMax; i++)
      {
        //North Wall

        instance = Instantiate(
          GetRandomGameObject(DungeonEnum.RandomTileType.Wall),
          new Vector3(
            5f * subDungeon.room.x - 2.5f,
            0f,
            5f * i - 2.5f
          ),
          Quaternion.identity * Quaternion.Euler(0, 90, 0)
        );
        instance.transform.SetParent(_container.transform);
        //South Wall

        instance = Instantiate(
          GetRandomGameObject(DungeonEnum.RandomTileType.Wall),
          new Vector3(
            5f * subDungeon.room.xMax - 2.5f,
            0f,
            5f * i - 2.5f
          ),
          Quaternion.identity * Quaternion.Euler(0, 90, 0)
        );
        instance.transform.SetParent(_container.transform);
      }
    }
    else
    {
      DrawRooms(subDungeon.left);
      DrawRooms(subDungeon.right);
    }
  }

  void DrawCorridors(SubDungeon subDungeon)
  {
    if (subDungeon == null)
    {
      return;
    }

    DrawCorridors(subDungeon.left);
    DrawCorridors(subDungeon.right);

    foreach (Rect corridor in subDungeon.corridors)
    {
      for (int i = (int)corridor.x; i < corridor.xMax; i++)
      {
        for (int j = (int)corridor.y; j < corridor.yMax; j++)
        {
          if (boardPositionsFloor[i, j] == null)
          {
            GameObject instance = Instantiate(_corridorTile, new Vector3(5f * i, 0f, 5f * j), Quaternion.identity);
            instance.transform.SetParent(_container.transform);

            instance = Instantiate(_corridorTile, new Vector3(5f * i, 10f, 5f * j), Quaternion.Euler(0, 180, 0));
            instance.transform.SetParent(_container.transform);
            boardPositionsFloor[i, j] = instance;

            GameObject wallInstance1, wallInstance2;
            if (corridor.yMax - corridor.y <= 1f)
            {
              DestroyAtPosition(new Vector3(5f * i, 5f, 5f * j));
              wallInstance1 = Instantiate(GetRandomGameObject(DungeonEnum.RandomTileType.Wall), new Vector3(5f * i + 2.5f, 0f, 5f * j + 2.5f), Quaternion.identity);
              wallInstance2 = Instantiate(GetRandomGameObject(DungeonEnum.RandomTileType.Wall), new Vector3(5f * i + 2.5f, 0f, 5f * j - 2.5f), Quaternion.identity);
            }
            else
            {
              DestroyAtPosition(new Vector3(5f * i, 5f, 5f * j));
              wallInstance1 = Instantiate(GetRandomGameObject(DungeonEnum.RandomTileType.Wall), new Vector3(5f * i + 2.5f, 0f, 5f * j - 2.5f),
                Quaternion.identity * Quaternion.Euler(new Vector3(0, 90, 0))
              );
              wallInstance2 = Instantiate(GetRandomGameObject(DungeonEnum.RandomTileType.Wall), new Vector3(5f * i - 2.5f, 0f, 5f * j - 2.5f),
                Quaternion.identity * Quaternion.Euler(new Vector3(0, 90, 0))
              );

            }
            wallInstance1.transform.SetParent(_container.transform);
            wallInstance2.transform.SetParent(_container.transform);

          }

        }
      }
    }
  }

  private void DestroyAtPosition(Vector3 location)
  {
    float radius = 3f;

    Collider[] hitColliders = Physics.OverlapSphere(location, radius);
    int i = 0;

    while (i < hitColliders.Length)
    {
      Destroy(hitColliders[i].gameObject);
      i++;
    }
  }

  private GameObject GetRandomGameObject(DungeonEnum.RandomTileType roomType)
  {
    if (_floorTiles == null || _floorTiles.Count == 0)
    {
      Debug.LogError("floorTiles is null or empty!");
    }
    foreach (var tile in _floorTiles)
    {
      if (tile == null)
      {
        Debug.LogError("A tile in floorTiles is null!");
        return null; // Hoặc xử lý lỗi phù hợp
      }
    }
    return roomType switch
    {
      DungeonEnum.RandomTileType.Wall => _wallTiles[Random.Range(0, _wallTiles.Count)],
      DungeonEnum.RandomTileType.Floor => _floorTiles[Random.Range(0, _floorTiles.Count)].gameObject,
      _ => _ceilTiles[Random.Range(0, _ceilTiles.Count)],
    };
  }



  private void InitializeSpawnWeight()
  {
    // Kiểm tra nếu danh sách enemies không rỗng
    if (CurrentDungeonData.Enemies == null || CurrentDungeonData.Enemies.Count == 0)
    {
      Debug.LogError("Enemies list is empty or null.");
      return;
    }

    for (int i = 0; i < CurrentDungeonData.Enemies.Count; i++)
    {
      // Kiểm tra nếu dữ liệu WeightedSpawnData có sẵn
      if (CurrentDungeonData.Enemies[i].weightedSpawnData != null)
      {
        _weightedSpawnEnemyData.Add(CurrentDungeonData.Enemies[i].weightedSpawnData);
      }
      else
      {
        Debug.LogWarning("Weighted spawn _currentDungeonData is missing for enemy: " + CurrentDungeonData.Enemies[i].name);
      }
    }

    // Khởi tạo _spawnWeights
    _spawnWeights = new float[_weightedSpawnEnemyData.Count];

    float totalWeight = 0;
    // Tính tổng trọng số
    for (int i = 0; i < _weightedSpawnEnemyData.Count; i++)
    {
      _spawnWeights[i] = _weightedSpawnEnemyData[i].GetWeight();
      totalWeight += _spawnWeights[i];
    }

    // Đảm bảo tổng trọng số > 0 để tránh chia cho 0 sau này
    if (totalWeight == 0)
    {
      Debug.LogError("Total weight of enemies is zero.");
      return;
    }

    // Tính tỷ lệ trọng số cho mỗi enemy
    for (int i = 0; i < _spawnWeights.Length; i++)
    {
      _spawnWeights[i] /= totalWeight;
    }
  }

  private void InitializeDungeonRoom()
  {
    Debug.Log("Room Count: " + _roomCount);
    Debug.Log("Dungeon Rooms Count: " + _dungeonRooms.Count);

    // Kiểm tra nếu _dungeonRooms có phòng không
    if (_dungeonRooms == null || _dungeonRooms.Count == 0)
    {
      Debug.LogError("Dungeon rooms list is empty or null.");
      return;
    }

    foreach (var room in _dungeonRooms)
    {
      // Nếu roomCount bằng 0, tránh chia cho 0
      int tokens = _roomCount > 0 ? TotalTokens / _roomCount : TotalTokens;
      room.Initialize(tokens);
    }
  }
  #endregion


  #region Event Dungeon Handling




  #endregion

  #region Pooling Methods

  private void ConfigEnemyPool()
  {
    // Đảm bảo enemyPool được khởi tạo đúng cách
    if (_enemyPool == null)
    {
      _enemyPool = new ObjectPool<IEnemy>(
          CreateEnemy,
          OnTakeEnemyFromPool,
          OnReturnEnemyFromPool,
          OnDestroyEnemy,
          true,
          100,
          2000
      );
    }
  }

  private IEnemy CreateEnemy()
  {
    // Kiểm tra trước khi gọi SpawnWeightRandomEnemy để tránh lỗi null
    IEnemy enemyToBeReturned = SpawnWeightRandomEnemy(UnityEngine.Random.value);
    if (enemyToBeReturned == null)
    {
      Debug.LogError("Failed to create enemy.");
    }
    return enemyToBeReturned;
  }

  private IEnemy SpawnWeightRandomEnemy(float value)
  {
    // Kiểm tra nếu _spawnWeights có dữ liệu hợp lệ
    if (_spawnWeights == null || _spawnWeights.Length == 0)
    {
      Debug.LogError("Spawn weights are not initialized or empty.");
      return null;
    }

    for (int i = 0; i < _spawnWeights.Length; i++)
    {
      if (value <= _spawnWeights[i])
      {
        if (CurrentDungeonData.Enemies != null && CurrentDungeonData.Enemies.Count > i && CurrentDungeonData.Enemies[i] != null)
        {
          // Đảm bảo enemy có thể được Instantiate
          Enemy enemyInstance = Instantiate(CurrentDungeonData.Enemies[i]);
          if (enemyInstance != null)
          {
            enemyInstance.SetPool(_enemyPool);
            return enemyInstance;
          }
          else
          {
            Debug.Log("Failed to instantiate enemy at index: " + i);
          }
        }
        else
        {
          Debug.LogError("Enemy _currentDungeonData is null or out of bounds for index: " + i);
        }
      }
      value -= _spawnWeights[i];
    }

    Debug.Log("No Enemy Found");
    return null;
  }

  // private void ResetSpawnWeight()
  // {
  //     float totalWeight = 0;
  //     for (int i = 0; i < _weightedSpawnEnemyData.Count; i++)
  //     {
  //         totalWeight += _weightedSpawnEnemyData[i].GetWeight();
  //     }
  //     for (int i = 0; i < _weightedSpawnEnemyData.Count; i++)
  //     {
  //         _spawnWeights[i] = _weightedSpawnEnemyData[i].GetWeight() / totalWeight;
  //     }
  // }




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


  #region SpawnEnemy


  private void OnRoomEntered(DungeonRoom room)
  {
    if (_isFinalRoom) return;
    _currentEnemies = new List<Enemy>();
    int roomToken = room.GetToken();
    _currentRoom = room;
    Transform roomTrans = room.transform;

    Debug.Log("CurrentToken: " + TokenManager.Instance.CurrentTokens);
    Debug.Log("TotalToken: " + TotalTokens);
    float tokenPercentage = (float)TokenManager.Instance.CurrentTokens / TotalTokens * 100;

    Debug.Log("Token Percentage: " + tokenPercentage);

    if (tokenPercentage >= 50) SpawnBossEnemy(roomTrans);
    else if (tokenPercentage >= 40)
    {
      Debug.Log("80% tokens reached - 50% chance to spawn boss.");
      if (Random.value <= 0.5f) SpawnBossEnemy(roomTrans);
      else SpawnNormalEnemy(roomTrans, roomToken);
    }
    else SpawnNormalEnemy(roomTrans, roomToken);

  }

  private void SpawnBossEnemy(Transform roomTrans)
  {
    Debug.Log("Spawning Boss");
    _isFinalRoom = true;
    Enemy bossType = CurrentDungeonData.Bosses[Random.Range(0, CurrentDungeonData.Bosses.Count)];
    Instantiate(bossType.gameObject, GetRandomEnemyPosition(roomTrans), Quaternion.identity)
        .TryGetComponent(out Enemy bossInstance);
    _boss = bossInstance;



    _currentEnemies.Add(bossInstance);
    bossInstance.SetPool(_enemyPool);
    bossInstance.Agent.Warp(GetRandomEnemyPosition(roomTrans));
    // data.Bosses[Random.Range(0, data.Bosses.Count)].Agent.Warp(GetRandomEnemyPosition(roomTrans));
  }

  private void SpawnNormalEnemy(Transform roomTrans, int roomToken)
  {
    while (roomToken > 0)
    {
      Debug.Log("Spawning Enemy. Remaining Tokens: " + roomToken);
      IEnemy enemy = _enemyPool.Get(); // Lấy một kẻ thù từ pool
      _currentEnemies.Add(enemy as Enemy);
      Debug.Log("Enemy Spawned. Remaining Tokens: " + roomToken);
      if (enemy != null)
      {
        Enemy enemyInstance = enemy as Enemy;
        // Nếu enemy tồn tại, trừ đi số token tương ứng với enemy đã spawn
        roomToken -= enemyInstance.enemyData.Token; // Giả sử IEnemy có phương thức GetTokenCost để lấy số token của kẻ thù
        Debug.Log("Enemy Spawned. Remaining Tokens: " + roomToken);
        // Làm cho enemy active và thêm vào phòng
        enemyInstance.Agent.Warp(GetRandomEnemyPosition(roomTrans));
      }
      else
      {
        Debug.Log("No valid enemy to spawn.");
      }
    }
  }


  private Vector3 GetRandomEnemyPosition(Transform roomTrans)
  {
    // Định nghĩa phạm vi tìm kiếm, có thể thay đổi giá trị này cho phù hợp
    float searchRadius = 10f; // Khoảng cách tìm kiếm trên NavMesh
    Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * searchRadius;

    // Tạo một vị trí ngẫu nhiên quanh điểm hiện tại
    randomDirection += roomTrans.position;

    Debug.Log("Random Direction: " + randomDirection);
    NavMeshHit hit;
    while (NavMesh.SamplePosition(randomDirection, out hit, searchRadius, NavMesh.AllAreas))
    {
      return hit.position;
    }

    // Nếu không tìm thấy vị trí hợp lệ, có thể trả về vị trí gốc hoặc một giá trị khác
    return roomTrans.position;
  }

  #endregion


  #region Other Methods

  IEnumerator CheckEnemiesExistInRoom()
  {
    while (true)
    {
      yield return new WaitForSeconds(5f);
      if (_isFinalRoom) yield break;
      if (_currentRoom == null) continue;
      foreach (var enemy in _currentEnemies)
      {
        if (enemy.gameObject.activeSelf == false)
        {
          _currentEnemies.Remove(enemy);
          break;
        }
      }
      if (_currentEnemies.Count == 0)
      {
        Debug.Log("Room Cleared");
        _currentRoom.barrier.SetActive(false);
        _currentRoom = null;
      }
    }
  }


  private void SpawnPlayer()
  {
    Debug.Log("Spawning Player");
    DungeonRoom playerRoom = _dungeonRooms[Random.Range(0, _dungeonRooms.Count)];
    _playerTrans = PlayerSpawnManager.Instance.GetPlayerTrans();
    _playerTrans.position = playerRoom.PlayerSpawnpoint.position;
  }




  private void HandlePlayerDefeatBoss(IEnemy enemy, int amount)
  {
    Enemy enemyInstance = enemy as Enemy;
    if (enemyInstance == null) return;
    if (enemyInstance.enemyData.DifficultyType != DifficultyType.Boss) return;

    Debug.Log("Boss Defeated");
    DungeonEventManager.Instance.dungeonEvents.DungeonFinished();
    SoundManager.PlaySound(SoundType.WIN_MUSIC);
  }

  #endregion


  private void ResetValue()
  {
    CurrentDungeonData = null;
    _weightedSpawnEnemyData.Clear();
    _spawnWeights = null;
    _enemyPool = null;
    _roomCount = 0;
    _dungeonRooms.Clear();
    _isFinalRoom = false;
    _floorTiles.Clear();
    _wallTiles.Clear();
    _ceilTiles.Clear();
    _corridorTile = null;
    _currentEnemies.Clear();
    _currentRoom = null;
    _boss = null;
    _container = null;
  }


}

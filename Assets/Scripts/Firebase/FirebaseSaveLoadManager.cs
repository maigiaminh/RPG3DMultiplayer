using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirebaseSaveLoadManager : Singleton<FirebaseSaveLoadManager>
{

    private const float SAVEINVENTORY_COOLDOWN = 20f; // Thời gian chờ giữa các lần lưu
    private float _timeInventorySinceLastSave = 0f;
    private const float SAVEUSERDATA_COOLDOWN = 5f; // Thời gian chờ giữa các lần lưu
    private float _timeUserDataSinceLastSave = 0f;
    private const float RANKINGBOARDLOAD_COOLDOWN = 30;
    private float _timeRankingBoardSinceLastLoad = 0;

    #region Events
    public event Action OnRankingBoardUpdated;
    public void RankingBoardUpdated()
    {
        OnRankingBoardUpdated?.Invoke();
    }
    #endregion

    private bool _isSave = false;


    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;

    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!scene.name.Equals("MAINGAMESCENE")) return;
        _isSave = true;
    }

    private void Start()
    {
        if (FirebaseAuthManager.Instance == null)
        {
            Debug.LogError("FirebaseAuthManager is null in FirebaseSaveLoadManager");
            return;
        }

        FirebaseAuthManager.Instance.LoadRankingBoardData();
        FirebaseAuthManager.Instance.SaveUserData();
        SkillManager.Instance.SaveSkillLevel();
        RankingBoardUpdated();
    }

    private void Update()
    {
        if (!_isSave) return;
        if (SceneManager.GetActiveScene().name == "TradingArea") return;
        if (FirebaseAuthManager.Instance == null) return;


        _timeUserDataSinceLastSave += Time.deltaTime;
        _timeRankingBoardSinceLastLoad += Time.deltaTime;
        _timeInventorySinceLastSave += Time.deltaTime;



        if (_timeInventorySinceLastSave >= SAVEINVENTORY_COOLDOWN)
        {
            _timeInventorySinceLastSave = 0;
            InventoryContainer.Instance.UpdateItemFirebase(InventoryManager.Instance.GetAllInventoryItems);
        }

        if (_timeUserDataSinceLastSave >= SAVEUSERDATA_COOLDOWN)
        {
            FirebaseAuthManager.Instance.SaveUserData();

            SkillManager.Instance.SaveSkillLevel();

            _timeUserDataSinceLastSave = 0;
        }
        if (_timeRankingBoardSinceLastLoad >= RANKINGBOARDLOAD_COOLDOWN)
        {
            FirebaseAuthManager.Instance.LoadRankingBoardData();

            RankingBoardUpdated();
            _timeRankingBoardSinceLastLoad = 0;
        }
    }
}

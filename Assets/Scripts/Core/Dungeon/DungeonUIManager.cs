using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DungeonUIManager : Singleton<DungeonUIManager>
{
    public RewardBoard RewardBoard;
    public Button BackToMainSceneBtn;
    public GameObject BackToMainSceneContainer;

    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        Debug.Log("DungeonUIManager OnEnable");
        SceneManager.sceneLoaded += OnSceneLoaded;
        DungeonEventManager.Instance.dungeonEvents.OnDungeonFinished += HandleDungeonFinished;

    }


    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        DungeonEventManager.Instance.dungeonEvents.OnDungeonFinished -= HandleDungeonFinished;
    }

    private void HandleDungeonFinished()
    {
        ActivateDungeonRewardPanel();
        SetRewardImages();
    }




    public void ActivateDungeonRewardPanel()
    {
        RewardBoard.gameObject.SetActive(true);
    }

    public void DeactivateDungeonRewardPanel()
    {
        RewardBoard.gameObject.SetActive(false);
    }


    public void SetRewardImages()
    {
        List<ItemData> items = ChooseItemData();
        int randomReward = UnityEngine.Random.Range(1, 4);
        List<ItemData> itemEarns = new List<ItemData>();
        for (int i = 0; i < randomReward; i++)
        {
            itemEarns.Add(items[UnityEngine.Random.Range(0, items.Count)]);
        }
        RewardBoard.SetRewardItems(itemEarns);
    }

    private List<ItemData> ChooseItemData()
    {
        var mode = DungeonDescriptManager.Instance.CurrentDungeonDescriptItem.dungeonDescript.DungeonData.DungeonMode;
        switch (mode)
        {
            case DungeonMode.Traditional:
                return DungeonManager.Instance.CurrentDungeonData.GetItemDatas(DungeonManager.Instance.DungeonType);
            case DungeonMode.Special:
                return DungeonSpecialManager.Instance.CurrentDungeonData.GetItemDatas(DungeonSpecialManager.Instance.CurrentDungeonType);
        }
        Debug.LogError("Dungeon Mode is not found");
        return null;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
        BackToMainSceneContainer.gameObject.SetActive(scene.name == PlayerSpawnManager.Instance.DUNGEON_SCENE_NAME ||
                                                scene.name == PlayerSpawnManager.Instance.DUNGEON_SPECIAL_SCENE_NAME ||
                                                scene.name == PlayerSpawnManager.Instance.DUNGEON_ENDLESS_SCENE_NAME);
    }

    public void BackToMainScene()
    {
        SceneManager.LoadScene(PlayerSpawnManager.Instance.GAMESCENE);
    }

}

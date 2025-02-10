using System;
using System.Collections.Generic;
using UnityEngine;

public class RankingManager : Singleton<RankingManager>
{
    [Header("Config")]
    public int MaxRankingLevelPlayers = 10;

    private List<RankingLevelData> _rankingLevelDatas = new List<RankingLevelData>();
    private Dictionary<string, Sprite> _avatarSprites = new Dictionary<string, Sprite>();
    public Transform rankingLevelItemsParent;
    public RankingLevelItem rankingLevelItemPrefab;


    protected override void Awake(){
        base.Awake();
        ConfigAvatarSprites();
    }


    private void Start()
    {
        InitializeRankingLevelItems();
        ConfigRankingLevelItems();
    }

    private void OnEnable()
    {
        if (FirebaseSaveLoadManager.Instance == null)
        {
            Debug.LogError("FirebaseSaveLoadManager is null in RankingManager");
            return;
        }

        FirebaseSaveLoadManager.Instance.OnRankingBoardUpdated += UpdateRankingBoard;

    }

    private void OnDisable()
    {
        if (FirebaseSaveLoadManager.Instance == null)
        {
            Debug.LogError("FirebaseSaveLoadManager is null in RankingManager");

            return;
        }

        FirebaseSaveLoadManager.Instance.OnRankingBoardUpdated -= UpdateRankingBoard;
    }



    private void Update()
    {
        // _counter += Time.deltaTime;
        // if(_counter < _timeRankingBoardUpdate) return;

        // _counter = 0;


        // _rankingLevelItems = 

    }


    private void UpdateRankingBoard()
    {
        ConfigRankingLevelItems();
    }

    private void ConfigAvatarSprites()
    {
        _avatarSprites = new Dictionary<string, Sprite>();

        Sprite[] sprites = Resources.LoadAll<Sprite>("Avatar");

        foreach (Sprite sprite in sprites)
        {
            _avatarSprites.Add(sprite.name, sprite);
        }
    }


    private void InitializeRankingLevelItems()
    {
        if (rankingLevelItemsParent == null)
        {
            Debug.LogError("rankingLevelItemsParent is null in RankingManager");
            return;
        }
        for (int i = 0; i < MaxRankingLevelPlayers; i++)
        {
            RankingLevelItem rankingLevelItem = Instantiate(rankingLevelItemPrefab);
            rankingLevelItem.transform.SetParent(rankingLevelItemsParent);
            rankingLevelItem.transform.localScale = Vector3.one;
        }
    }

    private void ConfigRankingLevelItems()
    {

        if (RankingBoardContainer.Instance == null)
        {
            Debug.LogError("RankingBoardContainer is null in RankingManager");
            return;
        }


        _rankingLevelDatas = RankingBoardContainer.Instance.RankingLevelItems;

        for(int i = 0; i < _rankingLevelDatas.Count && i < MaxRankingLevelPlayers; i++)
        {
            RankingLevelItem rankingLevelItem = rankingLevelItemsParent.GetChild(i).GetComponent<RankingLevelItem>();

            Sprite itemSprite = _avatarSprites[_rankingLevelDatas[i].AvatarName];
            string itemName = _rankingLevelDatas[i].Name;
            int itemLevel = _rankingLevelDatas[i].Level;
            
            rankingLevelItem.SetAppearance(itemSprite, itemName, itemLevel, i + 1);
        }
    }



}
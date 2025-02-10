using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class QuestManager : Singleton<QuestManager>
{
    // Start a quest by its unique id
    // Instantiate the first quest step
    [SerializeField] private bool loadQuestState = true;

    private Dictionary<string, Quest> _questMap;
    // private Dictionary<string, Quest> CreateQuestMap()
    // {
    //     QuestInfo[] allQuests = Resources.LoadAll<QuestInfo>("Quests");
    //     Dictionary<string, Quest> questMap = new Dictionary<string, Quest>(); 
    //     foreach (var questInfo in allQuests)
    //     {
    //         if (questMap.ContainsKey(questInfo.id))
    //         {
    //             Debug.LogWarning("Duplicate quest id found: " + questInfo.id);
    //         }
    //         Quest quest = LoadQuest(questInfo);
    //         questMap.Add(questInfo.id, quest);
    //     }

    //     if(QuestUIManager.Instance != null){
    //         Debug.Log("QuestUIManager found");
    //         Debug.Log("Quests loaded: " + questMap.Count);
    //         QuestUIManager.Instance.InitializeAllQuestUI(questMap);
    //     }
    //     return questMap;
    // }

    private Dictionary<string, Quest> CreateQuestMap()
    {
        _questMap = QuestContainer.Instance.QuestMap;
        if (_questMap != null)
        {
            return _questMap;
        }
        _questMap = new Dictionary<string, Quest>();
        QuestInfo[] allQuests = Resources.LoadAll<QuestInfo>("Quests");
        Debug.Log("Loading quests: " + allQuests.Length);
        foreach (var questInfo in allQuests)
        {
            if (_questMap.ContainsKey(questInfo.id))
            {
                Debug.LogWarning("Duplicate quest id found: " + questInfo.id);
            }
            Quest quest = LoadQuest(questInfo);
            _questMap.Add(questInfo.id, quest);
        }
        QuestContainer.Instance.QuestMap = _questMap;


        return _questMap;
    }

    public Dictionary<string, Quest> GetQuestMap()
    {
        return _questMap;
    }
    private int _currentPlayerLevel = 0;


    protected override void Awake()
    {
        base.Awake();
        CreateQuestMap();
    }

    private void OnEnable()
    {
        GameEventManager.Instance.QuestEvents.onStartQuest += StartQuest;
        GameEventManager.Instance.QuestEvents.onAdvanceQuest += AdvanceQuest;
        GameEventManager.Instance.QuestEvents.onFinishQuest += FinishQuest;

        GameEventManager.Instance.QuestEvents.onQuestStepStateChange += QuestStepStateChange;

        GameEventManager.Instance.PlayerEvents.OnPlayerLevelChange += PlayerLevelChange;
    }



    private void OnDisable()
    {
        GameEventManager.Instance.QuestEvents.onStartQuest -= StartQuest;
        GameEventManager.Instance.QuestEvents.onAdvanceQuest -= AdvanceQuest;
        GameEventManager.Instance.QuestEvents.onFinishQuest -= FinishQuest;

        GameEventManager.Instance.QuestEvents.onQuestStepStateChange -= QuestStepStateChange;

        GameEventManager.Instance.PlayerEvents.OnPlayerLevelChange -= PlayerLevelChange;

    }

    private void Start()
    {
        QuestUIManager.Instance.InitializeQuest(_questMap);
        InitializeQuestStep();
    }

    private void InitializeQuestStep()
    {
        Debug.Log("Quests loaded: " + _questMap.Count);
        foreach (Quest quest in _questMap.Values)
        {
            Debug.Log("Quest id: " + quest.questInfo.id);
            if (quest.state == QuestState.IN_PROGRESS || quest.state == QuestState.CAN_FINISH)
            {
                Debug.Log("Quest in progress: " + quest.questInfo.id);
                quest.InstantiateCurrentQuestStep(this.transform);
            }
        }
    }

    private bool CheckRequirementMet(Quest quest)
    {
        bool requirementMet = true;

        if (quest.questInfo.playerLevelRequirement > _currentPlayerLevel)
        {
            requirementMet = false;
        }

        foreach (QuestInfo info in quest.questInfo.questPrerequisites)
        {
            if (GetQuestById(info.id).state != QuestState.FINISHED)
            {
                requirementMet = false;
                break;
            }
        }
        return requirementMet;
    }

    private void Update()
    {
        foreach (Quest quest in _questMap.Values)
        {
            if (quest.state == QuestState.REQUIREMENTS_NOT_MET && CheckRequirementMet(quest))
            {
                ChangeQuestState(quest.questInfo.id, QuestState.CAN_START);
            }
        }
    }


    private void StartQuest(string id)
    {
        Quest quest = GetQuestById(id);
        quest.InstantiateCurrentQuestStep(transform);
        ChangeQuestState(id, QuestState.IN_PROGRESS);

        QuestUIManager.Instance.InitializeQuestUI(quest, QuestState.IN_PROGRESS);
    }
    private void AdvanceQuest(string id)
    {
        Quest quest = GetQuestById(id);

        quest.MoveToNextStep();

        if (quest.CurrentStepExist())
        {
            quest.InstantiateCurrentQuestStep(this.transform);
        }
        else
        {
            ChangeQuestState(id, QuestState.CAN_FINISH);
            QuestUIManager.Instance.InitializeQuestUI(quest, QuestState.CAN_FINISH);
        }
    }
    private void FinishQuest(string id)
    {
        Quest quest = GetQuestById(id);
        ClaimRewards(quest);
        ChangeQuestState(id, QuestState.FINISHED);

        SoundManager.PlaySound(SoundType.QUEST_DONE);
        QuestUIManager.Instance.InitializeQuestUI(quest, QuestState.FINISHED);
    }

    private void PlayerLevelChange(int level)
    {
        _currentPlayerLevel = level;
    }


    private void ClaimRewards(Quest quest)
    {
        GameEventManager.Instance.PlayerEvents.PlayerGainExperience(quest.questInfo.experienceReward);
        GameEventManager.Instance.ResourceEvents.PlayerGainGold(quest.questInfo.goldReward);
    }


    private void ChangeQuestState(string id, QuestState state)
    {
        Quest quest = GetQuestById(id);
        quest.state = state;
        GameEventManager.Instance.QuestEvents.QuestStateChange(quest);
    }

    private void QuestStepStateChange(string id, int index, QuestStepState questStepState)
    {
        Quest quest = GetQuestById(id);
        quest.StoreQuestStepState(questStepState, index);
        ChangeQuestState(id, quest.state);
    }



    private Quest GetQuestById(string id)
    {
        Quest quest = _questMap[id];
        if (quest == null)
        {
            Debug.LogWarning("Quest with id " + id + " not found");
        }
        return quest;
    }

    private void OnApplicationQuit()
    {
        foreach (Quest quest in _questMap.Values)
        {
            SaveQuest(quest);
        }
    }

    private void SaveQuest(Quest quest)
    {
        try
        {
            QuestData questData = quest.GetQuestData();
            string serializedData = JsonUtility.ToJson(questData);
            FirebaseAuthManager.Instance.SaveQuest(quest);
            PlayerPrefs.SetString(quest.questInfo.id, serializedData);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save quest data: " + e.Message);
        }
    }

    private Quest LoadQuest(QuestInfo questInfo)
    {
        Quest quest = null;

        try
        {
            if (PlayerPrefs.HasKey(questInfo.id) && loadQuestState)
            {
                string serializedData = PlayerPrefs.GetString(questInfo.id);
                QuestData questData = JsonUtility.FromJson<QuestData>(serializedData);
                quest = new Quest(questInfo, questData.questState, questData.currentQuestStepIndex, questData.questStepStates);

            }
            else
            {
                quest = new Quest(questInfo);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load quest data: " + e.Message);
        }

        return quest;
    }



}

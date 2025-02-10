using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

public class QuestUIManager : Singleton<QuestUIManager>
{
    // Quest State Panel
    [Header("Quest State Panel")]
    public GameObject questStatePanel;
    public TextMeshProUGUI questStateDescriptText;
    public TextMeshProUGUI questStateTitleText;
    public float timeStatePanelAppear = 5f;


    // Quest Appearance Panel On Screen
    [Header("Quest Appearance Panel On Screen")]
    [SerializeField] private int _maxQuestAppearance = 3;
    [SerializeField] private Transform _questApperanceItemParent;
    [SerializeField] private QuestAppearanceItem _questAppearanceItemPrefab;

    private ObjectPool<QuestAppearanceItem> _questAppearanceItemPool;
    private Dictionary<string, QuestAppearanceItem> _questAppearanceItems = new Dictionary<string, QuestAppearanceItem>();


    // Quest State Panel

    private bool _isQuestStatePanelActive = false;
    private string questStatePanelDescript;
    private string _questId;
    private Coroutine countToHideQuestStatePanel;

    // Quest Appearance Panel On Screen

    private Dictionary<string, Quest> _currentQuests = new Dictionary<string, Quest>();

    public void InitializeQuest(Dictionary<string, Quest> quests)
    {
        _currentQuests = quests;
    }


    private void Start()
    {
        StartCoroutine(InitializeQuestAppearancePanel());
    }

    private void OnEnable()
    {
        InitializePool();
        _questAppearanceItems = new Dictionary<string, QuestAppearanceItem>();

        GameEventManager.Instance.QuestEvents.onStartQuest += OnStartQuest;
        GameEventManager.Instance.QuestEvents.onQuestStepStateChange += OnQuestStepStateChange;
        GameEventManager.Instance.QuestEvents.onFinishQuest += OnFinishQuest;
    }

    private void OnDisable()
    {
        GameEventManager.Instance.QuestEvents.onStartQuest -= OnStartQuest;
        GameEventManager.Instance.QuestEvents.onQuestStepStateChange -= OnQuestStepStateChange;
        GameEventManager.Instance.QuestEvents.onFinishQuest -= OnFinishQuest;
    }

    private void OnFinishQuest(string id)
    {
        ControlQuestStatePanel(true);
        countToHideQuestStatePanel = StartCoroutine(QuestStatePanelAppear());
        string text = "Quest Completed";
        questStateTitleText.text = id;
        HandleQuestStateDescript(text);
    }


    private void OnStartQuest(string id)
    {
        ControlQuestStatePanel(true);
        countToHideQuestStatePanel = StartCoroutine(QuestStatePanelAppear());
        string text = "Quest Started";
        questStateTitleText.text = id;
        HandleQuestStateDescript(text);

    }


    private void OnQuestStepStateChange(string id, int index, QuestStepState state)
    {
        Quest quest = _currentQuests[id];
        if (quest == null)
        {
            Debug.LogError("Quest not found");
        }
        Debug.Log("Quest step state change + " + state.state);
        Debug.Log("Count of quest steps: " + quest.questSteps.Count);
        if (!_questAppearanceItems.ContainsKey(quest.questInfo.id)) return;

        _questAppearanceItems[id].UpdateQuestNameText(GetQuestName(quest), GetQuestProgressContent(quest));
    }


    IEnumerator QuestStatePanelAppear()
    {
        yield return new WaitForSeconds(timeStatePanelAppear);
        questStatePanel.SetActive(false);

        countToHideQuestStatePanel = null;

    }

    public void ControlQuestStatePanel(bool isActive)
    {
        if (_isQuestStatePanelActive)
        {
            questStatePanel.SetActive(false);
            countToHideQuestStatePanel = null;
        }
        _isQuestStatePanelActive = isActive;
        questStatePanel.SetActive(isActive);
    }

    public void HandleQuestStateDescript(string text)
    {
        questStateDescriptText.text = text;
    }

    public void InitializeAllQuestUI(Dictionary<string, Quest> quests)
    {
        foreach (Quest quest in quests.Values)
        {
            Debug.Log("Initializing Quest UI");
            InitializeQuestUI(quest, quest.state);
        }
    }

    public void InitializeQuestUI(Quest quest, QuestState state)
    {
        UpdateQuestAppearancePanel(quest, state);
    }

    private void UpdateQuestAppearancePanel(Quest quest, QuestState state)
    {
        Debug.Log("Updating Quest Appearance Panel");

        switch (state)
        {
            case QuestState.IN_PROGRESS:
                InProgressUpdate(quest);
                break;
            case QuestState.CAN_FINISH:
                CanFinishUpdate(quest);
                break;
            case QuestState.FINISHED:
                FinishUpdate(quest);
                break;
        }
    }



    IEnumerator ReleaseQuestAppearanceItem(QuestAppearanceItem questAppearanceItem)
    {
        yield return new WaitForSeconds(.2f);
        _questAppearanceItemPool.Release(questAppearanceItem);
        _questAppearanceItems.Remove(questAppearanceItem.QuestName);
    }

    private string GetQuestName(Quest quest)
    {
        return quest.questInfo.displayName;
    }

    private string GetQuestProgressContent(Quest quest)
    {
        if (quest.questSteps == null)
        {
            Debug.Log("Quest steps is null");
            return "";
        }
        if (quest.questSteps.Count == 0)
        {
            Debug.Log("Quest steps count is 0");
            return quest.questInfo.displayName;
        }
        if (quest.questSteps[quest.questInfo.id] == null)
        {
            Debug.Log("Quest step is null");
            return "";
        }
        Debug.Log("Quest step is not null in quest ui manager");
        return quest.questSteps[quest.questInfo.id].GetQuestProgressContent();
    }

    #region Pool Methods
    private void InitializePool()
    {
        _questAppearanceItemPool = new ObjectPool<QuestAppearanceItem>(() =>
        {
            QuestAppearanceItem go = Instantiate(_questAppearanceItemPrefab);
            go.transform.SetParent(transform);
            go.transform.localScale = Vector3.one;
            go.gameObject.SetActive(false);
            return go;
        });
    }

    #endregion

    #region Quest State Panel Methods
    private void InProgressUpdate(Quest quest)
    {
        QuestAppearanceItem questAppearanceItem;

        if (_questAppearanceItems.ContainsKey(quest.questInfo.id))
        {
            questAppearanceItem = _questAppearanceItems[quest.questInfo.id];
            questAppearanceItem.UpdateQuestNameText(GetQuestName(quest), GetQuestProgressContent(quest));
            return;
        }
        questAppearanceItem = _questAppearanceItemPool.Get();
        _questAppearanceItems.Add(quest.questInfo.id, questAppearanceItem);
        questAppearanceItem.gameObject.SetActive(true);
        questAppearanceItem.transform.SetParent(_questApperanceItemParent);
        questAppearanceItem.UpdateQuestNameText(GetQuestName(quest), GetQuestProgressContent(quest));
        questAppearanceItem.transform.localScale = Vector3.one;

        GameEventManager.Instance.QuestEvents.QuestStateChange(quest);
    }
    private void FinishUpdate(Quest quest)
    {
        foreach (Transform child in _questApperanceItemParent)
        {
            QuestAppearanceItem item = child.GetComponent<QuestAppearanceItem>();
            if (item.QuestName != quest.questInfo.displayName) continue;
            item.transform.localScale = Vector3.one;
            Animator anim = child.GetComponent<Animator>();
            anim.SetTrigger("Finished");
            StartCoroutine(ReleaseQuestAppearanceItem(item));
            GameEventManager.Instance.QuestEvents.QuestStateChange(quest);
        }
    }

    private void CanFinishUpdate(Quest quest)
    {
        if (!_questAppearanceItems.ContainsKey(quest.questInfo.id))
        {
            QuestAppearanceItem questAppearanceItem = _questAppearanceItemPool.Get();
            _questAppearanceItems.Add(quest.questInfo.id, questAppearanceItem);
            questAppearanceItem.gameObject.SetActive(true);
            questAppearanceItem.transform.SetParent(_questApperanceItemParent);
            questAppearanceItem.transform.localScale = Vector3.one;
            questAppearanceItem.UpdateQuestNameText(GetQuestName(quest), GetQuestProgressContent(quest));
            Animator anim = questAppearanceItem.GetComponent<Animator>();
            anim.SetTrigger("Active");
            GameEventManager.Instance.QuestEvents.QuestStateChange(quest);
            return;
        }
        foreach (Transform child in _questApperanceItemParent)
        {
            QuestAppearanceItem item = child.GetComponent<QuestAppearanceItem>();
            if (item.QuestName != quest.questInfo.displayName) continue;
            Animator anim = child.GetComponent<Animator>();
            anim.SetTrigger("Active");
            GameEventManager.Instance.QuestEvents.QuestStateChange(quest);
        }
    }

    #endregion




    IEnumerator InitializeQuestAppearancePanel()
    {
        yield return new WaitForSeconds(1f);
        InitializeAllQuestUI(_currentQuests);
    }


}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(Collider))]
public class QuestPoint : MonoBehaviour
{
    [SerializeField] InputReader _inputReader;


    [Header("Quest")]
    [SerializeField]
    private QuestInfo questInfo;
    [Header("Config")]
    [SerializeField]
    private bool StartPoint = false;
    [SerializeField]
    private bool FinishPoint = false;
    [SerializeField]
    private DialogeItem dialogeItem;

    [Header("State Icon")]
    [SerializeField]
    private GameObject canStartIcon;
    [SerializeField]
    private GameObject inProgressIcon;
    [SerializeField]
    private GameObject canFinishIcon;
    [Header("State Minimap Mark Icon")]
    [SerializeField]
    private GameObject canStartMinimapMarkIcon;
    [SerializeField]
    private GameObject inProgressMinimapMarkIcon;
    [SerializeField]
    private GameObject canFinishMinimapMarkIcon;


    private bool playerInRange = false;
    private string questId;
    private QuestState currentQuestState;

    private AudioSource _audioSource;
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        questId = questInfo.id;
        _inputReader = InputReaderManager.Instance.InputReader;
    }

    private void Update()
    {
        if (!playerInRange) return;


        // SummitPressed();
        // DialogeUIManager.Instance.ActivateContactPressEBoard(dialogeItem.actorName, dialogeItem.actorIcon.sprite, false);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;
        DialogeUIManager.Instance.ActivateContactPressEBoard(dialogeItem.actorName, questInfo.dialogeInfo.actorIcon, true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
        DialogeUIManager.Instance.ActivateContactPressEBoard(dialogeItem.actorName, questInfo.dialogeInfo.actorIcon, false);
        if (!DialogeUIManager.Instance.IsDialogOpen) return;
        DialogeUIManager.Instance.CloseDialoge(dialogeItem);
    }


    private void OnEnable()
    {
        GameEventManager.Instance.QuestEvents.onQuestStateChange += OnQuestStateChange;
        _inputReader = InputReaderManager.Instance.InputReader;
        _inputReader.OpenDialogeEvent += HandlePlayerOpenDialoge;
    }




    private void OnDisable()
    {
        if(GameEventManager.Instance != null) GameEventManager.Instance.QuestEvents.onQuestStateChange -= OnQuestStateChange;
        if(_inputReader != null) _inputReader.OpenDialogeEvent -= HandlePlayerOpenDialoge;
    }
    private void OnQuestStateChange(Quest quest)
    {
        if (quest.questInfo.id.Equals(questId))
        {
            currentQuestState = quest.state;
            UpdateQuestPointIcon(currentQuestState);
        }
    }

    private void UpdateQuestPointIcon(QuestState currentQuestState)
    {
        canStartIcon.SetActive(currentQuestState == QuestState.CAN_START);
        canStartMinimapMarkIcon.SetActive(currentQuestState == QuestState.CAN_START);
        inProgressIcon.SetActive(currentQuestState == QuestState.IN_PROGRESS);
        inProgressMinimapMarkIcon.SetActive(currentQuestState == QuestState.IN_PROGRESS);
        canFinishIcon.SetActive(currentQuestState == QuestState.CAN_FINISH);
        canFinishMinimapMarkIcon.SetActive(currentQuestState == QuestState.CAN_FINISH);
    }

    private void HandlePlayerOpenDialoge()
    {
        if (!playerInRange) return;
        SummitPressed();
    }


    private void SummitPressed()
    {
        Debug.Log("SummitPressed");

        bool canOpen = (currentQuestState == QuestState.CAN_START && StartPoint)
                    || (currentQuestState == QuestState.CAN_FINISH && FinishPoint)
                    || (currentQuestState == QuestState.IN_PROGRESS)
                    || (currentQuestState == QuestState.REQUIREMENTS_NOT_MET && StartPoint)
                    || (currentQuestState == QuestState.FINISHED && FinishPoint);

        if (!canOpen) return;


        GameEventManager.Instance.DialogeEvents.DialogeQuestStart(questInfo, currentQuestState, dialogeItem, _audioSource);
        GameEventManager.Instance.PlayerEvents.PlayerInteractStateEnter(transform);
    }

    public QuestInfo GetQuestInfo()
    {
        return questInfo;
    }
}

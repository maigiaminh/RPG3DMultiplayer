using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest
{
    public QuestInfo questInfo;
    public QuestState state;
    private int _currentQuestStepIndex;
    private QuestStepState[] _questStepStates;
    public QuestStepState CurrentQuestStepState => _questStepStates[_currentQuestStepIndex];
    public Dictionary<string, QuestStep> questSteps = new Dictionary<string, QuestStep>();

    public Quest(QuestInfo questInfo)
    {
        questSteps = new Dictionary<string, QuestStep>();
        this.questInfo = questInfo;
        this.state = QuestState.REQUIREMENTS_NOT_MET;
        this._currentQuestStepIndex = 0;
        _questStepStates = new QuestStepState[questInfo.questStepPrefabs.Length];
        for (int i = 0; i < _questStepStates.Length; i++)
        {
            _questStepStates[i] = new QuestStepState();
        }
    }

    public Quest(QuestInfo questInfo, QuestState questState, int currentQuestStepIndex, QuestStepState[] questStepStates)
    {
        questSteps = new Dictionary<string, QuestStep>();
        this.questInfo = questInfo;
        this.state = questState;
        this._currentQuestStepIndex = currentQuestStepIndex;
        _questStepStates = questStepStates;

        if (questStepStates == null || questStepStates.Length == 0)
        {
            Debug.LogError($"Quest '{questInfo.id}' has invalid questStepStates data.");
            questStepStates = new QuestStepState[questInfo.questStepPrefabs.Length];
            for (int i = 0; i < questStepStates.Length; i++)
            {
                questStepStates[i] = new QuestStepState();
            }
        }

        _questStepStates = questStepStates;

        if (_questStepStates.Length != this.questInfo.questStepPrefabs.Length)
        {
            Debug.LogWarning("Quest step states length does not match quest step prefabs length.");
        }
    }

    public void MoveToNextStep()
    {
        _currentQuestStepIndex++;
    }

    public bool CurrentStepExist()
    {
        return _currentQuestStepIndex < questInfo.questStepPrefabs.Length;
    }

    public void InstantiateCurrentQuestStep(Transform parentTransform)
    {
        Debug.Log("Instantiating quest step");
        GameObject questStepPrefab = GetCurrentQuestStepPrefab();
        if (questStepPrefab != null)
        {
            Debug.Log("Quest step prefab is not null");
            QuestStep questStep = Object.Instantiate(questStepPrefab, parentTransform).GetComponent<QuestStep>();
            Debug.Log(" _questStepStates[_currentQuestStepIndex].state: " + _questStepStates[_currentQuestStepIndex].state);
            questStep.InitializeQuestStep(questInfo.id, _currentQuestStepIndex, _questStepStates[_currentQuestStepIndex].state);

            questSteps.Add(questInfo.id, questStep);
        }
    }

    private GameObject GetCurrentQuestStepPrefab()
    {
        GameObject questStepPrefab = null;
        if (CurrentStepExist())
        {
            if (questInfo.questStepPrefabs[_currentQuestStepIndex] == null)
            {
                Debug.LogWarning("Quest step prefab is null");
                return null;
            }
            questStepPrefab = questInfo.questStepPrefabs[_currentQuestStepIndex];
        }
        else
        {
            Debug.LogWarning("No more quest steps to instantiate");
        }
        return questStepPrefab;
    }

    public void StoreQuestStepState(QuestStepState state, int index)
    {
        if (index > _questStepStates.Length)
        {
            Debug.LogWarning("Index out of range");
            return;
        }
        _questStepStates[index] = state;
    }

    public QuestData GetQuestData()
    {
        return new QuestData(state, _currentQuestStepIndex, _questStepStates);
    }
}




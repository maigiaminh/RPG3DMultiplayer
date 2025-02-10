using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class QuestStep : MonoBehaviour
{
    private bool _isFinished = false;
    protected string _questId;
    protected int _stepIndex;

    [SerializeField] protected bool isTimied = false;


    public void InitializeQuestStep(string questId, int stepIndex, string questStepState){
        Debug.Log("QuestStep initialized");
        _questId = questId;
        _stepIndex = stepIndex;
        if(questStepState != null && questStepState != ""){
            SetQuestStepState(questStepState);
        } 
    }

    protected void FinishQuestStep()
    {
        _isFinished = true;
        // send out the advancequest event
        GameEventManager.Instance.QuestEvents.AdvanceQuest(_questId);
    
        Destroy(gameObject);
    }

    protected void ChangeState(string newState){
        GameEventManager.Instance.QuestEvents.QuestStepStateChange(_questId, _stepIndex, new QuestStepState(newState));
    }

    protected abstract void SetQuestStepState(string state);
    public abstract string GetQuestProgressContent();
}

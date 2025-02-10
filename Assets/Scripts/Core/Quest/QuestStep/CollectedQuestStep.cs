using System;
using UnityEngine;

public abstract class CollectedQuestStep : QuestStep
{
    public int TargetItemAmount;
    protected int _currentItemAmount;


    protected abstract void OnEnable();

    protected abstract void OnDisable();


    protected virtual void OnPlayerCollect(int amount)
    {
        _currentItemAmount += amount;
        UpdateState();

        if (_currentItemAmount >= TargetItemAmount)
        {
            FinishQuestStep();
        }
    }



    protected override void SetQuestStepState(string state)
    {
        int tempState = System.Int32.Parse(state);

        _currentItemAmount = tempState;
        OnPlayerCollect(tempState);
    }

    protected void UpdateState()
    {
        Debug.Log("Update state called");
        string state = _currentItemAmount.ToString();
        ChangeState(state);
    }

}

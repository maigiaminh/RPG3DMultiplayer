using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GoToDestinationStep : QuestStep
{
    private Dictionary<Destination, bool> _destinationDict;
    public List<Destination> _destinationList = new List<Destination>();
    private void Awake()
    {
        _destinationDict = new Dictionary<Destination, bool>();
        foreach (Destination des in _destinationList)
        {
            _destinationDict.Add(des, false);
        }
    }
    private void OnEnable()
    {
        Destination.onDestinationReached += OnDestinationReached;
    }

    private void Start()
    {
        int index = 0;
        Debug.Log("QuestId: " + _questId);
        foreach (Destination child in _destinationDict.Keys)
        {
            Instantiate(child);
            child.SetDestination(_questId, index);
            index++;
        }
    }

    private void OnDisable()
    {
        Destination.onDestinationReached -= OnDestinationReached;

        // foreach (Destination child in _destinationDict.Keys)
        // {
        //     if (child == null) continue;
        //     DestroyImmediate(child.gameObject);
        // }
        _destinationDict.Clear();
    }

    protected override void SetQuestStepState(string state)
    {

    }



    private void OnDestinationReached(string questId, int id)
    {
        if (questId != _questId) return;
        if (_destinationDict.Count == 0)
        {
            Debug.LogError("No destinations found");
        }
        Destination tempDes = null;
        foreach (Destination des in _destinationDict.Keys)
        {
            if (des.destinationId == id)
            {
                tempDes = des;
            }
        }
        if (tempDes)
        {
            _destinationDict[tempDes] = true;
        }
        else
        {
            Debug.LogError("Destination not found");
        }

        foreach (bool value in _destinationDict.Values)
        { // make sure that all destinations have been reached
            if (!value) return;
        }

        FinishQuestStep();
    }

    public override string GetQuestProgressContent()
    {
        return _questId;
    }
}

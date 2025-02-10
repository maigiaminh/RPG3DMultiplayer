using System;
using System.Collections.Generic;
using UnityEngine;

public class TokenManager : MonoBehaviour
{
    public static TokenManager Instance;

    public int CurrentTokens { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        CurrentTokens = 0;
    }

    private void OnEnable()
    {
        DungeonEventManager.Instance.dungeonTokenEvents.OnTokenAdded += OnTokenAdded;
        DungeonEventManager.Instance.dungeonTokenEvents.OnTokenRemoved += OnTokenRemoved;
    }

    private void OnDisable() {
        DungeonEventManager.Instance.dungeonTokenEvents.OnTokenAdded -= OnTokenAdded;
        DungeonEventManager.Instance.dungeonTokenEvents.OnTokenRemoved -= OnTokenRemoved;
    }

    private void OnTokenAdded(int amount)
    {
        CurrentTokens += amount;
    }

    private void OnTokenRemoved(int amount)
    {
        if(CurrentTokens - amount < 0) return;

        CurrentTokens -= amount;
    }


}

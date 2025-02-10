using System;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    public int LevelRequired;


    private void OnEnable()
    {
        if (GameEventManager.Instance) GameEventManager.Instance.PlayerEvents.OnPlayerLevelChange += CheckLevel;
        if (UserContainer.Instance)
        {
            Invoke("CheckLevel", 1f);
            return;
        }
    }

    private void OnDisable()
    {
        if (GameEventManager.Instance) GameEventManager.Instance.PlayerEvents.OnPlayerLevelChange -= CheckLevel;
    }

    private void Start()
    {
        if (PlayerLevelManager.Instance)
        {
            CheckLevel(PlayerLevelManager.Instance.Level);
            return;
        }

    }

    private void CheckLevel(int level)
    {
        if (level >= LevelRequired)
        {
            gameObject.SetActive(false);
        }
    }
    private void CheckLevel()
    {
        var level = UserContainer.Instance.UserData != null ? UserContainer.Instance.UserData.Level : 1;
        if (level >= LevelRequired)
        {
            gameObject.SetActive(false);
        }
    }
}

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DungeonDescriptPanel : MonoBehaviour
{
    [Header("Diffculty Descript")]
    public List<DungeonDifficultyItem> DungeonDifficultyItems = new List<DungeonDifficultyItem>();
    public DungeonRewardPanel DungeonRewardPanel;
    public TextMeshProUGUI DungeonName;
    
    [Header("Button")]
    public Button EnterDungeonButton;
    public GameObject DungeonButtonUnlock;
    private Animator _entryButtonAnimator;

    [HideInInspector] public DungeonDifficultyItem CurrentDungeonDifficultyItem;


    private void OnEnable() {
        _entryButtonAnimator = EnterDungeonButton.GetComponent<Animator>();
    }

    private void OnDisable()
    {
        foreach (DungeonDifficultyItem item in DungeonDifficultyItems)
        {
            var item1 = item;
            item.ActivateButton.onClick.RemoveListener(() => OnEnterButtonClick(item1));
        }
    }



    public virtual void SetDungeonDescript(DungeonDescriptScriptableObject dungeonDescript)
    {
        int playerLevel = PlayerLevelManager.Instance.Level;
        
        foreach (DungeonDifficultyItem item in DungeonDifficultyItems)
        {
            item.SetDifficultyDescript(playerLevel, dungeonDescript);
        }
    }

    private void OnEnterButtonClick(DungeonDifficultyItem item)
    {
        CurrentDungeonDifficultyItem = item;
        DungeonRewardPanel.SetDungeonReward(item.DungeonType, item.DungeonDescript);
        EnterDungeonButton.interactable = !item.isLock;
        DungeonButtonUnlock.SetActive(item.isLock);
        _entryButtonAnimator.SetTrigger(item.isLock ? "Disabled" : "Enabled");
    }

    public void Initialize()
    {
        Debug.Log("Initialize Dungeon Descript Panel");
        OnEnterButtonClick(DungeonDifficultyItems[0]);
        DungeonName.text = DungeonDifficultyItems[0].DungeonDescript.DungeonName;

        foreach (DungeonDifficultyItem item in DungeonDifficultyItems)
        {
            if (item != null && item.ActivateButton != null)
                item.ActivateButton.onClick.AddListener(() => OnEnterButtonClick(item));
            else
                Debug.LogError("DungeonDifficultyItem or ActivateButton is null!");
        }
    }


}

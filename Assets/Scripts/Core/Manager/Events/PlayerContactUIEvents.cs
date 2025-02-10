using System;
using UnityEngine.Rendering;

public class PlayerContactUIEvents
{
    public event Action OnPlayerOpenInventory;
    public void PlayerOpenInventory()
    {
        OnPlayerOpenInventory?.Invoke();
    }

    public event Action OnPlayerCloseInventory;
    public void PlayerCloseInventory()
    {
        OnPlayerCloseInventory?.Invoke();
    }

    public event Action<DialogeItem> OnPlayerOpenDialogeQuest;
    public void PlayerOpenDialogeQuest(DialogeItem item)
    {
        OnPlayerOpenDialogeQuest?.Invoke(item);
    }

    public event Action<DialogeItem> OnPlayerCloseDialogeQuest;
    public void PlayerCloseDialogeQuest(DialogeItem item)
    {
        OnPlayerCloseDialogeQuest?.Invoke(item);
    }

    public event Action<FeatureItem> OnPlayerOpenDialogeFeature;
    public void PlayerOpenDialogeFeature(FeatureItem item)
    {
        OnPlayerOpenDialogeFeature?.Invoke(item);
    }

    public event Action<FeatureItem> OnPlayerCloseDialogeFeature;
    public void PlayerCloseDialogeFeature(FeatureItem item)
    {
        OnPlayerCloseDialogeFeature?.Invoke(item);
    }

    public event Action OnPlayerOpenSkillPanel;
    public void PlayerOpenSkillPanel()
    {
        OnPlayerOpenSkillPanel?.Invoke();
    }

    public event Action OnPlayerCloseSkillPanel;
    public void PlayerCloseSkillPanel()
    {
        OnPlayerCloseSkillPanel?.Invoke();
    }

    public event Action<DungeonDescriptItem> OnPlayerOpenDungeonDescriptPanel;
    public void PlayerOpenDungeonDescriptPanel(DungeonDescriptItem item)
    {
        OnPlayerOpenDungeonDescriptPanel?.Invoke(item);
    }

    public event Action<DungeonDescriptItem> OnPlayerCloseDungeonDescriptPanel;
    public void PlayerCloseDungeonDescriptPanel(DungeonDescriptItem item)
    {
        OnPlayerCloseDungeonDescriptPanel?.Invoke(item);
    }

    public event Action OnPlayerOpenEcsMode;
    public void PlayerOpenEcsMode()
    {
        OnPlayerOpenEcsMode?.Invoke();
    }

    public event Action OnPlayerCloseEcsMode;
    public void PlayerCloseEcsMode()
    {
        OnPlayerCloseEcsMode?.Invoke();
    }

    public event Action OnPlayerOpenStore;
    public void PlayerOpenStore()
    {
        OnPlayerOpenStore?.Invoke();
    }
    public event Action OnPlayerCloseStore;
    public void PlayerCloseStore()
    {
        OnPlayerCloseStore?.Invoke();
    }

    public event Action OnPlayerOpenStatBoard;
    public void PlayerOpenStatBoard()
    {
        OnPlayerOpenStatBoard?.Invoke();
    }

    public event Action OnPlayerCloseStatBoard;
    public void PlayerCloseStatBoard()
    {
        OnPlayerCloseStatBoard?.Invoke();
    } 

    public event Action OnPlayerOpenMap;
    public void PlayerOpenMap()
    {
        OnPlayerOpenMap?.Invoke();
    }

    public event Action OnPlayerCloseMap;
    public void PlayerCloseMap()
    {
        OnPlayerCloseMap?.Invoke();
    } 
}

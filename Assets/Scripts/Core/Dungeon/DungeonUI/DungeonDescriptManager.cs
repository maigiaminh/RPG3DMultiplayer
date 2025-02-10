using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DungeonDescriptManager : Singleton<DungeonDescriptManager>
{
    public DungeonDescriptPanel DungeonDescriptPanel;
    public DungeonSpecialDescriptPanel SpecialDescriptPanel;
    public DungeonContactName DungeonContactName;
    public Button EnterDungeonButton;
    public Button EnterSpecialDungeonButton;
    public GameObject loadingPanel;


    public DungeonDescriptItem CurrentDungeonDescriptItem { get; set; }


    protected override void Awake()
    {
        base.Awake();
        EnterDungeonButton.onClick.AddListener(() => EnterDungeon());
        EnterSpecialDungeonButton.onClick.AddListener(() => EnterDungeon());
    }


    private void OnEnable()
    {
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerOpenDungeonDescriptPanel += OpenDungeonDescriptPanel;
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerCloseDungeonDescriptPanel += CloseDungeonDescriptPanel;


    }

    private void OnDisable()
    {
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerOpenDungeonDescriptPanel -= OpenDungeonDescriptPanel;
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerCloseDungeonDescriptPanel -= CloseDungeonDescriptPanel;
    }

    private void OpenDungeonDescriptPanel(DungeonDescriptItem item)
    {
        CurrentDungeonDescriptItem = item;
        if (!DungeonDescriptPanel.gameObject)
        {
            Debug.LogError("Dungeon Descript Panel is not assigned");
            return;
        }
        if (!DungeonDescriptPanel)
        {
            Debug.LogError("Dungeon Descript is not assigned");
            return;
        }
        if (!DungeonDescriptPanel.gameObject)
        {
            Debug.LogError("Dungeon Descript GameObject is not assigned");
            return;
        }
        if (item.dungeonDescript.DungeonData.DungeonMode == DungeonMode.Traditional)
        {
            DungeonDescriptPanel.gameObject.SetActive(true);
            DungeonDescriptPanel.SetDungeonDescript(item.dungeonDescript);
            DungeonDescriptPanel.Initialize();
            return;
        }
        else if (item.dungeonDescript.DungeonData.DungeonMode == DungeonMode.Special)
        {
            SpecialDescriptPanel.gameObject.SetActive(true);
            SpecialDescriptPanel.SetDungeonDescript(item.dungeonDescript);
            SpecialDescriptPanel.Initialize();
            return;
        }

    }

    private void CloseDungeonDescriptPanel(DungeonDescriptItem item)
    {
        if (!CurrentDungeonDescriptItem) return;
        DungeonDescriptPanel.gameObject.SetActive(false);
        SpecialDescriptPanel.gameObject.SetActive(false);
    }


    private void EnterDungeon()
    {
        GameEventManager.Instance.PlayerContactUIEvents.PlayerCloseDungeonDescriptPanel(CurrentDungeonDescriptItem);
        DeactiveAllPanel();

        if (CurrentDungeonDescriptItem.dungeonDescript.DungeonData.DungeonMode == DungeonMode.Traditional)
        {
            if (LoadingManager.Instance)
            {
                LoadingManager.Instance.NewLoadScene(PlayerSpawnManager.Instance.DUNGEON_SCENE_NAME);
            }
            else
            {
                SceneManager.LoadScene(PlayerSpawnManager.Instance.DUNGEON_SCENE_NAME);
            }
        }
        else if (CurrentDungeonDescriptItem.dungeonDescript.DungeonData.DungeonMode == DungeonMode.Special)
        {
            if (LoadingManager.Instance)
            {
                LoadingManager.Instance.NewLoadScene(PlayerSpawnManager.Instance.DUNGEON_SPECIAL_SCENE_NAME);
            }
            else
            {
                SceneManager.LoadScene(PlayerSpawnManager.Instance.DUNGEON_SPECIAL_SCENE_NAME);
            }
        }
        // SceneManager.LoadScene(PlayerSpawnManager.Instance.DUNGEON_SPECIAL_SCENE_NAME);

        // ActivateLoadingPanel();
        // Invoke("DeactivateLoadingPanel", 2f);
    }

    public void ActivateLoadingPanel()
    {
        loadingPanel.SetActive(true);
    }
    public void DeactivateLoadingPanel()
    {
        loadingPanel.SetActive(false);
    }

    public void DeactiveAllPanel()
    {
        DungeonDescriptPanel.gameObject.SetActive(false);
        DungeonContactName.gameObject.SetActive(false);
    }


}

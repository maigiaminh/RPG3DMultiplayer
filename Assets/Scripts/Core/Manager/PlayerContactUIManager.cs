using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerContactUIManager : Singleton<PlayerContactUIManager>
{
    [SerializeField] private GraphicRaycaster raycaster;
    [SerializeField] private PointerEventData pointerEventData;
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private InputReader InputReader;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject exitMenu;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private GameObject tiredUI;

    private bool _isInventoryOpen = false;
    private bool _isDialogeOpen = false;
    private bool _isSkillPanelOpen = false;
    private bool _isDungeonPanelOpen = false;
    private bool _isInEcsMode = false;
    private bool _isStoreOpen = false;
    private bool _isStatBoardOpen = false;
    private bool _isMapOpen = false;

    protected override void Awake()
    {
        base.Awake();
        pointerEventData = new PointerEventData(eventSystem);
    }

    private void OnEnable()
    {
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerOpenDialogeQuest += OnPlayerOpenDialoge;
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerOpenDialogeFeature += OnPlayerOpenDialoge;
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerCloseDialogeFeature += OnPlayerCloseDialoge;
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerOpenInventory += OnPlayerOpenInventory;
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerCloseDialogeQuest += OnPlayerCloseDialoge;
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerCloseInventory += OnPlayerCloseInventory;
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerOpenSkillPanel += OnPlayerOpenSkillPanel;
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerCloseSkillPanel += OnPlayerCloseSkillPanel;
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerOpenDungeonDescriptPanel += OnPlayerOpenDungeonDescriptPanel;
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerCloseDungeonDescriptPanel += OnPlayerCloseDungeonDescriptPanel;
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerOpenEcsMode += OnPlayerOpenEcsMode;
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerCloseEcsMode += OnPlayerCloseEcsMode;
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerOpenStore += OnPlayerOpenStore;
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerCloseStore += OnPlayerCloseStore;
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerOpenStatBoard += OnPlayerOpenStatBoard;
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerCloseStatBoard += OnPlayerCloseStatBoard;
        GameEventManager.Instance.PlayerEvents.OnPlayerTired += EnableTiredUI;
        GameEventManager.Instance.PlayerEvents.OnPlayerNotTired += DisableTiredUI;

        InputReader.OpenInventoryEvent += HandleOpenInventory;
        InputReader.OpenSkillPanelEvent += HandleOpenSkillPanel;
        InputReader.OpenEcsModeEvent += HandleOpenEcsMode;
        InputReader.OpenStatBoardEvent += HandleOpenStatBoard;
        InputReader.OpenMapEvent += HandleOpenMap;


        resumeButton.onClick.AddListener(HandleOpenEcsMode);
        exitButton.onClick.AddListener(ExitGame);
    }


    private void OnDisable()
    {
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerOpenDialogeQuest -= OnPlayerOpenDialoge;
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerOpenDialogeFeature -= OnPlayerOpenDialoge;
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerCloseDialogeFeature -= OnPlayerCloseDialoge;
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerOpenInventory -= OnPlayerOpenInventory;
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerOpenSkillPanel -= OnPlayerOpenSkillPanel;
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerCloseDialogeQuest -= OnPlayerCloseDialoge;
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerCloseInventory -= OnPlayerCloseInventory;
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerCloseSkillPanel -= OnPlayerCloseSkillPanel;
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerOpenDungeonDescriptPanel -= OnPlayerOpenDungeonDescriptPanel;
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerCloseDungeonDescriptPanel -= OnPlayerCloseDungeonDescriptPanel;
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerOpenEcsMode -= OnPlayerOpenEcsMode;
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerCloseEcsMode -= OnPlayerCloseEcsMode;
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerOpenStore -= OnPlayerOpenStore;
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerCloseStore -= OnPlayerCloseStore;
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerOpenStatBoard -= OnPlayerOpenStatBoard;
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerCloseStatBoard -= OnPlayerCloseStatBoard;
        GameEventManager.Instance.PlayerEvents.OnPlayerTired -= EnableTiredUI;
        GameEventManager.Instance.PlayerEvents.OnPlayerNotTired -= DisableTiredUI;

        InputReader.OpenInventoryEvent -= HandleOpenInventory;
        InputReader.OpenSkillPanelEvent -= HandleOpenSkillPanel;
        InputReader.OpenEcsModeEvent -= HandleOpenEcsMode;
        InputReader.OpenStatBoardEvent -= HandleOpenStatBoard;
        InputReader.OpenMapEvent -= HandleOpenMap;


        resumeButton.onClick.RemoveListener(HandleOpenEcsMode);
        exitButton.onClick.RemoveListener(ExitGame);
    }

    private void OnPlayerOpenDialoge(DialogeItem item)
    {
        _isDialogeOpen = true;
        HandleCursorMode();
    }
    private void OnPlayerOpenDialoge(FeatureItem item)
    {
        _isDialogeOpen = true;
        HandleCursorMode();
    }

    private void OnPlayerCloseDialoge(DialogeItem item)
    {
        _isDialogeOpen = false;
        HandleCursorMode();
    }
    private void OnPlayerCloseDialoge(FeatureItem item)
    {
        _isDialogeOpen = false;
        HandleCursorMode();
    }

    private void OnPlayerOpenInventory() => HandleCursorMode();

    private void OnPlayerCloseInventory() => HandleCursorMode();

    private void OnPlayerOpenSkillPanel() => HandleCursorMode();

    private void OnPlayerCloseSkillPanel() => HandleCursorMode();

    private void OnPlayerOpenStatBoard() => HandleCursorMode();

    private void OnPlayerCloseStatBoard() => HandleCursorMode();

    private void EnableTiredUI() => tiredUI.SetActive(true);
    private void DisableTiredUI() => tiredUI.SetActive(false);

    private void OnPlayerOpenDungeonDescriptPanel(DungeonDescriptItem item)
    {
        _isDungeonPanelOpen = true;
        HandleCursorMode();
    }

    private void OnPlayerCloseDungeonDescriptPanel(DungeonDescriptItem item)
    {
        _isDungeonPanelOpen = false;
        HandleCursorMode();
    }

    private void OnPlayerOpenEcsMode() => HandleCursorMode();

    private void OnPlayerCloseEcsMode() => HandleCursorMode();


    private void OnPlayerOpenStore()
    {
        _isStoreOpen = true;
        GameEventManager.Instance.PlayerEvents.PlayerInteractStateEnter();
        HandleCursorMode();
    }

    private void OnPlayerCloseStore()
    {
        _isStoreOpen = false;
        GameEventManager.Instance.PlayerEvents.PlayerInteractStateExit();
        HandleCursorMode();
    }


    private void HandleCursorMode()
    {
        if (_isDialogeOpen ||
            _isInventoryOpen ||
            _isSkillPanelOpen ||
            _isDungeonPanelOpen ||
            _isInEcsMode ||
            _isStoreOpen)
        {
            CursorManager.Instance.ChangeCursorMode(CursorLockMode.None, true);
        }
        else
        {
            CursorManager.Instance.ChangeCursorMode(CursorLockMode.Locked, false);
        }
    }



    private void Update()
    {
        if (InventoryManager.Instance.IsInventoryOpen)
        {
            if (Input.GetMouseButtonDown(0))
            {
                HandleInventorySlotLeftClicked();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                HandleInventorySlotRightClicked();
            }
        }

    }


    private void HandleOpenSkillPanel()
    {
        if (_isSkillPanelOpen)
        {
            _isSkillPanelOpen = false;
            GameEventManager.Instance.PlayerContactUIEvents.PlayerCloseSkillPanel();
            GameEventManager.Instance.PlayerEvents.PlayerInteractStateExit();
        }
        else
        {
            _isSkillPanelOpen = true;
            GameEventManager.Instance.PlayerContactUIEvents.PlayerOpenSkillPanel();
            GameEventManager.Instance.PlayerEvents.PlayerInteractStateEnter();
        }
    }

    private void HandleOpenInventory()
    {
        if (_isInventoryOpen)
        {
            _isInventoryOpen = false;
            GameEventManager.Instance.PlayerContactUIEvents.PlayerCloseInventory();
            GameEventManager.Instance.PlayerEvents.PlayerInteractStateExit();
        }
        else
        {
            _isInventoryOpen = true;
            GameEventManager.Instance.PlayerContactUIEvents.PlayerOpenInventory();
            GameEventManager.Instance.PlayerEvents.PlayerInteractStateEnter();
        }
    }

    private void HandleOpenEcsMode()
    {
        if (_isInEcsMode)
        {
            _isInEcsMode = false;
            GameEventManager.Instance.PlayerContactUIEvents.PlayerCloseEcsMode();
            SoundManager.ResumeSkillAudio();
            Time.timeScale = 1;

            pauseMenu.SetActive(false);
            optionsMenu.SetActive(false);
            mainMenu.SetActive(false);
            exitMenu.SetActive(false);
        }
        else
        {
            _isInEcsMode = true;
            GameEventManager.Instance.PlayerContactUIEvents.PlayerOpenEcsMode();
            SoundManager.PauseSkillAudio();
            Time.timeScale = 0;

            pauseMenu.SetActive(true);
            mainMenu.SetActive(true);
            optionsMenu.SetActive(false);
            exitMenu.SetActive(false);
        }
    }

    private void HandleOpenStatBoard()
    {
        if (_isStatBoardOpen)
        {
            _isStatBoardOpen = false;
            GameEventManager.Instance.PlayerContactUIEvents.PlayerCloseStatBoard();
            GameEventManager.Instance.PlayerEvents.PlayerInteractStateExit();
        }
        else
        {
            _isStatBoardOpen = true;
            GameEventManager.Instance.PlayerContactUIEvents.PlayerOpenStatBoard();
            GameEventManager.Instance.PlayerEvents.PlayerInteractStateEnter();
        }
    }

    private void HandleOpenMap()
    {
        if (_isMapOpen)
        {
            _isMapOpen = false;
            GameEventManager.Instance.PlayerContactUIEvents.PlayerCloseMap();
            GameEventManager.Instance.PlayerEvents.PlayerInteractStateExit();
        }
        else
        {
            _isMapOpen = true;
            GameEventManager.Instance.PlayerContactUIEvents.PlayerOpenMap();
            GameEventManager.Instance.PlayerEvents.PlayerInteractStateEnter();
        }
    }

    private void HandleInventorySlotLeftClicked()
    {
        Debug.Log("Mouse Clicked");
        pointerEventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();

        raycaster.Raycast(pointerEventData, results);

        foreach (RaycastResult result in results)
        {
            if (!result.gameObject.CompareTag("InventorySlot")) continue;

            InventorySlot slot = result.gameObject.GetComponent<InventorySlot>();

            if (slot == null)
            {
                Debug.LogError("No Inventory Slot Found");
                return;
            }
            GameEventManager.Instance.InventoryEvents.InventorySlotLeftClicked(slot);
            return;
        }
    }

    private void HandleInventorySlotRightClicked()
    {
        pointerEventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();

        raycaster.Raycast(pointerEventData, results);

        foreach (RaycastResult result in results)
        {
            if (!result.gameObject.CompareTag("InventorySlot")) continue;

            InventorySlot slot = result.gameObject.GetComponent<InventorySlot>();

            if (slot == null)
            {
                Debug.LogError("No Inventory Slot Found");
                return;
            }
            GameEventManager.Instance.InventoryEvents.InventorySlotLeftClicked(slot);
            return;
        }
    }

    private void ExitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
    }
}

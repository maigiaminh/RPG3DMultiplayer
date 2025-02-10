using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatUIManager : Singleton<PlayerStatUIManager>
{
    [Header("Player Stat UI")]
    public Image HealthBar;
    public Slider ManaBar;
    public Slider StaminaBar;
    public TextMeshProUGUI HealthStatusText;

    public GameObject TakeDamageBackgroundVignette;


    [Header("Player Stat Panel")]
    public PlayerStatPanel PlayerStatPanel;

    [Header("Failure")]
    public FailBoard FailBoard;


    // Stat

    private PlayerStatManager _playerStatManager;

    private void OnEnable()
    {
        //UI Manager
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerOpenStatBoard += OpenStatBoard;
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerCloseStatBoard += CloseStatBoard;


        // Stat

        _playerStatManager.OnDie += HandlePlayerDie;

        _playerStatManager.OnPlayerHealthStatChanged += HandlePlayerHealthStatChanged;
        _playerStatManager.OnPlayerManaStatChanged += HandlePlayerManaStatChanged;
        _playerStatManager.OnPlayerStaminaStatChanged += HandlePlayerStaminaStatChanged;
        _playerStatManager.OnPlayerIntelligenceStatChanged += UpdatePlayerStatPanel;
        _playerStatManager.OnPlayerDefenseStatChanged += UpdatePlayerStatPanel;
        _playerStatManager.OnPlayerAgilityStatChanged += UpdatePlayerStatPanel;
        _playerStatManager.OnPlayerLuckStatChanged += UpdatePlayerStatPanel;
        _playerStatManager.OnPlayerStrengthStatChanged += UpdatePlayerStatPanel;
        _playerStatManager.OnAttributePointsChanged += UpdatePlayerStatPanel;


        // Current health, mana, stamina
        _playerStatManager.OnPlayerCurrentHealthChanged += HandlePlayerHealthChanged;
        _playerStatManager.OnPlayerCurrentManaChanged += HandlePlayerManaChanged;
        _playerStatManager.OnPlayerCurrentStaminaChanged += HandlePlayerStaminaChanged;

        _playerStatManager.OnTakeDamage += HandlePlayerTakeDamage;
        PlayerStatPanel.OnStatUpgradeButtonClicked += HandleStatUpgradeButtonClicked;
    }



    private void OnDisable()
    {
        //UI Manager
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerOpenStatBoard -= OpenStatBoard;
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerCloseStatBoard -= CloseStatBoard;

        // Stat
        _playerStatManager.OnPlayerHealthStatChanged -= HandlePlayerHealthStatChanged;
        _playerStatManager.OnPlayerManaStatChanged -= HandlePlayerManaStatChanged;
        _playerStatManager.OnPlayerStaminaStatChanged -= HandlePlayerStaminaStatChanged;
        _playerStatManager.OnPlayerIntelligenceStatChanged -= UpdatePlayerStatPanel;
        _playerStatManager.OnPlayerDefenseStatChanged -= UpdatePlayerStatPanel;
        _playerStatManager.OnPlayerAgilityStatChanged -= UpdatePlayerStatPanel;
        _playerStatManager.OnPlayerLuckStatChanged -= UpdatePlayerStatPanel;
        _playerStatManager.OnPlayerStrengthStatChanged -= UpdatePlayerStatPanel;
        _playerStatManager.OnAttributePointsChanged -= UpdatePlayerStatPanel;


        // Current health, mana, stamina
        _playerStatManager.OnPlayerCurrentHealthChanged -= HandlePlayerHealthChanged;
        _playerStatManager.OnPlayerCurrentManaChanged -= HandlePlayerManaChanged;
        _playerStatManager.OnPlayerCurrentStaminaChanged -= HandlePlayerStaminaChanged;


        _playerStatManager.OnTakeDamage -= HandlePlayerTakeDamage;
        PlayerStatPanel.OnStatUpgradeButtonClicked -= HandleStatUpgradeButtonClicked;
    }



    protected override void Awake()
    {
        base.Awake();
        
        _playerStatManager = PlayerStatManager.Instance;
        if (_playerStatManager == null)
        {
            Debug.LogError("PlayerStatUIManager: PlayerStatManager is null");
            return;
        }

        HealthBar.fillAmount = 1;
        ManaBar.value = 1;
        StaminaBar.value = 1;


        UpdateStatStatus();

        InitializePlayerStatPanel();
    }

    private void HandlePlayerStaminaChanged(int currentAmount)
    {
        StaminaBar.value = (float)currentAmount / _playerStatManager.MaxStamina;
    }


    private void HandlePlayerManaChanged(int currentAmount)
    {
        ManaBar.value = (float)currentAmount / _playerStatManager.MaxMana;
    }

    private void HandlePlayerHealthChanged(int currentAmount)
    {
        HealthBar.fillAmount = (float)_playerStatManager.CurrentHealth / _playerStatManager.MaxHealth;
        UpdateStatStatus();
    }

    private void UpdateStatStatus()
    {
        Debug.Log("UpdateStatStatus + " + _playerStatManager.CurrentHealth + " / " + _playerStatManager.MaxHealth);
        HealthStatusText.text = $"{_playerStatManager.CurrentHealth}/{_playerStatManager.MaxHealth}";
    }

    private void HandlePlayerTakeDamage()
    {
        ActivateVignetteBackground();
    }

    private void ActivateVignetteBackground()
    {
        TakeDamageBackgroundVignette.SetActive(false);
        TakeDamageBackgroundVignette.SetActive(true);
    }


    private void InitializePlayerStatPanel()
    {
        string username;
        string characterClassName;
        Sprite portrait;
        if (UserContainer.Instance == null)
        {
            username = "Unknown";
            characterClassName = "Unknown";
            portrait = null;
        }
        else
        {
            username = UserContainer.Instance.UserData.Name;
            characterClassName = UserContainer.Instance.UserData.CharacterClassName;
            portrait = Resources.Load<Sprite>("Avatar/" + UserContainer.Instance.UserData.AvatarName);
        }

        PlayerStatPanel.Initialize(username, characterClassName, portrait, _playerStatManager.GetCharacterStat());
    }


    private void HandleStatUpgradeButtonClicked(StatType type)
    {
        _playerStatManager.HandleStatUpgradeButtonClicked(type);
    }


    #region Stat Changed Event Handler
    private void UpdatePlayerStatPanel(int amount)
    {
        PlayerStatPanel.UpdateStat(_playerStatManager.GetCharacterStat());
    }

    private void HandlePlayerStaminaStatChanged(int amount)
    {
        HandlePlayerStaminaChanged(_playerStatManager.CurrentStamina);
        UpdatePlayerStatPanel(amount);
    }

    private void HandlePlayerManaStatChanged(int amount)
    {
        HandlePlayerManaChanged(_playerStatManager.CurrentMana);
        UpdatePlayerStatPanel(amount);
    }

    private void HandlePlayerHealthStatChanged(int amount)
    {
        Debug.Log("HandlePlayerHealthStatChanged: " + amount);
        HandlePlayerHealthChanged(_playerStatManager.CurrentHealth);
        UpdatePlayerStatPanel(amount);
    }

    #endregion


    private void HandleAttributePointChanged()
    {
        PlayerStatPanel.UpdateStat(_playerStatManager.GetCharacterStat());
    }

    private void OpenStatBoard()
    {
        PlayerStatPanel.gameObject.SetActive(true);
    }

    private void CloseStatBoard()
    {
        PlayerStatPanel.gameObject.SetActive(false);
    }


    private void HandlePlayerDie()
    {
        FailBoard.gameObject.SetActive(true);
    }
}


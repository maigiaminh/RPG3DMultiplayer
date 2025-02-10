using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStatManager : Singleton<PlayerStatManager>
{
    public CharacterStat CharacterStat { get; private set; }

    public int amountAttributePoints = 3;


    //Core Stats
    public int MaxHealth { get; private set; }
    public int CurrentHealth { get; private set; }
    public int MaxMana { get; private set; }
    public int CurrentMana { get; private set; }
    public int MaxStamina { get; private set; }
    public int CurrentStamina { get; private set; }
    private float StaminaRegenRate = 1f;
    public int Strength { get; private set; }
    public int Intelligence { get; private set; }
    public int Defense { get; private set; }
    public int Agility { get; private set; }
    public int Luck { get; private set; }



    // status
    private bool _isTired = false;
    private bool _isInvulnerable;


    // properties
    public bool IsDead => CurrentHealth == 0;

    // events
    public event Action OnTakeDamage;
    public event Action OnDie;
    //


    // current state
    public event Action<int> OnPlayerCurrentHealthChanged;
    public event Action<int> OnPlayerCurrentManaChanged;
    public event Action<int> OnPlayerCurrentStaminaChanged;

    // stat attribute
    public event Action<int> OnPlayerHealthStatChanged;
    public event Action<int> OnPlayerManaStatChanged;
    public event Action<int> OnPlayerStaminaStatChanged;
    public event Action<int> OnPlayerStrengthStatChanged;
    public event Action<int> OnPlayerDefenseStatChanged;
    public event Action<int> OnPlayerIntelligenceStatChanged;
    public event Action<int> OnPlayerAgilityStatChanged;
    public event Action<int> OnPlayerLuckStatChanged;
    public event Action<int> OnAttributePointsChanged;

    public int AttributePoints { get; private set; }


    private CharacterStatsSO _characterStats;
    private PlayerLevelManager _playerLevelManager;

    private float _timeToRecoverStamina = 3f;

    private int _staminaSave = 0;
    private bool _isStaminaReduce = false;

    private float _timeToRecoverMana = 5f;
    private int _manaSave = 0;
    private bool _isManaReduce = false;

    protected override void Awake()
    {
        base.Awake();
        string characterName = UserContainer.Instance ? UserContainer.Instance.UserData.CharacterClassName : "Wheel Fortune";
        InitializeStats(characterName);
        ConfigInitStats();
    }


    private void OnEnable()
    {
        GameEventManager.Instance.PlayerEvents.OnPlayerLevelChange += HandlePlayerLevelChange;
        GameEventManager.Instance.PlayerEvents.OnPlayerResetOnDie += ResetStatWhenDie;

        GameEventManager.Instance.InventoryEvents.OnItemEquipped += HandleItemEquipped;
        GameEventManager.Instance.InventoryEvents.OnItemUnequipped += HandleItemUnequipped;
        GameEventManager.Instance.InventoryEvents.OnItemUsed += HandleItemUsed;
    }



    private void OnDisable()
    {
        GameEventManager.Instance.PlayerEvents.OnPlayerLevelChange -= HandlePlayerLevelChange;
        GameEventManager.Instance.PlayerEvents.OnPlayerResetOnDie -= ResetStatWhenDie;

        GameEventManager.Instance.InventoryEvents.OnItemEquipped -= HandleItemEquipped;
        GameEventManager.Instance.InventoryEvents.OnItemUnequipped -= HandleItemUnequipped;
        GameEventManager.Instance.InventoryEvents.OnItemUsed -= HandleItemUsed;
    }

    private void Start()
    {
        _playerLevelManager = PlayerLevelManager.Instance;
    }



    private void Update()
    {
        CheckRecoverStatmina();
        CheckRecoverMana();

        if (Input.GetKeyDown(KeyCode.M))
        {
            HandlePlayerCurrentManaChanged(-50);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            HandleAttributePointChanged(1);
        }
    }

    private void CheckRecoverMana()
    {
        if (_isManaReduce)
        {
            if (_manaSave != CurrentMana)
            {
                _timeToRecoverMana = 5f;
                _manaSave = CurrentMana;
            }
            else
            {
                _timeToRecoverMana -= Time.deltaTime;
                if (_timeToRecoverMana <= 0)
                {
                    _isManaReduce = false;
                    _timeToRecoverMana = 5f;
                }
            }
        }
        else
        {
            if (CurrentMana < MaxMana)
            {
                HandlePlayerCurrentManaChanged((int)(MaxMana * 0.1f));
            }
        }

    }

    private void
    CheckRecoverStatmina()
    {
        if (_isStaminaReduce)
        {
            if (_staminaSave != CurrentStamina)
            {
                _timeToRecoverStamina = 3f;
                _staminaSave = CurrentStamina;
            }
            else
            {
                _timeToRecoverStamina -= Time.deltaTime;
                if (_timeToRecoverStamina <= 0)
                {
                    _isStaminaReduce = false;
                    _timeToRecoverStamina = 3f;
                }
            }
        }
        else
        {
            if (CurrentStamina < MaxStamina)
            {
                RecoverStamina();
            }
        }
    }

    public void ReduceMana(int amount)
    {
        _isManaReduce = true;
        HandlePlayerCurrentManaChanged(-amount);
    }


    public void RecoverStamina()
    {
        HandlePlayerCurrentStaminaChanged((int)(StaminaRegenRate));
    }

    public void ReduceStamina(int amount)
    {
        _isStaminaReduce = true;
        HandlePlayerCurrentStaminaChanged(-amount);
    }



    public void SetInvulnerable(bool isInvulnerable)
    {
        _isInvulnerable = isInvulnerable;
    }

    public void DealDamage(int damage)
    {
        if (CurrentHealth == 0) { return; }
        if (_isInvulnerable) { return; }

        Debug.Log("Player take damage: " + damage);


        OnTakeDamage?.Invoke();

        HandlePlayerCurrentHealthChanged(-damage);

        if (CurrentHealth <= 0)
        {
            OnDie?.Invoke();
        }

    }



    private void ConfigInitStats()
    {
        CurrentHealth = MaxHealth;
        CurrentMana = MaxMana;
        CurrentStamina = MaxStamina == 0 ? 100 : MaxStamina;
    }



    private void ResetStatWhenDie()
    {
        HandlePlayerCurrentManaChanged(MaxMana);
        HandlePlayerCurrentHealthChanged(MaxHealth);
        HandlePlayerCurrentStaminaChanged(MaxStamina);
    }


    public void InitializeStats(string characterClassName)
    {
        if (characterClassName == null || characterClassName == "")
        {
            Debug.Log("Character class name is null or empty, set to default");
            characterClassName = "Wheel Fortune";
        }
        _characterStats = Resources.Load<CharacterStatsSO>("CharacterStats/" + characterClassName);
        if (UserContainer.Instance.CharacterStat != null)
        {
            MaxHealth = UserContainer.Instance.CharacterStat.MaxHealth;
            MaxMana = UserContainer.Instance.CharacterStat.MaxMana;
            MaxStamina = UserContainer.Instance.CharacterStat.Stamina;
            Strength = UserContainer.Instance.CharacterStat.Strength;
            Intelligence = UserContainer.Instance.CharacterStat.Intelligence;
            Defense = UserContainer.Instance.CharacterStat.Defense;
            Agility = UserContainer.Instance.CharacterStat.Agility;
            Luck = UserContainer.Instance.CharacterStat.Luck;
            AttributePoints = UserContainer.Instance.CharacterStat.AttributePoints;
        }
        else
        {


            MaxHealth = _characterStats.MaxHealth;
            MaxMana = _characterStats.MaxMana;
            MaxStamina = _characterStats.Stamina;
            Strength = _characterStats.Strength;
            Intelligence = _characterStats.Intelligence;
            Defense = _characterStats.Defense;
            Agility = _characterStats.Agility;
            Luck = _characterStats.Luck;
            AttributePoints = 0;
        }

        // stat attribute
        HandlePlayerAgilityGain(0);
        HandlePlayerDefenseGain(0);
        HandlePlayerIntelligenceGain(0);
        HandlePlayerLuckGain(0);
        HandlePlayerStrengthGain(0);
        HandlePlayerHealthStatChanged(0);
        HandlePlayerManaStatChanged(0);
        HandlePlayerStaminaStatChanged(0);

        // current state
        HandlePlayerCurrentHealthChanged(0);
        HandlePlayerCurrentManaChanged(0);
        HandlePlayerCurrentStaminaChanged(0);

        // attribute points
        HandleAttributePointChanged(0);
    }





    #region Handle Stat Changes

    private void HandlePlayerHealthStatChanged(int amount)
    {
        MaxHealth += amount;
        HandlePlayerCurrentHealthChanged(amount);
        OnPlayerHealthStatChanged?.Invoke(MaxHealth);
    }


    private void HandlePlayerManaStatChanged(int amount)
    {
        MaxMana += amount;
        HandlePlayerCurrentManaChanged(amount);
        OnPlayerManaStatChanged?.Invoke(MaxMana);
    }


    private void HandlePlayerStaminaStatChanged(int amount)
    {
        MaxStamina += amount;
        HandlePlayerCurrentStaminaChanged(amount);
        OnPlayerStaminaStatChanged?.Invoke(MaxStamina);
    }


    private void HandlePlayerLuckGain(int amount)
    {
        Luck += amount;
        OnPlayerLuckStatChanged?.Invoke(Luck);
    }

    public void HandlePlayerLuckReduce(int amount)
    {
        Luck -= amount;
        OnPlayerLuckStatChanged?.Invoke(Luck);
    }

    private void HandlePlayerAgilityGain(int amount)
    {
        Agility += amount;
        OnPlayerAgilityStatChanged?.Invoke(Agility);
    }

    public void HandlePlayerAgilityReduce(int amount)
    {
        Agility -= amount;
        OnPlayerAgilityStatChanged?.Invoke(Agility);
    }

    private void HandlePlayerDefenseGain(int amount)
    {
        Defense += amount;
        OnPlayerDefenseStatChanged?.Invoke(Defense);
    }

    public void HandlePlayerDefenseReduce(int amount)
    {
        Defense -= amount;
        OnPlayerDefenseStatChanged?.Invoke(Defense);
    }

    private void HandlePlayerIntelligenceGain(int amount)
    {
        Intelligence += amount;
        OnPlayerIntelligenceStatChanged?.Invoke(Intelligence);
    }

    public void HandlePlayerIntelligenceReduce(int amount)
    {
        Intelligence -= amount;
        OnPlayerIntelligenceStatChanged?.Invoke(Intelligence);
    }

    private void HandlePlayerStrengthGain(int amount)
    {
        Strength += amount;
        OnPlayerStrengthStatChanged?.Invoke(Strength);
    }

    public void HandlePlayerStrengthReduce(int amount)
    {
        Strength -= amount;
        OnPlayerStrengthStatChanged?.Invoke(Strength);
    }


    private void HandleAttributePointChanged(int amount)
    {
        AttributePoints = Mathf.Max(AttributePoints + amount, 0);
        OnAttributePointsChanged?.Invoke(AttributePoints);
    }


    private void HandlePlayerCurrentStaminaChanged(int amount)
    {
        CurrentStamina = Mathf.Clamp(CurrentStamina + amount, 0, MaxStamina);
        OnPlayerCurrentStaminaChanged?.Invoke(CurrentStamina);
    }

    private void HandlePlayerCurrentManaChanged(int amount)
    {
        CurrentMana = Mathf.Clamp(CurrentMana + amount, 0, MaxMana);
        OnPlayerCurrentManaChanged?.Invoke(CurrentMana);
    }

    private void HandlePlayerCurrentHealthChanged(int amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, MaxHealth);
        OnPlayerCurrentHealthChanged?.Invoke(CurrentHealth);
    }



    public CharacterStat GetCharacterStat()
    {
        return new CharacterStat
        {
            Strength = Strength,
            MaxHealth = MaxHealth,
            MaxMana = MaxMana,
            Stamina = MaxStamina,
            Defense = Defense,
            Agility = Agility,
            Intelligence = Intelligence,
            Luck = Luck,
            AttributePoints = AttributePoints
        };
    }

    public void HandleStatUpgradeButtonClicked(StatType type)
    {
        switch (type)
        {
            case StatType.Strength:
                HandlePlayerStrengthGain(_characterStats.StrengthDelta);
                break;
            case StatType.Health:
                HandlePlayerHealthStatChanged(_characterStats.MaxHealthDelta);
                break;
            case StatType.Mana:
                HandlePlayerManaStatChanged(_characterStats.MaxManaDelta);
                break;
            case StatType.Stamina:
                HandlePlayerStaminaStatChanged(_characterStats.StaminaDelta);
                break;
            case StatType.Intelligence:
                HandlePlayerIntelligenceGain(_characterStats.IntelligenceDelta);
                break;
            case StatType.Defense:
                HandlePlayerDefenseGain(_characterStats.DefenseDelta);
                break;
            case StatType.Agility:
                HandlePlayerAgilityGain(_characterStats.AgilityDelta);
                break;
            case StatType.Luck:
                HandlePlayerLuckGain(_characterStats.LuckDelta);
                break;
        }
        HandleAttributePointChanged(-1);
        PlayerStatUIManager.Instance.PlayerStatPanel.UpdateStat(GetCharacterStat());
    }

    #endregion



    private void HandlePlayerLevelChange(int level)
    {
        Debug.Log("PlayerStatManager: HandlePlayerLevelChange: " + level);
        Debug.Log("PlayerStatManager: MaxLevel: " + _playerLevelManager.MaxLevel);
        if (_playerLevelManager.MaxLevel > level) return;

        HandleAttributePointChanged(amountAttributePoints);
    }


    private void HandleItemUsed(ItemData data)
    {
        if (data.itemType != ItemType.Consumable) return;

        HandlePlayerCurrentHealthChanged(data.healthRestore);
        HandlePlayerCurrentManaChanged(data.manaRestore);
        HandlePlayerCurrentStaminaChanged(data.staminaRestore);
        HandlePlayerStrengthGain(data.strengthBoost);
        HandlePlayerIntelligenceGain(data.intelligenceBoost);
        HandlePlayerAgilityGain(data.agilityBoost);
        HandlePlayerLuckGain(data.luckBoost);
    }

    private void HandleItemEquipped(ItemData data)
    {
        if (data.itemType != ItemType.Equipment) return;

        HandlePlayerStrengthGain(data.strength);
        HandlePlayerIntelligenceGain(data.intelligence);
        HandlePlayerAgilityGain(data.agility);
        HandlePlayerLuckGain(data.luck);
        HandlePlayerDefenseGain(data.defense);
        HandlePlayerHealthStatChanged(data.health);
        HandlePlayerManaStatChanged(data.manaBoost);
        HandlePlayerStaminaStatChanged(data.stamina);

    }

    private void HandleItemUnequipped(ItemData data)
    {
        if (data.itemType != ItemType.Equipment) return;

        HandlePlayerStrengthReduce(data.strength);
        HandlePlayerIntelligenceReduce(data.intelligence);
        HandlePlayerAgilityReduce(data.agility);
        HandlePlayerLuckReduce(data.luck);
        HandlePlayerDefenseReduce(data.defense);
        HandlePlayerHealthStatChanged(-data.health);
        HandlePlayerManaStatChanged(-data.mana);
        HandlePlayerStaminaStatChanged(-data.stamina);
    }



}

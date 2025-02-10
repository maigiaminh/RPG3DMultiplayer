using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;
using Unity.VisualScripting;

public class SkillUIPanelItem : MonoBehaviour
{
    public int LevelRequired;

    [Header("Required Level")]
    public Image UnlockedImage;
    public TextMeshProUGUI LevelText;
    [Header("Skill Icon")]
    public Image SkillIcon;
    [Header("Button")]
    public Button SkillButton;
    public Image ActiveHighlight;
    [Header("Upgrade")]
    public Button UpgradeButton;
    public TextMeshProUGUI UpgradeCostText;
    private Color _defaultColor;
    private Color _activeColor;
    private Animator _activeHighlightAnimator;
    private CharacterSkillScriptableObject _skillData;
    private int _currentLevel = 0;
    private int _currentPlayerLevel = 0;
    public int CurrentCostToUpgrade => _skillData.GetGoldCostToUpgrade(_currentLevel);


    private void Start()
    {

        _defaultColor = ActiveHighlight.color;
        _activeHighlightAnimator = ActiveHighlight.GetComponent<Animator>();
        GameEventManager.Instance.ResourceEvents.PlayerGainGold(0);
        HandlePlayerLevelChange(PlayerLevelManager.Instance.Level);

    }

    private void Awake()
    {
        GameEventManager.Instance.ResourceEvents.onPlayerGainGold += CheckIfPlayerCanUpgrade;
        GameEventManager.Instance.ResourceEvents.onPlayerLoseGold += CheckIfPlayerCanUpgrade;
        GameEventManager.Instance.PlayerEvents.OnPlayerLevelChange += HandlePlayerLevelChange;
        GameEventManager.Instance.SkillEvents.OnSkillUpgrade += OnSkillUpgrade;
    }

    private void OnDestroy()
    {
        GameEventManager.Instance.ResourceEvents.onPlayerGainGold -= CheckIfPlayerCanUpgrade;
        GameEventManager.Instance.ResourceEvents.onPlayerLoseGold -= CheckIfPlayerCanUpgrade;
        GameEventManager.Instance.PlayerEvents.OnPlayerLevelChange -= HandlePlayerLevelChange;
        GameEventManager.Instance.SkillEvents.OnSkillUpgrade -= OnSkillUpgrade;
    }




    public void Initialize(CharacterSkillScriptableObject skillData, int levelRequired)
    {
        _skillData = skillData;
        LevelRequired = levelRequired;
        LevelText.text = "Lvl " + LevelRequired.ToString() + " Required";
    }




    public void SetStateButton(int playerLevel)
    {
        UnlockedImage.gameObject.SetActive(playerLevel < LevelRequired);
    }

    public void SetSkillIcon(Sprite skillIcon, Color skillColor)
    {
        SkillIcon.sprite = skillIcon;
        SkillIcon.color = skillColor;
        _activeColor = skillColor;
    }

    public void ActivateSkill(bool active)
    {
        _activeHighlightAnimator.SetBool("Animate", active);
        ActiveHighlight.color = active ? _activeColor : _defaultColor;
    }




    void CheckIfPlayerCanUpgrade(int gold)
    {
        var _resourceManager = ResourceManager.Instance;
        var _skillManager = SkillManager.Instance;
        if (_skillManager == null)
        {
            Debug.LogError("_skillManager is null");
            return;
        }
        if (_resourceManager == null)
        {
            Debug.LogError("_resourceManager is null");
            return;
        }

        if (_skillData == null)
        {
            Debug.LogError("_skillData is null");
            return;
        }
        try
        {
            int goldCost = _skillData.GetGoldCostToUpgrade(_currentLevel);
            int index = Math.Clamp(_currentLevel, 0, _skillData.SkillLevelRequiredToUpdate.Count - 1);
            Debug.Log($"Index: {index}");
            Debug.Log($"Skill Level Required: {_skillData.SkillLevelRequiredToUpdate[index]}");
            SetUpgradeButtonInteractable(_resourceManager.Gold >= goldCost && _currentPlayerLevel >= _skillData.SkillLevelRequiredToUpdate[index]);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error in CheckIfPlayerCanUpgrade: " + ex.Message);
        }
    }


    private void SetUpgradeButtonInteractable(bool interactable)
    {
        if (!UpgradeButton) return;
        UpgradeButton.interactable = interactable;
    }
    private void ActivateUpgradeButton(bool active)
    {
        UpgradeButton.gameObject.SetActive(active);

    }

    private void OnSkillUpgrade(CharacterSkillScriptableObject data, int amount, bool isInitialize)
    {
        if (data != _skillData) return;

        _currentLevel = SkillManager.Instance.SkillLevels[data];
        Debug.Log("Current Level: " + _currentLevel);
        UpgradeCostText.text = _skillData.GetGoldCostToUpgrade(_currentLevel).ToString();

        if (isInitialize) return;

        GameEventManager.Instance.ResourceEvents.PlayerLoseGold(_skillData.GetGoldCostToUpgrade(_currentLevel));
        Debug.Log("Skill Upgraded to level: " + _currentLevel + " for " + data.name);

    }

    private void HandlePlayerLevelChange(int level)
    {
        _currentPlayerLevel = level;
        SetStateButton(level);
        ActivateUpgradeButton(level >= LevelRequired);
        CheckIfPlayerCanUpgrade(0);
    }

}

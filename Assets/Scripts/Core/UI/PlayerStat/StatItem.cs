using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatItem : MonoBehaviour
{
    public TextMeshProUGUI StatText;
    public Button LevelUpButton;
    public StatType StatType;
    private PlayerStatPanel _playerStatPanel;
    public void Initialize(PlayerStatPanel playerStatPanel, int value, int attributePoints)
    {
        _playerStatPanel = playerStatPanel;
        LevelUpButton.onClick.AddListener(() => _playerStatPanel.StatUpgradeButtonClicked(StatType));        
        UpdateItem(value, attributePoints);
    }

    public void UpdateItem(int value, int attributePoints)
    {
        StatText.text = value.ToString();
        LevelUpButton.transform.parent.gameObject.SetActive(attributePoints > 0);
    }
}

public enum StatType
{
    Strength,
    Health,
    Mana,
    Stamina,
    Intelligence,
    Defense,
    Agility,
    Luck
}
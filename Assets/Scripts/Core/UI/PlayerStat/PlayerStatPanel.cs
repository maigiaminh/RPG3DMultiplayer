using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatPanel : MonoBehaviour
{
    public Image UserPortraitIcon;
    public Sprite DefaultPortrait;

    public TextMeshProUGUI UsernameText;
    public TextMeshProUGUI CharacterNameText;
    public StatItem StrengthItem;
    public StatItem HealthItem;
    public StatItem ManaItem;
    public StatItem StaminaItem;
    public StatItem IntelligenceItem;
    public StatItem DefenseItem;
    public StatItem AgilityItem;
    public StatItem LuckItem;


    public TextMeshProUGUI AttributePointsText;
    public event Action<StatType> OnStatUpgradeButtonClicked;
    public void StatUpgradeButtonClicked(StatType statType)
    {
        Debug.Log("StatUpgradeButtonClicked: " + statType);
        OnStatUpgradeButtonClicked?.Invoke(statType);
    }

    public void Initialize(string username, string characterClassName, Sprite portrait, CharacterStat playerStat)
    {
        UsernameText.text = username;
        CharacterNameText.text = characterClassName;

        UserPortraitIcon.sprite = portrait ? portrait : DefaultPortrait;

        StrengthItem.Initialize(this, playerStat.Strength, playerStat.AttributePoints);
        HealthItem.Initialize(this, playerStat.MaxHealth, playerStat.AttributePoints);
        ManaItem.Initialize(this, playerStat.MaxMana, playerStat.AttributePoints);
        StaminaItem.Initialize(this, playerStat.Stamina, playerStat.AttributePoints);
        IntelligenceItem.Initialize(this, playerStat.Intelligence, playerStat.AttributePoints);
        DefenseItem.Initialize(this, playerStat.Defense, playerStat.AttributePoints);
        AgilityItem.Initialize(this, playerStat.Agility, playerStat.AttributePoints);
        LuckItem.Initialize(this, playerStat.Luck, playerStat.AttributePoints);

        AttributePointsText.text = playerStat.AttributePoints.ToString();
    }



    public void UpdateStat(CharacterStat playerStat)
    {
        var attributePoints = playerStat.AttributePoints;
        StrengthItem.UpdateItem(playerStat.Strength, attributePoints);
        HealthItem.UpdateItem(playerStat.MaxHealth, attributePoints);
        ManaItem.UpdateItem(playerStat.MaxMana, attributePoints);
        StaminaItem.UpdateItem(playerStat.Stamina, attributePoints);
        IntelligenceItem.UpdateItem(playerStat.Intelligence, attributePoints);
        DefenseItem.UpdateItem(playerStat.Defense, attributePoints);
        AgilityItem.UpdateItem(playerStat.Agility, attributePoints);
        LuckItem.UpdateItem(playerStat.Luck, attributePoints);
        AttributePointsText.text = attributePoints.ToString();
    }
}

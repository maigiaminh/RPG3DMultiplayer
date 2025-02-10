using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "ItemData", menuName = "Inventory/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("Item Data")]
    public string itemName;
    public string description;
    public Sprite icon;
    public bool isStackable;
    public int maxStack;
    public enum Rarity { Common, Uncommon, Rare, Epic, Legendary, Mythic }

    public Rarity rarity;

    [Header("Item Type")]
    public ItemType itemType;
    [Header("Weapon Type")]
    public WeaponType weaponType;
    

    // Equipment    


    [Header("Item Properties")]

    [ShowIf(nameof(itemType), ItemType.Equipment)]
    public string set2Stats;
    [ShowIf(nameof(itemType), ItemType.Equipment)]
    public Sprite set2Icon;
    [ShowIf(nameof(itemType), ItemType.Equipment)]
    public string set4Stats;
    [ShowIf(nameof(itemType), ItemType.Equipment)]
    public Sprite set4Icon;
    [ShowIf(nameof(itemType), ItemType.Equipment)]
    public int strength;
    [ShowIf(nameof(itemType), ItemType.Equipment)]
    public int defense;
    [ShowIf(nameof(itemType), ItemType.Equipment)]
    public int agility;
    [ShowIf(nameof(itemType), ItemType.Equipment)]
    // public int critChance;
    // [ShowIf(nameof(itemType), ItemType.Equipment)]
    // public int critDamage;
    // [ShowIf(nameof(itemType), ItemType.Equipment)]
    public int health;
    [ShowIf(nameof(itemType), ItemType.Equipment)]
    public int mana;
    [ShowIf(nameof(itemType), ItemType.Equipment)]
    public int stamina;
    [ShowIf(nameof(itemType), ItemType.Equipment)]
    public int intelligence;
    [ShowIf(nameof(itemType), ItemType.Equipment)]
    public int luck;



    // Consumable
    [ShowIf(nameof(itemType), ItemType.Consumable)]
    public int healthBoost;
    [ShowIf(nameof(itemType), ItemType.Consumable)]
    public int manaBoost;
    [ShowIf(nameof(itemType), ItemType.Consumable)]
    public int staminaBoost;
    [ShowIf(nameof(itemType), ItemType.Consumable)]
    public int healthRestore;
    [ShowIf(nameof(itemType), ItemType.Consumable)]
    public int manaRestore;
    [ShowIf(nameof(itemType), ItemType.Consumable)]
    public int staminaRestore;
    [ShowIf(nameof(itemType), ItemType.Consumable)]
    public int strengthBoost;
    [ShowIf(nameof(itemType), ItemType.Consumable)]
    public int agilityBoost;
    [ShowIf(nameof(itemType), ItemType.Consumable)]
    public int intelligenceBoost;
    [ShowIf(nameof(itemType), ItemType.Consumable)]
    public int luckBoost;
    [ShowIf(nameof(itemType), ItemType.Consumable)]
    public int reduceCooldownBoost;
    [ShowIf(nameof(itemType), ItemType.Consumable)]
    public int healthRegen;
    [ShowIf(nameof(itemType), ItemType.Consumable)]
    public int manaRegen;
    [ShowIf(nameof(itemType), ItemType.Consumable)]
    public int staminaRegen;
    [ShowIf(nameof(itemType), ItemType.Consumable)]
    public int duration;


    // Quest

    [ShowIf(nameof(itemType), ItemType.Quest)]
    public string questName;
    [ShowIf(nameof(itemType), ItemType.Quest)]
    public int questID;




    private void OnValidate()
    {
#if UNITY_EDITOR
        itemName = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }


    public List<KeyValuePair<string, string>> GetNonZeroEquipmentStatsAsString()
    {
        List<KeyValuePair<string, string>> stats = new List<KeyValuePair<string, string>>();

        if (this == null)
        {
            Debug.LogError("ItemData is null!");
            return stats;
        }

        FieldInfo[] fields = this.GetType().GetFields();

        foreach (FieldInfo field in fields)
        {
            if (!ShouldShowFieldForEquipment(field)) continue;

            if (field.FieldType == typeof(Sprite)) continue;
            if (field.FieldType == typeof(string)) continue;


            object value = field.GetValue(this);

            if (value != null)
            {
                string stringValue = value.ToString();
                if (!string.IsNullOrEmpty(stringValue) && stringValue != "0") //Added a check to remove 0 string value
                {
                    stats.Add(new KeyValuePair<string, string>(field.Name, stringValue));
                }


            }
        }

        return stats;
    }

    public List<KeyValuePair<string, string>> GetNonZeroConsumableStatsAsString()
    {
        List<KeyValuePair<string, string>> stats = new List<KeyValuePair<string, string>>();

        if (this == null)
        {
            Debug.LogError("ItemData is null!");
            return stats;
        }

        FieldInfo[] fields = this.GetType().GetFields();

        foreach (FieldInfo field in fields)
        {
            if (!ShouldShowFieldForConsumable(field)) continue;

            if (field.FieldType == typeof(Sprite)) continue;
            if (field.FieldType == typeof(string)) continue;


            object value = field.GetValue(this);

            if (value != null)
            {
                string stringValue = value.ToString();
                if (!string.IsNullOrEmpty(stringValue) && stringValue != "0") //Added a check to remove 0 string value
                {
                    stats.Add(new KeyValuePair<string, string>(field.Name, stringValue));
                }


            }
        }

        return stats;
    }

    private bool ShouldShowFieldForEquipment(FieldInfo field)
    {
        var showIfAttributes = field.GetCustomAttributes(typeof(ShowIfAttribute), true);

        // Check *specifically* for Equipment ShowIf attribute:
        foreach (var attribute in showIfAttributes)
        {
            var showIf = (ShowIfAttribute)attribute;

            if (showIf.ComparedPropertyName == nameof(itemType) &&
                showIf.ComparedValue.Equals(ItemType.Equipment))
            {
                return true; // Show the field ONLY if it has the Equipment ShowIf attribute
            }
        }

        return false; // Hide if no ShowIf for Equipment, or if itemType is not Equipment.
    }
    private bool ShouldShowFieldForConsumable(FieldInfo field)
    {
        var showIfAttributes = field.GetCustomAttributes(typeof(ShowIfAttribute), true);

        // Check *specifically* for Equipment ShowIf attribute:
        foreach (var attribute in showIfAttributes)
        {
            var showIf = (ShowIfAttribute)attribute;

            if (showIf.ComparedPropertyName == nameof(itemType) &&
                showIf.ComparedValue.Equals(ItemType.Equipment))
            {
                return true; // Show the field ONLY if it has the Equipment ShowIf attribute
            }
        }

        return false; // Hide if no ShowIf for Equipment, or if itemType is not Equipment.
    }
}

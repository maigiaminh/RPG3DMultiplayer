using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryContainer : Singleton<InventoryContainer>
{
    public Dictionary<ItemData, int> PlayerItemMap { get; set; } = new Dictionary<ItemData, int>();
    public Dictionary<string, ItemData> ResourceItemMap { get; set; } = new Dictionary<string, ItemData>();

    public Dictionary<string, string> InitItemMap = new Dictionary<string, string>(){
        {"The Chariot", "Prop_Bow_01"},
        {"Wheel Fortune", "SM_Wep_Sword_16"},
        {"The Hierophant", "SM_Wep_Sceptre_04"},
        {"Judgement", "Dagger_01"},
        {"Justice", "SM_Wep_Shield_05"},
    };
    private const string _initWeaponPath = "InitialWeapons";
    protected override void Awake()
    {
        base.Awake();
        InitializeResourceItemMap();
    }

    private void InitializeResourceItemMap()
    {
        foreach (ItemData item in Resources.LoadAll<ItemData>("ItemData"))
        {
            Debug.Log("Loaded item: " + item.itemName);
            ResourceItemMap.Add(item.itemName, item);
        }
    }

    public Dictionary<ItemData, int> GetFilteredPlayerItemMap(ItemType itemType)
    {
        Dictionary<ItemData, int> filteredMap = new Dictionary<ItemData, int>();

        foreach (KeyValuePair<ItemData, int> item in PlayerItemMap)
        {
            if (item.Key.itemType == itemType)
            {
                Debug.Log("Item added to filtered map: " + item.Key.itemName + " " + item.Key.itemType);
                filteredMap.Add(item.Key, item.Value);
            }
        }

        return filteredMap;
    }

    public void UpdateItemFirebase()
    {
        FirebaseAuthManager.Instance.SaveInventoryItems(PlayerItemMap);
    }
    public void UpdateItemFirebase(Dictionary<ItemData, int> PlayerItemMap)
    {
        this.PlayerItemMap = PlayerItemMap;
        FirebaseAuthManager.Instance.SaveInventoryItems(PlayerItemMap);
    }
}

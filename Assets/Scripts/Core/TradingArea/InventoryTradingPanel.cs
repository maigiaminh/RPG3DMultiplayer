using System;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.Netcode;
using UnityEngine;

public class InventoryTradingPanel : NetworkBehaviour
{
    public Transform ItemContainer;
    public List<InventoryTradingSlot> itemSlots = new List<InventoryTradingSlot>();
    public Dictionary<ItemData, int> ItemMap { get; set; } = new Dictionary<ItemData, int>();
    private InventoryTradingManager _inventoryTradingPanel;

    public List<ItemData> itemDatas = new List<ItemData>();



    public void UpdateItemMap()
    {
        ItemMap = InventoryContainer.Instance ? InventoryContainer.Instance.GetFilteredPlayerItemMap(ItemType.Equipment) : new Dictionary<ItemData, int>();

        if (InventoryContainer.Instance)
        {
            Initialize();
            return;
        }

        ItemMap.Clear();

        foreach (var item in itemDatas)
        {
            if (!ItemMap.ContainsKey(item) && item.itemType == ItemType.Equipment)
            {
                Debug.Log("Item added to map: " + item.itemName + " " + item.itemType);
                ItemMap.Add(item, 1);
            }
        }

        return;
    }

    public void ActivatePanel(bool isActive)
    {
        gameObject.SetActive(isActive);
        UpdateItemMap();
        Debug.Log("InventoryTradingPanel enabled");

    }

    public void Initialize()
    {
        InitializeSlots();
    }

    private void InitializeSlots()
    {
        itemSlots = new List<InventoryTradingSlot>();
        for (int i = 0; i < ItemContainer.childCount; i++)
        {
            var slot = ItemContainer.GetChild(i).GetComponent<InventoryTradingSlot>();
            slot.SetIndex(i);
            slot.Initialize();
            itemSlots.Add(slot);
        }
        foreach (InventoryTradingSlot slot in itemSlots)
        {
            slot.SetType(InventoryTradingSlot.SlotType.Inventory);
            slot.SetItem(null, "");
        }
        foreach (var item in ItemMap)
        {
            foreach (InventoryTradingSlot slot in itemSlots)
            {
                if (slot.HaveItem) continue;
                slot.SetItem(item.Key.icon, item.Key.name);
                break;
            }
        }
    }

    public void AddItemSpriteToSlot(Sprite sprite, string itemName)
    {
        foreach (InventoryTradingSlot slot in itemSlots)
        {
            if (slot.HaveItem) continue;

            slot.SetItem(sprite, itemName);
            break;
        }
    }

}

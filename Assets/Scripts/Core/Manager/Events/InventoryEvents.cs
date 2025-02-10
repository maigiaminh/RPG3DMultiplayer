

using System;
using UnityEngine;

public class InventoryEvents
{
    public event Action<ItemData> OnItemUsed;
    public void UseConsumable(ItemData item)
    {
        ItemRemoved(item, 1);
        OnItemUsed?.Invoke(item);
    }
    public event Action<ItemData, int, bool> OnItemAdded;
    public void ItemAdded(ItemData item, int quantity, bool isDictionaryChanged)
    {
        OnItemAdded?.Invoke(item, quantity, isDictionaryChanged);
    }

    public event Action<ItemData, int> OnItemRemoved;
    public void ItemRemoved(ItemData item, int quantity)
    {
        OnItemRemoved?.Invoke(item, quantity);
    }
    public event Action<ItemData> OnItemEquipped;
    public void ItemEquipped(ItemData item)
    {
        OnItemEquipped?.Invoke(item);
    }

    public event Action<ItemData> OnItemUnequipped;
    public void ItemUnequipped(ItemData item)
    {
        OnItemUnequipped?.Invoke(item);
    }

    public event Action<ItemData> OnItemDescriptionBoardOpened;
    public void ItemDescriptionBoardOpened(ItemData item)
    {
        OnItemDescriptionBoardOpened?.Invoke(item);
    }

    public event Action<ItemData> OnItemDescriptionBoardClosed;
    public void ItemDescriptBoardClosed(ItemData item)
    {
        OnItemDescriptionBoardClosed?.Invoke(item);
    }

    public event Action<InventorySlot> OnInventorySlotLeftClicked;
    public void InventorySlotLeftClicked(InventorySlot slot)
    {
        OnInventorySlotLeftClicked?.Invoke(slot);
    }

    public event Action<InventorySlot> OnInventorySlotRightClicked;
    public void InventorySlotRightClicked(InventorySlot slot)
    {
        OnInventorySlotRightClicked?.Invoke(slot);
    }

    public event Action<DraggableItem> OnInventoryItemEndDragged;
    public void InventoryItemEndDragged(DraggableItem draggableItem)
    {
        OnInventoryItemEndDragged?.Invoke(draggableItem);
    }

    public event Action<DraggableItem> OnInventoryItemDropped;
    public void InventoryItemDropped(DraggableItem draggableItem)
    {
        OnInventoryItemDropped?.Invoke(draggableItem);
    }

}

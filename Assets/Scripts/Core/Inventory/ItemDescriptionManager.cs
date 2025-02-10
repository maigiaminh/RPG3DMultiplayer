using System;
using UnityEngine;

public class DescriptionBoardManager : Singleton<DescriptionBoardManager>
{

    public ConsumableDescriptBoard consumableBoard;
    public EquipmentDescriptBoard equipmentBoard;

    private ItemData _currentItemData;

    private DraggableItem _focusedItem;

    protected override void Awake()
    {
        base.Awake();

        GameEventManager.Instance.InventoryEvents.OnItemUsed += OnItemUsed;


        GameEventManager.Instance.InventoryEvents.OnInventorySlotLeftClicked += OnInventorySlotClicked;
        GameEventManager.Instance.InventoryEvents.OnInventoryItemEndDragged += OnInventoryItemEndDragged;
        GameEventManager.Instance.InventoryEvents.OnItemDescriptionBoardOpened += OnItemDescriptionBoardOpened;
        GameEventManager.Instance.InventoryEvents.OnItemDescriptionBoardClosed += OnItemDescriptionBoardClosed;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        GameEventManager.Instance.InventoryEvents.OnItemUsed -= OnItemUsed;

        GameEventManager.Instance.InventoryEvents.OnInventorySlotLeftClicked -= OnInventorySlotClicked;
        GameEventManager.Instance.InventoryEvents.OnInventoryItemEndDragged -= OnInventoryItemEndDragged;
        GameEventManager.Instance.InventoryEvents.OnItemDescriptionBoardOpened -= OnItemDescriptionBoardOpened;
        GameEventManager.Instance.InventoryEvents.OnItemDescriptionBoardClosed -= OnItemDescriptionBoardClosed;
    }


    private void OnItemUsed(ItemData data)
    {
        _focusedItem = null;
        CloseAllBoards();
    }


    private void OnInventorySlotClicked(InventorySlot slot)
    {
        if (slot == null) return;
        if (slot.currentItem == null) return;


        if (slot.currentItem == _focusedItem)
        {
            Debug.Log("OnInventorySlotClicked + _focusedItem: " + _focusedItem);
            CloseAllBoards();
            _focusedItem = null;
            return;
        }
        _focusedItem = slot.currentItem;
        OpenBoard(slot.currentItem?.itemStack.item);
    }

    private void OnInventoryItemEndDragged(DraggableItem item)
    {
        _focusedItem = item;
        HandleBoardOpening(item.itemStack.item);
    }

    private void OnItemDescriptionBoardOpened(ItemData itemData)
    {
        if (_focusedItem != null) return;
        OpenBoard(itemData);
    }

    private void OnItemDescriptionBoardClosed(ItemData itemData)
    {
        if (_focusedItem) return;
        if (_currentItemData == itemData)
        {
            CloseAllBoards();
        }
    }

    private void HandleBoardOpening(ItemData itemData)
    {
        if (_focusedItem)
        {
            if (_focusedItem.parentSlot is QuickSlot)
            {
                _focusedItem = null;
                CloseAllBoards();
                return;
            }
        }

        if (itemData == null)
        {
            CloseAllBoards();
            return;
        }

        OpenBoard(itemData);
    }

    private void OpenBoard(ItemData itemData)
    {
        CloseAllBoards();

        _currentItemData = itemData;

        if (itemData.itemType == ItemType.Consumable)
        {
            consumableBoard.gameObject.SetActive(true);
            consumableBoard.SettingItemDescriptBoard(itemData);
            consumableBoard.UpdateUseButton(itemData);
        }
        else if (itemData.itemType == ItemType.Equipment)
        {
            equipmentBoard.gameObject.SetActive(true);
            equipmentBoard.SetItemDescript(itemData);
        }
    }

    public void CloseAllBoards()
    {
        consumableBoard.gameObject.SetActive(false);
        equipmentBoard.gameObject.SetActive(false);
        _currentItemData = null;
    }
}


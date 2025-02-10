using System;
using UnityEngine;

public class QuickSlot : InventorySlot
{

    private InventoryEvents _inventoryEvents;
    public DraggableItem currentQuickSlotItem;
    protected override void OnEnable()
    {
        if (_inventoryEvents == null) _inventoryEvents = GameEventManager.Instance.InventoryEvents;

        _inventoryEvents.OnInventoryItemEndDragged += OnInventoryItemEndDragged;
    }


    protected override void OnDisable()
    {
        _inventoryEvents.OnInventoryItemEndDragged -= OnInventoryItemEndDragged;
    }

    protected override void Awake()
    {

    }

    protected override void Update()
    {
    }

    protected override void OnInventorySlotLeftClicked(InventorySlot slot)
    {
        if (slot != this) return;
        if (currentQuickSlotItem == null) return;
    }


    protected override void OnInventoryItemEndDragged(DraggableItem item)
    {
        if(currentQuickSlotItem != null){
            var itemInQuickSlot = GetComponentInChildren<DraggableItem>();
            
            if (itemInQuickSlot == null){
                int weaponType = (int)currentQuickSlotItem.itemStack.item.weaponType;
                string weaponName = currentQuickSlotItem.itemStack.item.itemName;
                WeaponController.UnEquipWeaponModel(weaponType, weaponName);
                currentQuickSlotItem = null;
            }
        }
        if(item.parentSlot != this) return;
        HandleInventoryUI();
        EquipQuickSlotItem(item);
    }

    private void HandleInventoryUI()
    {
        if (currentQuickSlotItem)
        {
            InventoryManager.Instance.DeactiveAllHightlight();
            DescriptionBoardManager.Instance.CloseAllBoards();
        }
    }

    public void EquipQuickSlotItem(DraggableItem item)
    {
        if (currentQuickSlotItem)
        {
            var preWeaponType = (int)currentQuickSlotItem.itemStack.item.weaponType;
            var preWeaponName = currentQuickSlotItem.itemStack.item.itemName;
            WeaponController.UnEquipWeaponModel(preWeaponType, preWeaponName);
            _inventoryEvents.ItemUnequipped(currentQuickSlotItem.itemStack.item);
            currentQuickSlotItem = null;
            Debug.Log("QuickSlot OnInventoryItemEndDragged Unequipped");
        }



        if (item.parentSlot != this) return;

        currentQuickSlotItem = item;
        currentItem = item;
        
        Debug.Log("QuickSlot OnInventoryItemDropped Equipped");
        ClearSlot(); //Crucially, clear the QuickSlot when the item is dropped
        int weaponType = (int)currentQuickSlotItem.itemStack.item.weaponType;
        string weaponName = currentQuickSlotItem.itemStack.item.itemName;
        if (WeaponController)
        {
            WeaponController.EquipWeaponModel(weaponType, weaponName);
        }
        else
        {
            Debug.LogError("WeaponController.Instance is null");
        }


        _inventoryEvents.ItemEquipped(item.itemStack.item);
    }

}

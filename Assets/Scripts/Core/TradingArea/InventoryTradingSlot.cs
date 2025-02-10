using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class InventoryTradingSlot : NetworkBehaviour
{
    public Image ItemIcon;
    public Button Button;

    public enum SlotType
    {
        Inventory,
        PayTrading,
        ReceiveTrading,
    }
    public SlotType slotType;

    public string ItemName { get; private set; }
    public bool HaveItem => ItemIcon.sprite != null;
    public int Index = -1;



    public void Initialize()
    {
        Button.onClick.RemoveAllListeners();
        Button.onClick.AddListener(HandleButtonClicked);
    }

    private void HandleButtonClicked()
    {
        Debug.Log("Button clicked");
        if (!InventoryTradingManager.Instance.IsInTrading) return;

        var inventoryTradingManager = InventoryTradingManager.Instance;

        var ownerId = NetworkManager.LocalClientId;
        var partnerId = NetworkManager.LocalClientId == inventoryTradingManager.CurrentOwnerId
            ? inventoryTradingManager.CurrentPartnerId
            : inventoryTradingManager.CurrentOwnerId;
            
        Debug.Log("OwnerID: " + ownerId + " PartnerID: " + partnerId);

        switch (slotType)
        {
            case SlotType.Inventory:
                inventoryTradingManager.HandleInventorySlotClicked(this, Index, ownerId, partnerId);
                break;
            case SlotType.PayTrading:
                inventoryTradingManager.HandlePaySlotClicked(this, Index, ownerId, partnerId);
                break;
            case SlotType.ReceiveTrading:
                inventoryTradingManager.HandleReceiveSlotClicked(this, Index, ownerId, partnerId);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void SetType(SlotType type)
    {
        slotType = type;
    }
    public void SetIndex(int index)
    {
        Index = index;
    }

    public void SetItem(Sprite sprite, string itemName)
    {
        ItemIcon.sprite = sprite;
        ItemIcon.color = sprite != null ? Color.white : new Color(0, 0, 0, 0);
        ItemName = itemName;
    }
}

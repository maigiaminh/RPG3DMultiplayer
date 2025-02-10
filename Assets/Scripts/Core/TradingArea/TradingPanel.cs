using System;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TradingPanel : NetworkBehaviour
{

    [Header("Trading Slots")]
    public List<InventoryTradingSlot> tradingPaySlots = new List<InventoryTradingSlot>();
    public List<InventoryTradingSlot> tradingReceiveSlots = new List<InventoryTradingSlot>();

    [Header("Trading Buttons & Counter")]
    public TextMeshProUGUI counterText;
    public Button AcceptTradeButton;
    public Button CancelTradeButton;

    [Header("Confirm & Cancel")]
    public GameObject ConfirmOwnerGO;
    public GameObject CancelOwnerGO;
    public GameObject ConfirmPartnerGO;
    public GameObject CancelPartnerGO;

    private PlayerContactUITradingManager _playerContactUITradingManager;
    private InventoryTradingManager _inventoryTradingManager;

    private ulong _currentOwnerId;
    private ulong _currentPartnerId;

    public bool IsFullOwner => tradingPaySlots.TrueForAll(slot => slot.HaveItem);
    public bool IsFullPartner => tradingReceiveSlots.TrueForAll(slot => slot.HaveItem);


    public void ActivatePanel()
    {
        gameObject.SetActive(true);
        ResetInitState();
        int index = 0;
        foreach (InventoryTradingSlot slot in tradingPaySlots)
        {
            slot.SetType(InventoryTradingSlot.SlotType.PayTrading);
            slot.SetIndex(index);
            slot.Initialize();
            index++;
        }
        index = 0;
        foreach (InventoryTradingSlot slot in tradingReceiveSlots)
        {
            slot.SetType(InventoryTradingSlot.SlotType.ReceiveTrading);
            slot.SetIndex(index);
            slot.Initialize();
            index++;
        }
    }

    public void AddItemPaySlot(Sprite sprite, string itemName)
    {
        foreach (InventoryTradingSlot slot in tradingPaySlots)
        {
            if (slot.HaveItem) continue;

            slot.SetItem(sprite, itemName);
            break;
        }
    }

    public void AddItemReceiveSlot(Sprite sprite, string itemName)
    {
        foreach (InventoryTradingSlot slot in tradingReceiveSlots)
        {
            if (slot.HaveItem) continue;

            slot.SetItem(sprite, itemName);
            break;
        }
    }

    public void SetInventoryTradingManager(InventoryTradingManager inventoryTradingManager)
    {
        _inventoryTradingManager = inventoryTradingManager;
    }

    public void Initialize(PlayerContactUITradingManager playerContactUITradingManager, ulong ownerId, ulong partnerId)
    {
        _playerContactUITradingManager = playerContactUITradingManager;

        _currentOwnerId = NetworkManager.LocalClientId;
        _currentPartnerId = ownerId == NetworkManager.LocalClientId ? partnerId : ownerId;

        Optional<Button> optionalAcceptTradeButton = new Optional<Button>(AcceptTradeButton);
        Optional<Button> optionalCancelTradeButton = new Optional<Button>(CancelTradeButton);
        optionalAcceptTradeButton.Match(
            (button) =>
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(HandleAcceptTradeButton);
            },
            () => Debug.LogError("AcceptTradeButton is null")
        );

        optionalCancelTradeButton.Match(
            (button) =>
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(HandleCancelTradeButton);
            },
            () => Debug.LogError("CancelTradeButton is null")
        );


    }

    private void HandleCancelTradeButton()
    {
        _playerContactUITradingManager.HandlePlayerDeclineTradeDeal(_currentOwnerId, _currentPartnerId);
    }

    private void HandleAcceptTradeButton()
    {
        List<FixedString32Bytes> payItems = GetItems(tradingPaySlots);
        List<FixedString32Bytes> receiveItems = GetItems(tradingReceiveSlots);


        _playerContactUITradingManager.HandlePlayerAcceptTradeDeal(_currentOwnerId, _currentPartnerId, payItems, receiveItems);


    }

    public List<FixedString32Bytes> GetItems(List<InventoryTradingSlot> slots)
    {
        List<FixedString32Bytes> items = new List<FixedString32Bytes>();
        foreach (InventoryTradingSlot slot in slots)
        {
            if (!slot.HaveItem) continue;

            items.Add(slot.ItemName);
        }
        return items;
    }


    public void UpdateCounterText(int time)
    {
        counterText.text = time.ToString();
    }


    public void ActivateOwnerCancelGO(bool isActive)
    {
        CancelOwnerGO.SetActive(isActive);
    }

    public void ActivateOwnerConfirmGO(bool isActive)
    {
        ConfirmOwnerGO.SetActive(isActive);
    }

    public void ActivatePartnerCancelGO(bool isActive)
    {
        CancelPartnerGO.SetActive(isActive);
    }

    public void ActivatePartnerConfirmGO(bool isActive)
    {
        ConfirmPartnerGO.SetActive(isActive);
    }

    public void ActivateAcceptTradeButton(bool isActive)
    {
        AcceptTradeButton.interactable = isActive;
    }

    public void ResetInitState()
    {
        ActivateOwnerCancelGO(false);
        ActivateOwnerConfirmGO(false);
        ActivatePartnerCancelGO(false);
        ActivatePartnerConfirmGO(false);

        ActivateAcceptTradeButton(true);
        
        foreach (InventoryTradingSlot slot in tradingPaySlots)
        {
            slot.SetItem(null, "");
        }
        foreach (InventoryTradingSlot slot in tradingReceiveSlots)
        {
            slot.SetItem(null, "");
        }
    }

    public void RemoveItemPaySlot(int index)
    {
        tradingPaySlots[index].SetItem(null, "");
    }

    public void RemoveItemReceiveSlot(int index)
    {
        tradingReceiveSlots[index].SetItem(null, "");
    }

}

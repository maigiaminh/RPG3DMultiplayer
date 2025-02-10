using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class InvitedPlayerPanel : NetworkBehaviour
{
    public TextMeshProUGUI contentText;

    public Button acceptButton;
    public Button declineButton;

    private PlayerContactUITradingManager _playerContactUITradingManager;

    private ulong _currentOwnerId;
    
    private ulong _currentPartnerId;



    public void Initialize(PlayerContactUITradingManager playerContactUITradingManager, ulong ownerId, ulong partnerId)
    {
        _playerContactUITradingManager = playerContactUITradingManager;
        _currentOwnerId = ownerId;
        _currentPartnerId = partnerId;

        if (acceptButton) acceptButton.onClick.RemoveAllListeners();
        if (declineButton) declineButton.onClick.RemoveAllListeners();

        if (acceptButton) acceptButton.onClick.AddListener(AcceptTrade);
        if (declineButton) declineButton.onClick.AddListener(DeclineTrade);
    }

    public void UpdateInvitedPlayerPanel(string content)
    {
        contentText.text = content;

    }

    public void InitializeButtons()
    {

    }


    public void DeclineTrade()
    {
        Debug.Log("DeclineTrade");
        _playerContactUITradingManager.HandlePlayerDeclineToMakeTrading(_currentOwnerId, _currentPartnerId);
    }

    public void AcceptTrade()
    {
        Debug.Log("AcceptTrade");
        _playerContactUITradingManager.HandlePlayerAcceptToMakeTrading(_currentOwnerId, _currentPartnerId);
    }
}

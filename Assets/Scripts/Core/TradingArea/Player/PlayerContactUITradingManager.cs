using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerContactUITradingManager : NetworkBehaviour, IDisposable
{
    public static PlayerContactUITradingManager Instance;

    public AleartPlayerNearbyPanel AleartPlayerNearbyPanel;
    public InvitedPlayerPanel InvitedPlayerPanel;
    public InvitedPlayerPanel WaitingPlayerPanel;

    public Panel1T WarningPanel;
    public Button QuitTradingZoneBtn;



    public int TradingExistTime = 45;
    public int TradingCancelTime = 5;

    private Dictionary<KeyValuePair<ulong, ulong>, int> _tradingTimeDic = new Dictionary<KeyValuePair<ulong, ulong>, int>();

    private Dictionary<KeyValuePair<ulong, ulong>, Coroutine> _tradingTimerCoroutineDic = new Dictionary<KeyValuePair<ulong, ulong>, Coroutine>();

    private Dictionary<ulong, NetworkObject> _playerNetworkObjectDic = new Dictionary<ulong, NetworkObject>();

    private Dictionary<ulong, bool> _acceptTradeDic = new Dictionary<ulong, bool>();


    public event Action<ulong, ulong> OnPlayerAcceptToMakeTrading;
    public void HandlePlayerAcceptToMakeTrading(ulong first, ulong second)
    {
        OnPlayerAcceptToMakeTrading?.Invoke(first, second);
    }
    public event Action<ulong, ulong> OnPlayerDeclineToMakeTrading;
    public void HandlePlayerDeclineToMakeTrading(ulong first, ulong second)
    {
        OnPlayerDeclineToMakeTrading?.Invoke(first, second);
    }
    public event Action<ulong, ulong, List<FixedString32Bytes>, List<FixedString32Bytes>> OnPlayerAcceptTradeDeal;
    public void HandlePlayerAcceptTradeDeal(ulong first, ulong second, List<FixedString32Bytes> payItems, List<FixedString32Bytes> receiveItems)
    {
        OnPlayerAcceptTradeDeal?.Invoke(first, second, payItems, receiveItems);
    }

    public event Action<ulong, ulong> OnPlayerDeclineTradeDeal;
    public void HandlePlayerDeclineTradeDeal(ulong first, ulong second)
    {
        OnPlayerDeclineTradeDeal?.Invoke(first, second);
    }

    public event Action<bool, ulong, ulong, List<FixedString32Bytes>, List<FixedString32Bytes>> OnPlayerCompletedTrade;
    public void HandlePlayerCompletedTrade(bool isDeal, ulong ownerId, ulong partnerId, List<FixedString32Bytes> payItems, List<FixedString32Bytes> receiveItems)
    {
        OnPlayerCompletedTrade?.Invoke(isDeal, ownerId, partnerId, payItems, receiveItems);
    }
    public event Action<int, ulong, ulong> OnTradingCounterChange;
    public void HandleTradingCounterChange(int currentTime, ulong ownerId, ulong partnerId)
    {
        OnTradingCounterChange?.Invoke(currentTime, ownerId, partnerId);
    }






    private void Awake()
    {
        // Đảm bảo chỉ có một Instance duy nhất
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // Xóa các instance dư thừa
            return;
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer) HostSingleton.Instance.HostManager.NetworkServer.OnClientLeft += HandleClientLeft;
        OnPlayerAcceptToMakeTrading += AcceptTradingServerRpc;
        OnPlayerDeclineToMakeTrading += CancelTradingServerRpc;
        OnPlayerAcceptTradeDeal += PlayerAcceptTradeDealServerRpc;
        OnPlayerDeclineTradeDeal += PlayerDeclineTradeDealServerRpc;
        OnTradingCounterChange += HandleTimerChangedServerRpc;
        OnPlayerCompletedTrade += HandleTradeCompletedServerRpc;
        QuitTradingZoneBtn.onClick.AddListener(HandlePlayerQuit);
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (IsServer) HostSingleton.Instance.HostManager.NetworkServer.OnClientLeft -= HandleClientLeft;
        OnPlayerAcceptToMakeTrading -= AcceptTradingServerRpc;
        OnPlayerDeclineToMakeTrading -= CancelTradingServerRpc;
        OnPlayerAcceptTradeDeal -= PlayerAcceptTradeDealServerRpc;
        OnPlayerDeclineTradeDeal -= PlayerDeclineTradeDealServerRpc;
        OnTradingCounterChange -= HandleTimerChangedServerRpc;
        OnPlayerCompletedTrade -= HandleTradeCompletedServerRpc;

    }


    public override void OnDestroy()
    {
        base.OnDestroy();
        Dispose();
    }




    #region Warning Panel

    [ServerRpc(RequireOwnership = false)]
    public void ActivateWarningPanelServerRpc(bool isActive, ulong ownerId, ulong partnerId, string ownerName, string partnerName, bool isOwnerInTrading, bool isPartnerInTrading)
    {
        ActivateWarningPanelClientRpc(isActive, ownerId, partnerId, ownerName, partnerName, isOwnerInTrading, isPartnerInTrading);
    }


    [ClientRpc]
    private void ActivateWarningPanelClientRpc(bool isActive, ulong ownerId, ulong partnerId, string ownerName, string partnerName, bool isOwnerInTrading, bool isPartnerInTrading)
    {
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsClient)
        {
            Debug.LogWarning("NetworkManager is null or not a client. Skipping Dispose.");
            return;
        }
        if (NetworkManager.LocalClientId == ownerId)
        {
            string content = isOwnerInTrading ?
                            $"You are in trading." :
                            $"{partnerName} is in trading with another player.";

            ActivateWarningPanel(isActive, content);
        }

    }

    private void ActivateWarningPanel(bool isActive, string content)
    {
        WarningPanel.gameObject.SetActive(false);
        WarningPanel.gameObject.SetActive(isActive);
        WarningPanel.UpdatePanelText(content);
    }


    #endregion

    #region  Alert Panel

    [ServerRpc(RequireOwnership = false)]
    public void ActivateAlertPanelServerRpc(bool isActive, ulong ownerId, string partnerName) => ActivateAlertPanelClientRpc(isActive, ownerId, partnerName);

    [ClientRpc]
    private void ActivateAlertPanelClientRpc(bool isActive, ulong ownerId, string partnerName)
    {
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsClient)
        {
            Debug.LogWarning("NetworkManager is null or not a client. Skipping Dispose.");
            return;
        }
        if (ownerId != NetworkManager.LocalClientId) return;
        ActivateAlertPanel(isActive, partnerName);
    }

    public void ActivateAlertPanel(bool isActive, string partnerName)
    {
        if (AleartPlayerNearbyPanel == null)
        {
            Debug.LogError("AleartPlayerNearbyPanel is not assigned in the inspector.");
            return;
        }
        AleartPlayerNearbyPanel.gameObject.SetActive(isActive);
        AleartPlayerNearbyPanel.UpdateAleartPlayerNearbyPanel(partnerName);
    }

    #endregion

    #region Invited Panel

    [ClientRpc]
    private void ActivateInvitedPanelClientRpc(bool isActive, ulong ownerId, ulong partnerId, string partnerName)
    {
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsClient)
        {
            Debug.LogWarning("NetworkManager is null or not a client. Skipping Dispose.");
            return;
        }
        if (NetworkManager.LocalClientId != partnerId) return;

        ActivateInvitedPanel(isActive, ownerId, partnerId, partnerName);
    }

    public void ActivateInvitedPanel(bool isActive, ulong ownerId, ulong partnerId, string partnerName)
    {
        if (InvitedPlayerPanel == null)
        {
            Debug.LogError("InvitedPlayerPanel is not assigned in the inspector.");
            return;
        }
        string content = $"Player {partnerName} wants to trade with you.";

        InvitedPlayerPanel.gameObject.SetActive(isActive);

        if (!isActive) return;

        InvitedPlayerPanel.UpdateInvitedPlayerPanel(content);
        InvitedPlayerPanel.Initialize(this, ownerId, partnerId);
    }

    #endregion

    #region Waiting Panel



    [ClientRpc]
    private void ActivateWaitingPanelClientRpc(bool isActive, ulong ownerId, ulong partnerId, string parterName)
    {
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsClient)
        {
            Debug.LogWarning("NetworkManager is null or not a client. Skipping Dispose.");
            return;
        }
        if (NetworkManager.LocalClientId != ownerId) return;
        ActivateWaitingPanel(isActive, ownerId, partnerId, parterName);
    }

    public void ActivateWaitingPanel(bool isActive, ulong ownerId, ulong partnerId, string partnerName)
    {
        if (WaitingPlayerPanel == null)
        {
            Debug.LogError("InvitedPlayerPanel is not assigned in the inspector.");
            return;
        }

        string content = $"Waiting for the player {partnerName} accept the trade in {15} seconds...";
        WaitingPlayerPanel.gameObject.SetActive(isActive);

        if (!isActive) return;

        WaitingPlayerPanel.UpdateInvitedPlayerPanel(content);
        WaitingPlayerPanel.Initialize(this, ownerId, partnerId);
    }
    #endregion

    #region Request & Accept & Cancel To Make Trading

    [ServerRpc(RequireOwnership = false)]
    public void RequestTradingServerRpc(ulong ownerId, ulong partnerId, string ownerName, string partnerName, ulong ownerObjectId, ulong partnerObjectId)
    {
        NetworkObject ownerNetworkObject = NetworkManager.SpawnManager.SpawnedObjects[ownerObjectId];
        NetworkObject partnerNetworkObject = NetworkManager.SpawnManager.SpawnedObjects[partnerObjectId];

        PlayerInTradingDetector ownerDetector = ownerNetworkObject.GetComponent<PlayerInTradingDetector>();
        PlayerInTradingDetector partnerDetector = partnerNetworkObject.GetComponent<PlayerInTradingDetector>();

        _playerNetworkObjectDic.Add(ownerId, ownerNetworkObject);
        _playerNetworkObjectDic.Add(partnerId, partnerNetworkObject);

        if (ownerDetector.IsInTrading.Value || partnerDetector.IsInTrading.Value)
        {
            Debug.Log("One of the players is in trading");
            ActivateWarningPanelServerRpc(true, ownerId, partnerId, ownerName, partnerName, ownerDetector.IsInTrading.Value, partnerDetector.IsInTrading.Value);
            return;
        }

        ActivateWaitingPanelClientRpc(true, ownerId, partnerId, partnerName);
        ActivateInvitedPanelClientRpc(true, ownerId, partnerId, ownerName);
    }


    [ServerRpc(RequireOwnership = false)]
    private void AcceptTradingServerRpc(ulong ownerId, ulong partnerId)
    {
        ActivateWaitingPanelClientRpc(false, ownerId, partnerId, "");
        ActivateInvitedPanelClientRpc(false, ownerId, partnerId, "");

        ActivateInventoryInTradingPanelClientRpc(true, ownerId, partnerId);
        ActivateTradingPanelClientRpc(true, ownerId, partnerId);

        KeyValuePair<ulong, ulong> key = new KeyValuePair<ulong, ulong>(ownerId, partnerId);



        if (_tradingTimerCoroutineDic.ContainsKey(key))
        {
            StopCoroutine(_tradingTimerCoroutineDic[key]);

            _tradingTimeDic.Remove(key);
            _tradingTimerCoroutineDic.Remove(key);
        }

        _tradingTimeDic.Add(key, TradingExistTime);
        _tradingTimerCoroutineDic.Add(key, StartCoroutine(TradingExistTimer(key, ownerId, partnerId)));

        _acceptTradeDic.Add(ownerId, false);
        _acceptTradeDic.Add(partnerId, false);


        Debug.Log("Trading is accepted");
    }



    IEnumerator TradingExistTimer(KeyValuePair<ulong, ulong> key, ulong ownerId, ulong partnerId)
    {
        WaitForSeconds wait = new WaitForSeconds(1f);
        while (_tradingTimeDic[key] > 0)
        {
            _tradingTimeDic[key] -= 1;
            yield return wait;

            HandleTradingCounterChange(_tradingTimeDic[key], ownerId, partnerId);
        }

        HandlePlayerCompletedTrade(false, ownerId, partnerId, null, null);
    }

    [ClientRpc]
    private void ActivateInventoryInTradingPanelClientRpc(bool isActive, ulong ownerId, ulong partnerId)
    {
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsClient)
        {
            Debug.LogWarning("NetworkManager is null or not a client. Skipping Dispose.");
            return;
        }
        if (NeitherOwnerNorPartner(ownerId, partnerId)) return;

        Debug.Log("Activate Inventory In Trading Panel + " + ownerId + " " + partnerId);
        InventoryTradingManager inventoryTradingManager = InventoryTradingManager.Instance;
        if (inventoryTradingManager == null) 
            inventoryTradingManager = FindAnyObjectByType<InventoryTradingManager>();


        inventoryTradingManager.SetOwnerIdAndPartnerId(ownerId, partnerId);
        inventoryTradingManager.IsInTrading = true;
        inventoryTradingManager.ActivateInventoryPanelServerRpc(NetworkManager.LocalClientId);
    }
    [ClientRpc]
    private void ActivateTradingPanelClientRpc(bool isActive, ulong ownerId, ulong partnerId)
    {
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsClient)
        {
            Debug.LogWarning("NetworkManager is null or not a client. Skipping Dispose.");
            return;
        }
        if (NeitherOwnerNorPartner(ownerId, partnerId)) return;

        InventoryTradingManager inventoryTradingManager = InventoryTradingManager.Instance;
        if (inventoryTradingManager == null)
        {
            inventoryTradingManager = FindAnyObjectByType<InventoryTradingManager>();
        }
        if (NetworkManager.LocalClientId == ownerId)
        {
            inventoryTradingManager.TradingPanel.Initialize(this, ownerId, partnerId);
        }
        if (NetworkManager.LocalClientId == partnerId)
        {
            inventoryTradingManager.TradingPanel.Initialize(this, partnerId, ownerId);
        }
        inventoryTradingManager.TradingPanel.ActivatePanel();
    }



    [ServerRpc(RequireOwnership = false)]
    public void CancelTradingServerRpc(ulong ownerId, ulong partnerId)
    {
        ActivateWaitingPanelClientRpc(false, ownerId, partnerId, "");
        ActivateInvitedPanelClientRpc(false, ownerId, partnerId, "");

        _playerNetworkObjectDic.Remove(ownerId);
        _playerNetworkObjectDic.Remove(partnerId);

        Debug.Log("Trading is canceled");
    }

    #endregion

    #region Accept & Cancel Trade Deal

    [ServerRpc(RequireOwnership = false)]
    private void PlayerAcceptTradeDealServerRpc(ulong ownerId, ulong partnerId, List<FixedString32Bytes> payItems, List<FixedString32Bytes> receiveItems)
    {
        _acceptTradeDic[ownerId] = true;
        if (_acceptTradeDic[partnerId])
        {
            HandlePlayerCompletedTrade(true, ownerId, partnerId, payItems, receiveItems);
        }
        PlayerAcceptTradeDealClientRpc(ownerId, partnerId);
    }

    [ClientRpc]
    private void PlayerAcceptTradeDealClientRpc(ulong ownerId, ulong partnerId)
    {
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsClient)
        {
            Debug.LogWarning("NetworkManager is null or not a client. Skipping Dispose.");
            return;
        }
        if (NeitherOwnerNorPartner(ownerId, partnerId)) return;

        InventoryTradingManager inventoryTradingManager = InventoryTradingManager.Instance;
        if (inventoryTradingManager == null)
        {
            inventoryTradingManager = FindAnyObjectByType<InventoryTradingManager>();
        }

        if (ownerId == NetworkManager.LocalClientId)
        {
            inventoryTradingManager.TradingPanel.ActivateOwnerConfirmGO(true);
        }
        if (partnerId == NetworkManager.LocalClientId)
        {
            inventoryTradingManager.TradingPanel.ActivatePartnerConfirmGO(true);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void PlayerDeclineTradeDealServerRpc(ulong ownerId, ulong partnerId)
    {
        KeyValuePair<ulong, ulong> key1 = new KeyValuePair<ulong, ulong>(ownerId, partnerId);
        KeyValuePair<ulong, ulong> key2 = new KeyValuePair<ulong, ulong>(partnerId, ownerId);
        if (_tradingTimeDic.ContainsKey(key1)) _tradingTimeDic[key1] = TradingCancelTime;
        else if (_tradingTimeDic.ContainsKey(key2)) _tradingTimeDic[key2] = TradingCancelTime;


        PlayerDeclineTradeDealClientRpc(ownerId, partnerId);


        Debug.Log("Player decline trade deal server rpc + " + ownerId + " " + partnerId);
    }

    [ClientRpc]
    private void PlayerDeclineTradeDealClientRpc(ulong ownerId, ulong partnerId)
    {
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsClient)
        {
            Debug.LogWarning("NetworkManager is null or not a client. Skipping Dispose.");
            return;
        }
        InventoryTradingManager inventoryTradingManager = InventoryTradingManager.Instance;
        if (inventoryTradingManager == null)
        {
            inventoryTradingManager = FindAnyObjectByType<InventoryTradingManager>();
        }

        inventoryTradingManager.TradingPanel.ActivateAcceptTradeButton(false);

        if (NetworkManager.LocalClientId == ownerId)
        {
            inventoryTradingManager.TradingPanel.ActivateOwnerCancelGO(true);
        }

        if (NetworkManager.LocalClientId == partnerId)
        {
            inventoryTradingManager.TradingPanel.ActivatePartnerCancelGO(true);
        }
    }

    #endregion

    #region Trade Completed
    [ServerRpc(RequireOwnership = false)]
    private void HandleTradeCompletedServerRpc(bool isDeal, ulong ownerId, ulong partnerId, List<FixedString32Bytes> payItems, List<FixedString32Bytes> receiveItems)
    {
        RemoveAllOwnerPartnerAttr(ownerId, partnerId);
        HandleTradeCompletedClientRpc(isDeal, ownerId, partnerId, payItems, receiveItems);
    }

    [ClientRpc]
    private void HandleTradeCompletedClientRpc(bool isDeal, ulong ownerId, ulong partnerId, List<FixedString32Bytes> payItems, List<FixedString32Bytes> receiveItems)
    {
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsClient)
        {
            Debug.LogWarning("NetworkManager is null or not a client. Skipping Dispose.");
            return;
        }
        if (NeitherOwnerNorPartner(ownerId, partnerId)) return;

        if (!NetworkManager.IsConnectedClient) return;


        InventoryTradingManager inventoryTradingManager = InventoryTradingManager.Instance;
        if (inventoryTradingManager == null)
        {
            inventoryTradingManager = FindAnyObjectByType<InventoryTradingManager>();
        }
        InventoryContainer inventoryContainer = InventoryContainer.Instance;
        if (inventoryContainer == null)
        {
            inventoryContainer = FindAnyObjectByType<InventoryContainer>();
        }

        inventoryTradingManager.IsInTrading = false;
        inventoryTradingManager.TradingPanel.gameObject.SetActive(false);
        inventoryTradingManager.InventoryTradingPanel.gameObject.SetActive(false);

        if (isDeal == false)
        {
            Debug.Log("Trade is canceled");
            return;
        }

        if (inventoryTradingManager == null)
        {
            Debug.LogError("InventoryTradingManager is null");
            return;
        }

        if (inventoryContainer == null)
        {
            Debug.LogError("InventoryContainer is null");
            return;
        }

        var receive = NetworkManager.LocalClientId == ownerId ? receiveItems : payItems;
        var pay = NetworkManager.LocalClientId == ownerId ? payItems : receiveItems;



        foreach (var item in pay)
        {
            ItemData itemData = null;
            if (InventoryContainer.Instance.ResourceItemMap.ContainsKey(item.ToString()))
            {
                itemData = InventoryContainer.Instance.ResourceItemMap[item.ToString()];
            }
            else
            {
                Debug.LogError("Item is not found in the inventory container");
            }
            if (inventoryContainer.PlayerItemMap.ContainsKey(itemData))
            {
                inventoryContainer.PlayerItemMap.Remove(InventoryContainer.Instance.ResourceItemMap[item.ToString()]);
            }
            inventoryTradingManager.InventoryTradingPanel.UpdateItemMap();

        }
        foreach (var item in receive)
        {
            ItemData itemData = InventoryContainer.Instance.ResourceItemMap[item.ToString()];
            if (inventoryContainer.PlayerItemMap.ContainsKey(itemData))
            {
                inventoryContainer.PlayerItemMap[InventoryContainer.Instance.ResourceItemMap[item.ToString()]] += 1;
            }
            else inventoryContainer.PlayerItemMap.Add(itemData, 1);
            inventoryTradingManager.InventoryTradingPanel.UpdateItemMap();
        }


        inventoryContainer.UpdateItemFirebase();
        Debug.Log("Trade is completed");

    }

    #endregion




    #region Trading Timer Changed


    [ServerRpc(RequireOwnership = false)]

    private void HandleTimerChangedServerRpc(int currentTime, ulong ownerId, ulong partnerId)
    {
        UpdatCounterTextClientRpc(currentTime, ownerId, partnerId);


    }

    [ClientRpc]
    private void UpdatCounterTextClientRpc(int currentTime, ulong ownerId, ulong partnerId)
    {
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsClient)
        {
            Debug.LogWarning("NetworkManager is null or not a client. Skipping Dispose.");
            return;
        }
        if (NeitherOwnerNorPartner(ownerId, partnerId)) return;

        InventoryTradingManager.Instance.TradingPanel.UpdateCounterText(currentTime);


    }

    #endregion

    #region Other

    public bool NeitherOwnerNorPartner(ulong ownerId, ulong partnerId)
    {
        return NetworkManager.LocalClientId != ownerId && NetworkManager.LocalClientId != partnerId;
    }

    public void RemoveAllOwnerPartnerAttr(ulong ownerId, ulong partnerId)
    {
        _acceptTradeDic.Remove(ownerId);
        _acceptTradeDic.Remove(partnerId);
        var key1 = new KeyValuePair<ulong, ulong>(ownerId, partnerId);
        var key2 = new KeyValuePair<ulong, ulong>(partnerId, ownerId);
        if (_tradingTimeDic.ContainsKey(key1)) _tradingTimeDic.Remove(key1);
        if (_tradingTimeDic.ContainsKey(key2)) _tradingTimeDic.Remove(key2);
        if (_tradingTimerCoroutineDic.ContainsKey(key1))
        {
            StopCoroutine(_tradingTimerCoroutineDic[key1]);
            _tradingTimeDic.Remove(key1);
        }
        if (_tradingTimerCoroutineDic.ContainsKey(key2))
        {
            StopCoroutine(_tradingTimerCoroutineDic[key2]);
            _tradingTimeDic.Remove(key2);
        }



        NetworkObject ownerNetworkObject = _playerNetworkObjectDic[ownerId];
        NetworkObject partnerNetworkObject = _playerNetworkObjectDic[partnerId];

        if (ownerNetworkObject)
        {
            PlayerInTradingDetector ownerDetector = ownerNetworkObject.GetComponent<PlayerInTradingDetector>();
            if (ownerDetector) ownerDetector.IsInTrading.Value = false;
        }
        if (partnerNetworkObject)
        {
            PlayerInTradingDetector partnerDetector = partnerNetworkObject.GetComponent<PlayerInTradingDetector>();
            if (partnerDetector) partnerDetector.IsInTrading.Value = false;
        }
        _playerNetworkObjectDic.Remove(ownerId);
        _playerNetworkObjectDic.Remove(partnerId);
    }


    private void HandlePlayerQuit()
    {
        QuitServerRpc(NetworkManager.LocalClientId);
    }


    [ServerRpc(RequireOwnership = false)]
    private void QuitServerRpc(ulong clientId)
    {
        if (clientId == 0) QuitFromHost();
        else QuitFromClient(clientId);
    }


    private void QuitFromHost()
    {
        HostSingleton.Instance.HostManager.Dispose();
    }
    private void QuitFromClient(ulong clientId) => NetworkManager.Singleton.DisconnectClient(clientId);


    public ulong GetPartnerId(ulong ownerId)
    {
        foreach (var keyValuePair in _tradingTimeDic.Keys)
        {
            if (keyValuePair.Key == ownerId) return keyValuePair.Value;
            if (keyValuePair.Value == ownerId) return keyValuePair.Key;
        }
        return 0;
    }


    #endregion

    #region Handle Client Left


    private void HandleClientLeft(string authId, ulong clientId)
    {
        HandleTradeCompletedServerRpc(false, clientId, GetPartnerId(clientId), null, null);
    }

    public void Dispose()
    {
        foreach (var key in _tradingTimerCoroutineDic.Keys)
        {
            StopCoroutine(_tradingTimerCoroutineDic[key]);
        }
        _tradingTimerCoroutineDic.Clear();
        _tradingTimeDic.Clear();
        _playerNetworkObjectDic.Clear();
        _acceptTradeDic.Clear();
    }

    #endregion

}




public static class FixedStringListSerialization
{
    public static void WriteValueSafe(this FastBufferWriter writer, in List<FixedString32Bytes> list)
    {
        writer.WriteValueSafe(list.Count); // Ghi số lượng phần tử trong danh sách
        foreach (var item in list)
        {
            writer.WriteValueSafe(item); // Ghi từng phần tử
        }
    }

    public static void ReadValueSafe(this FastBufferReader reader, out List<FixedString32Bytes> list)
    {
        list = new List<FixedString32Bytes>();
        reader.ReadValueSafe(out int count); // Đọc số lượng phần tử
        for (int i = 0; i < count; i++)
        {
            reader.ReadValueSafe(out FixedString32Bytes item); // Đọc từng phần tử
            list.Add(item);
        }
    }

}

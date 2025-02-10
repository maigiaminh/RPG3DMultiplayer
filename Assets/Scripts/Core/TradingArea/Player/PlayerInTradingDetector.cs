using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System;
using NUnit.Framework;
using System.Collections;

public class PlayerInTradingDetector : NetworkBehaviour
{
    private List<Transform> _playerNearby = new List<Transform>();

    public PlayerNetworkTank NearestPlayerTank;

    private PlayerContactUITradingManager _playerContactUITradingManager;

    public NetworkVariable<bool> IsInTrading = new NetworkVariable<bool>();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        StartCoroutine(InitConfigSingleton());
        Debug.Log("OwnerClientId: " + OwnerClientId);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        if (_playerContactUITradingManager != null) _playerContactUITradingManager = null;
        if (_playerNearby != null) _playerNearby.Clear();
        if (NearestPlayerTank != null) NearestPlayerTank = null;

        StopAllCoroutines();
    }

    private void Update()
    {
        if (!IsOwner) return;
        if (_playerContactUITradingManager == null) return;
        if (_playerNearby.Count == 0)
        {
            NearestPlayerTank = null;
            _playerContactUITradingManager.ActivateAlertPanelServerRpc(false, OwnerClientId, "");
            return;
        }

        NearestPlayerTank = FindNearestPlayerName();
        if (NearestPlayerTank != null) _playerContactUITradingManager.ActivateAlertPanelServerRpc(true, OwnerClientId, NearestPlayerTank.Username.Value.ToString());

        if (Input.GetKeyDown(KeyCode.B))
        {
            ChangeTradingStateServerRpc();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (NearestPlayerTank == null) return;
            Debug.Log("E pressed");
            var ownerName = GetComponent<PlayerNetworkTank>().Username.Value.ToString();
            _playerContactUITradingManager.RequestTradingServerRpc(OwnerClientId, NearestPlayerTank.OwnerClientId,
                                                                    ownerName, NearestPlayerTank.Username.Value.ToString(),
                                                                    GetComponent<NetworkObject>().NetworkObjectId,
                                                                    NearestPlayerTank.GetComponent<NetworkObject>().NetworkObjectId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangeTradingStateServerRpc()
    {
        IsInTrading.Value = true;
    }

    private PlayerNetworkTank FindNearestPlayerName()
    {
        Transform nearestPlayer = null;
        float minDistance = float.MaxValue;
        foreach (var player in _playerNearby)
        {
            if (player == null) continue;
            float distance = Vector3.Distance(player.position, transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestPlayer = player;
            }
        }
        return nearestPlayer != null ? nearestPlayer.GetComponent<PlayerNetworkTank>() : null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsOwner) return;
        if (!other.CompareTag("Player")) return;  // Kiểm tra đối tượng va chạm có phải là Player không
        if (_playerNearby.Contains(other.transform)) return;  // Tránh thêm trùng lặp

        Debug.Log(
            $"Player {other.GetComponent<PlayerNetworkTank>().Username.Value} is nearby"
        );
        _playerNearby.Add(other.transform);

    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsOwner) return;
        if (!other.CompareTag("Player")) return;  // Kiểm tra đối tượng va chạm có phải là Player không
        if (!_playerNearby.Contains(other.transform)) return;  // Tránh xóa đối tượng không có trong danh sách

        _playerNearby.Remove(other.transform);

    }

    private IEnumerator InitConfigSingleton()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);
        while (PlayerContactUITradingManager.Instance == null)
        {
            yield return wait;
        }

        _playerContactUITradingManager = FindPlayerContactUITradingManager();
    }

    public PlayerContactUITradingManager FindPlayerContactUITradingManager()
    {
        var playerContactUITradingManager = PlayerContactUITradingManager.Instance;
        if (playerContactUITradingManager == null)
        {
            playerContactUITradingManager = FindAnyObjectByType<PlayerContactUITradingManager>();
        }

        if (playerContactUITradingManager == null)
        {
            Debug.LogError("PlayerContactUITradingManager is not found.");
        }

        return playerContactUITradingManager;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TradingAreaUIManager : NetworkBehaviour
{
    public static TradingAreaUIManager Instance { get; private set; }
    public NetworkList<PlayerInfo> PlayerInfos = new NetworkList<PlayerInfo>();

    public PlayerInRoomPanel playerInRoomPanel;

    public event Action<ulong> OnClientEnter;

    public enum PlayerInfoChangeState
    {
        PlayerEnter,
        PlayerExit
    }

    public void HandleClientEnter(ulong clientId)
    {
        HandlePlayerEnterServerRpc(clientId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void HandlePlayerEnterServerRpc(ulong clientId)
    {
        OnClientEnter?.Invoke(clientId);
    }
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one Network Spawn Manager in the scene.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            PlayerInfos.OnListChanged += HandlePlayerInfoChanged;
            HostSingleton.Instance.HostManager.NetworkServer.OnClientLeft += HandlePlayerExit;
            OnClientEnter += HandlePlayerEnter;
        }

    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            PlayerInfos.OnListChanged -= HandlePlayerInfoChanged;
            HostSingleton.Instance.HostManager.NetworkServer.OnClientLeft -= HandlePlayerExit;
            OnClientEnter -= HandlePlayerEnter;

            for (int i = 0; i < PlayerInfos.Count; i++)
            {
                PlayerInfos.RemoveAt(i);
            }
        }
    }


    private void HandlePlayerExit(string authId, ulong clientId) => UpdatePlayerInfo(clientId, PlayerInfoChangeState.PlayerExit);
    private void HandlePlayerEnter(ulong clientId) => UpdatePlayerInfo(clientId, PlayerInfoChangeState.PlayerEnter);



    private void UpdatePlayerInfo(ulong clientId, PlayerInfoChangeState state)
    {
        if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var client))
        {
            Debug.LogError($"Client {clientId} not found in ConnectedClients.");
        }
        Debug.Log("PlayerInfo changed on server in UpdatePlayerInfo.");
        switch (state)
        {
            case PlayerInfoChangeState.PlayerEnter:
                AddPlayerInfo(client.PlayerObject);
                break;
            case PlayerInfoChangeState.PlayerExit:
                RemovePlayerInfo(clientId);
                break;
        }
    }

    private void RemovePlayerInfo(ulong clientId)
    {
        if (IsServer)
        {
            int indexToRemove = -1;
            for (int i = 0; i < PlayerInfos.Count; i++)
            {
                if (PlayerInfos[i].PlayerId == clientId)
                {
                    indexToRemove = i;
                    break;
                }
            }
            if (indexToRemove >= 0)
            {
                Debug.Log("PlayerInfo changed on server in RemovePlayerInfo. + " + PlayerInfos.Count);
                PlayerInfos.RemoveAt(indexToRemove);
            }

            PlayerInfo[] temp = new PlayerInfo[PlayerInfos.Count];

            for (int i = 0; i < PlayerInfos.Count; i++)
            {
                temp[i] = PlayerInfos[i];
            }

            UpdatePanelUIServerRpc(temp);
        }
    }

    private void AddPlayerInfo(NetworkObject playerObject)
    {
        if (!playerObject.TryGetComponent(out PlayerNetworkTank player))
        {
            Debug.LogError($"PlayerNetworkTank component not found on PlayerObject for Client.");
            return;
        }

        if (IsServer)
        {
            PlayerInfos.Add(new PlayerInfo
            {
                PlayerId = player.OwnerClientId,
                PlayerName = player.Username.Value.ToString(),
                PlayerLevel = player.Level.Value
            });
        }

        Debug.Log($"Player {player.Username.Value} entered the room.");
    }

    private void HandlePlayerInfoChanged(NetworkListEvent<PlayerInfo> changeEvent)
    {
        PlayerInfo[] temp = new PlayerInfo[PlayerInfos.Count];
        for (int i = 0; i < PlayerInfos.Count; i++)
        {
            temp[i] = PlayerInfos[i];
        }
        UpdatePanelUIServerRpc(temp);
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdatePanelUIServerRpc(PlayerInfo[] playerInfos)
    {
        Debug.Log($"UpdatePanelUIServerRpc called. Player count: {playerInfos.Length}");
        playerInRoomPanel.UpdatePlayerInRoomItems(playerInfos);
        UpdatePanelUIClientRpc(playerInfos);
    }

    [ClientRpc]
    private void UpdatePanelUIClientRpc(PlayerInfo[] playerInfos)
    {
        Debug.Log($"UpdatePanelUIClientRpc called. Player count: {playerInfos.Length}");
        playerInRoomPanel.UpdatePlayerInRoomItems(playerInfos);
    }
}


[Serializable]
public struct PlayerInfo : INetworkSerializable, IEquatable<PlayerInfo>
{
    public ulong PlayerId;
    public FixedString32Bytes PlayerName;
    public int PlayerLevel;

    public bool Equals(PlayerInfo other)
    {
        return PlayerName.Equals(other.PlayerName) && PlayerLevel == other.PlayerLevel;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref PlayerName);
        serializer.SerializeValue(ref PlayerLevel);
    }
}

public static class PlayerInfoListSerialization
{
    public static void WriteValueSafe(this FastBufferWriter writer, in List<PlayerInfo> list)
    {
        writer.WriteValueSafe(list.Count);
        foreach (var item in list)
        {
            writer.WriteValueSafe(item);
        }
    }

    public static void ReadValueSafe(this FastBufferReader reader, out List<PlayerInfo> list)
    {
        list = new List<PlayerInfo>();
        reader.ReadValueSafe(out int count);
        for (int i = 0; i < count; i++)
        {
            reader.ReadValueSafe(out PlayerInfo item);
            list.Add(item);
        }
    }
}
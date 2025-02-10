using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class ClientManager : IDisposable
{
    private JoinAllocation _allocation;
    private NetworkClient _networkClient;



    public async Task<bool> InitAsync()
    {
        await UnityServices.InitializeAsync(new InitializationOptions()
                        .SetEnvironmentName("production"));

        _networkClient = new NetworkClient(NetworkManager.Singleton);

        var authState = await AuthenticationWrapper.DoAuth();

        return authState == AuthState.Authenticated;
    }

    public async Task StartClientAsync(string joinCode)
    {
        Debug.Log("Joining allocation with join code: " + joinCode + "...");
        try
        {
            _allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            // Kiểm tra _allocation
            if (_allocation == null)
            {
                Debug.LogError("Failed to join allocation: Allocation is null.");
                return;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to join allocation: {ex.Message}");
            Debug.LogError($"Stack Trace: {ex.StackTrace}");
            return;
        }
        UnityTransport unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        Debug.Log("Relay server data: " + _allocation.RelayServer.IpV4 + ":" + _allocation.RelayServer.Port);
        Debug.Log("Allocation ID: " + _allocation.AllocationIdBytes);
        Debug.Log("Connection data: " + _allocation.ConnectionData);
        Debug.Log("Key : " + _allocation.Key);

        RelayServerData relayServerData = new RelayServerData(
            _allocation, "dtls"
        );

        unityTransport.SetRelayServerData(relayServerData);

        if (UserContainer.Instance)
        {
            UserContainer.Instance.SetUserAuthId(AuthenticationService.Instance.PlayerId);
            UserData userData = UserContainer.Instance.UserData;

            string payload = JsonUtility.ToJson(userData);
            byte[] payloadBytes = System.Text.Encoding.UTF8.GetBytes(payload);

            NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;
        }

        Debug.Log("Starting client...");
        // if (!NetworkManager.Singleton.IsClient)
        // {
        //     Debug.LogError("Failed to start the client!");
        //     return;
        // }
        NetworkManager.Singleton.StartClient();
    }

    public void Dispose()
    {
        _networkClient?.Dispose();
    }

    public void ResetAllocation()
    {
        _allocation = null;
    }
}

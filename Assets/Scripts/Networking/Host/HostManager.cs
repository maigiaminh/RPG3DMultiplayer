using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Lobbies;
using Unity.Services.Relay;
using UnityEngine;
using System.Collections;
using Unity.Services.Core;
using Unity.Services.Relay.Models;
using Unity.Services.Lobbies.Models;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using UnityEngine.SceneManagement;
using Unity.Services.Core.Environments;

public class HostManager : IDisposable
{
    [Header("Host Settings")]
    public int MaxPlayerConnections = 4;

    public NetworkServer NetworkServer;
    private Allocation _allocation;
    private string _joinCode = "";
    private string _lobbyId;

    private bool _isStarted = false;

    public async Task StartHostAsync()
    {
        Debug.Log("Starting host...");
        if (_isStarted) return;
        _isStarted = true;

        try
        {
            // Initialize Unity Services
            await InitializeUnityServicesAsync();

            if (AuthenticationService.Instance.IsSignedIn)
            {
                Debug.Log("Authentication successful. Player ID: " + AuthenticationService.Instance.PlayerId);
            }
            else
            {
                Debug.LogError("Failed to sign in.");
                return;
            }

            // Initialize Network Server
            InitializeNetworkServer();

            // Create Relay Allocation
            await CreateRelayAllocation();

            // Configure Unity Transport
            ConfigureUnityTransport();

            // Create Lobby
            await CreateLobby();

            // Start Host
            StartNetworkHost();

            Debug.Log("Host started successfully.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error while starting host: {e.Message}");
            Shutdown();
        }
        finally
        {
            _isStarted = false;
        }
    }

    private void InitializeNetworkServer()
    {
        try
        {
            NetworkServer = new NetworkServer(NetworkManager.Singleton);
            Debug.Log("Network Server initialized successfully.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to initialize Network Server: {e.Message}");
            throw;
        }
    }

    private async Task CreateRelayAllocation()
    {
        try
        {
            _allocation = await RelayService.Instance.CreateAllocationAsync(MaxPlayerConnections);
            _joinCode = await RelayService.Instance.GetJoinCodeAsync(_allocation.AllocationId);

            Debug.Log($"Relay Allocation created. Join Code: {_joinCode}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to create Relay Allocation: {e.Message}");
            throw;
        }
    }

    private void ConfigureUnityTransport()
    {
        try
        {
            UnityTransport unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();

            if (unityTransport == null)
            {
                Debug.LogError("UnityTransport is missing on NetworkManager.");
                throw new NullReferenceException("UnityTransport component not found.");
            }

            RelayServerData relayServerData = new RelayServerData(_allocation, "dtls");
            unityTransport.SetRelayServerData(relayServerData);

            Debug.Log("UnityTransport configured successfully.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to configure Unity Transport: {e.Message}");
            throw;
        }
    }

    private async Task CreateLobby()
    {
        try
        {
            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = false,
                Data = new Dictionary<string, DataObject>
                {
                    {
                        "JoinCode", new DataObject(
                            visibility: DataObject.VisibilityOptions.Member,
                            value: _joinCode
                        )
                    }
                }
            };

            string lobbyName = UserContainer.Instance != null
                ? $"{UserContainer.Instance.UserData.Name}'s Lobby"
                : "Default Lobby";

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, MaxPlayerConnections, lobbyOptions);
            _lobbyId = lobby.Id;

            Debug.Log($"Lobby created successfully. Lobby ID: {_lobbyId}");

            // Start Lobby Heartbeat
            HostSingleton.Instance.StartCoroutine(SendHeartBeatLobby(15));
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to create Lobby: {e.Message}");
            throw;
        }
    }

    private void StartNetworkHost()
    {
        if (UserContainer.Instance)
        {
            UserContainer.Instance.SetUserAuthId(AuthenticationService.Instance.PlayerId);

            UserData userData = UserContainer.Instance.UserData;
            string payload = JsonUtility.ToJson(userData);
            byte[] payloadBytes = System.Text.Encoding.UTF8.GetBytes(payload);
            NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;
        }

        NetworkManager.Singleton.StartHost();
        NetworkServer.OnClientLeft += HandleClientLeft;

        NetworkManager.Singleton.SceneManager.LoadScene("TradingScene", LoadSceneMode.Single);
    }

    public IEnumerator SendHeartBeatLobby(int timeDelay)
    {
        WaitForSeconds delay = new WaitForSeconds(timeDelay);
        while (!string.IsNullOrEmpty(_lobbyId))
        {
            try
            {
                LobbyService.Instance.SendHeartbeatPingAsync(_lobbyId);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to send Lobby heartbeat: {e.Message}");
            }
            yield return delay;
        }
    }

    public void Dispose()
    {
        Shutdown();
    }

    private void Shutdown()
    {
        if (!string.IsNullOrEmpty(_lobbyId))
        {
            LobbyService.Instance.DeleteLobbyAsync(_lobbyId);
        }

        _lobbyId = string.Empty;
        _joinCode = string.Empty;

        if (NetworkServer != null)
        {
            NetworkServer.OnClientLeft -= HandleClientLeft;
            NetworkServer.Dispose();
            NetworkServer = null;
        }
    }

    private async void HandleClientLeft(string authId, ulong clientId)
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(_lobbyId, authId);
            Debug.Log($"Client with Auth ID: {authId} removed from Lobby.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to remove client from Lobby: {e.Message}");
        }
    }

    private async Task InitializeUnityServicesAsync()
    {
        try
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                await UnityServices.InitializeAsync(new InitializationOptions()
                    .SetEnvironmentName("production")); // Hoặc "development"

                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                    Debug.Log("Signed in anonymously to Unity Authentication.");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to initialize Unity Services: {e.Message}");
            throw;
        }
    }
}

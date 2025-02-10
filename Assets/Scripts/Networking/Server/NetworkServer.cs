using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkServer : IDisposable
{
    public NetworkManager NetworkManager;

    public event Action<string, ulong> OnClientLeft;

    public event Action<ulong> OnClientEnter;
    private Dictionary<ulong, string> _clientIdToAuth = new Dictionary<ulong, string>();

    private Dictionary<string, UserData> _authToUserData = new Dictionary<string, UserData>();


    private const string LoginSceneName = "Login Scene";



    public NetworkServer(NetworkManager networkManager)
    {
        NetworkManager = networkManager;

        NetworkManager.ConnectionApprovalCallback = null;
        NetworkManager.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.OnServerStarted += OnServerStarted;
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request,
                            NetworkManager.ConnectionApprovalResponse response)
    {
        string payloads = System.Text.Encoding.UTF8.GetString(request.Payload);
        UserData userData = JsonUtility.FromJson<UserData>(payloads);
        var clientId = request.ClientNetworkId;
        if (userData != null)
        {
            _clientIdToAuth.Add(clientId, userData.AuthId);
            _authToUserData.Add(userData.AuthId, userData);
            Debug.Log($"Client {clientId} connected with auth {userData.AuthId}");
        }

        response.Approved = true;
        response.CreatePlayerObject = true;

        // Debug.Log($"Client {clientId} connected with auth {userData.AuthId}");

        OnClientEnter?.Invoke(clientId);
    }

    private void OnServerStarted()
    {
        Debug.Log("Server started");
        NetworkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }
    private void Start() {
        
    }


    private void OnClientDisconnect(ulong clientId)
    {
        if (!_clientIdToAuth.ContainsKey(clientId)) return;

        Debug.Log($"Client {clientId} disconnected");

        var userData = _authToUserData[_clientIdToAuth[clientId]];
        _authToUserData.Remove(userData.AuthId);
        _clientIdToAuth.Remove(clientId);
        OnClientLeft(userData.AuthId, clientId);
    }

    public void Dispose()
    {
        if (!NetworkManager) return;
        _authToUserData.Clear();
        _clientIdToAuth.Clear();
        
        NetworkManager.ConnectionApprovalCallback -= ApprovalCheck;
        NetworkManager.OnServerStarted -= OnServerStarted;
        NetworkManager.OnClientDisconnectCallback -= OnClientDisconnect;
        if (NetworkManager.IsListening)
        {
            NetworkManager.Shutdown();
            if (SceneManager.GetActiveScene().name != LoginSceneName)
            {
                SceneManager.LoadScene(LoginSceneName);
            }
        }
    }
}

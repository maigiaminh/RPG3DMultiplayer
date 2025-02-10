using System;
using System.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkClient : IDisposable
{
    private NetworkManager _networkManager;

    private const string LoginSceneName = "Login Scene";


    public NetworkClient(NetworkManager networkManager)
    {
        _networkManager = networkManager;

        _networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong clientId)
    {
        Debug.Log("OnClientDisconnect + " + clientId);
        if (!_networkManager.IsConnectedClient) return;
        if (clientId == 0 || clientId != _networkManager.LocalClientId) return;

        Disconnect();
    }

    private void Disconnect()
    {
        if (_networkManager.IsConnectedClient)
        {
            _networkManager.Shutdown();
            Debug.Log("NetworkManager shutdown completed.");
        }
        _networkManager.StopAllCoroutines();
        _networkManager.StartCoroutine(WaitForShutdownAndLoadScene());
    }
    private IEnumerator WaitForShutdownAndLoadScene()
    {
        float timeout = 5f; // Thời gian chờ tối đa
        float startTime = Time.time;

        while ((_networkManager.IsListening || _networkManager.IsConnectedClient) && Time.time - startTime < timeout)
        {
            Debug.Log("Waiting for NetworkManager to shutdown...");
            yield return null;
        }

        if (Time.time - startTime >= timeout)
        {
            Debug.LogWarning("Shutdown timeout exceeded. Forcing scene load.");
        }

        // Kiểm tra UnityTransport đã hoàn tất shutdown
        ResetUnityTransport();


        // Chuyển scene nếu cần
        if (SceneManager.GetActiveScene().name != LoginSceneName)
        {
            Debug.Log("Loading Login Scene...");
            SceneManager.LoadScene(LoginSceneName);
        }
    }
    public void Dispose()
    {
        if (_networkManager != null)
        {
            _networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
        }
        _networkManager = null;
    }

    private void ResetUnityTransport()
    {
        UnityTransport unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        if (unityTransport != null)
        {
            Debug.Log("Resetting UnityTransport for reuse...");
            // Reset dữ liệu hoặc RelayServerData nếu cần
            unityTransport.SetRelayServerData(new RelayServerData());
        }
    }
}

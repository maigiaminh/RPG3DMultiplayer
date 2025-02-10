using System;
using System.Collections;
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerNetworkTank : NetworkBehaviour
{
    public NetworkVariable<FixedString32Bytes> Username = new NetworkVariable<FixedString32Bytes>();
    public NetworkVariable<int> Level = new NetworkVariable<int>();


    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        Initialize();
        
        StartCoroutine(InitConfig());
    }


    private void Start()
    {
        if (!IsOwner) return;
        StartCoroutine(InitSpawn());

    }

    IEnumerator InitSpawn()
    {
        WaitForSeconds wait = new WaitForSeconds(1);
        while (NetworkSpawnManager.Instance == null)
        {
            yield return wait;
        }
        GetComponent<ClientNetworkTransform>().Teleport(NetworkSpawnManager.Instance.GetRandomSpawnpoint().position, Quaternion.identity, Vector3.one);
    }


    IEnumerator InitConfig()
    {
        WaitForSeconds wait = new WaitForSeconds(0.5f);
        while (TradingAreaUIManager.Instance == null)
        {
            yield return wait;
        }

        TradingAreaUIManager.Instance.HandleClientEnter(OwnerClientId);
    }


    private void Initialize()
    {
        var userContainer = UserContainer.Instance;
        var username = userContainer ? userContainer.UserData.Name : "Player";
        var level = userContainer ? userContainer.UserData.Level : 1;
        InitializeServerRpc(username, level);
    }

    [ServerRpc(RequireOwnership = false)]
    private void InitializeServerRpc(FixedString32Bytes username, int level)
    {
        Debug.Log("InitializeServerRpc + " + username + " " + level);
        Username.Value = username.ToString();
        Level.Value = level;
    }


}

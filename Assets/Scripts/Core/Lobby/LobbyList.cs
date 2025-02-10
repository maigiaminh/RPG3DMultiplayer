using System.Collections.Generic;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyList : MonoBehaviour
{
    [SerializeField] private Transform lobbyListParent;
    [SerializeField] private Button refreshBtn;
    [SerializeField] private List<LobbyItem> _lobbyList;

    private bool _isJoining = false;
    private bool _isRefresh = false;



    public async void JoinAsync(Lobby lobby)
    {
        if (_isJoining) return;
        _isJoining = true;
        try
        {
            
            var lobbyJoining = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id);
            string joinCode = lobbyJoining.Data["JoinCode"].Value;

            Debug.Log("Joining lobby with join code: " + joinCode);

            await ClientSingleton.Instance.ClientManager.StartClientAsync(joinCode);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
    }

    private void OnEnable()
    {
        // try
        // {
        //     if (UnityServices.State != ServicesInitializationState.Initialized)
        //     {
        //         await UnityServices.InitializeAsync();
        //         Debug.Log("Unity Services initialized.");
        //     }
        // }
        // catch (System.Exception)
        // {
        //     Debug.LogError("Failed to initialize lobbies");
        // }

        RefreshLobbies();
        refreshBtn.onClick.AddListener(RefreshLobbies);
    }


    private async void RefreshLobbies()
    {
        if (_isRefresh) return;
        _isRefresh = true;
        refreshBtn.interactable = false;

        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions();
            queryLobbiesOptions.Count = 20;

            queryLobbiesOptions.Filters = new List<QueryFilter>(){
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0"
                ),
                new QueryFilter(
                    field: QueryFilter.FieldOptions.IsLocked,
                    op: QueryFilter.OpOptions.EQ,
                    value: "0"
                )
            };

            QueryResponse lobbies = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);
            foreach (LobbyItem item in _lobbyList)
            {
                item.gameObject.SetActive(false);
            }

            for (int i = 0; i < lobbies.Results.Count; i++)
            {
                LobbyItem item = _lobbyList[i];
                item.gameObject.SetActive(true);
                item.Initialize(this, lobbies.Results[i]);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
        finally
        {
            _isRefresh = false;
            refreshBtn.interactable = true;
        }
    }

}

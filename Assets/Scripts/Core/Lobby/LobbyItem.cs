
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyItem : MonoBehaviour
{
    private LobbyList _lobbyList;
    private Lobby _lobby;

    public TextMeshProUGUI lobbyName;
    public TextMeshProUGUI playerExistInRoom;
    public Button joinLobbyBtn;

    private void OnEnable()
    {
        joinLobbyBtn.onClick.AddListener(Join);
    }

    private void OnDisable() {
        joinLobbyBtn.onClick.RemoveListener(Join);
    }

    public void Initialize(LobbyList lobbyList, Lobby lobby)
    {
        _lobbyList = lobbyList;
        _lobby = lobby;

        UpdateLobbyUI(lobby);
    }

    private void UpdateLobbyUI(Lobby lobby)
    {
        lobbyName.text = lobby.Name;
        playerExistInRoom.text = $"{lobby.MaxPlayers - lobby.AvailableSlots}/{lobby.MaxPlayers}";
    }

    private void Join() => _lobbyList.JoinAsync(_lobby);
}

using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ChatBoxManager : NetworkBehaviour
{
    public static ChatBoxManager Instance;
    public TMP_InputField chatInputField;
    public Button sendButton;
    public Panel2T ChatItemPrefab;
    public Transform ChatItemParent;

    private string _currentContent = "";

    public bool IsEdit { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        chatInputField.onValueChanged.AddListener((value) =>
        {
            _currentContent = value;
        });
        chatInputField.onSelect.AddListener((value) =>
        {
            IsEdit = true;
        });
        chatInputField.onEndEdit.AddListener((value) =>
        {
            IsEdit = false;
        });

        sendButton.onClick.AddListener(() =>
        {
            Debug.Log("Send button clicked");
            if (_currentContent != "")
            {
                Debug.Log("Send message: " + _currentContent);
                var name = UserContainer.Instance ? UserContainer.Instance.UserData.Name : "Unknown";
                var content = _currentContent;
                SendChatMessageServerRpc(name, content);
                _currentContent = "";
                chatInputField.text = "";
            }
        });
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendChatMessageServerRpc(string name, string content)
    {
        SendChatMessageClientRpc(name, content);
    }

    [ClientRpc]
    private void SendChatMessageClientRpc(string name, string content)
    {
        var chatItem = Instantiate(ChatItemPrefab, ChatItemPrefab.transform.parent);
        chatItem.UpdatePanelText(name, content);
        chatItem.transform.SetParent(ChatItemParent);
        chatItem.transform.localScale = Vector3.one;
        chatItem.transform.localPosition = Vector3.zero;
    }
}

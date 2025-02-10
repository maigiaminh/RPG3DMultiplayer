using System.Collections.Generic;
using UnityEngine;

public class PlayerInRoomPanel : MonoBehaviour
{
    public List<PlayerInRoomItem> playerInRoomItems = new List<PlayerInRoomItem>();

    public void UpdatePlayerInRoomItems(PlayerInfo[] playerInfos)
    {
        Debug.Log($"UpdatePlayerInRoomItems called. Player count: {playerInfos.Length}");

        for (int i = 0; i < playerInRoomItems.Count; i++)
        {
            if (i >= playerInfos.Length)
            {
                Debug.Log($"Hiding item at index {i}");
                playerInRoomItems[i].gameObject.SetActive(false);
                continue;
            }

            Debug.Log($"Updating item at index {i}: {playerInfos[i].PlayerName}");
            playerInRoomItems[i].gameObject.SetActive(true);
            playerInRoomItems[i].UpdateItem(playerInfos[i].PlayerName.ToString(), playerInfos[i].PlayerLevel);
        }
    }
}

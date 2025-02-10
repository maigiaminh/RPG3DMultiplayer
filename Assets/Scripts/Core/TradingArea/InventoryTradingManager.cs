using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class InventoryTradingManager : NetworkBehaviour
{
    public static InventoryTradingManager Instance;
    public InventoryTradingPanel InventoryTradingPanel;
    public TradingPanel TradingPanel;


    public bool IsInTrading = false;

    public ulong CurrentOwnerId { get; private set; }
    public ulong CurrentPartnerId { get; private set; }






    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B) && !IsInTrading && ChatBoxManager.Instance.IsEdit == false)
        {
            Debug.Log("B pressed");
            ActivateInventoryPanelServerRpc(NetworkManager.LocalClientId);
        }
    }



    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        PlayerContactUITradingManager.Instance.OnPlayerCompletedTrade += HandlePlayerCompletedTrade;


        InventoryTradingPanel.Initialize();

        TradingPanel.SetInventoryTradingManager(this);
    }


    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        PlayerContactUITradingManager.Instance.OnPlayerCompletedTrade -= HandlePlayerCompletedTrade;



    }

    public void SetOwnerIdAndPartnerId(ulong ownerId, ulong partnerId)
    {
        CurrentOwnerId = NetworkManager.LocalClientId;
        CurrentPartnerId = NetworkManager.LocalClientId == ownerId ? partnerId : ownerId;
    }


    private void HandlePlayerCompletedTrade(bool isDeal, ulong ownerId, ulong partnerId, List<FixedString32Bytes> payItems, List<FixedString32Bytes> receiveItems)
    {
        if (ownerId != NetworkManager.LocalClientId && partnerId != NetworkManager.LocalClientId) return;

        CurrentOwnerId = NetworkManager.LocalClientId;
        CurrentPartnerId = 999;

    }

    [ServerRpc(RequireOwnership = false)]
    public void ActivateInventoryPanelServerRpc(ulong ownerId)
    {
        ActivateInventoryPanelClientRpc(ownerId);
    }

    [ClientRpc]
    private void ActivateInventoryPanelClientRpc(ulong ownerId)
    {
        if (NetworkManager.LocalClientId != ownerId) return;

        InventoryTradingPanel.ActivatePanel(!InventoryTradingPanel.gameObject.activeSelf);
    }

    #region Handle Slot Clicked

    public void HandleInventorySlotClicked(InventoryTradingSlot inventoryTradingSlot, int index, ulong ownerId, ulong partnerId)
    {
        if (!inventoryTradingSlot.HaveItem) return;

        HandleInventorySlotClickedServerRpc(inventoryTradingSlot.ItemName, index, ConvertSpriteToBytes(inventoryTradingSlot.ItemIcon.sprite), ownerId, partnerId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void HandleInventorySlotClickedServerRpc(string itemName, int index, byte[] spriteBytes, ulong ownerId, ulong partnerId)
    {
        HandleInventorySlotClickedClientRpc(itemName, index, spriteBytes, ownerId, partnerId);
    }

    [ClientRpc]
    private void HandleInventorySlotClickedClientRpc(string itemName, int index, byte[] spriteBytes, ulong ownerId, ulong partnerId)
    {
        if (NeitherOwnerNorPartner(ownerId, partnerId)) return;
        if (ownerId == NetworkManager.LocalClientId)
        {
            if (TradingPanel.IsFullOwner) return;
        }
        if(partnerId == NetworkManager.LocalClientId)
        {
            if (TradingPanel.IsFullPartner) return;
        }
        if (ownerId == NetworkManager.LocalClientId)
        {
            Debug.Log("Owner clicked");
            TradingPanel.AddItemPaySlot(ConvertBytesToSprite(spriteBytes), itemName);
            InventoryTradingPanel.itemSlots[index].SetItem(null, "");
        }

        if (partnerId == NetworkManager.LocalClientId)
        {
            Debug.Log("Partner clicked");
            TradingPanel.AddItemReceiveSlot(ConvertBytesToSprite(spriteBytes), itemName);
        }
    }

    public void HandlePaySlotClicked(InventoryTradingSlot inventoryTradingSlot, int index, ulong ownerId, ulong partnerId)
    {
        if (!inventoryTradingSlot.HaveItem) return;

        HandlePaySlotClickedServerRpc(inventoryTradingSlot.ItemName, index, ConvertSpriteToBytes(inventoryTradingSlot.ItemIcon.sprite), ownerId, partnerId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void HandlePaySlotClickedServerRpc(string itemName, int index, byte[] spriteBytes, ulong ownerId, ulong partnerId)
    {
        HandlePaySlotClickedClientRpc(itemName, index, spriteBytes, ownerId, partnerId);
    }

    [ClientRpc]
    private void HandlePaySlotClickedClientRpc(string itemName, int index, byte[] spriteBytes, ulong ownerId, ulong partnerId)
    {
        if (NeitherOwnerNorPartner(ownerId, partnerId)) return;

        if (ownerId == NetworkManager.LocalClientId)
        {
            InventoryTradingPanel.AddItemSpriteToSlot(ConvertBytesToSprite(spriteBytes), itemName);
            TradingPanel.RemoveItemPaySlot(index);
        }

        if (partnerId == NetworkManager.LocalClientId)
        {
            TradingPanel.RemoveItemReceiveSlot(index);
        }
    }

    public void HandleReceiveSlotClicked(InventoryTradingSlot inventoryTradingSlot, int index, ulong ownerId, ulong partnerId)
    {
        Debug.Log("Receive Slot Clicked + " + inventoryTradingSlot.ItemName + " " + ownerId + " " + partnerId);
    }


    private byte[] ConvertSpriteToBytes(Sprite sprite)
    {
        // Tạo Texture2D từ sprite với TextureFormat phù hợp
        Texture2D readableTexture = new Texture2D(
            (int)sprite.rect.width,
            (int)sprite.rect.height,
            TextureFormat.RGBA32, // Dùng RGBA32 để không bị nén
            false,                // Không dùng Mipmap
            true                  // Linear Color Space
        );

        // Tạo RenderTexture tạm thời để vẽ Sprite
        RenderTexture tempRenderTexture = RenderTexture.GetTemporary(
            sprite.texture.width,
            sprite.texture.height,
            0,
            RenderTextureFormat.Default,
            RenderTextureReadWrite.Linear // Đảm bảo Linear Color Space
        );

        // Vẽ texture lên RenderTexture
        Graphics.Blit(sprite.texture, tempRenderTexture);

        // Lấy RenderTexture làm Texture2D
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = tempRenderTexture;

        // Đọc pixel từ vùng rect của sprite
        readableTexture.ReadPixels(
            new Rect(sprite.rect.x, sprite.rect.y, sprite.rect.width, sprite.rect.height),
            0,
            0
        );
        readableTexture.Apply();

        // Khôi phục RenderTexture
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(tempRenderTexture);

        // Mã hóa Texture2D thành PNG
        byte[] textureBytes = readableTexture.EncodeToPNG();

        // Giải phóng bộ nhớ
        Destroy(readableTexture);

        return textureBytes;
    }


    private Sprite ConvertBytesToSprite(byte[] imageData)
    {
        // Tạo một Texture2D mới với cấu hình phù hợp
        Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false, true);

        // Load dữ liệu ảnh vào Texture2D
        if (texture.LoadImage(imageData))
        {
            // Tạo Sprite từ Texture2D
            Sprite sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f) // Pivot ở giữa
            );

            return sprite;
        }

        Debug.LogError("Failed to load image data into Texture2D.");
        return null;
    }

    #endregion


    public bool NeitherOwnerNorPartner(ulong ownerId, ulong partnerId) => NetworkManager.LocalClientId != ownerId && NetworkManager.LocalClientId != partnerId;

}



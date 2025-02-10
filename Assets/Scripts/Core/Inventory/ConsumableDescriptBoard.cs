using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ConsumableDescriptBoard : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemRarityText;
    public Image itemRerityBackground;
    public Button useButton;

    public void SettingItemDescriptBoard(ItemData data)
    {
        itemNameText.text = data.itemName;
        itemRarityText.text = data.rarity.ToString();
        Color rarityColor = RarityColor.GetRarityColor(data.rarity); // Assuming you have a RarityColor helper class
        itemRerityBackground.color = rarityColor;
        itemRarityText.color = rarityColor;
        icon.sprite = data.icon;
    }

    public void UpdateUseButton(ItemData itemData)
    {
        useButton.onClick.RemoveAllListeners();
        useButton.onClick.AddListener(() =>
        {
            SoundManager.PlaySound(SoundType.POTIONS);
            GameEventManager.Instance.InventoryEvents.UseConsumable(itemData);
            GameEventManager.Instance.InventoryEvents.InventorySlotLeftClicked(null);
        });
        
    }
}

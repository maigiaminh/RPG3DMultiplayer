using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemNoticePanelControl : Singleton<ItemNoticePanelControl>
{
    public GameObject panel;
    public Image icon;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI pickupKey;

    protected override void Awake()
    {
        base.Awake();
    }

    public void Initialize(ItemData itemData)
    {
        icon.sprite = itemData.icon;
        itemName.text = itemData.itemName;
        itemName.color = RarityColor.GetRarityColor(itemData.rarity);
    }
}

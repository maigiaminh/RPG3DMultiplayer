using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemButton : InteractableObject
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemName;

    [SerializeField] private TextMeshProUGUI itemPrice;

    public ShopItem shopItem;
    public void SetupItem(ShopItem item)
    {
        shopItem = item;
        itemIcon.sprite = item.data.icon;
        itemName.text = item.data.itemName;
        itemPrice.text = item.itemPrice.ToString();
    }

    public override void OnClick()
    {
        ShopUIManager.Instance?.OnItemChange(this);
    }
}

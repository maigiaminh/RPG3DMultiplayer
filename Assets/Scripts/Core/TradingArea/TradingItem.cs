using UnityEngine.UI;
using UnityEngine;

public class TradingItem : MonoBehaviour
{
    public Image itemImage;

    public void UpdateItem(ItemData itemData)
    {
        itemImage.sprite = itemData.icon;
    }
}

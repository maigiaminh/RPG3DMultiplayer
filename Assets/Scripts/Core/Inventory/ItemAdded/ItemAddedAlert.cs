using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemAddedAlert : MonoBehaviour
{
    public Image icon;
    public TMPro.TextMeshProUGUI itemName;
    public TMPro.TextMeshProUGUI amount;

    public void Initialize(ItemData itemData, int amount){
        icon.sprite = itemData.icon;
        itemName.text = itemData.itemName;
        itemName.color = RarityColor.GetRarityColor(itemData.rarity);
        this.amount.text = "x" + amount.ToString();
    }

}

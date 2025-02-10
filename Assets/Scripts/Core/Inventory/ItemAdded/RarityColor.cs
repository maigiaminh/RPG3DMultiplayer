

using UnityEngine;

public static class RarityColor 
{   
    public static Color GetRarityColor(ItemData.Rarity rarity){
        Color color = Color.white;
        switch(rarity){
            case ItemData.Rarity.Common:
                color = Color.gray;
            break;
            case ItemData.Rarity.Uncommon:
                color = Color.green * 0.75f;
            break;
            case ItemData.Rarity.Rare:
                color = Color.blue;
            break;
            case ItemData.Rarity.Epic:
                color = Color.magenta;
            break;
            case ItemData.Rarity.Legendary:
                color = Color.yellow;
            break;
            case ItemData.Rarity.Mythic:
                color = Color.red;
            break;
        }
        return color;
    }
}

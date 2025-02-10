using UnityEngine;

public class ChangeQuantityButton : InteractableObject
{
    public override void OnClick()
    {
        if(name == "Add Btn"){
            ShopUIManager.Instance?.OnIncreaseQuantity();
        }
        else if(name == "Sub Btn"){
            ShopUIManager.Instance?.OnDecreaseQuantity();
        }
    }
}

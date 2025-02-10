using UnityEngine;

public class BuyButton : InteractableObject
{
    public override void OnClick()
    {
        ShopUIManager.Instance?.Purchase();
    }
}

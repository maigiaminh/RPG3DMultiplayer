using UnityEngine;

public class CloseButton : InteractableObject
{
    public override void OnHoverEnter() { }
    public override void OnHoverExit() { }
    public override void OnClick()
    {
        ShopUIManager.Instance?.OnCloseShopPanel();
    }
}

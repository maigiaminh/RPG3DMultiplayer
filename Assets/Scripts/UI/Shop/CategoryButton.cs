using UnityEngine;

public class CategoryButton : InteractableObject
{
    public override void OnClick()
    {
        ShopUIManager.Instance?.OnChangeCategory(name);
    }
}

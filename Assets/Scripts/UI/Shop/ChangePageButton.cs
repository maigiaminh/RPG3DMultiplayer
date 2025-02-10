using UnityEngine;

public class ChangePageButton : InteractableObject
{
    public override void OnClick()
    {
        if(name == "BackwardBtn"){
            ShopUIManager.Instance?.OnDecreasePage();
        }
        else if(name == "ForwardBtn"){
            ShopUIManager.Instance?.OnIncreasePage();
        }
    }
}

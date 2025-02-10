using UnityEngine;
using UnityEngine.EventSystems;

public class InteractableObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    protected Animator animator;
    private void Start(){
        TryGetComponent<Animator>(out animator);

        ShopUIManager.Instance?.RegisterUI(name, gameObject);
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        OnHoverEnter();
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        OnHoverExit();
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        OnClick();
    }

    public virtual void OnHoverEnter() {
        Debug.Log($"{name}: Hover Enter"); 
        if(animator == null) return;

        animator.SetBool("Hover", true);
    }
    public virtual void OnHoverExit() {
        Debug.Log($"{name}: Hover Exit"); 
        if(animator == null) return;

        animator.SetBool("Hover", false);
    }
    public virtual void OnClick() { Debug.Log($"{name}: Clicked"); }
}

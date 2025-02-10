using UnityEngine;
using UnityEngine.EventSystems;

public class WorldMapUIHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    
    [SerializeField] MapData mapData;

    private Animator animator;

    private void Start(){
        animator = GetComponent<Animator>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        animator.SetBool("Hover", true);
        CursorManager.Instance.SetClickCursor();
        WorldMapUIManager.Instance.MapHover(mapData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        animator.SetBool("Hover", false);
        CursorManager.Instance.SetDefaultCursor();
        WorldMapUIManager.Instance.MapExit();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        WorldMapUIManager.Instance.MapClicked();
    }
}

using UnityEngine;
using UnityEngine.EventSystems;

public class UIButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        SoundManager.PlaySound(SoundType.BUTTON_CLICK);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundManager.PlaySound(SoundType.BUTTON_HOVER);
    }
}

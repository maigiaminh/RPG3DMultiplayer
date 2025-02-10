using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class HoverButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Button's Components")]
    public Image background;
    public Image icon;
    public TextMeshProUGUI label;

    private Color normalBackgroundColor = Color.white;
    private Color hoverBackgroundColor = new Color32(212, 164, 77, 232);
    private Color normalIconColor = Color.black;
    private Color hoverIconColor = Color.black;
    private Color normalLabelColor = Color.black;
    private Color hoverLabelColor = new Color32(46, 46, 46, 255);

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (background)
        {
            background.color = hoverBackgroundColor;
            icon.color = hoverIconColor;
            label.color = hoverLabelColor;
        }
        else
        {
            label.color = hoverBackgroundColor;
            icon.color = hoverBackgroundColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (background)
        {
            background.color = normalBackgroundColor;
            icon.color = normalIconColor;
            label.color = normalLabelColor;
        }
        else
        {
            label.color = Color.white;
            icon.color = Color.white;
        }
    }
}

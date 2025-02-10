using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class SkillPanelDescriptItem : MonoBehaviour
{
    public Image AttributeIcon;
    public TextMeshProUGUI AttributeDescript;

    public void SetSkillDescription(Sprite icon, string descript)
    {
        AttributeIcon.sprite = icon;
        AttributeDescript.text = descript;
    }

}

using TMPro;
using UnityEngine;

public class Panel2T : MonoBehaviour
{
    public TextMeshProUGUI Content1Text;
    public TextMeshProUGUI Content2Text;

    public void UpdatePanelText(string content1, string content2)
    {
        Content1Text.text = content1;
        Content2Text.text = content2;
    }
}

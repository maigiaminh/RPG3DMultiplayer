using TMPro;
using UnityEngine;

public class Panel1T : MonoBehaviour
{
    public TextMeshProUGUI contentText;

    public void UpdatePanelText(string content)
    {
        contentText.text = content;
    }
}

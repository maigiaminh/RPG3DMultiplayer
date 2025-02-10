using TMPro;
using UnityEngine;

public class InputFieldHandler : MonoBehaviour
{
    [Header("Fields")]
    public TMP_InputField usernameField;
    public TMP_InputField passwordField;

    private Color normalColor = Color.white;
    private Color32 focusedColor = new Color32(250, 208, 136, 255);

    void Start()
    {
        usernameField.onSelect.AddListener(delegate { OnFocus(usernameField); });
        usernameField.onDeselect.AddListener(delegate { OnDefocus(usernameField); });
        passwordField.onSelect.AddListener(delegate { OnFocus(passwordField); });
        passwordField.onDeselect.AddListener(delegate { OnDefocus(passwordField); });
    }

    void OnFocus(TMP_InputField field)
    {
        field.textComponent.color = focusedColor;
        if (field.placeholder is TextMeshProUGUI placeholderText){
            placeholderText.color = focusedColor;
        }
    }

    void OnDefocus(TMP_InputField field)
    {
        field.textComponent.color = normalColor;
        
        if (field.placeholder is TextMeshProUGUI placeholderText){
            placeholderText.color = normalColor;
        }
    }
}

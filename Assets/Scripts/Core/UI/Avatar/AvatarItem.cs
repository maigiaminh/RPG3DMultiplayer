using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class AvatarItem : MonoBehaviour
{
    [HideInInspector] public Sprite AvatarSprite;
    [HideInInspector] public string AvatarName;

    [HideInInspector] public Image Image;
    [HideInInspector] public GameObject HightlightGO;
    [HideInInspector] public Button Button;

    private void Awake() {
        Image = transform.GetChild(1).GetComponent<Image>();
        HightlightGO = transform.GetChild(2).gameObject;
        Button = transform.GetChild(3).GetComponent<Button>();
    }

    public void SetAvatarSprite(Sprite sprite) {
        AvatarSprite = sprite;
        Image.sprite = AvatarSprite;
        AvatarName = AvatarSprite.name;
    }
}

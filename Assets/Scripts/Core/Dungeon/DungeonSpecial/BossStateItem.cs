using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossStateItem : MonoBehaviour
{
    public Image BossPortrait;
    public Image BossHPBar;
    public TextMeshProUGUI BossHPText;


    public void UpdateItem(Sprite portrait, float hp, float maxHp)
    {
        BossPortrait.sprite = portrait;
        BossHPBar.fillAmount = (float) (hp / maxHp);
        BossHPText.text = $"{hp} / {maxHp}";
    }
}

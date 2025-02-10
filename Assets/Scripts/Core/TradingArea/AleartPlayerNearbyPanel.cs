using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AleartPlayerNearbyPanel : MonoBehaviour
{
    public TextMeshProUGUI PlayerNameText;

    public void UpdateAleartPlayerNearbyPanel(string playerName)
    {
        PlayerNameText.text = playerName;
    }
}

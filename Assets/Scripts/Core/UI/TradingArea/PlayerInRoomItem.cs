using TMPro;
using UnityEngine;

public class PlayerInRoomItem : MonoBehaviour
{
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI playerLevel;
    public void UpdateItem(string name, int level)
    {
        playerName.text = name;
        playerLevel.text = "Lvl " + level.ToString();
    }
}

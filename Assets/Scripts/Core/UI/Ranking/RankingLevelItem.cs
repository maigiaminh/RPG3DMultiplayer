using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankingLevelItem : MonoBehaviour
{
    public Image Portrait;
    public TextMeshProUGUI NameTxt;
    public TextMeshProUGUI LevelTxt;
    public TextMeshProUGUI RankTxt;

    public void SetAppearance(Sprite portrait, string name, int level, int rank)
    {
        Portrait.sprite = portrait;
        NameTxt.text = name;
        LevelTxt.text = "Lvl " + level.ToString();
        RankTxt.text = "#" + rank.ToString();
        SetRankTextColor(rank);
    }       



    public void SetRankTextColor(int rank)
    {
        if(rank == 1)
        {
            RankTxt.color = new Color(1, 0.8f, 0);
        }
        else if(rank == 2)
        {
            RankTxt.color = new Color(0.8f, 0.8f, 0.8f);
        }
        else if(rank == 3)
        {
            RankTxt.color = new Color(0.8f, 0.4f, 0);
        }
        else
        {
            RankTxt.color = new Color(0.8f, 0.8f, 0.8f);
        }
    }


}


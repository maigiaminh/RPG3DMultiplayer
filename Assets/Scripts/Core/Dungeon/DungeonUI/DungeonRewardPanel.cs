using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DungeonRewardPanel : MonoBehaviour
{
    [Header("Dungeon Descript")]
    public Image DungeonImage;
    public Image DungeonBackgroundImage;
    public List<Image> RewardImages;
    public TextMeshProUGUI DungeonDescriptionText;




    public void SetDungeonReward(DungeonDescriptScriptableObject.DungeonType dungeonType, DungeonDescriptScriptableObject dungeonDescript)
    {
        SetRewardImage(dungeonDescript.GetRewardSprites(dungeonType));
        string description = "";
        Sprite sprite = null;
        Color color = Color.white;
        switch (dungeonType)
        {
            case DungeonDescriptScriptableObject.DungeonType.Whispers:
                description = dungeonDescript.WhisperDescription;
                sprite = dungeonDescript.WhispersSprite;
                color = dungeonDescript.WhispersColor;
                break;
            case DungeonDescriptScriptableObject.DungeonType.Coils:
                description = dungeonDescript.CoilsDescription;
                sprite = dungeonDescript.CoilsSprite;
                color = dungeonDescript.CoilsColor;
                break;
            case DungeonDescriptScriptableObject.DungeonType.Tempest:
                description = dungeonDescript.TempestDescription;
                sprite = dungeonDescript.TempestSprite;
                color = dungeonDescript.TempestColor;
                break;
            case DungeonDescriptScriptableObject.DungeonType.Abyss:
                description = dungeonDescript.AbyssDescription;
                sprite = dungeonDescript.AbyssSprite;
                color = dungeonDescript.AbyssColor;
                break;
        }
        DungeonDescriptionText.text = description;
        DungeonImage.sprite = sprite;
        DungeonBackgroundImage.color = color;
    }

    public void SetRewardImage(List<Sprite> rewardImages)
    {
        for (int i = 0; i < RewardImages.Count; i++)
        {
            if (i < rewardImages.Count)
            {
                RewardImages[i].sprite = rewardImages[i];
            }
            else
            {
                RewardImages[i].gameObject.SetActive(false);
            }
        }
    }
}

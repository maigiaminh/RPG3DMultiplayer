using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RewardBoard : MonoBehaviour
{
    public List<Image> rewardImages = new List<Image>();
    public Sprite defaultRewardSprite;
    public Button claimButton;

    private List<ItemData> _currentRewardItems;



    private void OnDisable()
    {
        claimButton.onClick.RemoveListener(ClaimReward);
    }

    public void SetRewardImages(List<Sprite> rewardSprites)
    {
        for (int i = 0; i < rewardImages.Count; i++)
        {
            if (i < rewardSprites.Count)
            {
                rewardImages[i].sprite = rewardSprites[i];
            }
            else
            {
                rewardImages[i].sprite = defaultRewardSprite;
                rewardImages[i].color = Color.black;
            }

        }
    }

    public void SetRewardItems(List<ItemData> rewardItems)
    {
        _currentRewardItems = rewardItems;

        claimButton.onClick.AddListener(ClaimReward);

        List<Sprite> sprites = new List<Sprite>();
        foreach (var item in rewardItems)
        {
            sprites.Add(item.icon);
        }
        SetRewardImages(sprites);
    }

    private void ClaimReward()
    {
        Debug.Log("Claim Reward");
        foreach (var item in _currentRewardItems)
        {
            InventoryManager.Instance.AddItemToInventory(item, 1);
        }
        DeactiveRewardPanel();

        ChangeSceneToMain();
    }

    public void DeactiveRewardPanel()
    {
        gameObject.SetActive(false);
    }

    public void ActiveRewardPanel()
    {
        gameObject.SetActive(true);
    }

    private void ChangeSceneToMain()
    {
        var playerSpawnManager = PlayerSpawnManager.Instance;
        SceneManager.LoadScene(playerSpawnManager.GAMESCENE);
    }


}

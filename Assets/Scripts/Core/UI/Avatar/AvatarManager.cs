using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AvatarManager : MonoBehaviour
{
    public static AvatarManager Instance { get; private set; }
    public Transform AvatarContainer;
    public Image AvatarAppearanceImage;
    public Button ChangeButton;
    private Image ChangeButtonImage;
    private List<AvatarItem> _avatarItems = new List<AvatarItem>();
    private List<Sprite> _sprites = new List<Sprite>();

    private AvatarItem _currentChooseAvatarItem;
    public string GetCurrentChooseAvatarItemName() => _currentChooseAvatarItem.AvatarName;
    private AvatarItem _currentAvatarItem;
    private Color _defaultChangeBtnColor;


    private void Awake() {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        InitializeChageButton();
        InitializeAvatarList();
        InitializeButton();

        FirstApplyAvatar();
        StartCoroutine(FirstApplyAvatar());

    }


    private void InitializeButton()
    {
        foreach (var avatarItem in _avatarItems)
        {
            avatarItem.Button.onClick.AddListener(() =>
            {
                if (_currentChooseAvatarItem == avatarItem)
                {
                    ChangeButtonImage.color = Color.gray;
                    ChangeButton.interactable = false;
                }
                else
                {
                    ChangeButtonImage.color = _defaultChangeBtnColor;
                    ChangeButton.interactable = true;
                }

                foreach (var item in _avatarItems)
                {
                    item.HightlightGO.SetActive(false);
                }
                avatarItem.HightlightGO.SetActive(true);
                _currentAvatarItem = avatarItem;
            });
        }
    }

    private void InitializeAvatarList()
    {
        _sprites = Resources.LoadAll<Sprite>("Avatar").ToList();
        Debug.Log("_spritesCount: " + _sprites.Count);
        for (int i = 0; i < AvatarContainer.childCount; i++)
        {
            var avatarItem = AvatarContainer.GetChild(i).GetComponent<AvatarItem>();
            avatarItem.SetAvatarSprite(_sprites[i]);

            _avatarItems.Add(avatarItem);
        }
    }

    private void InitializeChageButton()
    {
        ChangeButtonImage = ChangeButton.GetComponent<Image>();
        _defaultChangeBtnColor = ChangeButtonImage.color;
        ChangeButton.onClick.AddListener(() =>
        {
            _currentChooseAvatarItem = _currentAvatarItem;
            AvatarAppearanceImage.sprite = _currentAvatarItem.AvatarSprite;
            ChangeButtonImage.color = Color.gray;
        });
    }

    IEnumerator FirstApplyAvatar()
    {
        // Wait until UserData is available
        while (UserContainer.Instance.UserData == null)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Check if AvatarName is empty
        if (string.IsNullOrEmpty(UserContainer.Instance.UserData.AvatarName))
        {
            Debug.Log("AvatarName is empty");
            var firstItem = _avatarItems[0];
            ApplyAvatar(firstItem);
            yield break;
        }

        // Find and apply the avatar item
        var avatarItem = _avatarItems.Find(x => x.AvatarName == UserContainer.Instance.UserData.AvatarName);
        if (avatarItem != null)
        {
            Debug.Log("Avatar item found");
            ApplyAvatar(avatarItem);
        }
        else
        {
            Debug.LogError("Avatar item not found");
        }
    }

    public void ApplyAvatar(AvatarItem item)
    {
        item.Button.onClick.Invoke();
        _currentChooseAvatarItem = item;
        AvatarAppearanceImage.sprite = item.AvatarSprite;
        ChangeButtonImage.color = Color.gray;
        ChangeButton.interactable = false;
    }


}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChooseOutfitManager : MonoBehaviour
{
    public static ChooseOutfitManager Instance;
    [Tooltip("Hair = 0, Beard = 1, Helmet = 2")]
    [Header("Outfit Buttons")]
    public Button[] GenderButtons; // Male, Female
    public Button[] OutfitButtons; // Hair, Beard, Helmet
    [Header("Skin")]
    public Transform SkinContentParent;
    public OutfitPartUIItem SkinUIItemPrefab;
    [Header("Outfir Parts")]
    public Transform OutfitContentParent;
    public OutfitPartUIItem OutfitUIItemPrefab;

    [Header("Model")]
    public CharacterSkins CharacterSkins;
    [Header("Enter Game Button")]
    public Button EnterGameBtn;



    private Dictionary<OutfitPartType, List<OutfitPartConfig>> _skinsDict;
    private string OUTFIT_PATH = "Outfit";

    public event Action<OutfitPartType, string> OnOutfitPartWear;
    public void HandleOnOutfitPartWear(OutfitPartType type, string name)
    {
        OnOutfitPartWear?.Invoke(type, name);
    }
    public event Action<OutfitPartType> OnOutfitPartUnwear;
    public void HandleOnOutfitPartUnwear(OutfitPartType type)
    {
        OnOutfitPartUnwear?.Invoke(type);
    }


    private const string GAME_SCENE_NAME = "MAINGAMESCENE";



    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        LoadSkinList();
        InitalizeGenderButtons();
        InitalizeOutfitButtons();
        InitializeSkin();
        InitializeEnterGameBtn();
    }



    private void OnEnable()
    {
        OnOutfitPartWear += Wear;
        OnOutfitPartUnwear += Unwear;

        OutfitButtons[0].onClick.Invoke();
        GenderButtons[0].onClick.Invoke();

    }


    private void OnDisable()
    {
        OnOutfitPartWear -= Wear;
        OnOutfitPartUnwear -= Unwear;
    }

    private void Start()
    {
        InitialChangeOutfit();
    }

    private void InitialChangeOutfit()
    {
        CharacterSkins.ChangeOutfit(OutfitPartType.Hair, CharacterSkins.InitialHair);
        CharacterSkins.ChangeOutfit(OutfitPartType.Beard, CharacterSkins.InitialBeard);
        CharacterSkins.ChangeOutfit(OutfitPartType.Helmet, CharacterSkins.InitialHelmet);
        CharacterSkins.ChangeOutfit(OutfitPartType.Skin_Male, CharacterSkins.InitialSkin);
    }

    public void LoadSkinList()
    {
        _skinsDict = new Dictionary<OutfitPartType, List<OutfitPartConfig>>();
        var skins = Resources.LoadAll<OutfitPartConfig>(OUTFIT_PATH).ToList();
        foreach (var skin in skins)
        {
            if (!_skinsDict.ContainsKey(skin.PartType))
            {
                _skinsDict[skin.PartType] = new List<OutfitPartConfig>();
            }
            _skinsDict[skin.PartType].Add(skin);
        }
    }
    private void InitalizeGenderButtons()
    {
        for (int i = 0; i < GenderButtons.Length; i++)
        {
            var index = i;
            GenderButtons[i].onClick.AddListener(() => OnGenderButtonClicked(index));
        }
    }
    private void InitalizeOutfitButtons()
    {
        for (int i = 0; i < OutfitButtons.Length; i++)
        {
            var index = i;
            OutfitButtons[i].onClick.AddListener(() => OnOutfitButtonClicked(index));
        }
    }

    private void OnGenderButtonClicked(int index)
    {
        SoundManager.PlaySound(SoundType.BUTTON_CLICK);
        switch (index)
        {
            case 0:
                UpdateGenderPartPanel(OutfitPartType.Skin_Male);
                break;
            case 1:
                UpdateGenderPartPanel(OutfitPartType.Skin_Female);
                break;
        }
    }

    private void UpdateGenderPartPanel(OutfitPartType skin)
    {
        var parts = _skinsDict[skin];
        foreach (Transform child in SkinContentParent)
        {
            Destroy(child.gameObject);
        }
        int index = 1;
        foreach (var part in parts)
        {
            var outfitPartUIItem = Instantiate(SkinUIItemPrefab, SkinContentParent);
            outfitPartUIItem.Initalize(this, part, index);
            outfitPartUIItem.transform.SetParent(SkinContentParent);
            index++;
        }
    }

    private void OnOutfitButtonClicked(int index)
    {
        SoundManager.PlaySound(SoundType.BUTTON_CLICK);
        switch (index)
        {
            case 0:
                UpdateOutfitPartPanel(OutfitPartType.Hair);
                break;
            case 1:
                UpdateOutfitPartPanel(OutfitPartType.Beard);
                break;
            case 2:
                UpdateOutfitPartPanel(OutfitPartType.Helmet);
                break;
        }
    }

    private void UpdateOutfitPartPanel(OutfitPartType type)
    {
        var parts = _skinsDict[type];
        foreach (Transform child in OutfitContentParent)
        {
            Destroy(child.gameObject);
        }
        int index = 1;
        foreach (var part in parts)
        {
            Optional<OutfitPartUIItem> outfitPartUIItem = Instantiate(OutfitUIItemPrefab, OutfitContentParent);
            outfitPartUIItem.Match(
                (item) =>
                {
                    item.Initalize(this, part, index);
                    item.transform.SetParent(OutfitContentParent);
                },
                () => Debug.LogError("OutfitPartUIItem is null")
            );
            index++;
        }

        var clearOutfitPartUIItem = Instantiate(OutfitUIItemPrefab, OutfitContentParent);
        clearOutfitPartUIItem.Initalize(this, parts.First(), -1);
        clearOutfitPartUIItem.transform.SetParent(OutfitContentParent);

    }

    private void InitializeSkin()
    {
        var skins = _skinsDict[OutfitPartType.Skin_Male];
        int index = 1;
        foreach (var skin in skins)
        {
            var outfitPartUIItem = Instantiate(SkinUIItemPrefab, SkinContentParent);
            outfitPartUIItem.Initalize(this, skin, index);
            outfitPartUIItem.transform.SetParent(SkinContentParent);
            index++;
        }
    }

    private void InitializeEnterGameBtn()
    {
        EnterGameBtn.onClick.AddListener(ChooseCharacter);
    }



    private void Unwear(OutfitPartType type)
    {
        CharacterSkins.ChangeOutfit(type, "None");
    }

    private void Wear(OutfitPartType type, string partName)
    {
        Debug.Log("Wear: " + type + " " + partName);
        CharacterSkins.ChangeOutfit(type, partName);
    }



    private void ChooseCharacter()
    {
        StartCoroutine(UpdateCharacter());
    }

    IEnumerator UpdateCharacter()
    {
        var firebaseManager = FirebaseAuthManager.Instance;

        OutfitData outfitData = CharacterSkins.GetOutfitData();
        string currentCharacterName = ChooseCharacterManager.Instance.CurrentCharacterName;

        yield return firebaseManager.StartCoroutine(firebaseManager.UpdateCharacterOutfits(outfitData));
        yield return firebaseManager.StartCoroutine(firebaseManager.UpdateCharacterClass(currentCharacterName));

        string initItem = InventoryContainer.Instance.InitItemMap[currentCharacterName];
        ItemData item = InventoryContainer.Instance.ResourceItemMap[initItem];

        OutfitContainer.Instance.SetOutfitData(outfitData);
        UserContainer.Instance.SetUserCharacterClassData(currentCharacterName);
        InventoryContainer.Instance.UpdateItemFirebase(new Dictionary<ItemData, int>(){
            {item, 1}
        });

        LoadingManager.Instance.NewLoadScene(GAME_SCENE_NAME);
        //SceneManager.LoadScene(GAME_SCENE_NAME);
    }

}


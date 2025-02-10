using System;
using NUnit.Framework;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class OutfitPartUIItem : MonoBehaviour, IOutfitPart
{
    public TextMeshProUGUI NameText;
    public Image IconImage;
    public Button Button;
    private ChooseOutfitManager _chooseOutfitManager;

    private OutfitPartType _partType = OutfitPartType.None;
    public OutfitPartType PartType => _partType;

    private string _name;

    public void Initalize(ChooseOutfitManager chooseOutfitManager, OutfitPartConfig outfitPart, int index)
    {
        _chooseOutfitManager = chooseOutfitManager;
        _partType = outfitPart.PartType;
        _name = outfitPart.Name;

        if(index == -1){
            if (NameText) NameText.text = "";
            if(IconImage) {
                IconImage.sprite = Resources.Load<Sprite>("SPR_Clear");
                IconImage.color = Color.red;
            }

            Button.onClick.AddListener(() => Unwear());

            return;
        }
        else{
            if (NameText) NameText.text = index.ToString();
            if(IconImage) IconImage.sprite = outfitPart.Icon;
            Button.onClick.AddListener(() => Wear());
        }
    }
    public void Wear()
    {
        SoundManager.PlaySound(SoundType.BUTTON_CLICK);
        _chooseOutfitManager.HandleOnOutfitPartWear(_partType, _name);
    }
    public void Unwear()
    {
        SoundManager.PlaySound(SoundType.BUTTON_CLICK);
        _chooseOutfitManager.HandleOnOutfitPartUnwear(_partType);
    }

}

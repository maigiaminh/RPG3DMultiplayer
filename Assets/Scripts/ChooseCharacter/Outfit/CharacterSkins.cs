using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSkins : MonoBehaviour
{

    public Transform SkinContentParent;
    public Transform HairContentParent;
    public Transform BeardContentParent;
    public Transform HelmetContentParent;

    private string _currentSkin;
    private string _currentHair;
    private string _currentBeard;
    private string _currentHelmet;

    public const string InitialSkin = "SM_Chr_Soldier_Female_02";
    public const string InitialHair = "SM_Chr_Hair_Female_01";
    public const string InitialBeard = "SM_Chr_Hair_Beard_01";
    public const string InitialHelmet = "SM_Chr_Attach_Crown_Leaf_01";


    protected virtual void Start()
    {
        var container = OutfitContainer.Instance;
        
        var hair = container != null ? container.OutfitData.HairName : InitialHair;
        var beard = container != null ? container.OutfitData.BeardName : InitialBeard;
        var helmet = container != null ? container.OutfitData.HelmetName : InitialHelmet;
        var skin = container != null ? container.OutfitData.SkinName : InitialSkin;


        ChangeOutfit(OutfitPartType.Hair, hair);
        ChangeOutfit(OutfitPartType.Beard, beard);
        ChangeOutfit(OutfitPartType.Helmet, helmet);
        ChangeOutfit(OutfitPartType.Skin_Male, skin);
    }


    public void ChangeOutfit(OutfitPartType type, string partName)
    {
        switch (type)
        {
            case OutfitPartType.Hair:
                ChangeHair(partName);
                break;
            case OutfitPartType.Beard:
                ChangeBeard(partName);
                break;
            case OutfitPartType.Helmet:
                ChangeHelmet(partName);
                break;
            case OutfitPartType.Skin_Male:
                ChangeSkin(partName);
                break;
            case OutfitPartType.Skin_Female:
                ChangeSkin(partName);
                break;

        }

    }

    private void ChangeSkin(string partName)
    {
        _currentSkin = partName;
        foreach (Transform child in SkinContentParent)
        {
            child.gameObject.SetActive(child.gameObject.name == partName);
        }
    }

    private void ChangeHelmet(string partName)
    {
        _currentHelmet = partName;
        foreach (Transform child in HelmetContentParent)
        {
            child.gameObject.SetActive(child.gameObject.name == partName);
        }
    }

    private void ChangeBeard(string partName)
    {
        _currentBeard = partName;
        foreach (Transform child in BeardContentParent)
        {
            child.gameObject.SetActive(child.gameObject.name == partName);
        }
    }

    private void ChangeHair(string partName)
    {
        _currentHair = partName;
        foreach (Transform child in HairContentParent)
        {
            child.gameObject.SetActive(child.gameObject.name == partName);
        }
    }

    public OutfitData GetOutfitData()
    {
        return new OutfitData
        {
            SkinName = _currentSkin,
            HairName = _currentHair,
            BeardName = _currentBeard,
            HelmetName = _currentHelmet
        };
    }
}

using UnityEngine;

public interface IOutfitPart
{
    public OutfitPartType PartType { get; }
    public void Wear();
    public void Unwear();
}

public enum OutfitPartType
{
    None,
    Hair,
    Helmet,
    Beard,
    Skin_Female,
    Skin_Male
}
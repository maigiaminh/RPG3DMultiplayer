using UnityEngine;

public class OutfitContainer : Singleton<OutfitContainer>
{
    public OutfitData OutfitData { get; private set; }

    public void SetOutfitData(OutfitData outfitData)
    {
        OutfitData = outfitData;
    }
}

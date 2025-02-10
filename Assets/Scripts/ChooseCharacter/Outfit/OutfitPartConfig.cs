using UnityEngine;


[CreateAssetMenu(fileName = "OutfitPartConfig", menuName = "OutfitPartConfig", order = 0)]
public class OutfitPartConfig : ScriptableObject {
    public string Name;
    public Sprite Icon;
    public OutfitPartType PartType;
}

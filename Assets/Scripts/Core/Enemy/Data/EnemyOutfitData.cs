using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EnemyOutfitData", menuName = "Enemy/EnemyOutfitData", order = 0)]
public class EnemyOutfitData : ScriptableObject
{
    public List<Material> Materials;
}

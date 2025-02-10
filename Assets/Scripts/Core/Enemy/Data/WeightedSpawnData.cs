using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeightedSpawnData", menuName = "Enemy/WeightedSpawnData", order = 1)]
public class WeightedSpawnData : ScriptableObject
{
    [Range(0,1)]
    public float MinWeight;
    [Range(0,1)]
    public float MaxWeight;

    public float GetWeight(){
        return Random.Range(MinWeight, MaxWeight);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileData", menuName = "Enemy/ProjectileData", order = 1)]
public class ProjectileData : ScriptableObject
{
    public string projectileName;
    // Trail Renderer for visual effect
    public TrailRenderer trailPrefab;

    // Early, Mid, and Late phase effects
    public GameObject earlyPhaseEffect;
    public GameObject midPhaseEffect;
    public GameObject latePhaseEffect;

    // Movement speed and damage
    public float speed;
    public int damage;
    public float existingTime;
    public int launchAngle;

}

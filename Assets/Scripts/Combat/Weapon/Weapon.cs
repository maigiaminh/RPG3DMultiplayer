using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Weapon : WeaponModel
{
    [Header("Weapon Info")]
    [field: SerializeField] public Attack[] comboAttacks;
    [field: SerializeField] public Transform projectileSpawnPoint;
}
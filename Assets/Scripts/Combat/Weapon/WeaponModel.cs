using UnityEngine;

public class WeaponModel : MonoBehaviour
{
    [Header("Weapon Info")]
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public WeaponType weaponType { get; private set; }
}

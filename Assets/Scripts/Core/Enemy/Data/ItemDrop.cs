using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    [Range(0f, 1f)]
    public float dropChance = 1f;
    public int minQuantity = 1; // Số lượng tối thiểu rớt ra
    public int maxQuantity = 1; // Số lượng tối đa rớt ra

    public int GetRandomQuantity()
    {
        return UnityEngine.Random.Range(minQuantity, maxQuantity + 1);
    }
}

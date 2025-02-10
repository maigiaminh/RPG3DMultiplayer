using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankPlayer : MonoBehaviour, IDamageable
{
    public Transform GetTransform() => transform;

    public void TakeDamage(Transform trans, int damage)
    {
        Debug.Log("Player Tank took " + damage + " damage");
    }

    public void Heal(int amount)
    {
        Debug.Log("Player heal " + amount + " health");
    }

    public void ApplyCC(CCType ccType, float ccDuration)
    {
        Debug.Log("Player Tank applied " + ccType + " for " + ccDuration + " seconds");
    }

}

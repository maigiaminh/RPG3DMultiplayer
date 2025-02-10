using System;
using UnityEngine;

public class DamageEvent
{
    public event Action<Transform, float> OnGlobalDamageTaken;

    public void NotifyDamage(Transform position, float damage)
    {
        OnGlobalDamageTaken?.Invoke(position, damage);
    }
}

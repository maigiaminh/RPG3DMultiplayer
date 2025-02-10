using System;
using UnityEngine;

public class MeteorHit : MonoBehaviour
{
    public int Damage {get; set;}
    
    private Collider[] targets;
    private void OnEnable() {
        ApplyDammage();    
    }

    private void ApplyDammage()
    {
        targets = Physics.OverlapSphere(transform.position, 1f, LayerMask.GetMask("Player"));
        foreach(var target in targets){
            target.TryGetComponent<IDamageable>(out var damageable);
            if(damageable == null) continue;
            Debug.Log("MeteorHit: ApplyDammage to " + damageable.GetTransform().name);
            damageable.TakeDamage(null, Damage);
        }

    }
}

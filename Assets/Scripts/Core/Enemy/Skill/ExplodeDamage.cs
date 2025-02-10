using System;
using System.Collections;
using UnityEngine;

public class ExplodeDamage : MonoBehaviour
{
    [HideInInspector] public int Damage;
    [HideInInspector] public float DelayTime;
    [HideInInspector] public float Radius;
    private void Start()
    {
        StartCoroutine(ApplyDamage());
    }

    IEnumerator ApplyDamage()
    {
        yield return new WaitForSeconds(DelayTime);

        Collider[] colliders = Physics.OverlapSphere(transform.position, Radius);
        foreach (Collider collider in colliders)
        {
            if (!collider.CompareTag("Player")) continue;
            if (!collider.TryGetComponent<IDamageable>(out IDamageable damageable)) continue;
            damageable.TakeDamage(transform, Damage);
        }
    }
}

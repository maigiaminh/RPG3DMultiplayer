using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantDamage : MonoBehaviour
{
    [HideInInspector] public float DelayTime = 0;
    [HideInInspector] public int Damage;
    [HideInInspector] public float DamageRate;

    private List<IDamageable> _damageables = new List<IDamageable>();

    private void Start()
    {
        StartCoroutine(ApplyDamage());
    }

    private IEnumerator ApplyDamage()
    {
        while (true)
        {
            yield return new WaitForSeconds(1 / DamageRate + DelayTime);
            DelayTime = 0;

            foreach (IDamageable damageable in _damageables)
            {
                damageable.TakeDamage(transform, Damage);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("Player")) return;
        if (!other.TryGetComponent<IDamageable>(out IDamageable damageable)) return;
        if (_damageables.Contains(damageable)) return;

        _damageables.Add(damageable);
    }

    private void OnTriggerExit(Collider other)
    {
        if(!other.CompareTag("Player")) return;
        if (!other.TryGetComponent<IDamageable>(out IDamageable damageable)) return;

        _damageables.Remove(damageable);
    }
}

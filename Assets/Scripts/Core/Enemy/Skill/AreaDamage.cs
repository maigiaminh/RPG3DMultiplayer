using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaDamage : MonoBehaviour
{
    public int damage;
    public float tickRate;
    private List<IDamageable> _damageable = new List<IDamageable>();
    private void OnEnable()
    {
        StartCoroutine(DealDamage());
    }

    private void OnDisable()
    {
        _damageable = null;
        StopAllCoroutines();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable == null) return;

        if (!_damageable.Contains(damageable))
        {
            _damageable.Add(damageable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (_damageable == null) return;
        
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (!_damageable.Contains(damageable)) return;
        _damageable.Remove(damageable);

    }

    private IEnumerator DealDamage()
    {
        WaitForSeconds Wait = new WaitForSeconds(tickRate);

        while (true)
        {
            if (_damageable != null && _damageable.Count > 0)
            {
                foreach (IDamageable damageable in _damageable)
                {
                    damageable.TakeDamage(transform, damage);
                }
            }
            yield return Wait;
        }
    }


}
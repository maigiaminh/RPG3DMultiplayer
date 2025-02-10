using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimingDamage : MonoBehaviour
{

    [HideInInspector] public List<float> timeToApplyDamage = new List<float>();
    [HideInInspector] public int Damage;
    [HideInInspector] public float Radius;


    private void Start()
    {
        StartCoroutine(ApplyDamage());
    }

    IEnumerator ApplyDamage()
    {
        for (int i = 0; i < timeToApplyDamage.Count; i++)
        {
            yield return new WaitForSeconds(timeToApplyDamage[i]);
            Collider[] colliders = Physics.OverlapSphere(transform.position, Radius);
            foreach (Collider collider in colliders)
            {
                if (!collider.CompareTag("Player")) continue;
                if (!collider.TryGetComponent<IDamageable>(out IDamageable damageable)) continue;
                damageable.TakeDamage(transform, Damage);

            }
        }


    }
}

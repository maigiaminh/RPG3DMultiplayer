using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Composites;

public class BurstDamage : MonoBehaviour
{
    public LayerMask impactLayer;
    public int damage = 50;
    public float explodeRadius = 3f;

    public float timeExisting = 3f;
    private bool _havingTarget = false;
    public Vector3 targetPoint = Vector3.zero;
    public event Action<BurstDamage> onExplode;


    private void OnEnable(){
        StartCoroutine(ExplodeTimer());
    }

    IEnumerator ExplodeTimer()
    {
        yield return new WaitForSeconds(timeExisting);

        Explode();
    }

    private void OnTriggerEnter(Collider other)
    {
        //  || (impactLayer.value & (1 << other.gameObject.layer)) != 0
        bool isTarget = other.CompareTag("Player") || other.CompareTag("CharacterSkill"); 
        if(!isTarget) return;

        _havingTarget = true;
        Explode();
    }

    private void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explodeRadius);


        foreach (Collider nearbyObject in colliders)
        {
            if(!nearbyObject.CompareTag("Player")) continue;
            if(nearbyObject.TryGetComponent<IDamageable>(out IDamageable target)){
                target.TakeDamage(transform, damage);
            }
        }
        onExplode?.Invoke(this);
    }
    private void OnDisable(){
        _havingTarget =  false;
    }


}

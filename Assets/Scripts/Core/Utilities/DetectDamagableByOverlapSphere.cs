using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class DetectDamagableByOverlapSphere : DetectDamageableInRange
{
    [SerializeField] private SphereCollider _sphereCollider;

    private void Awake()
    {
        if (!_sphereCollider) _sphereCollider = GetComponent<SphereCollider>();
    }

    private void OnEnable()
    {
        ManuallyTriggerEnter(_sphereCollider);
    }


    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
    }

    private void ManuallyTriggerEnter(SphereCollider sphereCollider)
    {
        float triggerRadius = sphereCollider.radius; // Sử dụng bán kính của SphereCollider
        Collider[] hitColliders = Physics.OverlapSphere(sphereCollider.transform.position, triggerRadius);
        foreach (var hitCollider in hitColliders)
        {
            OnTriggerEnter(hitCollider);
        }
    }

}

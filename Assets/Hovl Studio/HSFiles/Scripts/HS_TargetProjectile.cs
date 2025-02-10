using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HS_TargetProjectile : MonoBehaviour
{
    public float speed = 15f;
    public GameObject hit;
    public GameObject flash;
    public GameObject[] Detached;
    public bool LocalRotation = false;
    private Transform target;
    private Vector3 targetOffset;
    private float startDistanceToTarget;
    private int _damage;

    [Space]
    [Header("PROJECTILE PATH")]
    private float randomUpAngle;
    private float randomSideAngle;
    public float sideAngle = 25;
    public float upAngle = 20;

    [Space]
    [Header("FORWARD PROJECTILE SETTINGS")]
    public bool useForwardMode = false;
    public float forwardLifeTime = 3f;
    private Vector3 forwardDirection;

    void Start()
    {
        FlashEffect();
        newRandom();

        if (useForwardMode)
        {
            LaunchForward();
        }
    }

    void LaunchForward()
    {
        forwardDirection = Camera.main.transform.forward.normalized;

        StartCoroutine(ForwardProjectileLife());
    }

    IEnumerator ForwardProjectileLife()
    {
        float timer = 0f;
        while (timer < forwardLifeTime)
        {
            timer += Time.deltaTime;

            transform.Translate(forwardDirection * speed * Time.deltaTime, Space.World);
            transform.rotation = Quaternion.LookRotation(forwardDirection);

            yield return null;
        }

        HitTarget();
    }


    void newRandom()
    {
        randomUpAngle = Random.Range(0, upAngle);
        randomSideAngle = Random.Range(-sideAngle, sideAngle);
    }

    //Link from another script
    //TARGET POSITION + TARGET OFFSET
    public void UpdateTarget(Transform targetPosition , Vector3 Offset)
    {
        target = targetPosition;
        targetOffset = Offset;
        startDistanceToTarget = Vector3.Distance((target.position + targetOffset), transform.position);
    }

    void Update()
    {
        if (useForwardMode) return; 

        if (target == null)
        {
            foreach (var detachedPrefab in Detached)
            {
                if (detachedPrefab != null)
                {
                    detachedPrefab.transform.parent = null;
                }
            }
            Destroy(gameObject);
            return;
        }

        float distanceToTarget = Vector3.Distance((target.position + targetOffset), transform.position);
        float angleRange = (distanceToTarget - 10) / 60;
        if (angleRange < 0) angleRange = 0;

        float saturatedDistanceToTarget = (distanceToTarget / startDistanceToTarget);
        if (saturatedDistanceToTarget < 0.5)
            saturatedDistanceToTarget -= (0.5f - saturatedDistanceToTarget);
        saturatedDistanceToTarget -= angleRange;
        if (saturatedDistanceToTarget <= 0)
            saturatedDistanceToTarget = 0;

        Vector3 forward = ((target.position + targetOffset) - transform.position);
        Vector3 crossDirection = Vector3.Cross(forward, Vector3.up);
        Quaternion randomDeltaRotation = Quaternion.Euler(0, randomSideAngle*saturatedDistanceToTarget, 0) * Quaternion.AngleAxis(randomUpAngle * saturatedDistanceToTarget, crossDirection);
        Vector3 direction = randomDeltaRotation * forward;

        float distanceThisFrame = Time.deltaTime * speed;

        if (direction.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(direction.normalized * distanceThisFrame, Space.World);
        transform.rotation = Quaternion.LookRotation(direction);
    }

    void FlashEffect()
    {
        if (flash != null)
        {
            var flashInstance = Instantiate(flash, transform.position, Quaternion.identity);
            flashInstance.transform.forward = gameObject.transform.forward;
            var flashPs = flashInstance.GetComponent<ParticleSystem>();
            if (flashPs != null)
            {
                Destroy(flashInstance, flashPs.main.duration);
            }
            else
            {
                var flashPsParts = flashInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(flashInstance, flashPsParts.main.duration);
            }
        }
    }

    void HitTarget()
    {
        if (hit != null)
        {
            var hitRotation = transform.rotation;
            if (LocalRotation == true)
            {
                hitRotation = Quaternion.Euler(0, 0, 0);
            }
            var hitInstance = Instantiate(hit, (target != null ? target.position : transform.position)  + targetOffset , hitRotation);
            var hitPs = hitInstance.GetComponent<ParticleSystem>();
            if (hitPs != null)
            {
                Destroy(hitInstance, hitPs.main.duration);
            }
            else
            {
                var hitPsParts = hitInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(hitInstance, hitPsParts.main.duration);
            }
        }
        foreach (var detachedPrefab in Detached)
        {
            if (detachedPrefab != null)
            {
                detachedPrefab.transform.parent = null;
                Destroy(detachedPrefab, 1);
            }
        }
        Destroy(gameObject);
    }

    void OnParticleCollision(GameObject other)
    {
        HitTarget();

        if(other == null) return;
        if (!other.TryGetComponent(out IDamageable damageable)) return;
        if (!other.CompareTag("Enemy")) return;
        
        damageable.TakeDamage(transform,_damage);
    }

    public void SetDamage(int damage){
        _damage = damage;
    }
}

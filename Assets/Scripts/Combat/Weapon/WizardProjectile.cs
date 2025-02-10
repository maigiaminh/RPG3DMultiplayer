using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardProjectile : MonoBehaviour
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
    public ProjectilePool _parent;

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
    public Vector3 forwardDirection;
    private float timer = 0f;


    void OnEnable()
    {
        FlashEffect();
        newRandom();
    }

    void newRandom()
    {
        randomUpAngle = Random.Range(0, upAngle);
        randomSideAngle = Random.Range(-sideAngle, sideAngle);
    }

    public void UpdateTarget(Transform targetPosition , Vector3 Offset)
    {
        target = targetPosition;
        targetOffset = Offset;
        startDistanceToTarget = Vector3.Distance((target.position + targetOffset), transform.position);
    }

    void Update()
    {
        if (useForwardMode){
            if (timer < forwardLifeTime){
                timer += Time.deltaTime;

                transform.Translate(forwardDirection * speed * Time.deltaTime, Space.World);
                transform.rotation = Quaternion.LookRotation(forwardDirection);

            }
            else{
                timer = 0;
                HitTarget();
            }

            return;
        } 

        if (target == null)
        {
            foreach (var detachedPrefab in Detached)
            {
                if (detachedPrefab != null)
                {
                    var detachedGO = Instantiate(detachedPrefab);
                    Destroy(detachedGO, 1);
                    //detachedPrefab.transform.parent = null;
                }
            }
            ReturnToPool();
            // Destroy(gameObject);
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

            if (target.TryGetComponent(out IDamageable damageable) && target.tag == "Enemy"){
                damageable.TakeDamage(transform,_damage);
                SoundManager.PlaySound(SoundType.WIZARDSTAFF_HIT, target.GetComponent<AudioSource>());
            }

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
                //detachedPrefab.transform.parent = null;
                var detachedGO = Instantiate(detachedPrefab);
                Destroy(detachedGO, 1);
            }
        }

        ReturnToPool();
        // Destroy(gameObject);
    }

    void OnParticleCollision(GameObject other)
    {
        if(other == null) return;
        if (!other.TryGetComponent(out IDamageable damageable)) return;
        if (!other.CompareTag("Enemy")) return;
        HitTarget();
        damageable.TakeDamage(transform,_damage);
        SoundManager.PlaySound(SoundType.WIZARDSTAFF_HIT, other.GetComponent<AudioSource>());
    }

    public void SetDamage(int damage){
        _damage = damage;
    }

    public void SetParent(ProjectilePool parent){
        _parent = parent;
    }

    
    void ReturnToPool()
    {
        _parent.ReturnProjectile(this);
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Pool;
using System;
using Unity.VisualScripting;

public class HS_ParticleCollisionInstance : MonoBehaviour
{
    public GameObject[] EffectsOnCollision; // Prefabs to instantiate
    public float DestroyTimeDelay = 1; // Time to deactivate the pooled object
    public bool UseWorldSpacePosition;
    public float Offset = 0;
    public Vector3 rotationOffset = new Vector3(0, 0, 0);
    public bool useOnlyRotationOffset = true;
    public bool UseFirePointRotation;
    public bool DestroyMainEffect = false;

    private ParticleSystem part;
    private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();



    private VFXCollisionManager _vfxCollisionManager;


    private void Awake()
    {
        part = GetComponent<ParticleSystem>();
        _vfxCollisionManager = VFXCollisionManager.Instance;
        _vfxCollisionManager.AddCollisionPool(EffectsOnCollision[0], 1f);

    }
    void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);

        for (int i = 0; i < numCollisionEvents; i++)
        {
            foreach (var effectPrefab in EffectsOnCollision)
            {
                // Get object from the pool
                var instance = _vfxCollisionManager.CollisionPool[EffectsOnCollision[0]].Get();

                // Set position and rotation
                instance.transform.position = collisionEvents[i].intersection + collisionEvents[i].normal * Offset;

                if (UseFirePointRotation)
                {
                    instance.transform.LookAt(transform.position);
                }
                else if (rotationOffset != Vector3.zero && useOnlyRotationOffset)
                {
                    instance.transform.rotation = Quaternion.Euler(rotationOffset);
                }
                else
                {
                    instance.transform.LookAt(collisionEvents[i].intersection + collisionEvents[i].normal);
                    instance.transform.rotation *= Quaternion.Euler(rotationOffset);
                }

                if (!UseWorldSpacePosition)
                {
                    instance.transform.parent = transform;
                }

                // Return the object to the pool after the delay

            }
        }

        if (DestroyMainEffect)
        {
            Destroy(gameObject, DestroyTimeDelay + 0.5f);
        }
    }

}

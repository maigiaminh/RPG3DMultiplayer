using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float timeToDestroy = 1f;

    private void Start() {
        Invoke(nameof(DestroyAfterTimeCoroutine), timeToDestroy);
    }

    private void DestroyAfterTimeCoroutine()
    {
        Destroy(gameObject);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

public class VFXCollisionManager : Singleton<VFXCollisionManager>
{
    public Dictionary<GameObject, ObjectPool<GameObject>> CollisionPool = new Dictionary<GameObject, ObjectPool<GameObject>>();
    private Dictionary<GameObject, Coroutine> activeCoroutines = new Dictionary<GameObject, Coroutine>();

    protected override void Awake()
    {
        base.Awake();
        CollisionPool = new Dictionary<GameObject, ObjectPool<GameObject>>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void AddCollisionPool(GameObject prefab, float destroyTimeDelay)
    {
        if (prefab == null || CollisionPool.ContainsKey(prefab)) return;

        CollisionPool.Add(prefab, new ObjectPool<GameObject>(
            createFunc: () => Instantiate(prefab),
            actionOnGet: obj =>
            {
                obj.SetActive(true);

                // Start a Coroutine for deactivation if not already running
                if (!activeCoroutines.ContainsKey(obj))
                {
                    activeCoroutines[obj] = StartCoroutine(DeactivateAfterDelay(prefab, obj, destroyTimeDelay));
                }
            },
            actionOnRelease: obj =>
            {
                obj.SetActive(false);

                // Stop Coroutine if it exists
                if (activeCoroutines.ContainsKey(obj))
                {
                    StopCoroutine(activeCoroutines[obj]);
                    activeCoroutines.Remove(obj);
                }
            },
            actionOnDestroy: obj => Destroy(obj),
            defaultCapacity: 50,
            maxSize: 200
        ));
    }

    private IEnumerator DeactivateAfterDelay(GameObject prefab, GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);

        // Return the object to the pool
        if (CollisionPool.TryGetValue(prefab, out var pool))
        {
            pool.Release(obj);
        }

        // Remove the Coroutine reference
        if (activeCoroutines.ContainsKey(obj))
        {
            activeCoroutines.Remove(obj);
        }
    }

    public GameObject GetFromPool(GameObject prefab)
    {
        if (CollisionPool.TryGetValue(prefab, out var pool))
        {
            return pool.Get();
        }
        Debug.LogWarning($"Prefab {prefab.name} not found in CollisionPool.");
        return null;
    }

    public void ReleaseToPool(GameObject prefab, GameObject instance)
    {
        if (CollisionPool.TryGetValue(prefab, out var pool))
        {
            pool.Release(instance);
        }
        else
        {
            Debug.LogWarning($"Prefab {prefab.name} not found in CollisionPool.");
        }
    }



    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        foreach (var pool in CollisionPool)
        {
            pool.Value.Clear();
        }

        foreach(var coroutine in activeCoroutines)
        {
            StopCoroutine(coroutine.Value);
        }
        activeCoroutines.Clear();
        CollisionPool.Clear();


    }

}

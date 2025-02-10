using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class RarityLightVFXManager : Singleton<RarityLightVFXManager>
{
    [SerializeField] private RarityVFX[] rarityVFXs;
    private Dictionary<ItemData.Rarity, ObjectPool<RarityVFX>> rarityVFXPools = new Dictionary<ItemData.Rarity, ObjectPool<RarityVFX>>();


    protected override void Awake()
    {
        base.Awake();
        foreach (RarityVFX rvfx in rarityVFXs)
        {
            if (!rarityVFXPools.ContainsKey(rvfx.rarity)) // Prevent duplicate entries
            {
                rarityVFXPools.Add(rvfx.rarity, new ObjectPool<RarityVFX>(
                    createFunc: () => Instantiate(rvfx),
                    actionOnGet: vfx => vfx.gameObject.SetActive(true),
                    actionOnRelease: vfx => vfx.gameObject.SetActive(false),
                    actionOnDestroy: vfx => Destroy(vfx),
                    collectionCheck: false,
                    defaultCapacity: 20,
                    maxSize: 100
                ));
            }
            else
            {
                Debug.LogWarning($"Duplicate rarity VFX entry detected for {rvfx.rarity}.  Ignoring.");
            }

        }
    }


    public RarityVFX GetVFX(ItemData.Rarity rarity, Vector3 position, Quaternion rotation)
    {
        if (rarityVFXPools.TryGetValue(rarity, out var pool))
        {
            Debug.Log($"Getting VFX for rarity {rarity}");
            RarityVFX vfx = pool.Get();
            vfx.transform.position = position;
            vfx.transform.rotation = rotation;
            return vfx;
        }
        else
        {
            Debug.LogError($"No VFX pool found for rarity: {rarity}");
            return null;
        }
    }

    public void ReleaseVFX(ItemData.Rarity rarity, RarityVFX vfx)
    {
        Debug.Log($"Releasing VFX for rarity {rarity}");
        if (rarityVFXPools.TryGetValue(rarity, out var pool))
        {
            pool.Release(vfx);
        }
        else
        {
            Debug.LogWarning($"No VFX pool found for rarity {rarity} when releasing.  Destroying the VFX.");
            Destroy(vfx); // Or handle differently
        }

    }
}
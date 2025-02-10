using System;
using System.Collections;
using UnityEngine;

public class PlayerLevelUpVFX : MonoBehaviour
{
    public GameObject smallLevelUpVFX;
    public GameObject bigLevelUpVFX;

    private Coroutine _smallLevelUpVFXCoroutine;
    private Coroutine _bigLevelUpVFXCoroutine;
    private const float VFXDuration = .8f;

    private void OnEnable()
    {
        GameEventManager.Instance.PlayerEvents.OnPlayerLevelChange += HanldePlayerChanged;
    }

    private void OnDisable()
    {
        GameEventManager.Instance.PlayerEvents.OnPlayerLevelChange -= HanldePlayerChanged;
    }

    private void HanldePlayerChanged(int level)
    {
        if(level % 5 == 0)
        {
            if(_bigLevelUpVFXCoroutine != null) StopCoroutine(_bigLevelUpVFXCoroutine);
            
            _bigLevelUpVFXCoroutine = StartCoroutine(DisableVFX(bigLevelUpVFX));
        }
        else
        {
            if(_smallLevelUpVFXCoroutine != null) StopCoroutine(_smallLevelUpVFXCoroutine);
            _smallLevelUpVFXCoroutine = StartCoroutine(DisableVFX(smallLevelUpVFX));
        }
    }

    IEnumerator DisableVFX(GameObject vfxGO)
    {
        vfxGO.SetActive(true);
        yield return new WaitForSeconds(VFXDuration);
        vfxGO.SetActive(false);
        _smallLevelUpVFXCoroutine = null;
        _bigLevelUpVFXCoroutine = null;
    }
}

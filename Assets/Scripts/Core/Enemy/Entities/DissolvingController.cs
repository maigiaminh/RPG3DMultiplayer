using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DissolvingController
{
    private Enemy _enemy;
    private float _dissolveTime;
    private GameObject _dissolveEffect;

    public DissolvingController(Enemy enemy, float dissolveTime, GameObject dissolveEffect)
    {
        _enemy = enemy;
        _dissolveTime = dissolveTime;
        _dissolveEffect = dissolveEffect;
    }


    public void Dissolve()
    {
        if (_dissolveEffect != null)
        {
            _dissolveEffect.SetActive(true);
        }
        if (_enemy.dissolveCoroutine != null)
        {
            _enemy.StopCoroutine(_enemy.dissolveCoroutine);
        }
        _enemy.dissolveCoroutine = _enemy.StartCoroutine(DissolveCoroutine());
    }


    private IEnumerator DissolveCoroutine()
    {
        yield return new WaitForSeconds(_dissolveTime);
        _dissolveEffect.SetActive(false);
        _enemy.EnemyDestroy();
    }


}

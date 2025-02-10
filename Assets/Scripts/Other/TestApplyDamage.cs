using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestApplyDamage : MonoBehaviour
{
    [SerializeField] private Enemy enemy;
    private TankPlayer player;
    public int damage = 50;
    private void Awake() {
        player = FindAnyObjectByType<TankPlayer>();
    }
    private void Start() {
        StartCoroutine(ApplyDamage());
    }

    IEnumerator ApplyDamage()
    {
        while(true){
            yield return new WaitForSeconds(2);
            enemy.TakeDamage(player.GetComponent<Transform>(), damage);
        }
    }

}

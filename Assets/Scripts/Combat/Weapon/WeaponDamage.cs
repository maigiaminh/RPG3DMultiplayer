using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDamage : MonoBehaviour
{
    [SerializeField] private Collider myCollider;
    public int _damage;
    private float _knockback;
    private List<Collider> alreadyCollidedWith = new List<Collider>();
    private int _weaponType;

    private void OnTriggerEnter(Collider other)
    {
        if (other == myCollider) return; 
        if (other == null) return;
        if (alreadyCollidedWith.Contains(other)) return;
        if (!other.TryGetComponent(out IDamageable damageable)) return;
        if (!other.CompareTag("Enemy")) return;
        
        damageable.TakeDamage(transform, _damage);
        alreadyCollidedWith.Add(other);

        var sound = SoundManager.GetAttackHitSound(_weaponType);
        SoundManager.PlaySound(sound, other.GetComponent<AudioSource>());
        

        if(other.TryGetComponent<ForceReceiver>(out ForceReceiver forceReceiver))
        {
            Vector3 direction = (other.transform.position - myCollider.transform.position).normalized;
            forceReceiver.AddForce(direction * _knockback);
        }

    }

    public void SetAttack(int damage, float knockback, int weaponType){
        _damage = damage;
        _knockback = knockback;
        _weaponType = weaponType;
    }

    private void OnEnable() {
        alreadyCollidedWith.Clear();
    }
}

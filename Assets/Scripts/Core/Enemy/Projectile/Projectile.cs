using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Projectile : MonoBehaviour
{
    [SerializeField] private ProjectileData _projectileData;
    // [SerializeField] private GameObject _explosionEffect;
    [SerializeField] private bool isPenetration  = false;
    private TrailRenderer _trailRenderer;
    private Rigidbody _rigidbody;
    private Collider _collider;


    private string _projectileName;
    private int _damage;
    private float _speed;
    private float _existingTime;


    private ObjectPool<Projectile> _pool;

    private bool _isEarlyPhase = false;
    private bool _isMidPhase = false;
    private bool _isLatePhase = false;
    protected Transform _target;

    private const float gravity = -9.81f;

    private bool _isReleased = false;
    private void Awake()
    {
        SetUpProjectile();
    }
    private void OnEnable()
    {
        CancelInvoke(nameof(Disable));
        transform.forward = _rigidbody.linearVelocity.normalized;
        _collider.enabled = true;
        _isReleased = false;
        if (_trailRenderer) _trailRenderer.enabled = true;
        Invoke(nameof(Disable), _existingTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other == null) return;
        if (!other.TryGetComponent(out IDamageable damageable)) return;
        if(!other.CompareTag("Player") && other.CompareTag("CharacterSkill")) return;
        

        damageable.TakeDamage(transform,_damage);
        StartCoroutine(Disable());  

        if(isPenetration) return;

        // if(_explosionEffect){
        //     _explosionEffect.SetActive(true);
        // }
        
    }

    public virtual void Spawn(int damage, Transform target)
    {
        Vector3 newTarget = target.position + new Vector3(0, 1, 0);
        Vector3 directionToTarget = (newTarget - transform.position).normalized;

        transform.rotation = Quaternion.LookRotation(directionToTarget);
        if(_rigidbody != null){
            _rigidbody.AddForce(directionToTarget * _speed, ForceMode.VelocityChange);  // Sử dụng VelocityChange thay vì Impulse để điều khiển chính xác hơn
        }

        _damage += damage;
        this._target = target;
    }




    private void SetUpProjectile()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        // _explosionEffect.SetActive(false);
        // _trailRenderer = GetComponent<TrailRenderer>();
        _isEarlyPhase = _projectileData.earlyPhaseEffect != null;
        _isMidPhase = _projectileData.midPhaseEffect != null;
        _isLatePhase = _projectileData.latePhaseEffect != null;
        _projectileName = _projectileData.projectileName;
        _damage = _projectileData.damage;
        _speed = _projectileData.speed;
        _existingTime = _projectileData.existingTime;
    }

    public void SetPool(ObjectPool<Projectile> pool)
    {
        _pool = pool;

    }

    public IEnumerator Disable()
    {
        yield return new WaitForSeconds(.3f);
        if (_isReleased) yield break;
        if(!gameObject) yield break;
        _isReleased = true;
        if(_pool != null)
            _pool.Release(this);
        else 
            Debug.LogWarning("Pool is null");
        _rigidbody.linearVelocity = Vector3.zero;
        _collider.enabled = false;
        // _explosionEffect.SetActive(false);
        if (_trailRenderer) _trailRenderer.enabled = false;
    }





}

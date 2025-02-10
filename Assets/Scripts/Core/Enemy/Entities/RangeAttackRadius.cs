using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class RangedAttackRadius : AttackRadius
{
    public Projectile projectilePrefab;
    public Vector3 BulletSpawnOffset = new Vector3(0, 1, 0);
    public LayerMask Mask { get; set; }
    private ObjectPool<Projectile> _projectilePool;
    // private Dictionary<Projectile, ObjectPool<Projectile>> _projectilePools = new Dictionary<Projectile, ObjectPool<Projectile>>();
    [SerializeField]
    private float SpherecastRadius = 0.1f;
    private RaycastHit _Hit;
    private IDamageable _targetDamageable;
    private Projectile _bullet;

    private Vector3 _projectileSpawnpoint;

    private const string _projectileLayer = "Projectile";
    protected override void Awake()
    {
        base.Awake();
    }

    public void CreateBulletPool()
    {
        _projectilePool = new ObjectPool<Projectile>(
            CreateProjectile,
            OnTakeProjectileFromPool,
            OnReturnProjectileFromPool,
            OnDestroyProjectile,
            false,
            100,
            2000
        );
    }


    protected override void Attack()
    {
        if (_damageablesList.Count == 0)
        {
            Debug.Log("No Damageables in List");
            return;
        }

        for (int i = 0; i < _damageablesList.Count; i++)
        {
            // if (HasLineOfSightTo(_damageablesList[i].GetTransform()))
            // {
            _targetDamageable = _damageablesList[i];
            OnAttack?.Invoke(_damageablesList[i]);
            break;
            // }
        }
        Debug.Log("----- Target Damageable: " + _targetDamageable);
        if (_targetDamageable != null)
        {
            Debug.Log("----- Attacking Target -----");
            Projectile projectile = _projectilePool.Get();
            projectile.gameObject.layer = LayerMask.NameToLayer(_projectileLayer);
            if (projectile != null)
            {
                _bullet = projectile.GetComponent<Projectile>();

                _bullet.transform.position = transform.position + BulletSpawnOffset;

                _bullet.Spawn(damage, _targetDamageable.GetTransform());
            }
        }

        _damageablesList.RemoveAll(DisabledDamageables);
    }

    private bool HasLineOfSightTo(Transform Target)
    {
        if (Physics.SphereCast(transform.position + BulletSpawnOffset, SpherecastRadius, ((Target.position + BulletSpawnOffset) - (transform.position + BulletSpawnOffset)).normalized, out _Hit, Collider.radius, Mask))
        {
            IDamageable damageable;
            if (_Hit.collider.TryGetComponent<IDamageable>(out damageable))
            {
                return damageable.GetTransform() == Target;
            }
        }

        return false;
    }

    public void Initialize(Transform projectileSpawnpoint, LayerMask mask)
    {
        _projectileSpawnpoint = projectileSpawnpoint.position;
        Mask = mask;
    }


    #region  Projectile Pool Methods


    private Projectile CreateProjectile()
    {
        var projectile = Instantiate(projectilePrefab);
        projectile.SetPool(_projectilePool);
        // projectile.transform.SetParent(transform);
        return projectile;
    }
    private void OnTakeProjectileFromPool(Projectile projectile)
    {
        projectile.gameObject.SetActive(true);
    }

    private void OnReturnProjectileFromPool(Projectile projectile)
    {
        projectile.gameObject.SetActive(false);
    }
    private void OnDestroyProjectile(Projectile projectile)
    {
        Destroy(projectile.gameObject);
    }

    #endregion
}
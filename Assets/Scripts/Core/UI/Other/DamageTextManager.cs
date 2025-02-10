using UnityEngine;
using UnityEngine.Pool;

public class DamageTextManager : MonoBehaviour
{
    public static DamageTextManager Instance; // Singleton để dễ truy cập
    public DamageText damageTextPrefab; // Prefab của DamageText

    private IObjectPool<DamageText> pool;

    private void Awake()
    {
        // Singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Tạo pool với các callback
        pool = new ObjectPool<DamageText>(
            CreateDamageText,      // Tạo mới DamageText
            OnTakeFromPool,        // Khi lấy từ pool
            OnReturnedToPool,      // Khi trả về pool
            OnDestroyPoolObject,   // Khi hủy object trong pool
            false,                 // Không giới hạn số lượng trong pool
            10,                    // Kích thước mặc định
            50                     // Kích thước tối đa
        );
    }

    private void OnEnable() {
        GameEventManager.Instance.DamageEvent.OnGlobalDamageTaken += ShowDamage;
    }

    private void OnDisable() {
        GameEventManager.Instance.DamageEvent.OnGlobalDamageTaken -= ShowDamage;
    }

    private DamageText CreateDamageText()
    {
        DamageText damageText = Instantiate(damageTextPrefab);
        damageText.SetPool(pool); // Gán pool cho DamageText
        return damageText;
    }

    private void OnTakeFromPool(DamageText damageText)
    {
        damageText.gameObject.SetActive(true);
    }

    private void OnReturnedToPool(DamageText damageText)
    {
        damageText.gameObject.SetActive(false);
    }

    private void OnDestroyPoolObject(DamageText damageText)
    {
        Destroy(damageText.gameObject);
    }

    public void ShowDamage(Transform damagableTrans, float damage)
    {
        DamageText damageText = pool.Get();
        damageText.transform.SetParent(damagableTrans);
        damageText.SetDamage(damage);
    }
}

using UnityEngine;
using TMPro;
using UnityEngine.Pool;

public class DamageText : MonoBehaviour
{
    public TextMeshPro damageText;
    public float lifetime = 1f; // Thời gian tồn tại của text
    public float moveSpeed = 2f; // Tốc độ bay lên
    public float fadeSpeed = 2f; // Tốc độ fade out

    private Vector3 moveDirection = new Vector3(0, 1, 0); // Di chuyển lên trên
    private float timeElapsed;

    private IObjectPool<DamageText> pool;


    public void SetPool(IObjectPool<DamageText> objectPool)
    {
        pool = objectPool; // Gán pool quản lý
    }

    public void SetDamage(float damage)
    {
        damageText.text = damage.ToString();
        timeElapsed = 0f; // Reset thời gian
        transform.localPosition = Vector3.zero + Vector3.up; // Đặt lại vị trí nếu cần
    }

    private void Update()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // Fade out text

        // Trả lại pool sau khi hết thời gian
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= lifetime)
        {
            pool.Release(this);
        }
    }

    public void Reset()
    {
        timeElapsed = 0f;       // Đặt lại thời gian
        gameObject.SetActive(true); // Kích hoạt lại
    }
}

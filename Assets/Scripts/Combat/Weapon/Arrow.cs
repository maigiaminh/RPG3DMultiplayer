using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float lifetime = 5f; 
    public float speedForce = 20f;
    private float timer;
    private int _damage;
    private ProjectilePool _parent;
    public AudioSource AudioSource { get; set;}
    void OnEnable()
    {
        timer = lifetime;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            ReturnToPool();
        }
    }

    void ReturnToPool()
    {
        _parent.ReturnArrow(this);
    }

    private void OnTriggerEnter(Collider other) {

        if(other == null) return;
        if (!other.TryGetComponent(out IDamageable damageable)) return;
        if (!other.CompareTag("Enemy")) return;
        
        damageable.TakeDamage(transform,_damage);
        SoundManager.PlaySound(SoundType.ARROW_HIT, other.GetComponent<AudioSource>());
        ReturnToPool();
    }

    public void SetDamage(int damage){
        _damage = damage;
    }

    public void SetParent(ProjectilePool parent){
        _parent = parent;
    }

    public void SetAudioSource(AudioSource audioSource){
        AudioSource = audioSource;
        AudioSource.playOnAwake = false;
        AudioSource.maxDistance = 1f;
        AudioSource.spatialBlend = 1.0f;
    }

    void OnDrawGizmos()
    {
        if (AudioSource == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, AudioSource.minDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AudioSource.maxDistance);
    }
}

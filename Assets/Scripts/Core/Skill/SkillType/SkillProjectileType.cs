using UnityEngine;

public class SkillProjectileType : CharacterSkillBase
{
    private float _speed = 0;
    private Vector3 _direction = Vector3.zero;
    private float _delayTime = 0;
    private void Update()
    {
        if(_delayTime > 0)
        {
            _delayTime -= Time.deltaTime;
            return;
        }
        if (_direction != Vector3.zero)
            transform.position += _direction * _speed * Time.deltaTime;
    }


    public override void Execute(Transform spawnPos, Vector3 direct, CharacterSkillScriptableObject data)
    {
        base.Execute(spawnPos, direct, data);
        _direction = transform.forward;
        _speed = data.Speed;
        _delayTime = data.DelayTime;
    }


    public void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<IDamageable>(out var damageable)) return;
        if(other.GetComponent<SkillManager>() == _skillManager) return;
        if (!other.CompareTag("Enemy")) return;
        if(isReleased) return;

        damageable.TakeDamage(transform, _damage);
        _skillManager.ReleaseSkill(_skillData, this);
        isReleased = true;

    }
}

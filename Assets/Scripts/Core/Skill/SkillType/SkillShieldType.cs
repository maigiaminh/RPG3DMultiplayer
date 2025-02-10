using UnityEngine;

public class SkillShieldType : CharacterSkillBase
{
    private int _blockRate;


    protected override void ConfigSkillBase(CharacterSkillScriptableObject data)
    {
        base.ConfigSkillBase(data);
        _blockRate = data.BlockRate;
    }

    public override void Execute(Transform spawnPos, Vector3 direct, CharacterSkillScriptableObject data)
    {
        base.Execute(spawnPos, direct, data);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("EnemySkill")) return;

        if (Random.Range(0, 100) < _blockRate)
        {
            _skillManager.transform.GetComponent<TankPlayer>().TakeDamage(transform, _damage);
        }
    }


}

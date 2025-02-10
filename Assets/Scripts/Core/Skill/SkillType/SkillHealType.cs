using UnityEngine;

public class SkillHealType : CharacterSkillBase
{
    private int _healAmount;

    protected override void ConfigSkillBase(CharacterSkillScriptableObject data)
    {
        base.ConfigSkillBase(data);
        _healAmount = data.HealAmount;
    }

    public override void Execute(Transform spawnPos, Vector3 direct, CharacterSkillScriptableObject data)
    {
        base.Execute(spawnPos, direct, data);
    }

    public void ApplyHealEffect(){
        _skillManager.transform.GetComponent<TankPlayer>().TakeDamage(transform, _healAmount);
    }
}

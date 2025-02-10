using UnityEngine;

public class SkillImmuneType : CharacterSkillBase
{
    private float _immuneTime;

    protected override void ConfigSkillBase(CharacterSkillScriptableObject data)
    {
        base.ConfigSkillBase(data);
        _immuneTime = data.ImmuneTime;
    }
    public override void Execute(Transform spawnPos, Vector3 direct, CharacterSkillScriptableObject data)
    {
        base.Execute(spawnPos, direct, data);
        transform.localScale = Vector3.one;
    }

}

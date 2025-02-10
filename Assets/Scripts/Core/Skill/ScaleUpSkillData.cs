using UnityEngine;

[CreateAssetMenu(fileName = "ScaleUpSkillData", menuName = "Player/ScaleUpSkillData", order = 1)]
public class ScaleUpSkillData : ScriptableObject
{
    public AnimationCurve CooldownReduceCurve;
    public AnimationCurve DurationIncreaseCurve;    
    public AnimationCurve DamageIncreaseCurve;
    public AnimationCurve ManaCostIncreaseCurve;
    public AnimationCurve ExperienceToNextLevelIncreaseCurve;
    public AnimationCurve SpeedIncreaseCurve;
    public AnimationCurve AreaRadiusIncreaseCurve;
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScalingEnemyData", menuName = "Enemy/ScalingEnemyData", order = 0)]
public class ScalingEnemyData : ScriptableObject
{
    public AnimationCurve HealthCurve;
    public AnimationCurve AttackValueCurve;
    public AnimationCurve AttackRangeCurve;
    public AnimationCurve SpeedCurve;
    public AnimationCurve XPCurve;
    public AnimationCurve GoldCurve;

}

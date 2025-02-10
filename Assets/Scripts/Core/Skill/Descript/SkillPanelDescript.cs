using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillPanelDescript", menuName = "Player/Skill/SkillPanelDescript")]
public class SkillPanelDescript : ScriptableObject
{
    [Tooltip("Descript And Icon Must Be In The Same Order AND Length")]
    [Header("Skill Data")]
    public List<string> SkillDescripts = new List<string>();
    public List<Sprite> SkillIcons = new List<Sprite>();
}

using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSkillVisualEffect", menuName = "Player/Skill/PlayerSkillVisualEffect", order = 1)]
public class PlayerSkillVisualEffect : ScriptableObject
{
    [field: SerializeField] public float Duration { get; set;}
    [field: SerializeField] public GameObject VisualEffectPrefab { get; set; }
}

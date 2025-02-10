using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterDescript", menuName = "Character Descript", order = 1)]
public class CharacterDescript : ScriptableObject
{
    [Header("Character Stats")]
    public Sprite CharacterIcon;
    public string CharacterName;
    [Header("Skill Descriptions")]
    [TextArea(3, 10)]
    public string Skill1Description;
    public Sprite Skill1Icon;
    [TextArea(3, 10)]
    public string Skill2Description;
    public Sprite Skill2Icon;
    [TextArea(3, 10)]
    public string Skill3Description;
    public Sprite Skill3Icon;
    [TextArea(3, 10)]
    public string Skill4Description;
    public Sprite Skill4Icon;
    public Color SkillBackgroundColor;
    public Color IconBackgroundColor;

    [Header("Character Descriptions List")]
    public List<string> CharacterDescriptionsList = new List<string>();

    private void OnValidate()
    {
#if UNITY_EDITOR
        CharacterName = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

}


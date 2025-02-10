using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStats", menuName = "Character/CharacterStats")]
public class CharacterStatsSO : ScriptableObject
{
    public string CharacterName;
    
    [Header("Character Base Stats")]
    public int Strength;
    public int MaxHealth;
    public int MaxMana;
    public int Stamina;
    public int Defense;
    public int Agility;
    public int Intelligence;
    public int Luck;
    [Header("Character Delta Stats")]
    public int StrengthDelta;
    public int MaxHealthDelta;
    public int MaxManaDelta;
    public int StaminaDelta;
    public int DefenseDelta;
    public int AgilityDelta;
    public int IntelligenceDelta;
    public int LuckDelta;

    private void OnValidate()
    {
#if UNITY_EDITOR

        CharacterName = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }


}

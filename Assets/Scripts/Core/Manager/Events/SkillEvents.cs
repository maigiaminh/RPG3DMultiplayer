using System;
using UnityEngine;

public class SkillEvents 
{
    public event Action<CharacterSkillScriptableObject, int> OnSkillUsed;
    public void SkillUsed(CharacterSkillScriptableObject skill, int index)
    {
        OnSkillUsed?.Invoke(skill, index);
    }

    public event Action<CharacterSkillScriptableObject, int, bool> OnSkillUpgrade;
    public void SkillUpgrade(CharacterSkillScriptableObject skill, int amount, bool isInitialize)
    {
        OnSkillUpgrade?.Invoke(skill, amount, isInitialize);
    }

}


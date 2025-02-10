using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillUIManager : Singleton<SkillUIManager>
{
    public List<SkillCooldownItem> skillCooldownItems = new List<SkillCooldownItem>();
    public SkillUIPanel skillUIPanel;

    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        GameEventManager.Instance.SkillEvents.OnSkillUsed += OnSkillUsed;
    }
    private void OnDisable()
    {
        GameEventManager.Instance.SkillEvents.OnSkillUsed -= OnSkillUsed;
    }

    public void InitalizeSkillLevel(List<int> levelsRequired)
    {
        for (int i = 0; i < levelsRequired.Count; i++)
        {
            skillCooldownItems[i].Initialize(levelsRequired[i]);
        }
    }

    private void OnSkillUsed(CharacterSkillScriptableObject data, int index)
    {
        Debug.Log("Skill used: " + data.SkillName + " at index: " + index);
        skillCooldownItems[index].StartCooldown(data.Cooldown);
    }

    public void ActivateSkillPanel(bool isActive)
    {
        skillUIPanel.gameObject.SetActive(isActive);
    }



}

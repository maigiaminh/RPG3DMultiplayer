using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillUIPanel : MonoBehaviour
{
    public List<SkillUIPanelItem> skillUIPanelItems = new List<SkillUIPanelItem>();
    private List<SkillPanelDescript> _skillPanelDescripts = new List<SkillPanelDescript>();
    private List<CharacterSkillScriptableObject> _skillDatas = new List<CharacterSkillScriptableObject>();
    public Transform skillDescriptionParent;

    private SkillPanelDescript _currentSkillPanelDescript;
    public SkillPanelDescriptItem SkillPanelDescriptItemPrefab;


    private void Start()
    {
        InitializeItemButton();
    }



    private void OnEnable()
    {
        if (_skillPanelDescripts.Count == 0) return;
        if (_currentSkillPanelDescript == null)
            _currentSkillPanelDescript = _skillPanelDescripts[0];
        SetSkillDescription(0, _currentSkillPanelDescript);
    }



    private void OnDisable()
    {
        _currentSkillPanelDescript = null;
    }

    public void Initialize(List<CharacterSkillScriptableObject> skillDatas, List<SkillPanelDescript> skillPanelDescripts, List<int> levelsRequired)
    {
        gameObject.SetActive(true); // Enable the skill panel
        _skillPanelDescripts = skillPanelDescripts;
        _skillDatas = skillDatas;
        

        for (int i = 0; i < levelsRequired.Count; i++)
        {
            skillUIPanelItems[i].Initialize(skillDatas[i], levelsRequired[i]);
        }
        gameObject.SetActive(false); // Disable the skill panel
    }

    public void SetStateButtons(int playerLevel)
    {
        for (int i = 0; i < skillUIPanelItems.Count; i++)
        {
            skillUIPanelItems[i].SetStateButton(playerLevel);
        }
    }

    public void SetSkillDescription(int index, SkillPanelDescript skillPanelDescript)
    {
        _currentSkillPanelDescript = skillPanelDescript;
        foreach (Transform child in skillDescriptionParent)
        {
            if (child == skillDescriptionParent.GetChild(0)) continue;
            Destroy(child.gameObject);
        }
        if (skillPanelDescript == null) return;
        for (int i = 0; i < _currentSkillPanelDescript.SkillDescripts.Count; i++)
        {
            var skillDescription = Instantiate(SkillPanelDescriptItemPrefab);
            skillDescription.transform.SetParent(skillDescriptionParent);
            string descript = GetDescript(i, _currentSkillPanelDescript, _skillDatas[index]);
            skillDescription.SetSkillDescription(_currentSkillPanelDescript.SkillIcons[i], descript);
        }
    }
    private string GetDescript(int index, SkillPanelDescript skillPanelDescript, CharacterSkillScriptableObject characterSkillScriptableObject)
    {
        var output = "";
        switch (index)
        {
            case 0:
                output = skillPanelDescript.SkillDescripts[index] + characterSkillScriptableObject.spawnType.ToString();
                break;
            case 1:
                output = skillPanelDescript.SkillDescripts[index] + characterSkillScriptableObject.Damage;
                break;
            case 2:
                output = skillPanelDescript.SkillDescripts[index] + characterSkillScriptableObject.ManaCost;
                break;
            case 3:
                output = skillPanelDescript.SkillDescripts[index] + characterSkillScriptableObject.Cooldown;
                break;
        }
        return output;
    }


    private void InitializeItemButton()
    {
        for (int i = 0; i < skillUIPanelItems.Count; i++)
        {
            int index = i;
            skillUIPanelItems[i].SkillButton.onClick.RemoveAllListeners();
            skillUIPanelItems[i].UpgradeButton.onClick.RemoveAllListeners();
            skillUIPanelItems[i].SkillButton.onClick.AddListener(() => OnSkillButtonClicked(index));
            skillUIPanelItems[i].UpgradeButton.onClick.AddListener(() => OnUpgradeButtonClicked(_skillDatas[index]));
        }
    }

    private void OnSkillButtonClicked(int index)
    {
        _currentSkillPanelDescript = _skillPanelDescripts[index];
        SetSkillDescription(index, _currentSkillPanelDescript);

        for (int i = 0; i < skillUIPanelItems.Count; i++)
        {
            skillUIPanelItems[i].ActivateSkill(i == index);
        }
    }


    public void OnUpgradeButtonClicked(CharacterSkillScriptableObject characterSkillScriptableObject)
    {
        SkillManager.Instance.HandleUpgradeSkill(characterSkillScriptableObject, 1);
        GameEventManager.Instance.SkillEvents.SkillUpgrade(characterSkillScriptableObject, 1, false);
    }
}

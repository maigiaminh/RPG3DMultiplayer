using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class SkillManager : Singleton<SkillManager>
{
    [Header("Save & Load By PlayerPrefs")]
    private const string SKILL_LEVEL_DATA = "SkillLevelData";
    [SerializeField] private bool _loadSkillLevel = false;

    private enum CharacterType
    {
        TheFool,
        Justice = 3,
        TheHierophant = 5,
        Judgement = 4,
        TheChariot = 2,
        WheelFortune = 1
    }
    [Header("Skill Prefabs")]
    [SerializeField] private List<CharacterSkillScriptableObject> _theFoolSkillDatas = new List<CharacterSkillScriptableObject>(); // Sử dụng prefabs
    [SerializeField] private List<CharacterSkillScriptableObject> _justiceSkillDatas = new List<CharacterSkillScriptableObject>();
    [SerializeField] private List<CharacterSkillScriptableObject> _theHierophantSkillDatas = new List<CharacterSkillScriptableObject>();
    [SerializeField] private List<CharacterSkillScriptableObject> _judgementSkillDatas = new List<CharacterSkillScriptableObject>();
    [SerializeField] private List<CharacterSkillScriptableObject> _theChariotSkillDatas = new List<CharacterSkillScriptableObject>();
    [SerializeField] private List<CharacterSkillScriptableObject> _wheelFortuneSkillDatas = new List<CharacterSkillScriptableObject>();
    private Dictionary<CharacterSkillScriptableObject, ObjectPool<CharacterSkillBase>> _skillPools = new Dictionary<CharacterSkillScriptableObject, ObjectPool<CharacterSkillBase>>();
    private Dictionary<CharacterSkillScriptableObject, ObjectPool<GameObject>> _visualEffectPools = new Dictionary<CharacterSkillScriptableObject, ObjectPool<GameObject>>();
    public Dictionary<CharacterSkillScriptableObject, int> _skillLevels = new Dictionary<CharacterSkillScriptableObject, int>();
    public Dictionary<CharacterSkillScriptableObject, int> SkillLevels => _skillLevels;

    [Header("Skill Spawn Point")]
    [SerializeField] private List<SkillSpawnpoint> _skillSpawnPoints;
    private Dictionary<CharacterSkillScriptableObject, float> _timeUseSkills = new Dictionary<CharacterSkillScriptableObject, float>();
    private Dictionary<CharacterSkillBase, Coroutine> _releaseSkillCoroutines = new Dictionary<CharacterSkillBase, Coroutine>();
    private Dictionary<GameObject, Coroutine> _releaseVisualEffectCoroutines = new Dictionary<GameObject, Coroutine>();
    private bool[] _firstTimeUseSkills = new bool[4]; // 4 skills

    public event Action<CharacterSkillScriptableObject, int> PerformSkill;
    public InputReader inputReader;
    private List<CharacterSkillScriptableObject> _skillDatas = new List<CharacterSkillScriptableObject>();
    public List<CharacterSkillScriptableObject> SkillDatas => _skillDatas;
    private CharacterSkillScriptableObject _currentSkill = null;
    private int _currentSkillIndex = -1;
    private int _characterType;
    protected override void Awake()
    {
        base.Awake();

        CharacterType characterType = ChooseCharacterType();
        ChooseSkill(characterType);


        var skillLevelData = LoadSkillLevel();
        ApplySkillLevelData(skillLevelData);

        InitFirstTimeUse();
        InitializeSkillPools();
        InitializeVisualEffectsPools();
        InitializeSkillIcon();
        InitliazeSkillLevelUI();
        ConfigSkillBaseOnLevel(PlayerLevelManager.Instance.Level);

    }

    private void OnEnable()
    {
        inputReader.SkillEvent += ActivateSkillByIndex;

        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerOpenSkillPanel += HandlePlayerOpenSkillPanel;
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerCloseSkillPanel += HandlePlayerCloseSkillPanel;
        GameEventManager.Instance.PlayerEvents.OnPlayerLevelChange += ConfigSkillBaseOnLevel;
    }


    private void OnDisable()
    {
        inputReader.SkillEvent -= ActivateSkillByIndex;

        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerOpenSkillPanel -= HandlePlayerOpenSkillPanel;
        GameEventManager.Instance.PlayerContactUIEvents.OnPlayerCloseSkillPanel -= HandlePlayerCloseSkillPanel;
        GameEventManager.Instance.PlayerEvents.OnPlayerLevelChange -= ConfigSkillBaseOnLevel;
    }

    public void ActivateSkillByIndex(int skillIndex)
    {
        Debug.Log("Skill press" + skillIndex);
        if (skillIndex < 0 || skillIndex >= _skillDatas.Count)
        {
            Debug.LogError("Skill index out of range: " + skillIndex);
            return;
        }
        CharacterSkillScriptableObject dataSkill = _skillDatas[skillIndex].GetScaledUpSkillForLevel(_skillLevels[_skillDatas[skillIndex]]);

        _currentSkill = _skillDatas[skillIndex];
        Debug.Log("Current Skill: " + _currentSkill.SkillName);
        _currentSkillIndex = skillIndex;

        if (dataSkill == null)
        {
            Debug.LogError("Skill not found at index: " + skillIndex);
            return;
        }

        PerformSkill?.Invoke(dataSkill, skillIndex);
    }

    public void ActivateSkill()
    {
        if (_currentSkill == null) return;
        _timeUseSkills.TryGetValue(_currentSkill, out var lastUseTime);

        bool isFirstTime = _firstTimeUseSkills[_currentSkillIndex];
        if (!_currentSkill.CanUseSkill(lastUseTime, isFirstTime)) return;
        _firstTimeUseSkills[_currentSkillIndex] = false;

        Transform spawnPos = GetSkillSpawnpointPos(_currentSkill.skillSpawnType);
        Debug.Log("Current Skill: " + _currentSkill.SkillName + "in ActivateSkill");
        CharacterSkillBase skillPrefab = GetSkill();
        skillPrefab.Initilize(_currentSkill, this);

        Debug.Log("SpawnPos: " + spawnPos);

        skillPrefab.Execute(spawnPos, Camera.main.transform.forward, _currentSkill);
        UseSkill(_currentSkill);
        PlayerStatManager.Instance.ReduceMana((int)_currentSkill.ManaCost);
        GameEventManager.Instance.SkillEvents.SkillUsed(_currentSkill, _currentSkillIndex);
        _releaseSkillCoroutines[skillPrefab] = StartCoroutine(ReleaseSkillAfterCooldown(_currentSkill, skillPrefab, _currentSkill.Duration));
    }

    private void UseSkill(CharacterSkillScriptableObject characterSkillScriptableObject)
    {
        _timeUseSkills[characterSkillScriptableObject] = Time.time;
    }

    private IEnumerator ReleaseSkillAfterCooldown(CharacterSkillScriptableObject skillData, CharacterSkillBase skillPrefab, float duration)
    {
        yield return new WaitForSeconds(duration);
        if (skillPrefab.isReleased)
        {
            yield break;
        }
        if (!skillPrefab.gameObject.activeSelf) yield break;
        ReleaseSkill(skillData, skillPrefab);

    }

    private IEnumerator ReleaseVisualEffect(CharacterSkillScriptableObject skillData, GameObject skillVisualEffect, float duration)
    {
        yield return new WaitForSeconds(duration);
        ReleaseVisualEffectPool(skillData, skillVisualEffect);
    }

    public Transform GetSkillSpawnpointPos(SkillSpawnpoint.SpawnpointType type)
    {
        foreach (var spawnPoint in _skillSpawnPoints)
        {
            if (spawnPoint.type == type)
            {
                if (spawnPoint == null)
                {
                    Debug.LogError("Spawnpoint is missing");
                    return null;
                }
                Debug.Log("Spawnpoint is found " + spawnPoint.type.ToString());
                Debug.Log("Spawnpoint position " + spawnPoint.GetPosition());
                return spawnPoint.transform;
            }
        }
        return null;
    }



    public CharacterSkillBase GetSkill()
    {
        if (!_skillPools.ContainsKey(_currentSkill))
        {
            Debug.LogError("Pool not found for skill: " + _currentSkill.name);
            return null;
        }

        return _skillPools[_currentSkill].Get();
    }


    #region Skill Pool Callbacks
    private void InitializeSkillPools()
    {
        foreach (var data in _skillDatas)
        {
            CreateSkillPool(data, data.SkillPrefab);
        }
    }

    public void CreateSkillPool(CharacterSkillScriptableObject skillData, CharacterSkillBase skillPrefab, int defaultCapacity = 100, int maxCapacity = 2000)
    {
        Debug.Log("CreateSkillPool " + skillData.name);

        ObjectPool<CharacterSkillBase> pool = new ObjectPool<CharacterSkillBase>(
            () => Instantiate(skillPrefab),
            OnTakeSkillFromPool,
            OnReturnSkillFromPool,
            OnDestroySkill,
            true,
            defaultCapacity,
            maxCapacity
        );
        _skillPools.Add(skillData, pool);
    }



    public void ReleaseSkill(CharacterSkillScriptableObject skillData, CharacterSkillBase skillPrefab)
    {

        if (skillPrefab == null)
        {
            Debug.LogError("Skill prefab is null");
            return;
        }
        if (_releaseSkillCoroutines.TryGetValue(skillPrefab, out var coroutine))
        {
            StopCoroutine(coroutine); // Dừng coroutine
            _releaseSkillCoroutines.Remove(skillPrefab);
        }
        if (_skillPools.TryGetValue(skillData, out var pool))
        {
            pool.Release(skillPrefab);
            _currentSkill = null;
            _currentSkillIndex = -1;
        }
        else
        {
            Debug.LogError("Pool not found for skill: " + skillData.SkillName);
        }


        GameObject skillVisualEffect = GetSkillVisualEffect(skillData);
        if (skillVisualEffect != null)
        {
            Debug.Log("Skill Visual Effect is not null");
            skillVisualEffect.transform.position = skillPrefab.transform.position;
            skillVisualEffect.SetActive(true);
            _releaseVisualEffectCoroutines[skillVisualEffect] = StartCoroutine(ReleaseVisualEffect(skillData, skillVisualEffect, skillData.VisualEffect.Duration));
        }

    }

    private void OnDestroySkill(CharacterSkillBase @base)
    {
        Destroy(@base.gameObject);
    }

    private void OnReturnSkillFromPool(CharacterSkillBase @base)
    {
        @base.gameObject.SetActive(false);
    }

    private void OnTakeSkillFromPool(CharacterSkillBase @base)
    {
        @base.gameObject.SetActive(true);
    }



    #endregion

    #region Visual Effects Pool Callbacks

    private void InitializeVisualEffectsPools()
    {
        foreach (var data in _skillDatas)
        {
            if (data.VisualEffect == null) continue;
            CreateVisualEffectPool(data, data.VisualEffect.VisualEffectPrefab);
        }
    }

    private void CreateVisualEffectPool(CharacterSkillScriptableObject skillData, GameObject vfxPrefab)
    {
        if (vfxPrefab == null) return;
        if (!_visualEffectPools.ContainsKey(skillData))
        {
            ObjectPool<GameObject> pool = new ObjectPool<GameObject>(
                () => Instantiate(vfxPrefab),
                OnTakeVisualEffectFromPool,
                OnReturnVisualEffectFromPool,
                OnDestroyVisualEffect,
                false,
                100,
                2000
            );
            _visualEffectPools.Add(skillData, pool);
        }
    }
    public void ReleaseVisualEffectPool(CharacterSkillScriptableObject skillData, GameObject visualEffect)
    {
        if (visualEffect == null)
        {
            Debug.LogError("Visual effect is null");
            return;
        }

        // Stop and remove any running coroutine for this visual effect
        if (_releaseVisualEffectCoroutines.TryGetValue(visualEffect, out var coroutine))
        {
            StopCoroutine(coroutine);
            _releaseVisualEffectCoroutines.Remove(visualEffect);
        }

        // Release the visual effect back to its pool
        if (_visualEffectPools.TryGetValue(skillData, out var pool))
        {
            Debug.Log("Releasing visual effect: " + visualEffect.name);
            pool.Release(visualEffect);
        }
        else
        {
            Debug.LogError("Pool not found for visual effect: " + visualEffect.name);
        }
    }
    public GameObject GetSkillVisualEffect(CharacterSkillScriptableObject skillData)
    {
        if (!_visualEffectPools.ContainsKey(skillData))
        {
            Debug.Log("Pool is not contain visual effects" + skillData.name);
            return null;
        }

        return _visualEffectPools[skillData].Get();
    }


    private void OnDestroyVisualEffect(GameObject @object)
    {
        Destroy(@object);
    }

    private void OnReturnVisualEffectFromPool(GameObject @object)
    {
        @object.SetActive(false);
    }

    private void OnTakeVisualEffectFromPool(GameObject @object)
    {
        @object.SetActive(true);
    }



    #endregion



    #region Other Methods

    private void InitializeSkillIcon()
    {
        var skillUIManagerOptional = Optional<SkillUIManager>.Some(SkillUIManager.Instance);
        skillUIManagerOptional.Match(
            skillUIManager =>
            {
                for (int i = 0; i < _skillDatas.Count; i++)
                {
                    skillUIManager.skillCooldownItems[i].SetSkillIcon(_skillDatas[i].SkillIcon, _skillDatas[i].SkillColor);
                    skillUIManager.skillUIPanel.skillUIPanelItems[i].SetSkillIcon(_skillDatas[i].SkillIcon, _skillDatas[i].SkillColor);
                }
            },
            () => Debug.LogError("SkillUIManager is not found")
        );
    }
    private void InitliazeSkillLevelUI()
    {
        List<int> levelsRequired = new List<int>();
        List<SkillPanelDescript> skillPanelDescripts = new List<SkillPanelDescript>();
        foreach (var skill in _skillDatas)
        {
            Debug.Log("Skill: " + skill.SkillName);
            skillPanelDescripts.Add(skill.SkillPanelDescript);
            levelsRequired.Add(skill.UnlockSkillLevel);
        }
        var skillUIManagerOptional = Optional<SkillUIManager>.Some(SkillUIManager.Instance);
        skillUIManagerOptional.Match(
            manager =>
            {
                Debug.Log("SkillUIManager is found + " + manager);
                manager.skillUIPanel.Initialize(_skillDatas, skillPanelDescripts, levelsRequired);
                manager.InitalizeSkillLevel(levelsRequired);
                foreach (var skill in _skillDatas)
                {
                    Debug.Log("Skill: " + skill.SkillName + " " + _skillLevels[skill] + " " + skill.UnlockSkillLevel);
                    GameEventManager.Instance.SkillEvents.SkillUpgrade(skill, SkillLevels[skill], true);
                }
            },
            () =>
            {
                Debug.LogError("SkillUIManager is not found");
            }
        );
    }

    private void ConfigSkillBaseOnLevel(int level)
    {
        Optional<SkillUIManager>.Some(SkillUIManager.Instance).Match(
            manager =>
            {
                manager.skillUIPanel.SetStateButtons(level);
            },
            () => Debug.LogError("SkillUIManager is not found")
        );
    }

    public void HandleUpgradeSkill(CharacterSkillScriptableObject skillData, int amount)
    {
        int currentLevel = _skillLevels[skillData];
        currentLevel += amount;
        _skillLevels[skillData] = currentLevel;
    }


    private void InitFirstTimeUse()
    {
        for (int i = 0; i < _firstTimeUseSkills.Length; i++)
        {
            _firstTimeUseSkills[i] = true;
        }
    }


    private CharacterType ChooseCharacterType()
    {

        Optional<UserContainer> userContainerOptional = UserContainer.Instance != null
                                                            ? Optional<UserContainer>.Some(UserContainer.Instance)
                                                            : Optional<UserContainer>.None();

        string className = userContainerOptional.Match(
            userContainer => userContainer.UserData.CharacterClassName,
            () => "The Chariot"
        );
        Debug.Log("Character Class Name: " + className);

        CharacterType characterType = CharacterType.TheFool;

        switch (className)
        {
            case "The Fool":
                characterType = CharacterType.TheFool;
                break;
            case "Justice": //Shield
                characterType = CharacterType.Justice;
                break;
            case "The Hierophant": //Staff
                characterType = CharacterType.TheHierophant;
                break;
            case "Judgement": //DualKnife
                characterType = CharacterType.Judgement;
                break;
            case "The Chariot": //Bow
                characterType = CharacterType.TheChariot;
                break;
            case "Wheel Fortune": //GreatSword
                characterType = CharacterType.WheelFortune;
                break;
        }

        _characterType = (int)characterType;

        return characterType;
    }


    private void ChooseSkill(CharacterType type)
    {
        switch (type)
        {
            case CharacterType.TheFool:
                _skillDatas = _theFoolSkillDatas;
                break;
            case CharacterType.Justice:
                _skillDatas = _justiceSkillDatas;
                break;
            case CharacterType.TheHierophant:
                _skillDatas = _theHierophantSkillDatas;
                break;
            case CharacterType.Judgement:
                _skillDatas = _judgementSkillDatas;
                break;
            case CharacterType.TheChariot:
                _skillDatas = _theChariotSkillDatas;
                break;
            case CharacterType.WheelFortune:
                _skillDatas = _wheelFortuneSkillDatas;
                break;
        }
    }


    private void HandlePlayerOpenSkillPanel()
    {
        Debug.Log("HandlePlayerOpenSkillPanel");
        SkillUIManager.Instance.ActivateSkillPanel(true);
    }

    private void HandlePlayerCloseSkillPanel()
    {
        SkillUIManager.Instance.ActivateSkillPanel(false);
    }

    public void SaveSkillLevel()
    {
        // try
        // {
        //     SkillLevelData skillLevelData = GetSkillLevelData();
        //     string serializedData = JsonUtility.ToJson(skillLevelData);
        //     PlayerPrefs.SetString(SKILL_LEVEL_DATA, serializedData);

        // }
        // catch (System.Exception e)
        // {
        //     Debug.LogError("Failed to save quest data: " + e.Message);
        // }

        Optional<FirebaseAuthManager> optionalFirebaseAuthManager = Optional<FirebaseAuthManager>.Some(FirebaseAuthManager.Instance);
        optionalFirebaseAuthManager.Match(
            manager =>
            {
                SkillLevelData skillLevelData = GetSkillLevelData();
                manager.SaveSkillLeveData(skillLevelData);
            },
            () =>
            {
                Debug.LogError("FirebaseAuthManger Missing in SaveSkillLevel");
            }
        );

    }

    private SkillLevelData LoadSkillLevel()
    {

        if (SkillContainer.Instance == null)
        {
            Debug.LogError("SkillContainer is null");
            return null;
        }



        SkillLevelData skillData = SkillContainer.Instance.SkillLevelData;
        _skillLevels = new Dictionary<CharacterSkillScriptableObject, int>();

        Debug.Log("SkillData: " + skillData.Skill1Level + " " + skillData.Skill2Level + " " + skillData.Skill3Level + " " + skillData.Skill4Level);

        return skillData;
    }

    public int GetSkillNextUpgradeLevel(CharacterSkillScriptableObject characterSkillScriptableObject)
    {
        return _skillLevels[characterSkillScriptableObject] + 1;
    }

    public SkillLevelData GetSkillLevelData()
    {
        int level1 = _skillLevels[_skillDatas[0]];
        int level2 = _skillLevels[_skillDatas[1]];
        int level3 = _skillLevels[_skillDatas[2]];
        int level4 = _skillLevels[_skillDatas[3]];
        return new SkillLevelData(level1, level2, level3, level4);
    }

    private void ApplySkillLevelData(SkillLevelData skillLevelData)
    {
        if (skillLevelData == null)
        {
            Debug.LogError("SkillLevelData is null");
            return;
        }

        if (_skillDatas == null || _skillDatas.Count < 4)
        {
            Debug.LogError("SkillDatas does not contain enough elements.");
            return;
        }

        _skillLevels[_skillDatas[0]] = skillLevelData.Skill1Level;
        _skillLevels[_skillDatas[1]] = skillLevelData.Skill2Level;
        _skillLevels[_skillDatas[2]] = skillLevelData.Skill3Level;
        _skillLevels[_skillDatas[3]] = skillLevelData.Skill4Level;
    }

    public int GetCharacterTypeInt()
    {
        return _characterType;
    }

    #endregion
}
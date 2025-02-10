using System;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerLevelManager : Singleton<PlayerLevelManager>
{
    [SerializeField] private bool _loadLevel = true;
    public int Level { get; set; }
    public int MaxLevel { get; set; }
    public int Experience { get; set; }
    private int _experienceToNextLevel { get; set; }

    [SerializeField] private int _initialExperienceToNextLevel = 1000; // Base experience for level 1
    [SerializeField] private AnimationCurve _scaleupExperienceCurve; // Curve to scale experience requirement


    private void Start()
    {
        LoadLevel();
        UpdateExperienceToNextLevel();
        GameEventManager.Instance.PlayerEvents.PlayerLevelChange(Level);


        LevelView.Instance.UpdateLevel(Level);
    }



    private void OnEnable()
    {
        GameEventManager.Instance.PlayerEvents.OnPlayerGainExperience += GainExperience;
    }

    private void OnDisable()
    {
        GameEventManager.Instance.PlayerEvents.OnPlayerGainExperience -= GainExperience;
    }

    // private void OnDestroy()
    // {
    //     SaveLevel();
    // }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GainExperience(1000);
        }
    }

    // Called when the player gains experience
    private void GainExperience(int exp)
    {
        Experience += exp;
        Debug.Log($"Gained {exp} experience. Total: {Experience}/{_experienceToNextLevel} for next level.");
        while (Experience >= _experienceToNextLevel)
        {
            LevelUp();
        }
        LevelView.Instance.UpdateExperience(_experienceToNextLevel, Experience);
    }

    // Level up the player, reset experience, and recalculate experience to next level
    private void LevelUp()
    {
        Experience -= _experienceToNextLevel;
        Level++;
        MaxLevel = Level > MaxLevel ? Level : MaxLevel;
        GameEventManager.Instance.PlayerEvents.PlayerLevelChange(Level);
        UpdateExperienceToNextLevel(); // Keep this here to update for the *next* level
        LevelView.Instance.UpdateLevel(Level);
        LevelView.Instance.UpdateExperience(_experienceToNextLevel, Experience); // Update UI after level up
        // SoundManager.PlaySound(SoundType.LEVEL_UP);        
    }

    // Calculate the experience required to level up using the animation curve
    private void UpdateExperienceToNextLevel()
    {
        // Use the animation curve to scale the experience requirement for the next level
        _experienceToNextLevel = Mathf.FloorToInt(
            _initialExperienceToNextLevel * _scaleupExperienceCurve.Evaluate(Level)
        );
        Debug.Log($"Experience required for next level: {_experienceToNextLevel}");
    }

    // Load saved player level and experience from PlayerPrefs or another system
    private void LoadLevel()
    {
        if (UserContainer.Instance == null)
        {
            Debug.LogError("UserContainer is null");
            return;
        }

        int level = UserContainer.Instance.UserData.Level == 0 ? 1 : UserContainer.Instance.UserData.Level;
        int maxLevel = UserContainer.Instance.UserData.MaxLevel == 0 ? level : UserContainer.Instance.UserData.MaxLevel;
        int xp = UserContainer.Instance.UserData.Xp;
        SetLevel(level, xp);

        LevelView.Instance.UpdateExperience(_experienceToNextLevel, Experience);
    }



    // Save player level and experience to PlayerPrefs or another system
    // private void SaveLevel()
    // {
    //     try
    //     {
    //         LevelData data = GetLevelData();
    //         string serializedData = JsonUtility.ToJson(data);
    //         PlayerPrefs.SetString(LevelDataKey, serializedData);
    //         PlayerPrefs.Save();
    //     }
    //     catch (System.Exception)
    //     {
    //         Debug.LogError("Failed to save level data.");
    //     }

    // }


    private void SetLevel(int level, int xp)
    {
        Level = level;
        Experience = xp;
        UpdateExperienceToNextLevel();
    }


    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Experience"))
        {
            GainExperience(10);
            Destroy(other.gameObject);
        }
    }

}


using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Character Skill Data", menuName = "Player/SkillData", order = 1)]
public class CharacterSkillScriptableObject : ScriptableObject
{
    [Header("Skill Data")]
    public string SkillName;
    public float Cooldown; // Đổi tên thành ...
    public float Duration;
    public int Damage;
    public float ManaCost;
    public int ExperienceToNextLevel;
    public int UnlockSkillLevel;
    public CharacterSkillBase SkillPrefab;
    public ScaleUpSkillData ScalingData;
    public PlayerSkillVisualEffect VisualEffect;
    [Header("Skill Spawn Type")]
    public SkillSpawnpoint.SpawnpointType skillSpawnType;
    [Header("Delay")]
    public bool HaveDelay;
    public float DelayTime;

    [Header("Skill UI")]
    public Sprite SkillIcon;
    public Color SkillColor;

    [Header("Skill Description")]
    public SkillPanelDescript SkillPanelDescript;
    public List<int> SkillLevelRequiredToUpdate;
    public int GoldRequiredToUpdateBase;
    [Range(0, 100)]
    public float GoldIncreasePercentUpdate = 20;

    public enum SpawnType
    {
        Projectile,
        StickOwner,
        Area,
        Target,
        Explode,
        Burst,
        Shield,
        GroundArea,
        Immune,
        Heal,
        CCAOEStickEnemy,
        CCAOEStickPlayer,
        IncreateAfterTurn,
        AimHaveCast,
    }
    public enum ExecuteType
    {
        OverTime,
        Instant
    }
    [Header("Skill Type")]
    public SpawnType spawnType;
    public ExecuteType executeType;
    // Projectile data
    [ShowIf(nameof(spawnType), SpawnType.Projectile)]
    public float Speed;
    // StickOwner data


    // Area data
    [ShowIf(nameof(spawnType), SpawnType.Area)]
    public float DamageRateArea;
    [ShowIf(nameof(spawnType), SpawnType.Area)]
    public float AreaRadius;


    // Target data


    // Explode data
    [ShowIf(nameof(spawnType), SpawnType.Explode)]

    public float ExplodeRadius;

    // Burst data
    [ShowIf(nameof(spawnType), SpawnType.Burst)]
    public float DamageRateBurst;

    // Shield data
    [ShowIf(nameof(spawnType), SpawnType.Shield)]
    public int BlockRate;

    // GroundArea data
    [ShowIf(nameof(spawnType), SpawnType.GroundArea)]
    public float GroundDamageRate;

    // Immune data
    [ShowIf(nameof(spawnType), SpawnType.Immune)]
    public float ImmuneTime;

    [ShowIf(nameof(spawnType), SpawnType.Heal)]
    public int HealAmount;

    // CCAOE stick enemy data
    [ShowIf(nameof(spawnType), SpawnType.CCAOEStickEnemy)]
    public float CCRadius;
    [ShowIf(nameof(spawnType), SpawnType.CCAOEStickEnemy)]
    public CCType CCStickEnemyType;

    // CCAOE stick player data
    [ShowIf(nameof(spawnType), SpawnType.CCAOEStickPlayer)]
    public CCType CCStickPlayerType;
    [ShowIf(nameof(spawnType), SpawnType.CCAOEStickPlayer)]
    public float CCDamageRate;
    
    // Increate after turn data
    [ShowIf(nameof(spawnType), SpawnType.IncreateAfterTurn)]
    public int AmountTurns;
    [ShowIf(nameof(spawnType), SpawnType.IncreateAfterTurn)]
    public float IncreasePercentPerTurn;

    // Aim have cast data
    [ShowIf(nameof(spawnType), SpawnType.AimHaveCast)]
    public float DamageRateAim;

    // Hàm này trả về một bản sao đã được scale
    public CharacterSkillScriptableObject GetScaledUpSkillForLevel(int level)
    {
        return ScaleUpForLevel(level);
    }
    

    // Hàm này thực hiện việc scale, nhưng không lưu vào Scriptable Object
    private CharacterSkillScriptableObject ScaleUpForLevel(int level)
    {
        CharacterSkillScriptableObject characterSkillScriptableObject = CreateInstance<CharacterSkillScriptableObject>();
        if (ScalingData == null)
        {
            Debug.LogError("Scaling data is missing!");
            return null;
        }

        // Tính toán các giá trị đã scale, nhưng KHÔNG gán lại cho biến 

        float scaledCooldown = Cooldown * ScalingData.CooldownReduceCurve.Evaluate(level - 1);
        float scaledDuration = Duration * ScalingData.DurationIncreaseCurve.Evaluate(level - 1);
        float scaledDamage = Damage * ScalingData.DamageIncreaseCurve.Evaluate(level - 1);
        float scaledManaCost = ManaCost * ScalingData.ManaCostIncreaseCurve.Evaluate(level - 1);
        float scaledSpeed = Speed * ScalingData.SpeedIncreaseCurve.Evaluate(level - 1);

        // Gán các giá trị đã scale vào bản sao
        characterSkillScriptableObject.SkillName = SkillName;
        characterSkillScriptableObject.UnlockSkillLevel = UnlockSkillLevel;
        characterSkillScriptableObject.SkillPrefab = SkillPrefab;
        characterSkillScriptableObject.ScalingData = ScalingData;
        characterSkillScriptableObject.Cooldown = Mathf.Max(0, scaledCooldown); // Sử dụng Cooldown, Duration,... như là thuộc tính tạm thời của bản sao
        characterSkillScriptableObject.Duration = Mathf.Max(0, scaledDuration);
        characterSkillScriptableObject.Damage = (int)Mathf.Max(0, scaledDamage);
        characterSkillScriptableObject.ManaCost = Mathf.Max(0, scaledManaCost);
        characterSkillScriptableObject.Speed = Mathf.Max(0, scaledSpeed);
        ExperienceToNextLevel = ExperienceToNextLevel * (int)ScalingData.ExperienceToNextLevelIncreaseCurve.Evaluate(level - 1);
        characterSkillScriptableObject.skillSpawnType = skillSpawnType;
        //
        return characterSkillScriptableObject;
    }

    public int GetNextLevelExperience(int level)
    {
        return Mathf.FloorToInt(ExperienceToNextLevel * ScalingData.ExperienceToNextLevelIncreaseCurve.Evaluate(level - 1));
    }

    public bool CanUseSkill(float lastUseTime, bool isFirstTime)
    {
        if (isFirstTime) return true;
        return Time.time - lastUseTime > Cooldown;
    }

    public int GetGoldCostToUpgrade(int currentLevel)
    {
        return GoldRequiredToUpdateBase + (int)(GoldRequiredToUpdateBase * currentLevel * GoldIncreasePercentUpdate / 100);
    }



    private void OnValidate() {
        #if UNITY_EDITOR
            SkillName = this.name;
            UnityEditor.EditorUtility.SetDirty(this);
        #endif
    }

}
using System;
using System.Collections;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CharacterSkillBase : MonoBehaviour
{
    protected CharacterSkillScriptableObject _skillData;
    protected float _cooldown = 0;
    protected float _duration = 0;
    protected int _damage = 0;
    protected float _manaCost = 0;
    protected float _useTime = 0;
    protected bool isFirstTime = true;
    protected float _timerDamageRate = 0;
    protected SkillManager _skillManager;
    [HideInInspector] public bool isReleased = false;



    public CharacterSkillScriptableObject.SpawnType spawnType;
    public CharacterSkillScriptableObject.ExecuteType executeType;
    public Vector3 rotateOffset = Vector3.zero;


    public void Initilize(CharacterSkillScriptableObject data, SkillManager skillManager)
    {
        _skillManager = skillManager;
        isFirstTime = true;
        _useTime = 0;
        ConfigSkillBase(data);
        Debug.Log("Skill Initialized");
    }


    protected virtual void ConfigSkillBase(CharacterSkillScriptableObject data)
    {
        _skillData = data;
        _cooldown = data.Cooldown;
        _duration = data.Duration;
        _damage = data.Damage + PlayerStatManager.Instance.Intelligence / 3;
        _manaCost = data.ManaCost;
    }


    public virtual void Execute(Transform spawnPos, Vector3 direct, CharacterSkillScriptableObject data)
    {
        isReleased = false;
        Quaternion rotationOffset = Quaternion.Euler(rotateOffset);
        switch (spawnType)
        {
            case CharacterSkillScriptableObject.SpawnType.Projectile:
                transform.position = spawnPos.position;
                transform.forward = direct;
                break;
            case CharacterSkillScriptableObject.SpawnType.StickOwner:
                StickPlayer(spawnPos, rotationOffset);
                break;
            case CharacterSkillScriptableObject.SpawnType.Area:
                StickPoint(spawnPos);
                break;
            case CharacterSkillScriptableObject.SpawnType.Target:
                Debug.Log("Target");
                break;
            case CharacterSkillScriptableObject.SpawnType.Explode:
                StickPoint(spawnPos);
                break;
            case CharacterSkillScriptableObject.SpawnType.Burst:
                StickPlayer(spawnPos, rotationOffset);
                break;
            case CharacterSkillScriptableObject.SpawnType.Shield:
                StickPlayer(spawnPos, rotationOffset);
                break;
            case CharacterSkillScriptableObject.SpawnType.GroundArea:
                StickPoint(spawnPos);
                break;
            case CharacterSkillScriptableObject.SpawnType.Immune:
                StickPlayer(spawnPos, rotationOffset);
                break;
            case CharacterSkillScriptableObject.SpawnType.Heal:
                StickPlayer(spawnPos, rotationOffset);
                break;
            case CharacterSkillScriptableObject.SpawnType.CCAOEStickEnemy:
                break;
            case CharacterSkillScriptableObject.SpawnType.CCAOEStickPlayer:
                StickPlayer(spawnPos, rotationOffset);
                break;
            case CharacterSkillScriptableObject.SpawnType.IncreateAfterTurn:
                StickPoint(spawnPos);
                break;
            case CharacterSkillScriptableObject.SpawnType.AimHaveCast:
                StickPlayer(spawnPos, rotationOffset);
                break;
        }




    }

    private void StickPlayer(Transform spawnPos, Quaternion rotationOffset)
    {
        transform.SetParent(spawnPos);
        transform.localPosition = Vector3.zero;
        transform.forward = spawnPos.forward;
        transform.rotation = spawnPos.rotation * rotationOffset;
    }

    private void StickPoint(Transform spawnPos)
    {
        RaycastHit hit;
        if (Physics.Raycast(spawnPos.position, Vector3.down, out hit, 100f, LayerMask.GetMask("Default")))
        {
            transform.position = hit.point;
        }
        else
        {
            Debug.LogError("No ground found");
        }
    }

    public void CalculateDamage()
    {
        CharacterSkillScriptableObject scaleUp = _skillData.GetScaledUpSkillForLevel(SkillManager.Instance.SkillLevels[_skillData]);
        _damage = scaleUp.Damage + (int)(PlayerStatManager.Instance.Intelligence / 3);
    }
}

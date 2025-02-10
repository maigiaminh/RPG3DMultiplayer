using System.Collections;
using NUnit.Framework.Constraints;
using UnityEngine;

public class SkillAimHaveCastType : CharacterSkillBase
{
    private float _damageRate;
    private float _delayTime;
    private float _count;
    public DetectDamagableByOverlapSphere detectTargets;
    public GameObject AimPrefab;

    private Vector3 _aimSpawnPos;
    private Vector3 _direct;
    private float _damageRadius;

    protected override void ConfigSkillBase(CharacterSkillScriptableObject data)
    {
        base.ConfigSkillBase(data);
        _delayTime = data.DelayTime;
        _damageRate = data.DamageRateAim;
        ConfigDetectDamageableInRange();

    }
    public override void Execute(Transform spawnPos, Vector3 direct, CharacterSkillScriptableObject data)
    {
        base.Execute(spawnPos, direct, data);
        StartCoroutine(ReturnToGameObject());
        _count = 0;
        _direct = _skillManager.GetSkillSpawnpointPos(SkillSpawnpoint.SpawnpointType.Forward8cm).position;
    }

    private IEnumerator ReturnToGameObject()
    {
        yield return new WaitForSeconds(_duration - 0.5f);
        AimPrefab.SetActive(false);
        AimPrefab.transform.SetParent(transform);
    }

    private void Update()
    {
        if (_delayTime > 0)
        {
            _delayTime -= Time.deltaTime;
            return;
        }
        if (_count > 0)
        {
            _count -= Time.deltaTime;
            return;
        }
        Debug.Log("Apppppppppp");
        ApplyDamageToEnemy();
        _count = 1 / _damageRate;
    }


    public Vector3 FindAimSpawnPos()
    {
        RaycastHit hit;
        if (Physics.Raycast(_direct, Vector3.down, out hit, 100))
        {
            return hit.point;
        }
        return Vector3.zero;
    }

    private void ApplyDamageToEnemy()
    {
        Debug.Log("ApplyDamageToEnemy");
        Debug.Log("detectTargets.damageables.Count: " + detectTargets.damageables.Count);
        foreach (var target in detectTargets.damageables)
        {
            target.TakeDamage(null, _damage);
        }
    }

    public void SetRootTransformToAim()
    {
        AimPrefab.transform.SetParent(null);
        _aimSpawnPos = FindAimSpawnPos();
        AimPrefab.transform.position = _aimSpawnPos;
    }

    private void ConfigDetectDamageableInRange()
    {
        detectTargets.Config(_skillManager);
    }


}

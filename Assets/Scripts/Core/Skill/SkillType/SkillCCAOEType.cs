using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCCAOEType : CharacterSkillBase
{
    private float _radius;
    private float _ccDuration;
    private CCType _ccType;
    private CharacterSkillBase _ccEffectPrefab;
    private List<CharacterSkillBase> _spawnedCCEffects = new List<CharacterSkillBase>();
    private Collider[] _colliders = new Collider[128];
    private Coroutine _releaseSkillCoroutine;
    protected override void ConfigSkillBase(CharacterSkillScriptableObject data)
    {
        base.ConfigSkillBase(data);
        _ccType = data.CCStickEnemyType;
        _radius = data.CCRadius;
        _ccDuration = data.Duration;
    }

    public override void Execute(Transform spawnPos, Vector3 direct, CharacterSkillScriptableObject data)
    {
        base.Execute(spawnPos, direct, data);
        _ccEffectPrefab = data.SkillPrefab;

        ApplyEffectOnAOE();
    }

    public void ApplyEffectOnAOE()
    {
        int numColliders = Physics.OverlapSphereNonAlloc(_skillManager.transform.position, _radius, _colliders);
        Transform firstTargetTransform = null;
        for (int i = 0; i < numColliders; i++)
        {
            var collider = _colliders[i];

            if (!collider.TryGetComponent<IDamageable>(out var damageable) || !collider.CompareTag("Enemy")) continue;

            if (firstTargetTransform == null)
            {
                firstTargetTransform = collider.transform; 
                damageable.ApplyCC(_ccType, _ccDuration); 
                continue;
            }

            damageable.ApplyCC(_ccType, _ccDuration); 
            SpawnCCEffect(collider.transform);      
        }
        if (firstTargetTransform != null)
        {
            transform.SetParent(firstTargetTransform);
            transform.localPosition = Vector3.zero;
            transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            transform.localRotation = Quaternion.identity;
        }

        if (_releaseSkillCoroutine != null)
        {
            StopCoroutine(_releaseSkillCoroutine);
        }
        _releaseSkillCoroutine = StartCoroutine(ReleaseAllSkill());
    }



    private void SpawnCCEffect(Transform targetTransform)
    {
        CharacterSkillBase ccEffect = targetTransform.GetComponentInChildren<CharacterSkillBase>();

        if (ccEffect == null || ccEffect.GetType() != _ccEffectPrefab.GetType())
        {
            ccEffect = Instantiate(_ccEffectPrefab, targetTransform);
        }
        ccEffect.gameObject.SetActive(true);
        _spawnedCCEffects.Add(ccEffect);
        ccEffect.transform.localPosition = Vector3.zero;
        ccEffect.transform.localScale = new Vector3(.4f, .4f, .4f);
        ccEffect.transform.localRotation = Quaternion.identity;

    }

    private IEnumerator ReleaseAllSkill()
    {
        yield return new WaitForSeconds(_ccDuration);
        foreach (var ccEffect in _spawnedCCEffects) // Simplified loop
        {
            ccEffect.gameObject.SetActive(false);
        }
        _spawnedCCEffects.Clear();
    }

}

using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillIncreaseAfterTurnType : CharacterSkillBase
{
    [SerializeField]
    private List<DetectDamageableInRange> _colliderContainer = new List<DetectDamageableInRange>();
    private float _baseDamageValue = 0;
    private float _percentIncrease = 0;
    private int _turns = 0;
    private int _currentTurn = 0;
    private List<IDamageable> _targets = new List<IDamageable>();


    protected override void ConfigSkillBase(CharacterSkillScriptableObject data)
    {
        base.ConfigSkillBase(data);
        _baseDamageValue = _damage;
        _percentIncrease = data.IncreasePercentPerTurn;
        _turns = data.AmountTurns;
        _currentTurn = 0;
        ConfigDetectDamageableInRange();
    }


    public override void Execute(Transform spawnPos, Vector3 direct, CharacterSkillScriptableObject data)
    {
        base.Execute(spawnPos, direct, data);
        _currentTurn = 0;
        transform.forward = _skillManager.transform.position - transform.position;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, -30f, 0f));
    }

    public void ApplyDamageToTargets()
    {
        UpdateTargetsAfterTurn(_currentTurn);
        ApplyDamageToTargetsNotIncreaseTurn();
        IncreaseValue();
    }

    public void ApplyDamageToTargetsNotIncreaseTurn()
    {
        if(_targets.Count == 0) UpdateTargetsAfterTurn(_currentTurn);
        foreach (var target in _targets)
        {
            target.TakeDamage(null, _damage);
        }
    }
    private void UpdateTargetsAfterTurn(int _currentTurn)
    {
        _targets = _colliderContainer[_currentTurn].damageables;
    }

    private void IncreaseValue()
    {
        if (_currentTurn >= _turns) return;
        _currentTurn++;
        _damage = (int)(_baseDamageValue + _baseDamageValue * (_percentIncrease / 100) * _currentTurn);
    }

    private void ConfigDetectDamageableInRange()
    {
        foreach (var detect in _colliderContainer)
        {
            detect.Config(_skillManager);
        }
    }

}

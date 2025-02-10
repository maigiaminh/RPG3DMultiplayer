using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DefeatEnemyStep : QuestStep
{
    public int TargetEnemyDefeatCount;
    private int _currentEnemyDefeatCount;
    public EnemyType enemyType;


    public DefeatEnemyStep(int targetEnemyDefeatCount)
    {
        TargetEnemyDefeatCount = targetEnemyDefeatCount;
    }

    private void OnEnable()
    {
        GameEventManager.Instance.PlayerEvents.OnPlayerDefeatEnemy += OnPlayerDefeatEnemy;
    }
    private void OnDisable()
    {
        GameEventManager.Instance.PlayerEvents.OnPlayerDefeatEnemy -= OnPlayerDefeatEnemy;
    }

    private void OnPlayerDefeatEnemy(IEnemy enemy, int amount)
    {
        if (enemy is not Enemy) return;
        Enemy tempEnemy = (Enemy)enemy;
        if (tempEnemy.enemyData.EnemyType != enemyType) return;

        _currentEnemyDefeatCount += amount;

        UpdateState();

        if (_currentEnemyDefeatCount >= TargetEnemyDefeatCount)
        {
            FinishQuestStep();
        }
    }

    protected override void SetQuestStepState(string state)
    {
        int tempCount = System.Int32.Parse(state);

        OnPlayerDefeatEnemy(null, tempCount);
    }

    private void UpdateState()
    {
        string state = _currentEnemyDefeatCount.ToString();
        ChangeState(state);
    }

    public override string GetQuestProgressContent()
    {
        return _currentEnemyDefeatCount + "/" + TargetEnemyDefeatCount + " " + enemyType.ToString();
    }
}

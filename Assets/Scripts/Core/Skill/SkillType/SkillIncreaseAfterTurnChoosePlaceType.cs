using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillIncreaseAfterTurnChoosePlaceType : SkillIncreaseAfterTurnType
{
    [SerializeField] private List<GameObject> _turnPrefabs = new List<GameObject>();


    public override void Execute(Transform spawnPos, Vector3 direct, CharacterSkillScriptableObject data)
    {
        base.Execute(spawnPos, direct, data);
        SetUpTurnPrefabPosition();
    }

    private void SetUpTurnPrefabPosition()
    {
        var spawnpoint = _skillManager.GetSkillSpawnpointPos(SkillSpawnpoint.SpawnpointType.Forward8cm).position;
        foreach (var turn in _turnPrefabs)
        {
            turn.transform.position = spawnpoint;
        }
    }
}

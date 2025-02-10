using System;
using UnityEngine;

public class DungeonTimingItem : DungeonDifficultyItem
{
    public override void SetDifficultyDescript(int playerLevel, DungeonDescriptScriptableObject dungeonDescript)
    {
        DungeonDescript = dungeonDescript;
        HandlePlayerContactDungeonDependTime();
    }

    private void HandlePlayerContactDungeonDependTime()
    {
        isLock = !IsTimeInDungeonTime();
        LevelRequireUnlock.SetActive(isLock);
    }

    private bool IsTimeInDungeonTime()
    {
        DateTime curr = DateTime.Now;
        switch(DungeonType)
        {
            case DungeonDescriptScriptableObject.DungeonType.Whispers:
                return curr.Hour >=  0 && curr.Hour <= 6;
            case DungeonDescriptScriptableObject.DungeonType.Coils:
                return curr.Hour >= 6 && curr.Hour <= 12;
            case DungeonDescriptScriptableObject.DungeonType.Tempest:
                return curr.Hour >= 12 && curr.Hour <= 18;
            case DungeonDescriptScriptableObject.DungeonType.Abyss:
                return curr.Hour >= 18 && curr.Hour <= 24;
            default:
                return false;              
        }
    }
}

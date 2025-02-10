using UnityEngine;

public class DungeonSpecialDescriptPanel : DungeonDescriptPanel
{
    public override void SetDungeonDescript(DungeonDescriptScriptableObject dungeonDescript)
    {
        foreach (DungeonDifficultyItem item in DungeonDifficultyItems)
        {
            if (item is not DungeonTimingItem) continue;

            item.SetDifficultyDescript(PlayerLevelManager.Instance.Level, dungeonDescript);
        }
    }
}

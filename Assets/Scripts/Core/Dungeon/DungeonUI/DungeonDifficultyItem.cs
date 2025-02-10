using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DungeonDifficultyItem : MonoBehaviour
{
    public TextMeshProUGUI LevelRequireText;
    public GameObject LevelRequireUnlock;
    public Button ActivateButton;
    public DungeonDescriptScriptableObject.DungeonType DungeonType;
    [HideInInspector] public DungeonDescriptScriptableObject DungeonDescript;
    [HideInInspector] public bool isLock;

    public virtual void SetDifficultyDescript(int playerLevel, DungeonDescriptScriptableObject dungeonDescript)
    {
        DungeonDescript = dungeonDescript;
        int levelRequired = 0;
        switch (DungeonType)
        {
            case DungeonDescriptScriptableObject.DungeonType.Whispers:
                LevelRequireText.text = "Lvl " + dungeonDescript.WhispersLevelRequire.ToString() + " Required";
                levelRequired = dungeonDescript.WhispersLevelRequire;
                break;
            case DungeonDescriptScriptableObject.DungeonType.Coils:
                LevelRequireText.text = "Lvl " + dungeonDescript.CoilsLevelRequire.ToString() + " Required";
                levelRequired = dungeonDescript.CoilsLevelRequire;
                break;
            case DungeonDescriptScriptableObject.DungeonType.Tempest:
                LevelRequireText.text = "Lvl " + dungeonDescript.TempestLevelRequire.ToString() + " Required";
                levelRequired = dungeonDescript.TempestLevelRequire;
                break;
            case DungeonDescriptScriptableObject.DungeonType.Abyss:
                LevelRequireText.text = "Lvl " + dungeonDescript.AbyssLevelRequire.ToString() + " Required";
                levelRequired = dungeonDescript.AbyssLevelRequire;
                break;
        }

        HandlePlayerContactDungeonDependLevel(playerLevel, levelRequired, dungeonDescript);
    }

    private void HandlePlayerContactDungeonDependLevel(int playerLevel, int levelRequired, DungeonDescriptScriptableObject dungeonDescript)
    {
        isLock = playerLevel < levelRequired;
        LevelRequireUnlock.SetActive(isLock);
    }
}

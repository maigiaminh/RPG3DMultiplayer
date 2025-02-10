using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonDescriptScriptableObject", menuName = "Dungeon/DungeonDescriptData")]
public class DungeonDescriptScriptableObject : ScriptableObject
{

    [Header("Dungeon Data")]
    public DungeonData DungeonData;
    [Header("The name of the dungeon.")]
    public string DungeonName;
    public Sprite DungeonPortrait;
    [Header("Whispers")]
    [Tooltip("WhisperRewardSprites must have 3 sprites.")]
    public List<Sprite> WhisperRewardSprites;
    public int WhispersLevelRequire;
    public Sprite WhispersSprite;
    public Color WhispersColor;
    [TextArea(3, 10)]
    public string WhisperDescription;
    [Header("Coils")]
    [Tooltip("CoiCoilsRewardSprites must have 3 sprites.")]
    public List<Sprite> CoilsRewardSprites;
    public int CoilsLevelRequire;
    public Sprite CoilsSprite;
    public Color CoilsColor;
    [TextArea(3, 10)]
    public string CoilsDescription;
    [Header("Tempest")]
    [Tooltip("TempestRewardSprites must have 3 sprites.")]
    public List<Sprite> TempestRewardSprites;
    public int TempestLevelRequire;
    public Sprite TempestSprite;
    public Color TempestColor;
    [TextArea(3, 10)]
    public string TempestDescription;
    [Header("Abyss")]
    [Tooltip("AbyssRewardSprites must have 3 sprites.")]
    public List<Sprite> AbyssRewardSprites;
    public int AbyssLevelRequire;
    public Sprite AbyssSprite;
    public Color AbyssColor;
    [TextArea(3, 10)]
    public string AbyssDescription;

    public enum DungeonType
    {
        Whispers,
        Coils,
        Tempest,
        Abyss
    }


    public List<Sprite> GetRewardSprites(DungeonType dungeonType)
    {
        switch (dungeonType)
        {
            case DungeonType.Whispers:
                return WhisperRewardSprites;
            case DungeonType.Coils:
                return CoilsRewardSprites;
            case DungeonType.Tempest:
                return TempestRewardSprites;
            case DungeonType.Abyss:
                return AbyssRewardSprites;
            default:
                return null;
        }
    }
}

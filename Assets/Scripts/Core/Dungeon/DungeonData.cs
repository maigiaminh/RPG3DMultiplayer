using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonData", menuName = "Dungeon/DungeonData")]
public class DungeonData : ScriptableObject
{
    [Header("Dungeon Mode")]
    public DungeonMode DungeonMode;

    [Header("Tradition Dungeon Room Settings")]

    public int DungeonTokenBase;
    public int MinRoom;
    public int MaxRoom;


    public List<DungeonRoom> dungeonRooms = new List<DungeonRoom>();
    public List<GameObject> wallTiles;
    public List<GameObject> ceilTiles;
    public GameObject corridorTile;


    public List<Enemy> Enemies = new List<Enemy>();
    public List<Enemy> Bosses = new List<Enemy>();

    public List<ItemData> RewardItemDatas = new List<ItemData>();

    public int GetTokenBaseOnDifficulty(DungeonDescriptScriptableObject.DungeonType dungeonType)
    {
        switch (dungeonType)
        {
            case DungeonDescriptScriptableObject.DungeonType.Whispers:
                return DungeonTokenBase * 1;
            case DungeonDescriptScriptableObject.DungeonType.Coils:
                return (int)(DungeonTokenBase * (1 + 1));
            case DungeonDescriptScriptableObject.DungeonType.Tempest:
                return (int)(DungeonTokenBase * (1 + 2));
            case DungeonDescriptScriptableObject.DungeonType.Abyss:
                return (int)(DungeonTokenBase * (1 + 4));
            default:
                return DungeonTokenBase * 1;
        }
    }

    public List<ItemData> GetItemDatas(DungeonDescriptScriptableObject.DungeonType dungeonType)
    {
        Debug.Log("RewardItemData.Count: " + RewardItemDatas.Count);
        List<ItemData> items = new List<ItemData>();
        switch (dungeonType)
        {
            case DungeonDescriptScriptableObject.DungeonType.Whispers:
                items = RewardItemDatas.FindAll(x => x != null && (x.rarity == ItemData.Rarity.Common || x.rarity == ItemData.Rarity.Uncommon));
                break;
            case DungeonDescriptScriptableObject.DungeonType.Coils:
                items = RewardItemDatas.FindAll(x => x != null && (x.rarity == ItemData.Rarity.Uncommon || x.rarity == ItemData.Rarity.Rare || x.rarity == ItemData.Rarity.Common));
                break;
            case DungeonDescriptScriptableObject.DungeonType.Tempest:
                items = RewardItemDatas.FindAll(x => x != null && (x.rarity == ItemData.Rarity.Rare || x.rarity == ItemData.Rarity.Epic || x.rarity == ItemData.Rarity.Uncommon || x.rarity == ItemData.Rarity.Common));
                break;
            case DungeonDescriptScriptableObject.DungeonType.Abyss:
                items = RewardItemDatas.FindAll(x => x != null && (x.rarity == ItemData.Rarity.Epic || x.rarity == ItemData.Rarity.Legendary || x.rarity == ItemData.Rarity.Rare || x.rarity == ItemData.Rarity.Uncommon || x.rarity == ItemData.Rarity.Common));
                break;
        }
        return items;
    }

    public DungeonData GetNewDungeonData()
    {
        DungeonData newDungeonData = CreateInstance<DungeonData>();

        newDungeonData.DungeonMode = this.DungeonMode;
        newDungeonData.DungeonTokenBase = this.DungeonTokenBase;
        newDungeonData.MinRoom = this.MinRoom;
        newDungeonData.MaxRoom = this.MaxRoom;
        newDungeonData.dungeonRooms = new List<DungeonRoom>(this.dungeonRooms);
        newDungeonData.wallTiles = new List<GameObject>(this.wallTiles);
        newDungeonData.ceilTiles = new List<GameObject>(this.ceilTiles);
        newDungeonData.corridorTile = this.corridorTile;
        newDungeonData.Enemies = new List<Enemy>(this.Enemies);
        newDungeonData.Bosses = new List<Enemy>(this.Bosses);
        newDungeonData.RewardItemDatas = new List<ItemData>(this.RewardItemDatas);

        return newDungeonData;
    }

}

public enum DungeonMode
{
    Traditional,
    Special,
    Endless
}
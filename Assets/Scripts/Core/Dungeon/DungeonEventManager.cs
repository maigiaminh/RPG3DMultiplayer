using UnityEngine;

public class DungeonEventManager : Singleton<DungeonEventManager>
{
    public DungeonEvents dungeonEvents;
    public DungeonTokenEvents dungeonTokenEvents;


    protected override void Awake() {
        base.Awake();

        dungeonEvents = new DungeonEvents();
        dungeonTokenEvents = new DungeonTokenEvents();
    }

}

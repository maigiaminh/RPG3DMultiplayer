public class GameEventManager : Singleton<GameEventManager>
{

    public PlayerEvents PlayerEvents;
    public ResourceEvents ResourceEvents;
    public QuestEvents QuestEvents;
    public DialogeEvents DialogeEvents;
    public InventoryEvents InventoryEvents;
    public PlayerContactUIEvents PlayerContactUIEvents;
    public WeaponQuickSlotEvents WeaponQuickSlotEvents;
    public SkillEvents SkillEvents;
    public DamageEvent DamageEvent;

    protected override void Awake()
    {
        PlayerEvents = new PlayerEvents();
        ResourceEvents = new ResourceEvents();
        QuestEvents = new QuestEvents();
        DialogeEvents = new DialogeEvents();
        InventoryEvents = new InventoryEvents();
        PlayerContactUIEvents = new PlayerContactUIEvents();
        WeaponQuickSlotEvents = new WeaponQuickSlotEvents();
        SkillEvents = new SkillEvents();
        DamageEvent = new DamageEvent();
    }

}

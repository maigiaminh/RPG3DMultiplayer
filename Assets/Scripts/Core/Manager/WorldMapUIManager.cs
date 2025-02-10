using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WorldMapUIManager : Singleton<WorldMapUIManager>
{
    public GameObject worldMapGO;
    [SerializeField] GameObject mapTooltipGO;
    [SerializeField] GameObject mapDetailPanel;
    [SerializeField] Image mapTooltipImg;
    [SerializeField] Image worldMapImg;
    [SerializeField] Image mapPanelImg;
    [SerializeField] TextMeshProUGUI mapName;
    [SerializeField] TextMeshProUGUI mapDescription;
    [SerializeField] Button travelBtn;
    [SerializeField] GameObject lockGO;

    private Animator animator;
    public MapData CurrentMap { get; private set; }

    private void Start()
    {
        animator = mapDetailPanel.GetComponent<Animator>();
        travelBtn.onClick.AddListener(() => Travel());
    }
    public void MapHover(MapData mapData)
    {
        mapTooltipGO.SetActive(true);
        CurrentMap = mapData;

        SetMap();
    }

    public void MapExit()
    {
        mapTooltipGO.SetActive(false);
    }

    public void MapClicked()
    {
        mapDetailPanel.SetActive(true);
        animator.Play("MapDetailAppear");
    }

    private void SetMap()
    {
        mapTooltipImg.sprite = CurrentMap.reviewImage;
        mapPanelImg.sprite = CurrentMap.reviewImage;
        worldMapImg.sprite = CurrentMap.mapImage;
        mapName.text = CurrentMap.mapName;
        mapDescription.text = CurrentMap.description;

        travelBtn.interactable = CurrentMap.active;
        lockGO.SetActive(!CurrentMap.active);
    }

    private void Travel()
    {
        if (CurrentMap.active)
        {
            if (LoadingManager.Instance) LoadingManager.Instance.NewLoadScene(PlayerSpawnManager.Instance.DUNGEON_ENDLESS_SCENE_NAME);
            else SceneManager.LoadScene(PlayerSpawnManager.Instance.DUNGEON_ENDLESS_SCENE_NAME);

            DeactivateMap();
            GameEventManager.Instance.PlayerEvents.PlayerInteractStateExit();
        }
    }

    private void DeactivateMap()
    {
        mapTooltipGO.SetActive(false);
        mapDetailPanel.SetActive(false);
        worldMapGO.SetActive(false);
    }
}

using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonSpecialUIManager : MonoBehaviour
{
    public static DungeonSpecialUIManager Instance;
    public DungeonSpecialUIPanel DungeonSpecialUIPanel;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!scene.name.Equals(PlayerSpawnManager.Instance.DUNGEON_SPECIAL_SCENE_NAME)){
            DungeonSpecialUIPanel.gameObject.SetActive(false);
            return;
        }
        DungeonSpecialUIPanel.gameObject.SetActive(true);
    }
}

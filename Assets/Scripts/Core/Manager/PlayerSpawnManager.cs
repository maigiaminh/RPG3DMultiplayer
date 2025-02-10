using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawnManager : Singleton<PlayerSpawnManager>
{
    public Transform PlayerSpawnPosInGameScene;

    [HideInInspector] public string DUNGEON_SCENE_NAME = "DungeonTradional";
    [HideInInspector] public string DUNGEON_SPECIAL_SCENE_NAME = "DungeonSpecial";
    [HideInInspector] public string DUNGEON_ENDLESS_SCENE_NAME = "DungeonEndless";
    [HideInInspector] public string GAMESCENE = "MAINGAMESCENE";

    public Transform playerTrans;
    public Transform GetPlayerTrans()
    {
        if (playerTrans == null) FindPlayer();
        return playerTrans;
    }

    private void OnEnable()
    {
        FindPlayer();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.Equals(GAMESCENE))
        {
            playerTrans.position = PlayerSpawnPosInGameScene.position;
        }
    }



    public void FindPlayer()
    {
        playerTrans = FindAnyObjectByType<TankPlayer>().transform;
    }

}

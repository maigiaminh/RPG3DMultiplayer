using UnityEngine;
using UnityEngine.SceneManagement;

public class TestEnterDungeonEndless : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C)){
            SceneManager.LoadScene(PlayerSpawnManager.Instance.DUNGEON_ENDLESS_SCENE_NAME);
        }
    }
}

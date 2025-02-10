using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FailBoard : MonoBehaviour
{
    public void ReturnToMain()
    {
        DeactivePanel();
        ChangeSceneToMain();
        GameEventManager.Instance.PlayerEvents.PlayerResetOnDie();
    }

    private void ChangeSceneToMain()
    {
        var playerSpawnManager = PlayerSpawnManager.Instance;
        SceneManager.LoadScene(playerSpawnManager.GAMESCENE);
    }

    private void DeactivePanel()
    {
        gameObject.SetActive(false);
    }


}

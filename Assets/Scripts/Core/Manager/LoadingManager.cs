using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : Singleton<LoadingManager>
{
    [Header("Loading scene")]
    public GameObject loadingPanel;
    public Slider progressBarSlider;
    public TextMeshProUGUI progressBarText;

    public async void LoadScene(string sceneName)
    {
        progressBarSlider.value = 0;
        progressBarText.text = "0 %";
        
        loadingPanel.SetActive(true);

        var scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;

        do{
            await Task.Delay(100);
            progressBarSlider.value = scene.progress;
            progressBarText.text = (scene.progress * 100).ToString("0") + " %";
        } while (scene.progress < 0.9f);
        
        scene.allowSceneActivation = true;

        await Task.Delay(2000);
        
        loadingPanel.SetActive(false);
    }

    public void NewLoadScene(string sceneName)
    {
        progressBarSlider.value = 0;
        progressBarText.text = "0 %";

        Debug.Log("LOAD SCENE " + sceneName);
        
        loadingPanel.SetActive(true);
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    IEnumerator LoadSceneAsync(string sceneName){
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while(!operation.isDone){
            Debug.Log("Loading progress: " + (operation.progress * 100) + "%");
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            progressBarSlider.value = progress;
            progressBarText.text = (progress * 100).ToString("0") + " %";

            if (operation.progress >= 0.9f){
                operation.allowSceneActivation = true;
            }
            yield return null;
        }

        progressBarText.text = "100 %";

        yield return new WaitForSeconds(2f);

        loadingPanel.SetActive(false);
    }
}

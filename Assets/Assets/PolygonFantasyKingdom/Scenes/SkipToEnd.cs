using UnityEngine;
using UnityEngine.Playables;

public class SkipToEnd : MonoBehaviour
{
    public PlayableDirector playableDirector;
    public Camera finalCamera;
    public GameObject skipButton;
    
    private void Start(){
        // if (PlayerPrefs.GetInt("HasPlayedIntro", 0) == 1)
        // {
        //     Skip();
        // }
        // else
        // {
        //     PlayerPrefs.SetInt("HasPlayedIntro", 1);
        //     PlayerPrefs.Save();
        // }
    }

    public void Skip(){
        Debug.Log("CLICKED");
        playableDirector.time = playableDirector.duration;
        playableDirector.Evaluate();

        ActivateFinalCamera();
        DisableAudio();
    }

    private void ActivateFinalCamera()
    {
        Camera[] allCameras = Camera.allCameras;
        foreach (Camera cam in allCameras)
        {
            cam.enabled = cam == finalCamera;
        }
    }

    private void DisableAudio(){
        AudioSource[] allAudioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (AudioSource audioSource in allAudioSources)
        {
            audioSource.enabled = false;
        }
    }
    
}

using UnityEngine;

public class BackgroundMusicManager : Singleton<BackgroundMusicManager>
{
    public AudioClip[] backgroundTracks;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        PlayRandomTrack();
    }

    void Update()
    {
        if (!audioSource.isPlaying)
        {
            PlayRandomTrack();
        }
    }

    void PlayRandomTrack()
    {
        if (backgroundTracks.Length == 0) return; 

        int randomIndex = Random.Range(0, backgroundTracks.Length);
        audioSource.clip = backgroundTracks[randomIndex];
        audioSource.Play();
    }
}

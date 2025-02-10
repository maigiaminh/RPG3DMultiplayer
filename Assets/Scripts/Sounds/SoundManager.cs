using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    [Space(10)]
    [SerializeField] private AudioMixer audioMixer;

    [Space(10)]
    [SerializeField] private Toggle muteToggle;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Space(10)]
    [SerializeField] private SoundsSO SO;
    [SerializeField] private AudioMixerGroup soundSkillGroup; 
    private static SoundManager instance = null;
    private AudioSource audioSource;
    
    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            audioSource = GetComponent<AudioSource>();
        }
    }

    public static void PlaySound(SoundType sound, AudioSource source = null, float volume = 1)
    {
        SoundList soundList = instance.SO.sounds[(int)sound];
        AudioClip[] clips = soundList.sounds;
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];

        if (source)
        {
            source.outputAudioMixerGroup = soundList.mixer;
            source.clip = randomClip;
            source.volume = volume * soundList.volume;
            source.Play();
        }
        else
        {
            instance.audioSource.outputAudioMixerGroup = soundList.mixer;
            instance.audioSource.PlayOneShot(randomClip, volume * soundList.volume);
        }
    }

    public static SoundType GetBlockSound(int currentWeapon)
    {
        if (currentWeapon == (int)WeaponType.GreatSword) return SoundType.GREATSWORD_BLOCK_GETHIT;
        else if (currentWeapon == (int)WeaponType.Shield_and_Mace) return SoundType.SHIELD_BLOCK_GETHIT;
        else if (currentWeapon == (int)WeaponType.DualKnife) return SoundType.DUALKNIFE_BLOCK_GETHIT;
        else if (currentWeapon == (int)WeaponType.WizardStaff) return SoundType.WIZARDSTAFF_BLOCK_GETHIT;
        else return SoundType.UNARMED_BLOCK_GETHIT;
    }

    public static SoundType GetFootstepSound(string tag)
    {
        if (tag == "Sand") return SoundType.FOOTSTEP_IN_SAND;
        else if (tag == "Dirt") return SoundType.FOOTSTEP_IN_DIRT;
        else if (tag == "Stone") return SoundType.FOOTSTEP_IN_STONE;
        else if (tag == "Grass") return SoundType.FOOTSTEP_IN_GRASS;
        else if (tag == "Wood") return SoundType.FOOTSTEP_IN_WOOD;

        else return SoundType.FOOTSTEP_DEFAULT;
    }

    public static SoundType GetAttackSound(int currentWeapon)
    {
        if (currentWeapon == (int)WeaponType.GreatSword) return SoundType.GREATSWORD_ATTACK;
        else if (currentWeapon == (int)WeaponType.Bow) return SoundType.ARROW_FLY;
        else if (currentWeapon == (int)WeaponType.Shield_and_Mace) return SoundType.SHIELD_ATTACK;
        else if (currentWeapon == (int)WeaponType.DualKnife) return SoundType.DUALKNIFE_ATTACK;
        else if (currentWeapon == (int)WeaponType.WizardStaff) return SoundType.WIZARDSTAFF_ATTACK;
        else return SoundType.UNARMED_ATTACK;
    }

    public static SoundType GetAttackHitSound(int currentWeapon)
    {
        if (currentWeapon == (int)WeaponType.GreatSword) return SoundType.GREATSWORD_HIT;
        else if (currentWeapon == (int)WeaponType.Shield_and_Mace) return SoundType.SHIELD_HIT;
        else if (currentWeapon == (int)WeaponType.DualKnife) return SoundType.DUALKNIFE_HIT;
        else return SoundType.UNARMED_HIT;
    }

    public static void PauseSkillAudio()
    {
        AudioSource[] allAudioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);

        foreach (var audioSource in allAudioSources)
        {
            if (audioSource.outputAudioMixerGroup == instance.soundSkillGroup && audioSource.isPlaying)
            {
                audioSource.Pause();
            }
        }
    }

    public static void ResumeSkillAudio()
    {
        AudioSource[] allAudioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);

        foreach (var audioSource in allAudioSources)
        {
            if (audioSource.outputAudioMixerGroup == instance.soundSkillGroup)
            {
                audioSource.UnPause();
            }
        }
    }

    public void SetMasterVolume(float volume)
    {
        if(muteToggle.isOn) volume = 0.0001f;
        audioMixer?.SetFloat("masterVolume", Mathf.Log10(volume) * 20);
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer?.SetFloat("musicVolume", Mathf.Log10(volume) * 20);
    }
    public void SetSfxVolume (float volume)
    {
        audioMixer?.SetFloat("sfxVolume", Mathf.Log10(volume) * 20);
        audioMixer?.SetFloat("ingameVolume", Mathf.Log10(volume) * 20);
    }

    public void MuteVolume(bool isMuted)
    {
        if (isMuted)
        {
            SetMasterVolume(0);
        }
        else
        {
            SetMasterVolume(musicSlider.value);
        }
    }
    private void Start()
    {
        if (audioMixer == null) return;

        if (musicSlider == null || sfxSlider == null || masterSlider == null) return;
        masterSlider.value = PlayerPrefs.GetFloat("masterVolume", 1);
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume", 1);
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume", 1);

        SetMusicVolume(musicSlider.value);
        SetSfxVolume(sfxSlider.value);

        muteToggle.isOn = intToBool(PlayerPrefs.GetInt("isMuted", 0));
        if(muteToggle.isOn) {
            Debug.Log("IS MUTED? YES");
            SetMasterVolume(0.0001f);
        }
        else{
            Debug.Log("IS MUTED? NO" + masterSlider.value);

            SetMasterVolume(masterSlider.value);
        }
        
    }
 
    private void OnDisable()
    {
        if(!masterSlider || !musicSlider || !sfxSlider || !muteToggle) return;
        
        float masterVolume = masterSlider.value;
        float musicVolume = musicSlider.value;
        float sfxVolume = sfxSlider.value;
        bool isMuted = muteToggle.isOn;

        PlayerPrefs.SetFloat("masterVolume", masterVolume);
        PlayerPrefs.SetFloat("musicVolume", musicVolume);
        PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
        PlayerPrefs.SetInt("isMuted", boolToInt(isMuted));

        PlayerPrefs.Save();
    }

    int boolToInt(bool val)
    {
        if (val)
            return 1;
        else
            return 0;
    }

    bool intToBool(int val)
    {
        if (val != 0)
            return true;
        else
            return false;
    }
}

[Serializable]
public struct SoundList
{
    [HideInInspector] public string name;
    [Range(0, 1)] public float volume;
    public AudioMixerGroup mixer;
    public AudioClip[] sounds;
}

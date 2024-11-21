using System.Collections.Generic;
using UnityEngine;

public enum Sound
{
    //เอฟเฟค
    Effect1,
    Effect2,
    Effect3,
    
    //เพลง
    Music1,
    Music2,
    EndCredits
}

[System.Serializable]
public class SoundClip  
{
    public Sound sound;  
    public AudioClip clip;  
    public bool isBackground;  
}

public class SoundManage : MonoBehaviour
{
    public static SoundManage Instance;  
    public AudioSource BGaudioSource;  
    public AudioSource EffectAudioSource;  
    public List<SoundClip> soundClips;  

    [Range(0f, 1f)]
    public float masterVolume = 1f;    
    [Range(0f, 1f)]
    public float bgVolume = 0.5f;     
    [Range(0f, 1f)]
    public float effectVolume = 1f;

    private Dictionary<Sound, AudioClip> soundDictionary;  

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  
            InitializeSoundDictionary(); 
        }
        else
        {
            Destroy(gameObject);  
        }
    }

    void InitializeSoundDictionary()
    {
        soundDictionary = new Dictionary<Sound, AudioClip>();
        foreach (var soundClip in soundClips)
        {
            soundDictionary[soundClip.sound] = soundClip.clip;
        }
    }

    void Start()
    {
        LoadSettings();  
        PlayBackgroundMusic(Sound.Music1);  
    }

    public void PlayBackgroundMusic(Sound musicType = Sound.Music1)
    {
        if (soundDictionary.TryGetValue(musicType, out AudioClip clip))
        {
            BGaudioSource.clip = clip;
            BGaudioSource.volume = bgVolume * masterVolume;
            BGaudioSource.loop = true;
            BGaudioSource.Play();
        }
        else
        {
            Debug.LogWarning("Background music clip not found!");
        }
    }

    public void PlaySound(Sound sound)
    {
        if (soundDictionary.TryGetValue(sound, out AudioClip clip))
        {
            EffectAudioSource.PlayOneShot(clip, effectVolume * masterVolume);
        }
        else
        {
            Debug.LogWarning($"Sound {sound} not found!");
        }
    }

    public void UpdateVolumes()
    {
        BGaudioSource.volume = bgVolume * masterVolume;
        EffectAudioSource.volume = effectVolume * masterVolume;
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("BGVolume", bgVolume);
        PlayerPrefs.SetFloat("EffectVolume", effectVolume);
        PlayerPrefs.Save();  
    }

    public void LoadSettings()
    {
        if (PlayerPrefs.HasKey("MasterVolume"))
            masterVolume = PlayerPrefs.GetFloat("MasterVolume");

        if (PlayerPrefs.HasKey("BGVolume"))
            bgVolume = PlayerPrefs.GetFloat("BGVolume");

        if (PlayerPrefs.HasKey("EffectVolume"))
            effectVolume = PlayerPrefs.GetFloat("EffectVolume");

        UpdateVolumes();  
    }
    //เปลี่ยนเพลง
    public void ChangeToMusic()
    {
       BGaudioSource.Stop(); 
       PlayBackgroundMusic(Sound.Music1); 
    }
}

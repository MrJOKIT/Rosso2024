using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Sound
{
    Effect1,
    Effect2,
    Effect3,
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
        }
        else
        {
            Destroy(gameObject);
        }

        InitializeAudioSources();
        SceneManager.sceneLoaded += OnSceneLoaded;

        InitializeSoundDictionary(); 
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
        LoadSettings();  // โหลดค่าการตั้งค่าเสียง
        PlayBackgroundMusic(Sound.Music1);  
    }

    void InitializeAudioSources()
    {
        if (BGaudioSource == null)
        {
            BGaudioSource = GetComponent<AudioSource>();
        }
        if (EffectAudioSource == null)
        {
            EffectAudioSource = GetComponent<AudioSource>();
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeAudioSources();
        UpdateVolumes();  // อัปเดตระดับเสียงเมื่อเปลี่ยนซีน
    }

    public void PlayBackgroundMusic(Sound musicType = Sound.Music1)
    {
        if (soundDictionary.TryGetValue(musicType, out AudioClip clip))
        {
            BGaudioSource.clip = clip;
            BGaudioSource.volume = bgVolume * masterVolume;  // คำนวณระดับเสียงจริง
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
            EffectAudioSource.PlayOneShot(clip, effectVolume * masterVolume);  // คำนวณระดับเสียงจริง
        }
        else
        {
            Debug.LogWarning($"Sound {sound} not found!");
        }
    }

    public void UpdateVolumes()
    {
        BGaudioSource.volume = bgVolume * masterVolume;  // อัปเดตระดับเสียงของ BG
        EffectAudioSource.volume = effectVolume * masterVolume;  // อัปเดตระดับเสียงของ Effect
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("BGVolume", bgVolume);
        PlayerPrefs.SetFloat("EffectVolume", effectVolume);
        PlayerPrefs.Save();  // บันทึกค่าที่ตั้งไว้
    }

    public void LoadSettings()
    {
        if (PlayerPrefs.HasKey("MasterVolume"))
            masterVolume = PlayerPrefs.GetFloat("MasterVolume");

        if (PlayerPrefs.HasKey("BGVolume"))
            bgVolume = PlayerPrefs.GetFloat("BGVolume");

        if (PlayerPrefs.HasKey("EffectVolume"))
            effectVolume = PlayerPrefs.GetFloat("EffectVolume");

        UpdateVolumes();  // อัปเดตค่าระดับเสียงเมื่อโหลดการตั้งค่า
    }

    public void ChangeToMusic()
    {
        BGaudioSource.Stop(); 
        PlayBackgroundMusic(Sound.Music1);  
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

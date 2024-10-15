using System.Collections.Generic;
using UnityEngine;

public enum Sound
{
    Effect1, 
    Effect2,
    Music1
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
    public AudioSource audioSource;  
    public List<SoundClip> soundClips; 

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
        PlayBackgroundMusic();
    }

    private void PlayBackgroundMusic()
    {
        if (soundDictionary.TryGetValue(Sound.Music1, out AudioClip clip))
        {
            audioSource.clip = clip;
            audioSource.volume = bgVolume;
            audioSource.loop = true;
            audioSource.Play();
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
            audioSource.PlayOneShot(clip, effectVolume);
        }
        else
        {
            Debug.LogWarning($"Sound {sound} not found!");
        }
    }
}

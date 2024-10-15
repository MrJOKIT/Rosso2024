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
    public AudioClip clip; // AudioClip ที่เชื่อมโยง
}

public class SoundManage : MonoBehaviour
{
    public static SoundManage Instance;  
    public AudioSource audioSource;  
    public List<SoundClip> soundClips; // ใช้ List เพื่อเก็บ SoundClip

    [Range(0f, 1f)]
    public float bgVolume = 0.5f;  
    [Range(0f, 1f)]
    public float effectVolume = 1f; 

    private Dictionary<Sound, AudioClip> soundDictionary; // ใช้ Dictionary สำหรับการค้นหาเสียง

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
            InitializeSoundDictionary(); // เรียกใช้เมธอดเพื่อสร้าง Dictionary
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

    public void PlaySound(Sound sound)
    {
        if (soundDictionary.TryGetValue(sound, out AudioClip clip))
        {
            if (sound == Sound.Music1) // ถ้าเป็นเพลงพื้นหลัง
            {
                audioSource.clip = clip;
                audioSource.volume = bgVolume;
                audioSource.loop = true;
                audioSource.Play();
            }
            else // ถ้าเป็นเสียงเอฟเฟกต์
            {
                audioSource.PlayOneShot(clip, effectVolume);
            }
        }
        else
        {
            Debug.LogWarning($"Sound {sound} not found!");
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using VInspector;

[Serializable]
public class SoundPlay
{
    public SoundManager.SoundName soundName;
    public AudioClip clip;
    public AudioMixerGroup AudioMixerGroup;
    [HideInInspector]public AudioSource audioSource;
}
[Serializable]
public class VfxPlay
{
    public SoundManager.VfxName vfxName;
    public AudioClip clip;
}
public class SoundManager : MonoBehaviour
{
    public SoundName currentBGM;
    [SerializeField] private SoundPlay[] sounds;
    [SerializeField] private VfxPlay[] vfxs;
    public AudioSource vfxSource;
    public static SoundManager instace;
    public enum SoundName
    {
        Empty,
        BattleBGM,
        BonusBGM,
    }
    public enum VfxName
    {
        
    }
    
    //How to use > SoundManager.instace.Play( SoundManager.SoundName.sound name); <//
    private void Awake()
    {
        if (instace == null)
        {
            instace = this;
        }
        else
        {
            Destroy( this );
            return;
        }
        //DontDestroyOnLoad( gameObject );
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Play(SoundName.BattleBGM);
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            Play(SoundName.BonusBGM);
        }
    }

    public void PlayVFX(VfxName vfxName)
    {
        vfxSource.PlayOneShot(GetVfx(vfxName).clip);
    }

    public void Play(SoundName name)
    {
        if (currentBGM != name)
        {
            if (currentBGM != SoundName.Empty)
            {
                StartCoroutine(FadeOut(GetSound(currentBGM).audioSource,1f));
                currentBGM = name;
            }
            else
            {
                currentBGM = name;
            }
        }
        SoundPlay sound = GetSound(name);
        if (sound.audioSource == null)
        {
            sound.audioSource = gameObject.AddComponent<AudioSource>();

            sound.audioSource.outputAudioMixerGroup = sound.AudioMixerGroup;
            sound.audioSource.clip = sound.clip;
            sound.audioSource.loop = true;
            StartCoroutine(FadeIn(sound.audioSource, 1f));
        }
        else
        {
            StartCoroutine(FadeIn(sound.audioSource, 1f));
        }
    }
    public void Stop(SoundName name)
    {
        SoundPlay sound = GetSound(name);
        StartCoroutine(FadeOut(sound.audioSource, 1f));
    }
    IEnumerator FadeIn (AudioSource audioSource, float fadeTime) 
    {
        float startVolume = 0.2f;

        audioSource.volume = 0;
        audioSource.Play();

        while (audioSource.volume < 1.0f)
        {
            audioSource.volume += startVolume * Time.deltaTime / fadeTime;

            yield return null;
        }

        audioSource.volume = 1f;
        
    }
    IEnumerator FadeOut (AudioSource audioSource, float fadeTime) 
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeTime;

            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }
    

    
    
    private SoundPlay GetSound( SoundName name)
    {
        return Array.Find(sounds, s => s.soundName == name);
    }
    private VfxPlay GetVfx( VfxName name)
    {
        return Array.Find(vfxs, s => s.vfxName == name);
    }
        
        
}
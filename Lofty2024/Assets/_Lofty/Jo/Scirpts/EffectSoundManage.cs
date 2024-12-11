using System.Collections.Generic;
using UnityEngine;

public class EffectSoundManage : MonoBehaviour
{
    public static EffectSoundManage instance;

    [SerializeField] private AudioSource SFX;
    [SerializeField] private List<AudioClip> effect = new List<AudioClip>();

    private const string SFX_VOLUME_PREF_KEY = "SFXVolume";

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        float savedSFXVolume = PlayerPrefs.GetFloat(SFX_VOLUME_PREF_KEY, 1f);
        SetSFXVolume(savedSFXVolume);
    }

    public void PlaySFX(string effectName)
    {
        AudioClip clip = effect.Find(e => e.name == effectName);

        if (clip != null)
        {
            SFX.PlayOneShot(clip);
        }
    }

    public void SetSFXVolume(float value)
    {
        SFX.volume = value;
    }
}
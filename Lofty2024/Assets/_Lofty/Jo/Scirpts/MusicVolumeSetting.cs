using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class MusicVolumeSetting : MonoBehaviour
{
    public static MusicVolumeSetting instance;

    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [SerializeField] private TextMeshProUGUI musicVolumeText;
    [SerializeField] private TextMeshProUGUI sfxVolumeText;

    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private List<AudioClip> musicTracks;

    private const string MIXER_MUSIC = "MusicVolume";
    private const string MIXER_SFX = "SFXVolume";
    private const string MUSIC_VOLUME_PREF_KEY = "MusicVolume";
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

        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);

        float savedMusicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_PREF_KEY, 1f);
        float savedSFXVolume = PlayerPrefs.GetFloat(SFX_VOLUME_PREF_KEY, 1f);
        musicSlider.value = savedMusicVolume;
        sfxSlider.value = savedSFXVolume;

        UpdateMusicVolumeText(savedMusicVolume);
        UpdateSFXVolumeText(savedSFXVolume);

        if (musicTracks.Count > 0)
        {
            PlayMusic(musicTracks[0]);
        }
    }

    void SetMusicVolume(float value)
    {
        mixer.SetFloat(MIXER_MUSIC, Mathf.Log10(value) * 20);
        UpdateMusicVolumeText(value);
    }

    void SetSFXVolume(float value)
    {
        mixer.SetFloat(MIXER_SFX, Mathf.Log10(value) * 20);
        UpdateSFXVolumeText(value);
    }

    void UpdateMusicVolumeText(float value)
    {
        musicVolumeText.text = $"{(value * 100).ToString("0")}%";
    }

    void UpdateSFXVolumeText(float value)
    {
        sfxVolumeText.text = $"{(value * 100).ToString("0")}%";
    }

    public void ChangeMusic(string trackName)
    {
        AudioClip track = musicTracks.Find(t => t.name.Equals(trackName, System.StringComparison.OrdinalIgnoreCase));

        if (track != null)
        {
            PlayMusic(track);
        }
    }

    public void PlayMusic(AudioClip track)
    {
        if (track != null)
        {
            musicAudioSource.clip = track;
            musicAudioSource.Play();
            Debug.Log($"Now playing: {track.name}");
        }
        else
        {
           
        }
    }

    public void ApplySettings()
    {
        PlayerPrefs.SetFloat(MUSIC_VOLUME_PREF_KEY, musicSlider.value);
        PlayerPrefs.SetFloat(SFX_VOLUME_PREF_KEY, sfxSlider.value);
        PlayerPrefs.Save();
        Debug.Log("Settings Applied");
    }
    
}

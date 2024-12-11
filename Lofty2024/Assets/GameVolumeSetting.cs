using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameVolumeSetting : MonoBehaviour
{
    public bool settingOpen;
    public GameObject settingCanvas;
    [Space(20)]
    public AudioMixer mixer;
    public Slider musicSlider;
    public Slider sfxSlider;
    public TextMeshProUGUI musicVolumeText;
    public TextMeshProUGUI sfxVolumeText;
    private const string MIXER_MUSIC = "MusicVolume";
    private const string MIXER_SFX = "SFXVolume";
    private const string MUSIC_VOLUME_PREF_KEY = "MusicVolume";
    private const string SFX_VOLUME_PREF_KEY = "SFXVolume";

    private void Awake()
    {
        settingCanvas.SetActive(false);
        settingOpen = false;
        
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);

        float savedMusicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_PREF_KEY, 1f);
        float savedSFXVolume = PlayerPrefs.GetFloat(SFX_VOLUME_PREF_KEY, 1f);
        musicSlider.value = savedMusicVolume;
        sfxSlider.value = savedSFXVolume;

        UpdateMusicVolumeText(savedMusicVolume);
        UpdateSFXVolumeText(savedSFXVolume);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpenSetting();
        }
    }

    public void OpenSetting()
    {
        if (settingOpen)
        {
            settingCanvas.SetActive(false);
            settingOpen = false;
        }
        else
        {
            settingCanvas.SetActive(true);
            settingOpen = true;
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
}

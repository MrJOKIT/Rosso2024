using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsSound : MonoBehaviour
{
    [SerializeField] private Slider masterVolumeSlider;      
    [SerializeField] private Slider bgVolumeSlider;         
    [SerializeField] private Slider effectVolumeSlider;     
    [SerializeField] private Button applyButton;            

    [SerializeField] private TextMeshProUGUI masterVolumeText;  
    [SerializeField] private TextMeshProUGUI bgVolumeText;      
    [SerializeField] private TextMeshProUGUI effectVolumeText;  

    void Start()
    {
        masterVolumeSlider.value = SoundManage.Instance.masterVolume;
        bgVolumeSlider.value = SoundManage.Instance.bgVolume;
        effectVolumeSlider.value = SoundManage.Instance.effectVolume;

        UpdateMasterVolumeText(masterVolumeSlider.value);
        UpdateBGVolumeText(bgVolumeSlider.value);
        UpdateEffectVolumeText(effectVolumeSlider.value);

        masterVolumeSlider.onValueChanged.AddListener((value) => UpdateMasterVolume(value));  
        bgVolumeSlider.onValueChanged.AddListener((value) => UpdateVolumeLevels());  
        effectVolumeSlider.onValueChanged.AddListener((value) => UpdateVolumeLevels());  
        applyButton.onClick.AddListener(ApplySettings);  
    }

    void UpdateMasterVolume(float value)
    {
        SoundManage.Instance.masterVolume = value;
        UpdateMasterVolumeText(value);
        UpdateVolumeLevels();
    }

    void UpdateMasterVolumeText(float value)
    {
        masterVolumeText.text = $"{(value * 100).ToString("0")}%";  
    }

    void UpdateVolumeLevels()
    {
        float bgVolumeWithMaster = bgVolumeSlider.value * SoundManage.Instance.masterVolume;
        float effectVolumeWithMaster = effectVolumeSlider.value * SoundManage.Instance.masterVolume;

        SoundManage.Instance.bgVolume = bgVolumeWithMaster;
        SoundManage.Instance.effectVolume = effectVolumeWithMaster;

        UpdateBGVolumeText(bgVolumeSlider.value);
        UpdateEffectVolumeText(effectVolumeSlider.value);

        SoundManage.Instance.UpdateVolumes();
    }

    void ApplySettings()
    {
        SoundManage.Instance.bgVolume = bgVolumeSlider.value * SoundManage.Instance.masterVolume;
        SoundManage.Instance.effectVolume = effectVolumeSlider.value * SoundManage.Instance.masterVolume;

        SoundManage.Instance.SaveSettings();  
        SoundManage.Instance.UpdateVolumes();  

        UpdateBGVolumeText(bgVolumeSlider.value);
        UpdateEffectVolumeText(effectVolumeSlider.value);
    }

    void UpdateBGVolumeText(float value)
    {
        bgVolumeText.text = $"{(value * 100).ToString("0")}%";
    }

    void UpdateEffectVolumeText(float value)
    {
        effectVolumeText.text = $"{(value * 100).ToString("0")}%";
    }
}

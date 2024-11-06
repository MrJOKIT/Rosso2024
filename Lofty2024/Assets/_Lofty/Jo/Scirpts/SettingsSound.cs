using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsSound : MonoBehaviour
{
    [SerializeField] private Slider bgVolumeSlider;        
    [SerializeField] private Slider effectVolumeSlider;    
    [SerializeField] private Button applyButton;           

    void Start()
    {
        bgVolumeSlider.value = SoundManage.Instance.bgVolume;
        effectVolumeSlider.value = SoundManage.Instance.effectVolume;
        applyButton.onClick.AddListener(ApplySettings);
    }

    void ApplySettings()
    {
        SoundManage.Instance.bgVolume = bgVolumeSlider.value;
        SoundManage.Instance.effectVolume = effectVolumeSlider.value;

        SoundManage.Instance.UpdateVolumes();

        // Log for confirmation (optional)
        Debug.Log($"Applied settings: BG Volume = {bgVolumeSlider.value}, Effect Volume = {effectVolumeSlider.value}");
    }
}
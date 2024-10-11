using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageSliderTracker : MonoBehaviour
{
    public Slider trackSlider;

    public void UpdateImageFill()
    {
        GetComponent<Image>().fillAmount = trackSlider.value;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AnnouncementManager : Singeleton<AnnouncementManager>
{
    public GameObject announcementCanvas;
    public TextMeshProUGUI announcementText;
    public string currentText;
    public float textTime;

    public override void Awake()
    {
        announcementText.text = String.Empty;
        announcementCanvas.SetActive(false);
    }

    private void Update()
    {
        if (textTime > 0)
        {
            announcementText.text = currentText;
            textTime -= Time.deltaTime;
            if (textTime < 0)
            {
                currentText = String.Empty;
                announcementText.text = String.Empty;
                announcementCanvas.SetActive(false);
                textTime = 0;
            }
        }
    }

    public void ShowTextTimer(string text,float timeToShow)
    {
        announcementCanvas.SetActive(true);
        currentText = text;
        textTime = timeToShow;
    }
}

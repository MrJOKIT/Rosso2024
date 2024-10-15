using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AnnouncementManager : Singeleton<AnnouncementManager>
{
    public TextMeshProUGUI announcementText;
    public string currentText;
    public float textTime;

    public override void Awake()
    {
        announcementText.text = String.Empty;
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
                textTime = 0;
            }
        }
    }

    public void ShowTextTimer(string text,float timeToShow)
    {
        currentText = text;
        textTime = timeToShow;
    }
}

using System.Collections;
using TMPro;
using UnityEngine;

public class AnnouncementManager : Singeleton<AnnouncementManager>
{
    public GameObject announcementCanvas;
    public TextMeshProUGUI announcementText;
    
    private Coroutine announcementCoroutine;

    public override void Awake()
    {
        base.Awake();
        announcementCanvas.SetActive(false);
    }

    public void ShowTextTimer(string text, float timeToShow)
    {
        if (announcementCoroutine != null)
            StopCoroutine(announcementCoroutine);

        announcementCanvas.SetActive(true);
        announcementCoroutine = StartCoroutine(DisplayAnnouncement(text, timeToShow));
    }

    private IEnumerator DisplayAnnouncement(string text, float duration)
    {
        announcementText.text = text;
        yield return new WaitForSeconds(duration);
        announcementText.text = string.Empty;
        announcementCanvas.SetActive(false);
    }
}
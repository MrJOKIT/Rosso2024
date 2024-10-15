using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Wallpaper : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image backgroundImage; // ลาก Image ของพื้นหลังเข้ามาที่นี่ใน Inspector
    public Sprite[] backgroundSprites; // ลากภาพที่ต้องการเปลี่ยนเข้ามาที่นี่ใน Inspector
    private int currentIndex = 0;
    private bool isMouseOver = false;

    void Start()
    {
        backgroundImage.sprite = backgroundSprites[currentIndex]; // ตั้งค่าให้ backgroundImage เป็นภาพแรก
        StartCoroutine(ChangeBackground());
    }

    void Update()
    {
        if (isMouseOver)
        {
          
            float mouseX = Input.GetAxis("Mouse X") * 1f; 
            float mouseY = Input.GetAxis("Mouse Y") * 1f;

            backgroundImage.transform.position += new Vector3(mouseX, mouseY, 0);
        }
    }

    private IEnumerator ChangeBackground()
    {
        yield return new WaitForSeconds(8f); 

        while (true)
        {
           
            int nextIndex = (currentIndex + 1) % backgroundSprites.Length; 
            backgroundImage.sprite = backgroundSprites[nextIndex]; 

           
            yield return StartCoroutine(FadeIn(backgroundImage, 0.5f)); 
            currentIndex = nextIndex;
            yield return new WaitForSeconds(8f); 

            yield return StartCoroutine(FadeOut(backgroundImage, 0.5f)); 
        }
    }

    private IEnumerator FadeIn(Image img, float duration)
    {
        Color color = img.color;
        color.a = 0; 
        img.color = color;

        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, time / duration);
            img.color = color;
            yield return null;
        }
    }

    private IEnumerator FadeOut(Image img, float duration)
    {
        Color color = img.color;
        float startAlpha = color.a;

        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, 0, time / duration);
            img.color = color;
            yield return null;
        }
    }

    
    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
    }

    
    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
    }
    
}

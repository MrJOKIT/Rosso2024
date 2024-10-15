using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ImageMove : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image[] targetImages; // อาเรย์ของ Images ที่ต้องการขยับ
    private bool isMouseOver = false;

    void Update()
    {
        if (isMouseOver)
        {
            float mouseX = Input.GetAxis("Mouse X") * 0.6f; 
            float mouseY = Input.GetAxis("Mouse Y") * 0.6f;

            foreach (Image targetImage in targetImages)
            {
                targetImage.transform.position += new Vector3(mouseX, mouseY, 0);
            }
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
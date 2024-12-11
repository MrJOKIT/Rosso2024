using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TurnSlot : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public bool isAllySlot;
    public Enemy enemyOwner;
    public Image turnIcon;

    public void SetSlot(Enemy enemy,Sprite spriteIcon)
    {
        enemyOwner = enemy;
        turnIcon.sprite = spriteIcon;
    }
    public void ClearSlot()
    {
        Destroy(gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Mouse Enter");
        if (isAllySlot)
        {
            MouseSelectorManager.Instance.uiCanvas.SetActive(false);
        }
        else
        {
            enemyOwner.focusArrow.SetActive(true);
            MouseSelectorManager.Instance.permanentActive = true;
            MouseSelectorManager.Instance.ShowEnemyData(enemyOwner);
        }
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isAllySlot)
        {
            return;
        }
        enemyOwner.focusArrow.SetActive(false);
        Debug.Log("Mouse Exit");
        MouseSelectorManager.Instance.permanentActive = false;
        MouseSelectorManager.Instance.uiCanvas.SetActive(false);
    }
}

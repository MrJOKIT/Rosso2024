using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TurnSlot : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public bool isAllySlot;
    public Enemy enemyOwner;
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
        enemyOwner.focusArrow.SetActive(false);
        Debug.Log("Mouse Exit");
        MouseSelectorManager.Instance.permanentActive = false;
        MouseSelectorManager.Instance.uiCanvas.SetActive(false);
    }
}

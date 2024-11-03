using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MouseSelectorManager : Singeleton<MouseSelectorManager>
{
    
    public LayerMask layerToHit;

    [Header("Enemy Data UI")] 
    public GridMover selectedGrid;
    public Enemy selectedEnemy;
    public GameObject uiCanvas;
    public GameObject hearthPrefabUI;
    public Transform healthUiParent;
    public List<HealthUI> healthUI;
    
    
    void Update()
    {
        MouseRay();
        
    }

    private void MouseRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit; 
        if (Physics.Raycast(ray,out hit,Mathf.Infinity,layerToHit))
        {
            if (hit.collider != null)
            {
                if (hit.transform.GetComponent<GridMover>() != selectedGrid || hit.transform.GetComponent<GridMover>() == null )
                {
                    selectedGrid = hit.transform.GetComponent<GridMover>();
                    GetComponent<GameManager>().currentRoomPos.GetComponent<RoomManager>().ClearSelectedGird();
                    selectedGrid.onHover = true;
                }
            
                if (hit.transform.GetComponent<GridMover>().enemy != selectedEnemy || hit.transform.GetComponent<GridMover>() == null )
                {
                    if (hit.transform.GetComponent<GridMover>().gridState == GridState.OnEnemy)
                    {
                        uiCanvas.SetActive(true);
                        selectedEnemy = hit.transform.GetComponent<GridMover>().enemy;
                        uiCanvas.GetComponent<EnemyHealthData>().SetEnemyData(selectedEnemy.enemyData.name,selectedEnemy.enemyData.enemySprite);
                        CreateHearth();
                    }
                    else
                    {
                        uiCanvas.SetActive(false);
                    }
                
                }
                else if (hit.transform.GetComponent<GridMover>().enemy == selectedEnemy && hit.transform.GetComponent<GridMover>().enemy != null)
                {
                    uiCanvas.SetActive(true);
                }
                else
                {
                    uiCanvas.SetActive(false);
                }
            }
        }
        else
        {
            GetComponent<GameManager>().currentRoomPos.GetComponent<RoomManager>().ClearSelectedGird();
            selectedGrid = null;
            selectedEnemy = null;
            uiCanvas.SetActive(false);
        }
    }
    private void CreateHearth()
    {
        foreach (HealthUI health in healthUI.ToList())
        {
            Destroy(health.gameObject);
            healthUI.Remove(health);
        }

        for (int a = 0; a < selectedEnemy.enemyData.enemyMaxHealth; a++)
        { 
            GameObject health = Instantiate(hearthPrefabUI, healthUiParent);
            healthUI.Add(health.GetComponent<HealthUI>());
        }

        UpdateHearthUI();
        
    }

    public void UpdateHearthUI()
    {
        for (int a = 0; a < selectedEnemy.enemyMaxHealth; a++)
        {
            healthUI[a].ActiveHearth(a < selectedEnemy.enemyHealth);
        }
    }

    public void ClearSelector(Enemy enemy)
    {
        if (enemy != selectedEnemy)
        {
            return;
        }
        foreach (HealthUI health in healthUI.ToList())
        {
            Destroy(health.gameObject);
            healthUI.Remove(health);
        }
        selectedEnemy = null;
        uiCanvas.SetActive(false);
    }
    
    
}

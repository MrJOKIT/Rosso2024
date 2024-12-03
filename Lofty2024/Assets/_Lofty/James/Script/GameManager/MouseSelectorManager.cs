using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MouseSelectorManager : Singeleton<MouseSelectorManager>
{
    
    public LayerMask layerToHit;
    public bool permanentActive;
    
    [Header("Enemy Data UI")] 
    public GridMover selectedGrid;
    public Enemy selectedEnemy;
    public GameObject uiCanvas;
    public GameObject hearthPrefabUI;
    public Transform healthUiParent;
    public List<HealthUI> healthUI;
    
    
    void Update()
    {
        if (permanentActive || GetComponent<GameManager>().OnLoad || GetComponent<SceneLoading>().loadSucces == false)
        {
            return;
        }
        MouseRay();
    }

    private void MouseRay()
    {
        if (Camera.main == null)
        {
            Debug.Log("Main camera is disable"); 
            return;
        }
        if (GetComponent<RandomCardManager>().isRandom)
        {
            return;
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit; 
        if (Physics.Raycast(ray,out hit,Mathf.Infinity,layerToHit))
        {
            if (hit.collider != null)
            {

                if (hit.transform.GetComponent<GridMover>().gridState != GridState.OnEnemy)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        Transform spawnTrans = new GameObject().transform;
                        spawnTrans.position = new Vector3(hit.transform.position.x, 0.25f, hit.transform.position.z);
                        VisualEffectManager.Instance.CallEffect(EffectName.Pop, spawnTrans,1.5f);
                        Destroy(spawnTrans.gameObject,0.5f);
                    }
                }

                if (hit.transform.GetComponent<GridMover>().gridState == GridState.OnEnemy)
                {
                    GetComponent<GameManager>().ChangeCursor(CursorType.AttackCursor);
                }
                else
                {
                    GetComponent<GameManager>().ChangeCursor(CursorType.DataCursor);
                }
                
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
                        uiCanvas.GetComponent<EnemyHealthData>().SetEnemyData(selectedEnemy.enemyData.name,selectedEnemy.enemyData.enemySprite,selectedEnemy.enemyHealth,selectedEnemy.enemyData.damage);
                        CreateHearth(selectedEnemy);
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
            GetComponent<GameManager>().ChangeCursor(CursorType.DefaultCursor);
            GetComponent<GameManager>().currentRoomPos.GetComponent<RoomManager>().ClearSelectedGird();
            selectedGrid = null;
            selectedEnemy = null;
            uiCanvas.SetActive(false);
        }
    }

    public void ShowEnemyData(Enemy enemy)
    {
        uiCanvas.SetActive(true);
        uiCanvas.GetComponent<EnemyHealthData>().SetEnemyData(enemy.enemyData.name,enemy.enemyData.enemySprite,enemy.enemyHealth,enemy.enemyData.damage);
        CreateHearth(enemy);
    }
    private void CreateHearth(Enemy enemy)
    {
        foreach (HealthUI health in healthUI.ToList())
        {
            Destroy(health.gameObject);
            healthUI.Remove(health);
        }

        for (int a = 0; a < enemy.enemyData.enemyMaxHealth; a++)
        { 
            GameObject health = Instantiate(hearthPrefabUI, healthUiParent);
            healthUI.Add(health.GetComponent<HealthUI>());
        }

        UpdateHearthUI(enemy);
        
    }

    public void UpdateHearthUI(Enemy enemy)
    {
        for (int a = 0; a < enemy.enemyMaxHealth; a++)
        {
            healthUI[a].ActiveHearth(a < enemy.enemyHealth);
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

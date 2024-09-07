using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/*************This class contains the functions for the placed objects such as Move etc. It can easily be expanded to contain more.**************/
public class GridObjectOptions : MonoBehaviour
{
    //Cached variables
    GridSelector gridSelector;
    ObjectPlacer objectPlacer;
    ObjectRemover objectRemover;
    ObjectSelector objectSelector;
    GameObject selectedObj;

    //Private variables
    bool moving = false;
    float moveTimeout = 0;

    //Serialized variables
    [Tooltip("Make sure to link up your camera to the canvas in world space and link your grid object options function to the relevent button")]
    [SerializeField] GameObject menuCanvas;
    [SerializeField] float menuHover;
    [SerializeField] bool dragMove;
    [SerializeField] bool dragAfterSelect;
    [SerializeField] Material upgradeMaterial;
    [SerializeField] bool swapMeshBeforeUpgrade;

    //Accessors
    public GameObject SelectedObject
    {
        set { selectedObj = value; }
    }
    public bool DragMove
    {
        get { return dragMove; }
    }
    public bool Moving
    {
        get { return moving; }
    }
    public bool DragAfterSelect
    {
        get { return dragAfterSelect; }
    }
    public GameObject MenuCanvas
    {
        get { return menuCanvas; }
    }
    public float MoveTimeout
    {
        get { return moveTimeout; }
    }
    private void Awake()
    {

    }
    private void Start()
    {
        //Caches objects
        gridSelector = GridBuilder2Manager.Instance.GridSelector;
        objectPlacer = GridBuilder2Manager.Instance.ObjectPlacer;
        objectRemover = GridBuilder2Manager.Instance.ObjectRemover;
        objectSelector = GridBuilder2Manager.Instance.ObjectSelector;
    }

    private void Update()
    {
        if (dragMove)
        {
            if (gridSelector && gridSelector.SelectedGameObjectToPlace && moving)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    DragMoveEnd();
                }
            }  
        }
        if(!moving)
        {
            if(moveTimeout > 0)
            {
                moveTimeout -= Time.deltaTime;
            }
            else
            {
                moveTimeout = 0;
            }
        }
    }

    //Opens the menu canvas assigned in the inspector 
    public void OpenMenu(Vector3 pos, GameObject obj)
    {
        if (menuCanvas)
        {
            menuCanvas.SetActive(true);
            menuCanvas.transform.position = new Vector3(pos.x, pos.y + (objectPlacer.GetObjectHeight(obj) + menuHover), pos.z);
        }
    }

    public void CloseMenu()
    {
        if (menuCanvas)
        {
            menuCanvas.SetActive(false);
        }
    }

    //The standard move function, essentially sending the placed object back to the GridSelecter to preview it again
    public void Move()
    {
        if(selectedObj && selectedObj.GetComponent<GridObject>()) 
        {
            if (objectPlacer && objectRemover && gridSelector)
            {
                moving = true;
                moveTimeout = 0;
                //Gets the data from the selected object
                var selectedObjData = selectedObj.GetComponent<GridObject>().data;

                //Unblock cells so can replace in the same spot if we want to
                objectRemover.UnblockCells(selectedObjData.CheckPositions, selectedObjData.GridSquare);

                if (objectPlacer)
                {
                    if (objectPlacer.HideCellsUnderPlacedObj)
                    {
                        //If this option is enabled, unhides the objects taken cells
                        objectPlacer.ShowRemovedObjCells(selectedObjData.GridSquare, selectedObjData.CheckPositions);
                    }
                }

                //Sends the selected object to the grid selector for previewing, moving and rotation
                gridSelector.SetGameObjectToPlace(selectedObj.GetComponent<Building>());

                CloseMenu();

                //Recieves an event to deselect after moving an object
                objectPlacer.MovedObjEvent.AddListener(() =>
                {
                    moveTimeout = 0.1f;
                    moving = false;
                    objectSelector.DeselectObject();
                });
            }
        }
    }

    //Drag variation of the move function
    public void DragMoveStart()
    {
        if (selectedObj && selectedObj.GetComponent<GridObject>())
        {
            if (objectPlacer && objectRemover && gridSelector)
            {
                gridSelector.DragMove = true;
                moving = true;
                var selectedObjData = selectedObj.GetComponent<GridObject>().data;
                objectRemover.UnblockCells(selectedObjData.CheckPositions, selectedObjData.GridSquare);
                if (objectPlacer.HideCellsUnderPlacedObj)
                {
                    objectPlacer.ShowRemovedObjCells(selectedObjData.GridSquare, selectedObjData.CheckPositions);
                }
                gridSelector.SetGameObjectToPlace(selectedObj.GetComponent<Building>());
                CloseMenu();
            }
        }
    }

    public void DragMoveEnd()
    {
        if (objectPlacer && objectRemover && gridSelector)
        {
            gridSelector.PlaceObjectIfEmpty(gridSelector.CurrentGrid);

            if (selectedObj && objectSelector.SelectedMaterial != null)
            {
                var selectedObject = selectedObj.GetComponent<GridObject>();
                selectedObject.ReapplyOriginalMaterials();
            }
            objectSelector.DeselectObject();
            moving = false;
            gridSelector.DragMove = false;
        }
    }

    public void StartUpgrade()
    {
        StartCoroutine(Upgrade());
    }

    private IEnumerator Upgrade()
    {
        if (objectPlacer && objectRemover && gridSelector)
        {
            if (selectedObj && selectedObj.GetComponent<UpgradeData>())
            {
                UpgradeData upgradeData = selectedObj.GetComponent<UpgradeData>();

                //This prevents from upgrading if there is no upgrade available
                if (upgradeData.UpgradePrefabs.Count > upgradeData.CurrentUpgradeLevel - 1)
                {
                    //Gets the next upgrade object
                    GameObject nextUpgradeGO = upgradeData.UpgradePrefabs[upgradeData.CurrentUpgradeLevel - 1];

                    //Current level mesh and mats
                    MeshFilter meshFilter = selectedObj.GetComponent<MeshFilter>();
                    MeshRenderer meshRend = selectedObj.GetComponent<MeshRenderer>();

                    MeshFilter upgradeMeshFilter = null;
                    MeshRenderer upgradeMeshRend = null;
                    if (upgradeData.UpgradePrefabs[upgradeData.CurrentUpgradeLevel - 1])
                    {
                        //Next level mesh and mats
                        upgradeMeshFilter = nextUpgradeGO.GetComponent<MeshFilter>();
                        upgradeMeshRend = nextUpgradeGO.GetComponent<MeshRenderer>();
                    }
                    CloseMenu();


                    GameObject timerCanvas = null;

                    //If the time to wait is greater than 0
                    if (upgradeData.UpgradeTimes[upgradeData.CurrentUpgradeLevel - 1] > 0)
                    {
                        //There is a canvas ready to add
                        if (objectPlacer.TimerCanvas)
                        {
                            //If building on a timer to begin with
                            if (selectedObj.GetComponent<GridObject>().data.BuildTimeRemaining == -1 || selectedObj.GetComponent<GridObject>().data.BuildTimeRemaining == 0)
                            {
                                selectedObj.GetComponent<GridObject>().data.BuildTimeRemaining = upgradeData.UpgradeTimes[upgradeData.CurrentUpgradeLevel - 1];
                            }

                            timerCanvas = objectPlacer.CreateTimerCanvas(
                                upgradeData.UpgradeTimes[upgradeData.CurrentUpgradeLevel - 1],
                                selectedObj,
                                selectedObj.GetComponent<GridObject>());
                        }
                    }


                    //Swaps object before it has upgraded
                    if (swapMeshBeforeUpgrade)
                    {
                        if (selectedObj != null)
                        {
                            if (upgradeData.UpgradePrefabs[upgradeData.CurrentUpgradeLevel - 1])
                            {
                                meshFilter.sharedMesh = upgradeMeshFilter.sharedMesh;
                                meshRend.sharedMaterials = upgradeMeshRend.sharedMaterials;
                            }
                        }

                        if (upgradeMaterial)
                        {
                            gridSelector.ChangeObjMat(selectedObj, upgradeMaterial);
                        }

                        //Upgrading was saved, and midway through
                        if(selectedObj.GetComponent<GridObject>().data.BuildTimeRemaining < upgradeData.UpgradeTimes[upgradeData.CurrentUpgradeLevel - 1])
                        {
                            //Waits for the remaining upgrade time
                            yield return new WaitForSeconds(selectedObj.GetComponent<GridObject>().data.BuildTimeRemaining);
                        }
                        else
                        {
                            //Waits for the full upgrade time
                            yield return new WaitForSeconds(upgradeData.UpgradeTimes[upgradeData.CurrentUpgradeLevel - 1]);
                        }



                        if (timerCanvas != null)
                        {
                            Destroy(timerCanvas);
                        }

                        if (selectedObj != null)
                        {
                            //Gets the previous object and sets it build time so it can be selected again
                            meshRend.gameObject.GetComponent<GridObject>().data.BuildTimeRemaining = -1;
                            meshRend.gameObject.GetComponent<GridObject>().ReapplyOriginalMaterials();
                        }
                    }
                    //Swaps object after upgrading
                    else
                    {
                        if(upgradeMaterial)
                        {
                            gridSelector.ChangeObjMat(selectedObj, upgradeMaterial);
                        }


                        if (selectedObj.GetComponent<GridObject>().data.BuildTimeRemaining < upgradeData.UpgradeTimes[upgradeData.CurrentUpgradeLevel - 1])
                        {
                            //Waits for the remaining upgrade time
                            yield return new WaitForSeconds(selectedObj.GetComponent<GridObject>().data.BuildTimeRemaining);
                        }
                        else
                        {
                            //Waits for the full upgrade time
                            yield return new WaitForSeconds(upgradeData.UpgradeTimes[upgradeData.CurrentUpgradeLevel - 1]);
                        }

                        if (timerCanvas != null)
                        {
                            Destroy(timerCanvas);
                        }

                        //Check to see if the object has not been removed during upgrade time
                        if (selectedObj != null)
                        {
                            //Gets the previous object and sets it build time so it can be selected again
                            meshRend.gameObject.GetComponent<GridObject>().data.BuildTimeRemaining = -1;

                            if (upgradeData.UpgradePrefabs[upgradeData.CurrentUpgradeLevel - 1])
                            {
                                meshFilter.sharedMesh = upgradeMeshFilter.sharedMesh;
                                meshRend.sharedMaterials = upgradeMeshRend.sharedMaterials;
                            }
                        }
                    }

                    if(meshRend.gameObject == selectedObj)
                    {
                        objectSelector.DeselectObject();
                    }
                    upgradeData.CurrentUpgradeLevel += 1;
                    meshRend.gameObject.GetComponent<GridObject>().data.UpgradeLevel += 1;
                }
            }
        }
        


        //Add/Remove/Change any classes relevant to upgrading here
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*************This class handles object selection, hovering capability and a picker canvas to clearly highlight objects**************/
public class ObjectSelector : MonoBehaviour
{
    //General
    Ray ray;
    RaycastHit hit;
    
    //Selector canvas variables
    float left = 0;
    float top = 0;
    float right = 0;
    float bottom = 0;
    float cellSize;
    RectTransform cornerImage;
    Vector2 widthHeight;
    Vector2 canvasPos;
    float width;
    float height;
    float distance;
    bool adjusting = false;
    bool reset = false;

    //Hovering variables
    bool hovered = false;
    bool previewObj;
    bool dragMove;
    bool dragAfterSelect;
    GameObject hoveredObject;
    GameObject previousHoveredObject;

    //Cached variables
    GridSelector gridSelector;
    GridObjectOptions gridObjectOptions;
    ObjectRemover objectRemover;
    GridSquare gridSquare;
    GameObject selectedObj;

    //Serialized variable
    [SerializeField] Material selectedMaterial;
    [SerializeField] Material hoveredMaterial;
    [SerializeField] Canvas selectorCanvas;
    [SerializeField] bool hideCanvasOnPreview;
    [SerializeField] float selectorCanvasHeight;
    [SerializeField] bool smoothCornerAdjust;
    [SerializeField] float cornerMoveSpeedPerSecond;
    [SerializeField] float cornerAdjustPadding;

    //Accessors
    public Material SelectedMaterial
    {
        get { return selectedMaterial; }
        set { selectedMaterial = value; }
    }
    public GameObject SelectedObj
    {
        get { return selectedObj; }
    }
    public Canvas SelectorCanvas
    {
        get { return selectorCanvas; }
    }
    public float SelectorCanvasHeight
    {
        get { return selectorCanvasHeight; }
    }
    public bool HideCanvasOnPreview
    {
        get { return hideCanvasOnPreview; }
    }
    public GameObject HoveredObject
    {
        set { hoveredObject = value; }
    }
    public bool PreviewObj
    {
        get { return previewObj; }
        set { previewObj = value; }
    }
    void Awake()
    {
        gridSquare = FindObjectOfType<GridSquare>();

        if(gridSquare)
        {
            cellSize = gridSquare.CellSize;
        }
    }

    private void Start()
    {
        //Caches objects
        gridSelector = GridBuilder2Manager.Instance.GridSelector;
        objectRemover = GridBuilder2Manager.Instance.ObjectRemover;
        gridObjectOptions = GridBuilder2Manager.Instance.GridObjectOptions;
        if (gridObjectOptions)
        {
            dragMove = gridObjectOptions.DragMove;
            dragAfterSelect = gridObjectOptions.DragAfterSelect;
        }

        if (selectorCanvas)
        {
            ResetCanvas();
        }
    }

    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        //Checks to see if there is a grid selector and that the selector is not holding an object to place
        if(gridSelector && !gridSelector.SelectedGameObjectToPlace)
        {
            if (Physics.Raycast(ray, out hit))
            {
                if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
                {
                    var hitGridObject = hit.collider.GetComponent<GridObject>();
                    //Hits a gridobject and selects it
                    if (hitGridObject)
                    {
                        //Hits a different gridObject so deselects the old one
                        if (hitGridObject.gameObject != selectedObj && selectedObj)
                        {
                            DeselectObject();
                        }

                        //Before selecting, checks to see if the object remover is enabled so it doesnt select as well as delete. 
                        if(objectRemover && objectRemover.enabled)
                        {
                            return;
                        }

                        //Another check to see if it is building, usually you cannot select objects that are being built
                        if(CheckForBuildingStatus())
                        {
                            //Select if nothing is selected, on Deselect sets selectedObj to null
                            if(!selectedObj)
                            {
                                if (!dragMove || dragAfterSelect)
                                {
                                    //This line stops selection if you have just moved an object as you want to deselect it, not select it again.
                                    if (gridObjectOptions)
                                    {
                                        if (gridObjectOptions.MoveTimeout == 0)
                                        {
                                            selectedObj = hit.collider.gameObject;
                                            SelectObject(selectedObj);
                                        } 
                                    }
                                    else
                                    {
                                        selectedObj = hit.collider.gameObject;
                                        SelectObject(selectedObj);
                                    }
                                }
                                else
                                {
                                    if (!dragAfterSelect)
                                    {
                                        //Starts the drag move process
                                        selectedObj = hit.collider.gameObject;
                                        gridObjectOptions.SelectedObject = this.selectedObj;
                                        gridObjectOptions.DragMoveStart();
                                    }
                                }
                            }
                            //Allows selecting once from above if, and then to be able to drag
                            else if(dragAfterSelect && dragMove)
                            {
                                if (selectedObj)
                                {
                                    selectedObj = hit.collider.gameObject;
                                    SelectObject(selectedObj);
                                    //Starts the drag move process
                                    selectedObj = hit.collider.gameObject;
                                    gridObjectOptions.SelectedObject = this.selectedObj;
                                    gridObjectOptions.DragMoveStart();
                                }
                                else
                                {
                                    selectedObj = hit.collider.gameObject;
                                    SelectObject(selectedObj);
                                }
                            }
                        }
                    }

                    //Doesnt hit a gridObject, deselects the old one
                    else
                    {
                        if (!EventSystem.current.IsPointerOverGameObject())
                        {
                            DeselectObject();
                            ResetCanvas();
                        }
                    }
                }

                //Hovered preview
                if (hoveredMaterial || selectorCanvas)
                {
                    //Not hovering over UI
                    if (!EventSystem.current.IsPointerOverGameObject())
                    {
                        //Hits a grid object
                        if (hit.collider.gameObject.GetComponent<GridObject>())
                        {
                            //Check that you are not already hovering
                            if (!hovered)
                            {
                                //Sets the previously selected object to change it back when not hovering over it
                                previousHoveredObject = hit.collider.gameObject;
                                
                                //Changes the material if there is one
                                IsHovered();
                                hovered = true;

                                //If you have Selector canvas, manipulate it to be the size of your object using the SelectObject dimensions
                                if(selectorCanvas)
                                {
                                    cornerImage = selectorCanvas.transform.GetChild(0).GetComponent<RectTransform>();
                                    ShowCanvas();
                                    GetObjectWidthHeight(GetGridObjectCheckPositions());
                                    AdjustCanvasImage();
                                } 
                            }

                            //Different grid object
                            if (previousHoveredObject && previousHoveredObject != hit.collider.gameObject)
                            {
                                if (previousHoveredObject != selectedObj)
                                {
                                    ApplyOriginalMaterial(previousHoveredObject);
                                    hovered = false;
                                }
                                else
                                {
                                    hovered = false;
                                }
                            }
                        }
                        //Doesnt hit a grid object
                        else
                        {
                            if (previousHoveredObject != null)
                            {
                                if (hovered && selectedObj != hoveredObject)
                                {
                                    ApplyOriginalMaterial(previousHoveredObject);
                                    ResetCanvas();
                                }
                                if (selectedObj && !reset)
                                {
                                   ResetCanvas();
                                }
                            }
                            else
                            {
                                ResetCanvas();
                            }
                        }
                    }
                    //If hovering over UI
                    else
                    {
                        if (previousHoveredObject != null)
                        {
                            if (hovered && selectedObj != hoveredObject)
                            {
                                ApplyOriginalMaterial(previousHoveredObject);
                                ResetCanvas();
                            }
                        }
                        if(selectedObj != null && !hit.collider.gameObject.GetComponent<GridObject>())
                        {
                            ResetCanvas();
                        }


                    }
                }
            }
            //Outside grid area
            else
            {
                if (previousHoveredObject != null)
                {
                    if (hovered)
                    {
                        ApplyOriginalMaterial(previousHoveredObject);
                        ResetCanvas();
                        HideCanvas();
                    }
                }
            }
        }

        //Performs the smooth adjusting of the Selector canvas
        if(cornerImage && adjusting)
        {
            cornerImage.sizeDelta = Vector2.MoveTowards(cornerImage.sizeDelta, widthHeight, (distance / cornerMoveSpeedPerSecond) * Time.deltaTime);
            if(cornerImage.sizeDelta == widthHeight)
            {
                adjusting = false;
            }
        }
    }

    public void HideCanvas()
    {
        if (selectorCanvas)
        {
            selectorCanvas.gameObject.SetActive(false);
        }
    }

    public void ShowCanvas()
    {
        if (selectorCanvas)
        {
            selectorCanvas.gameObject.SetActive(true);
        }
    }

    //Changes the canvas back to your chosen cell size
    public void ResetCanvas()
    {
        if (!reset)
        {
            if (selectorCanvas && !EventSystem.current.IsPointerOverGameObject())
            {
                width = cellSize;
                height = cellSize;
                widthHeight = new Vector2(width + cornerAdjustPadding, height + cornerAdjustPadding);
                cornerImage = selectorCanvas.transform.GetChild(0).GetComponent<RectTransform>();
                if (!smoothCornerAdjust)
                {
                    cornerImage.sizeDelta = new Vector2(width + cornerAdjustPadding, height + cornerAdjustPadding);
                }
                else
                {
                    CalcDistance();
                    adjusting = true;
                }
                hovered = false;
                previewObj = false;
                reset = true;
            }
        }
    }

    private void CalcDistance()
    {
        distance = Vector2.Distance(cornerImage.sizeDelta, widthHeight);
    }

    //Starts the adjusting process to a different size of canvas
    public void AdjustCanvasImage()
    {
        widthHeight = new Vector2(width + cornerAdjustPadding, height + cornerAdjustPadding);

        //Sets the position of the canvas to center of the object 
        selectorCanvas.transform.position = new Vector3(
                canvasPos.x,
                selectorCanvasHeight + gridSelector.transform.position.y,
                canvasPos.y);

        if (smoothCornerAdjust)
        {
            CalcDistance();
            adjusting = true;
        }
        else
        {
            cornerImage.sizeDelta = widthHeight;
        }
        reset = false;
    }

    private List<Vector3> GetGridObjectCheckPositions()
    {
        if (hoveredObject.GetComponent<GridObject>().data.CheckPositions != null)
        {
            return hoveredObject.GetComponent<GridObject>().data.CheckPositions;
        }
        else
        {
            return new List<Vector3> { hoveredObject.GetComponent<GridObject>().data.Position };
        }

    }

    //Calculates the size the canvas should be based on what the checkedPositions are
    public Vector2 GetObjectWidthHeight(List<Vector3> checkPosList)
    {
        if (checkPosList.Count > 0)
        {
            left = checkPosList[0].x;
            right = checkPosList[0].x;
            top = checkPosList[0].z;
            bottom = checkPosList[0].z;

            foreach (Vector3 pos in checkPosList)
            {
                //Finds furthest left check pos
                if (pos.x < left)
                {
                    left = pos.x;
                }
                //Finds furthest right check pos
                if (pos.x > right)
                {
                    right = pos.x;
                }
                //Finds furthest top check pos
                if (pos.z > top)
                {
                    top = pos.z;
                }
                //Finds furthest bottom check pos
                if (pos.z < bottom)
                {
                    bottom = pos.z;
                }
            }

            GetCanvasPos();
            //Works out the difference to give you the gap inbetween
            width = Mathf.Abs(left - right) + 1;
            height = Mathf.Abs(top - bottom) + 1;
        }
        else
        {
            width = cellSize;
            height = cellSize;
        }
        return new Vector2(width, height);
    }

    private Vector2 GetCanvasPos()
    {
        canvasPos = new Vector2((left + right) * 0.5f, (top + bottom) * 0.5f);
        return canvasPos;
    }

    //Checks to see if the object is timed and being built
    private bool CheckForBuildingStatus()
    {
        bool isBuilt = true;

        if (selectedObj && selectedObj.GetComponent<GridObject>())
        {
            if (selectedObj.GetComponent<GridObject>().data.BuildTimeRemaining > 0)
            {
                isBuilt = false;
            }
            else
            {
                isBuilt = true;
            }
        }

        if (hit.collider.gameObject.GetComponent<GridObject>())
        {
            if (hit.collider.gameObject.GetComponent<GridObject>().data.BuildTimeRemaining > 0)
            {
                isBuilt = false;
            }
            else
            {
                isBuilt = true;
            }
        }
        return isBuilt;
    }

    //Changes the material to the hovered material
    private void IsHovered()
    {
        if (gridSelector)
        {
            hoveredObject = hit.collider.gameObject;
            if(hoveredMaterial && selectedObj != hoveredObject)
            {
                if (CheckForBuildingStatus())
                {
                    gridSelector.ChangeObjMat(hoveredObject, hoveredMaterial);
                    CallOnBuildingOverInt(hoveredObject);
                }
            }
        }
    }

    //Applies the original material aswell closing any menu if open
    public void DeselectObject()
    {
        if(selectedObj)
        {
            if (selectedMaterial)
            {
                if (CheckForBuildingStatus())
                {
                    ApplyOriginalMaterial(selectedObj);
                }
            }
            CallOnBuildingDeselectInt(selectedObj);
        }

        if (gridObjectOptions)
        {
            gridObjectOptions.CloseMenu();
        }

        selectedObj = null;
    }

    //Applies the selected material and opens the menu for other options if GridObjectOptions are also in the scene
    public void SelectObject(GameObject selectedObj)
    {
        //Used if you call this function from outside but do not set the SelectedObj
        if(selectedObj != this.selectedObj)
        {
            this.selectedObj = selectedObj;
        }

        if(!EventSystem.current.IsPointerOverGameObject())
        {
            if(selectedMaterial)
            {
                if(gridSelector)
                {
                    gridSelector.ChangeObjMat(selectedObj, selectedMaterial);
                }
            }

            //Sets the selected object to use for more functions in gridObjectOptions and opens the menu
            if(gridObjectOptions)
            {
                gridObjectOptions.SelectedObject = this.selectedObj;
                CallOnBuildingSelectInt(this.selectedObj);
                gridObjectOptions.OpenMenu(selectedObj.transform.position, selectedObj);


                //Adjusts the canvas to match the selected object
                if (selectorCanvas)
                {
                    if (selectedObj.GetComponent<GridObject>())
                    {
                        GetObjectWidthHeight(selectedObj.GetComponent<GridObject>().data.CheckPositions);
                        AdjustCanvasImage();
                    }
                }
            }
            hovered = false;
        }
    }

    //Applies the objects original material the GridObject class
    private void ApplyOriginalMaterial(GameObject obj)
    {
        if (obj.GetComponent<GridObject>())
        {
            if (obj.GetComponent<GridObject>().data.BuildTimeRemaining <= 0)
            {
                GridObject hitObjectData = obj.GetComponent<GridObject>();
                hitObjectData.ReapplyOriginalMaterials();
                CallOnBuildingExitInt(obj);
                hovered = false;
            }
        }
    }

    private static void CallOnBuildingSelectInt(GameObject selectedObj)
    {
        IOnBuildingSelect[] onSelectedList = selectedObj.GetComponents<IOnBuildingSelect>();
        for (int i = 0; i < onSelectedList.Length; i++)
        {
            onSelectedList[i].OnSelect(selectedObj);
        }
    }
    private static void CallOnBuildingDeselectInt(GameObject deselectedObj)
    {
        IOnBuildingDeselect[] onDeselectedList = deselectedObj.GetComponents<IOnBuildingDeselect>();
        for (int i = 0; i < onDeselectedList.Length; i++)
        {
            onDeselectedList[i].OnDeselect(deselectedObj);
        }
    }
    private static void CallOnBuildingOverInt(GameObject overObj)
    {
        IOnBuildingOver[] overList = overObj.GetComponents<IOnBuildingOver>();
        for (int i = 0; i < overList.Length; i++)
        {
            overList[i].OnBuildingOver(overObj);
        }
    }
    private static void CallOnBuildingExitInt(GameObject exitObj)
    {
        IOnBuildingExit[] exitList = exitObj.GetComponents<IOnBuildingExit>();
        for (int i = 0; i < exitList.Length; i++)
        {
            exitList[i].OnBuildingExit(exitObj);
        }
    }
}

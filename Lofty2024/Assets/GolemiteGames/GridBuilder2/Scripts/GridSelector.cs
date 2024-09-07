using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*************This class is the core of previewing the movement and placement on the grid**************/
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GridSelector : MonoBehaviour
{
    /*******Grid private variables********/
    //Raycast variables
    Ray ray;
    RaycastHit hit;
    bool hitSomething;

    //Positions
    Vector3 gridSelectorPos;
    Vector3 currentPos;
    Vector3 previewObjOffset = new Vector3();

    //Check positions for the placed objects
    Vector3 placementCheckPosition;
    Vector3[] gridCheckPositions;
    List<GameObject> GORemoveList;

    //Rotation variables
    int rotate = 0;
    bool rotating = false;
    GameObject rotateParent;
    Vector3 originalOffset;

    //GridSquare variables
    float selectedCellSize;
    GridSquare gridsquare;
    GridSquare currentGrid;

    //SelectObject variables
    GameObject selectedGameObjectToPlace;
    GameObject previewObj;
    Building.ObjectSize objectSize;
    int prefabId;
    string instanceId;
    int startingLayer;
    bool moveOnPoints;
 
    int buildTime;
    bool snap = false;

    //Materials and object manipulation variables
    MeshRenderer meshRenderer;
    Material selectorMat;
    Renderer[] children;
    Material[] previewObjOwnMats;

    //Cached components
    ObjectPlacer objectPlacer;
    ObjectSelector objectSelector;
    ObjectRemover objectRemover;
    GridObjectOptions gridObjectOptions;

    bool dragMove;
    bool dragBuild;
    bool dragAndDrop;
    bool dragging = false;

    /*****************Serialised inspector variables**********************/
    //General settings
    [SerializeField] bool smoothMove;
    [SerializeField] float moveSpeed = 0.2f;
    [SerializeField] bool rotation;
    [SerializeField] bool smoothRotate;
    [SerializeField] float rotateSpeed = 600f;
    [SerializeField] float previewObjHoverDistance;
    [SerializeField] bool hideSelectorOnPreview;
    [SerializeField] bool previewObjFloorTiles;
    [SerializeField] GameObject previewObjFloorTilePrefab;
    [SerializeField] bool placeTilesWithObject;
    [SerializeField] bool usePreviewMatsForFloorTiles;

    [SerializeField] bool disableAnimationForPreviewObj = true;
    [SerializeField] float hoverDistance = 0.01f;

    //Invalid placement options
    [SerializeField] bool invalidPlacementFeedback = false;
    [SerializeField] bool showInvalidPreviewObj = false;
    [SerializeField] Material invalidPlacementMat;
    [SerializeField] Material objPreviewMat;
    [SerializeField] bool changeMatColorNotMat;

    //Accessors
    public GameObject SelectedGameObjectToPlace
    {
        get { return selectedGameObjectToPlace; }
    }
    public Vector3 PreviewObjOffset
    {
        get { return previewObjOffset; }
    }
    public GridSquare CurrentGrid
    {
        get { return currentGrid; }
    }
    public bool DragMove
    {
        get { return dragMove; }
        set { dragMove = value; }
    }
    public bool Dragging
    {
        get { return dragging; }
        set { dragging = value; }
    }
    public float HoverDistance
    {
        get { return hoverDistance; }
    }
    public float SelectedCellSize
    {
        get { return selectedCellSize; }
    }
    public GameObject PreviewObj
    {
        get { return previewObj; }
    }
    public bool Rotation
    {
        get { return rotation; }
    }
    public bool PlaceTilesWithObject
    {
        get { return placeTilesWithObject; }
    }
    public bool PreviewObjFloorTiles
    {
        get { return previewObjFloorTiles; }
    }
    public GameObject PreviewObjFloorTilePrefab
    {
        get { return previewObjFloorTilePrefab; }
    }
    private void Awake()
    {
        //Finds a gridSquare object in the scene to find your set cell size
        gridsquare = FindObjectOfType<GridSquare>();
        if (gridsquare)
        {
            //This assumes all of your grids use the same grid cell size
            selectedCellSize = gridsquare.CellSize;
        }
        else
        {
            Debug.Log("You have no GridSquare objects, to do anything with this component you need at least 1 gridSquare");
        }

        

        selectorMat = GetComponent<Renderer>().sharedMaterial;

        //Removes all shadow options by default and hides the selector at runtime
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
        meshRenderer.receiveShadows = false;
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }
    // Start is called before the first frame update
    void Start()
    {
        //Finds and assigns cached objects if in the scene
        objectPlacer = GridBuilder2Manager.Instance.ObjectPlacer;
        objectSelector = GridBuilder2Manager.Instance.ObjectSelector;
        objectRemover = GridBuilder2Manager.Instance.ObjectRemover;
        gridObjectOptions = GridBuilder2Manager.Instance.GridObjectOptions;

        //Scales the transform of this component to match the found grids cell size
        Vector3 scale = transform.localScale;
        scale.x *= selectedCellSize;
        scale.z *= selectedCellSize;
        transform.localScale = scale;
        currentPos = gridSelectorPos;
        if (objectSelector && objectSelector.SelectorCanvas)
        {
            objectSelector.SelectorCanvas.transform.position = gridSelectorPos + new Vector3(0, hoverDistance, 0);
        }

        //Initiates an array to the amount of positions needed for objects
        if (FindObjectOfType<Building>())
        {
            gridCheckPositions = new Vector3[FindObjectOfType<Building>().objSize.GetNum()];
        }
        else
        {
            gridCheckPositions = new Vector3[25];
        }
    }
    // Update is called once per frame
    void Update()
    {
        
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            //If the cursor is hovered over the GridSquare object
            if (hit.collider.GetComponent<GridSquare>() && !EventSystem.current.IsPointerOverGameObject())
            {
                //Hits something so tells other functions to perform
                hitSomething = true;

                //Get the center of each hovered cell
                GetCellPositions();

                //If an object placer is present in the scene, inputs here control object placement
                if (objectPlacer)
                { 
                    //Moves the preview object to the centre of the grid cell
                    if (previewObj != null)
                    {
                        //Smooth transition
                        if (smoothMove && currentPos != Vector3.zero)
                        {
                            if (moveOnPoints)
                            {
                                previewObj.transform.position =
                                    Vector3.MoveTowards(previewObj.transform.position,
                                    (placementCheckPosition) - 
                                    new Vector3((selectedCellSize * 0.5f), 0, (selectedCellSize * 0.5f)) +
                                    new Vector3(0, previewObjHoverDistance, 0),
                                    moveSpeed * Time.deltaTime);
                            }
                            else
                            {
                                previewObj.transform.position =
                                    Vector3.MoveTowards(previewObj.transform.position,
                                    (placementCheckPosition) +
                                    new Vector3(0, previewObjHoverDistance, 0),
                                    moveSpeed * Time.deltaTime);
                            }
                        }
                        //Snap transition
                        else
                        {
                            if(moveOnPoints)
                            {
                                previewObj.transform.position = 
                                    placementCheckPosition - new Vector3(selectedCellSize * 0.5f, 0, selectedCellSize * 0.5f) + new Vector3(0, previewObjHoverDistance, 0);
                            }
                            else
                            {
                                previewObj.transform.position = placementCheckPosition + new Vector3(0, previewObjHoverDistance, 0);
                            }
                        }
                    }
                    
                    //Drag and drop build
                    if(Input.GetMouseButtonUp(0))
                    {
                        if (!dragMove && objectPlacer.DragAndDrop && dragging)
                        {
                            //Ensures if drag and drop is selected, it will not interfere with moving
                            if (previewObj && !previewObj.GetComponent<GridObject>())
                            {
                                //Checks to see if an object has been passed to the grid selector to be placed
                                if (selectedGameObjectToPlace != null)
                                {
                                    currentGrid = hit.collider.GetComponent<GridSquare>();
                                    PlaceObjectIfEmpty(currentGrid);
                                    DeselectPreview();
                                }
                            }
                        }
                    }

                    //Single and multiple click build
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (!dragMove)
                        {
                            //Checks to see if an object has been passed to the grid selector to be placed
                            if (selectedGameObjectToPlace != null)
                            {
                                currentGrid = hit.collider.GetComponent<GridSquare>();
                                PlaceObjectIfEmpty(currentGrid);
                            }
                        }
                        if(objectPlacer && objectPlacer.PlaceOne)
                        {
                            DeselectPreview();
                        }
                    }

                    //Drag build
                    if (objectPlacer.DragBuild)
                    {
                        if (!dragMove)
                        {
                            if (Input.GetMouseButton(0))
                            {
                                if (selectedGameObjectToPlace != null)
                                {
                                    currentGrid = hit.collider.GetComponent<GridSquare>();
                                    PlaceObjectIfEmpty(currentGrid);
                                }
                            }
                        }
                    }

                    //Rotate preview object
                    if(rotation && previewObj)
                    {
                        if(Input.GetKeyDown(KeyCode.R))
                        {
                            previewObjOffset = Vector3.zero;

                            if (rotate == 270)
                            {
                                rotate = 0;
                            }
                            else
                            {
                                rotate += 90;
                            }
                            
                            //Smooth rotation
                            if (smoothRotate)
                            {
                                rotating = true;
                            }
                            //Instant rotation
                            else
                            {
                                previewObj.transform.rotation = Quaternion.Euler(0, rotate, 0);
                            }

                            ChangeCheckedPositions();
                            UpdatePreviewObjOffset();
                            ShowHideSelectorAndPreview();

                            if (objectSelector && objectSelector.SelectorCanvas)
                            {
                                objectSelector.GetObjectWidthHeight(GetObjectSize(objectSize));
                                objectSelector.AdjustCanvasImage();
                            }
                        }

                        
                        //This performs the rotation after pressing 'R' if using smooth rotate
                        if (rotating)
                        {
                            /*rotateParent.transform.rotation = Quaternion.RotateTowards
                                (rotateParent.transform.rotation, Quaternion.Euler(0, rotate, 0), rotateSpeed * Time.deltaTime);*/
                            
                            previewObj.transform.rotation = Quaternion.RotateTowards
                                (previewObj.transform.rotation, Quaternion.Euler(0, rotate, 0), rotateSpeed * Time.deltaTime);
                            if (previewObj.transform.eulerAngles.y == rotate)
                            {
                                rotating = false;
                            }
                        }
                    }  
                }



                //Assign the nearest position on the grid to the grid selector object
                if (transform.position != gridSelectorPos)
                {
                    if (smoothMove)
                    {
                        //Snaps if hitting the grid for the first time so selector does not remember its old position
                        if (!snap)
                        {
                            transform.position = gridSelectorPos;
                            if (objectSelector)
                            {
                                if (previewObj)
                                {
                                    //This will snap the object selector to the new position
                                    if (objectSelector && objectSelector.SelectorCanvas)
                                    {
                                        objectSelector.SelectorCanvas.transform.position =
                                            (placementCheckPosition + previewObjOffset) + new Vector3(0, objectSelector.SelectorCanvasHeight, 0);
                                    }
                                }
                                else
                                {
                                    //This will snap the object selector to the new position
                                    if (objectSelector && objectSelector.SelectorCanvas)
                                    {
                                        objectSelector.SelectorCanvas.transform.position =
                                        placementCheckPosition + new Vector3(0, objectSelector.SelectorCanvasHeight, 0);
                                    }
                                }
                            }
                            ShowHideSelectorAndPreview();
                            snap = true;
                        }
                        else
                        {
                            //This will move towards the new position at the given moveSpeed inspector value
                            transform.position = Vector3.MoveTowards(transform.position, gridSelectorPos, moveSpeed * Time.deltaTime);
                        }

                        
                    }
                    else
                    {
                        //This will snap the grid selector to the new position
                        transform.position = gridSelectorPos;
                    }
                }

                //This makes sure the objectSelector canvas is always in the correct position
                if (objectSelector && objectSelector.SelectorCanvas)
                {
                    if (transform.position.x != objectSelector.SelectorCanvas.transform.position.x || 
                        transform.position.z != objectSelector.SelectorCanvas.transform.position.z)
                    {
                        if (!Input.GetMouseButtonDown(1) || !Input.GetKeyDown(KeyCode.Escape))
                        {
                            MoveSelectorCanvas();
                        }
                    }
                }


                //Shows/Hides the selector tile depending if it is a placeable tile each time selector changes position
                //This only runs each time you are on a different cell
                if (gridSelectorPos != currentPos)
                {
                    ShowHideSelectorAndPreview();
                }
            }
            //Doesnt hit the GridSquare
            else
            {
                HideGridSelectorAndPreview();
                snap = false;
            }

            //Edge cases to hide and show the selector canvas and the preview obj
            if (hit.collider.GetComponent<GridObject>() || hit.collider.GetComponent<GridSquare>())
            {
                if(previewObj && hit.collider.GetComponent<GridObject>())
                {
                    if(showInvalidPreviewObj)
                    {
                        previewObj.SetActive(true);
                    }
                    if (objectSelector && objectSelector.SelectorCanvas)
                    {
                        objectSelector.SelectorCanvas.gameObject.SetActive(false);
                    } 
                }
            }
            else
            {
                if (objectSelector && objectSelector.SelectorCanvas)
                {
                    objectSelector.SelectorCanvas.gameObject.SetActive(false);
                }
            }
        }
        //Doesnt hit anything at all
        else
        {
            //Hits nothing so tells other functions to do nothing
            hitSomething = false;
            snap = false;
            //If the user isnt hovered on anything, hide the selector and any preview object
            HideGridSelectorAndPreview();
            if (objectSelector && objectSelector.SelectorCanvas)
            {
                objectSelector.HideCanvas();
            }
        }

        //Manually unselect and delete the preview
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (previewObj && previewObj.GetComponent<GridObject>())
            {
                if(gridObjectOptions && gridObjectOptions.Moving)
                {
                    ReturnObjectToOriginalPlace();
                }

            }
            else
            {
                DeselectPreview();
            }
            if (objectSelector)
            {
                objectSelector.ShowCanvas();
            }
        }

        
    }

    private void MoveSelectorCanvas()
    {
        if (smoothMove)
        {
            if (moveOnPoints)
            {
                //Moves selector canvas on the placementcheckpos + the previewobjoffset
                objectSelector.SelectorCanvas.transform.position =
                    Vector3.MoveTowards(objectSelector.SelectorCanvas.transform.position,
                    (placementCheckPosition + previewObjOffset) + new Vector3(0, objectSelector.SelectorCanvasHeight, 0),
                    moveSpeed * Time.deltaTime);
            }
            else
            {
                if (previewObj)
                {
                    //Moves selector canvas on the placementcheckpos + the previewobjoffset
                    objectSelector.SelectorCanvas.transform.position =
                        Vector3.MoveTowards(objectSelector.SelectorCanvas.transform.position,
                        (placementCheckPosition + previewObjOffset) + new Vector3(0, objectSelector.SelectorCanvasHeight, 0),
                        moveSpeed * Time.deltaTime);
                }
                else
                {
                    //Moves on middle of cells without preview obj
                    objectSelector.SelectorCanvas.transform.position =
                        Vector3.MoveTowards(objectSelector.SelectorCanvas.transform.position,
                        placementCheckPosition + new Vector3(0, objectSelector.SelectorCanvasHeight, 0),
                        moveSpeed * Time.deltaTime);
                }
            }
        }
        else
        {
            if (gridSelectorPos != currentPos)
            {
                if (moveOnPoints)
                {
                    //This will move towards the new position 
                    objectSelector.SelectorCanvas.transform.position =
                        (placementCheckPosition + previewObjOffset) + new Vector3(0, objectSelector.SelectorCanvasHeight, 0);
                }
                else
                {
                    if (previewObj)
                    {
                        //This will snap the object selector to the new position
                        objectSelector.SelectorCanvas.transform.position =
                            placementCheckPosition + previewObjOffset + new Vector3(0, objectSelector.SelectorCanvasHeight, 0);
                    }
                    else
                    {
                        //This will snap the object selector to the new position
                        objectSelector.SelectorCanvas.transform.position =
                            placementCheckPosition + new Vector3(0, objectSelector.SelectorCanvasHeight, 0);
                    }
                }
            }
        }
    }

    //Hides the preview object and this objects mesh renderer
    //Also resets the currentPos to force a recheck on gridSquare entry
    private void HideGridSelectorAndPreview()
    {
        if (previewObj)
        {
            previewObj.SetActive(false);
        }
        meshRenderer.enabled = false;

        //Will hide the selector canvas if not hitting a grid object
        if(objectSelector && objectSelector.SelectorCanvas)
        {
            if (hit.collider != null && !hit.collider.GetComponent<GridObject>())
            {
                if (gridObjectOptions && gridObjectOptions.MenuCanvas)
                {
                    if(!gridObjectOptions.MenuCanvas.activeSelf)
                    {
                        objectSelector.HideCanvas();
                    }
                }
                else
                {
                    objectSelector.HideCanvas();
                }
            }
        }
        //Sets the current position to something else other than newPos forcing the Selector to recheck if valid cell
        currentPos = Vector3.zero;
    }

    //This function calculates an offset based on your SelectObject.ObjectSize 5x5 grid selection
    //to try to best position the object for rotation
    private void UpdatePreviewObjOffset()
    {
        float cellSizeOffset = selectedCellSize / 2;

        if (previewObj && objectSize != null)
        {
            //Inner ring
            //Top row
            if (objectSize.pos_m1_p1 || objectSize.pos_0_p1 || objectSize.pos_p1_p1)
            {
                previewObjOffset += new Vector3(0, 0, cellSizeOffset);
            }
            //Right column
            if (objectSize.pos_p1_p1 || objectSize.pos_p1_0 || objectSize.pos_p1_m1)
            {
                previewObjOffset += new Vector3(cellSizeOffset, 0, 0);
            }
            //Bottom row
            if (objectSize.pos_p1_m1 || objectSize.pos_0_m1 || objectSize.pos_p1_m1)
            {
                previewObjOffset += new Vector3(0, 0, -cellSizeOffset);
            }
            //Left column
            if (objectSize.pos_m1_m1 || objectSize.pos_m1_0 || objectSize.pos_m1_p1)
            {
                previewObjOffset += new Vector3(-cellSizeOffset, 0, 0);
            }

            //Outer ring
            //Top row
            if (objectSize.pos_m2_p2 || objectSize.pos_m1_p2 || objectSize.pos_0_p2 || objectSize.pos_p1_p2 || objectSize.pos_p2_p2)
            {
                previewObjOffset += new Vector3(0, 0, cellSizeOffset);
            }
            //Right column
            if (objectSize.pos_p2_p2 || objectSize.pos_p2_p1 || objectSize.pos_p2_0 || objectSize.pos_p2_m1 || objectSize.pos_p2_m2)
            {
                previewObjOffset += new Vector3(cellSizeOffset, 0, 0);
            }
            //Bottom row
            if (objectSize.pos_m2_m2 || objectSize.pos_m1_m2 || objectSize.pos_0_m2 || objectSize.pos_p1_m2 || objectSize.pos_p2_m2)
            {
                previewObjOffset += new Vector3(0, 0, -cellSizeOffset);
            }
            //Left column
            if (objectSize.pos_m2_p2 || objectSize.pos_m2_p1 || objectSize.pos_m2_0 || objectSize.pos_m2_m1 || objectSize.pos_m2_m2)
            {
                previewObjOffset += new Vector3(-cellSizeOffset, 0, 0);
            }
        }
    }

    //This will update the boolean values when using rotate, as different cells will now need to be checked
    private void ChangeCheckedPositions()
    {
        if (objectSize != null)
        {
            //Temp variables
            //Inner ring
            bool temptl = objectSize.pos_m1_p1;
            bool temptc = objectSize.pos_0_p1;
            bool temptr = objectSize.pos_p1_p1;
            bool tempcl = objectSize.pos_m1_0;
            bool tempcr = objectSize.pos_p1_0;
            bool tempbl = objectSize.pos_m1_m1;
            bool tempbc = objectSize.pos_0_m1;
            bool tempbr = objectSize.pos_p1_m1;

            //Outer ring
            bool tempm2_p2 = objectSize.pos_m2_p2;
            bool tempm1_p2 = objectSize.pos_m1_p2;
            bool temp0_p2 = objectSize.pos_0_p2;
            bool tempp1_p2 = objectSize.pos_p1_p2;
            bool tempp2_p2 = objectSize.pos_p2_p2;

            bool tempp2_p1 = objectSize.pos_p2_p1;
            bool tempp2_0 = objectSize.pos_p2_0;
            bool tempp2_m1 = objectSize.pos_p2_m1;

            bool tempp2_m2 = objectSize.pos_p2_m2;
            bool tempp1_m2 = objectSize.pos_p1_m2;
            bool temp0_m2 = objectSize.pos_0_m2;
            bool tempm1_m2 = objectSize.pos_m1_m2;
            bool tempm2_m2 = objectSize.pos_m2_m2;

            bool tempm2_m1 = objectSize.pos_m2_m1;
            bool tempm2_0 = objectSize.pos_m2_0;
            bool tempm2_p1 = objectSize.pos_m2_p1;

            if (!moveOnPoints)
            {

                //Reassign to rotate locations
                objectSize.pos_m1_p1 = tempbl;
                objectSize.pos_p1_p1 = temptl;
                objectSize.pos_p1_m1 = temptr;
                objectSize.pos_m1_m1 = tempbr;
                objectSize.pos_0_p1 = tempcl;
                objectSize.pos_p1_0 = temptc;
                objectSize.pos_0_m1 = tempcr;
                objectSize.pos_m1_0 = tempbc;

                //Reassign to rotate locations
                objectSize.pos_m2_p2 = tempm2_m2;
                objectSize.pos_m1_p2 = tempm2_m1;
                objectSize.pos_0_p2 = tempm2_0;
                objectSize.pos_p1_p2 = tempm2_p1;
                objectSize.pos_p2_p2 = tempm2_p2;
                objectSize.pos_p2_p1 = tempm1_p2;
                objectSize.pos_p2_0 = temp0_p2;
                objectSize.pos_p2_m1 = tempp1_p2;
                objectSize.pos_p2_m2 = tempp2_p2;
                objectSize.pos_p1_m2 = tempp2_p1;
                objectSize.pos_0_m2 = tempp2_0;
                objectSize.pos_m1_m2 = tempp2_m1;
                objectSize.pos_m2_m2 = tempp2_m2;
                objectSize.pos_m2_m1 = tempp1_m2;
                objectSize.pos_m2_0 = temp0_m2;
                objectSize.pos_m2_p1 = tempm1_m2;
            }
            else
            {
                //Reassign to rotate locations
                objectSize.pos_m1_p1 = tempm2_m1;
                objectSize.pos_p1_p1 = tempm2_p1;
                objectSize.pos_p1_m1 = temptc;
                objectSize.pos_m1_m1 = tempbc;
                objectSize.pos_0_p1 = tempm2_0;
                objectSize.pos_p1_0 = temptl;
                objectSize.pos_m1_0 = tempbl;
                objectSize.pos_m2_p2 = tempp2_p2;
                objectSize.pos_p2_p1 = tempm2_p2;
                objectSize.pos_p2_0 = tempm1_p2;
                objectSize.pos_p2_m1 = temp0_p2;
                objectSize.pos_p2_m2 = tempp2_p2;
                objectSize.pos_p1_m2 = temptr;
                objectSize.pos_0_m2 = tempcr;
                objectSize.pos_m1_m2 = tempbr;
                objectSize.pos_m2_m2 = tempp1_m2;
                objectSize.pos_m2_m1 = temp0_m2;
                objectSize.pos_m2_0 = tempm1_m2;
                objectSize.pos_m2_p1 = tempm2_m2;
            }
        }
    }

    //Calculates if the selector and the preview should be hidden or not and use of the invalid feedback options
    public void ShowHideSelectorAndPreview()
    {
        //Gets the grid that the selector is currently hovering over
        if(hitSomething && hit.collider.GetComponent<GridSquare>())
        {
            currentGrid = hit.collider.GetComponent<GridSquare>();
        }
        if(currentGrid == null)
        {
            return;
        }

        //If there is a grid
        if (currentGrid && currentGrid.Created)
        {
            //Can place in these cells
            //Performs multiple checks on each cell that requires space
            if (CheckCellsAreEmpty(currentGrid))
            {
                //These cells are clear, so show the grid selector mesh and change its material to its assigned one
                ChangeObjMat(meshRenderer.gameObject, selectorMat);
                meshRenderer.enabled = true;

                if(objectSelector && objectSelector.SelectorCanvas)
                {
                    objectSelector.ShowCanvas();
                }

                if (previewObj)
                {
                    if(hideSelectorOnPreview)
                    {
                        meshRenderer.enabled = false;
                    }
                    if (objectSelector && objectSelector.SelectorCanvas && objectSelector.HideCanvasOnPreview)
                    {
                        objectSelector.HideCanvas();
                    }

                    //Changes the preview objects material to the one assigned, otherwise it will use its own material
                    if (objPreviewMat)
                    {
                        ChangeObjMat(previewObj, objPreviewMat);
                    }
                    else
                    {
                        ReapplyOriginalMats();
                    }

                    previewObj.SetActive(true);
                }
            }
            //Cannot place in these cells
            else
            {
                //Shows the selector and the preview obj if invalid and changes material
                if (invalidPlacementFeedback)
                {
                    //Changes the grid selector tile
                    ChangeObjMat(meshRenderer.gameObject, invalidPlacementMat);
                    meshRenderer.enabled = true;

                    if (objectSelector && objectSelector.SelectorCanvas && !objectSelector.HideCanvasOnPreview)
                    {
                        objectSelector.ShowCanvas();
                    }

                    if (previewObj)
                    {
                        if (showInvalidPreviewObj)
                        {
                            if (invalidPlacementMat) 
                            { 
                                ChangeObjMat(previewObj, invalidPlacementMat);
                            }
                            else
                            {
                                ReapplyOriginalMats();
                            }
                        }
                    }
                }
                //Hides the selector and the preview obj if invalid
                else
                {
                    meshRenderer.enabled = false;
                    if (objectSelector && objectSelector.SelectorCanvas)
                    {
                        objectSelector.HideCanvas();
                    }
                }

                //Shows/hides the preview obj if invalid
                if (previewObj)
                {
                    if (hideSelectorOnPreview)
                    {
                        meshRenderer.enabled = false;
                    }

                    if (invalidPlacementFeedback && showInvalidPreviewObj)
                    {
                        previewObj.SetActive(true);
                    }
                    else
                    {
                        previewObj.SetActive(false);
                        if (objectSelector && objectSelector.SelectorCanvas && !objectSelector.HideCanvasOnPreview)
                        {
                            objectSelector.HideCanvas();
                        }
                    }
                }
            }
        }
        //This makes sure this function gets called every time the cell position changes
        currentPos = gridSelectorPos;
    }

    //This checks every cell that is used by SelectObject.ObjectSize on the placeable objects
    private bool CheckCellsAreEmpty(GridSquare currentGrid)
    {
        //Get the rest of the check positions
        GetObjectSizeCheckPositions();

        //Starts by saying that the cells are false/full
        bool allEmpty = false;

        //If overwrite is on, it does not matter if a cell is clear, only that it is on the grid
        if (objectPlacer && objectPlacer.Overwrite && !dragBuild)
        {
            if (selectedGameObjectToPlace)
            {
                //All position booleans added to a List
                List<bool> checkPosList = new List<bool>();

                int j = 0;

                if (objectSize != null)
                {
                    //Loop over each objectSize boolean
                    foreach (System.Reflection.FieldInfo prop in objectSize.GetType().GetFields())
                    {
                        //If true
                        if ((bool)prop.GetValue(objectSize))
                        {
                            //Add it to the list
                            checkPosList.Add((bool)prop.GetValue(objectSize));

                            //Check the bool using the objectSizeCheckPos array created in function GetObjectSizePositions
                            if (currentGrid.CheckIfOnGrid(gridCheckPositions[j]))
                            {
                                return allEmpty;
                            }
                        }
                        j++;
                    }
                }
            }

            //Checks the center position
            if (currentGrid.CheckIfOnGrid(placementCheckPosition))
            {
                return allEmpty;
            }
        }
        else
        {
            if (selectedGameObjectToPlace)
            {
                //All position booleans added to a List
                List<bool> checkPosList = new List<bool>();

                int j = 0;

                if (objectSize != null)
                {
                    //Loop over each objectSize boolean
                    foreach (System.Reflection.FieldInfo prop in objectSize.GetType().GetFields())
                    {
                        //If true
                        if ((bool)prop.GetValue(objectSize))
                        {
                            //Add it to the list
                            checkPosList.Add((bool)prop.GetValue(objectSize));

                            //Check the bool using the objectSizeCheckPos array created in function GetObjectSizePositions
                            if (!currentGrid.CheckCellStatus(gridCheckPositions[j]))
                            {
                                return allEmpty;
                            }
                        }
                        j++;
                    }
                }
            }

            //Checks the center position
            if (!currentGrid.CheckCellStatus(placementCheckPosition))
            {
                return allEmpty;
            }
        }

        //After it has checked every cell that needs to be checked, if none of them return out of this function,
        //then the cells must be clear, so this function returns true, and the object is placeable
        allEmpty = true;
        return allEmpty;
    }

    //Passes on data to the ObjectPlacer after checking the cell status
    public void PlaceObjectIfEmpty(GridSquare currentGrid)
    {
        //Checks the cells
        if (CheckCellsAreEmpty(currentGrid) && !EventSystem.current.IsPointerOverGameObject())
        {
            GetObjectSizeCheckPositions();
            if (objectPlacer)
            {
                //If you have overwrite enabled and dragbuild is not the placement type
                if(objectPlacer.Overwrite && !dragBuild)
                {
                    //Init new list each time
                    GORemoveList = new List<GameObject>();

                    int j = 0;

                    if (objectSize != null)
                    {
                        //Loop over each objectSize boolean
                        foreach (System.Reflection.FieldInfo prop in objectSize.GetType().GetFields())
                        {
                            //If true
                            if ((bool)prop.GetValue(objectSize))
                            {
                                //Check the bool using the objectSizeCheckPos array created in function GetObjectSizePositions
                                if (!currentGrid.CheckIfOnGrid(gridCheckPositions[j]))
                                {
                                    //Get object on cell if it exits
                                    GameObject obj = currentGrid.GetGridObject(gridCheckPositions[j]);

                                    //Check to see if it is already in the list and the obj is not null
                                    if (obj != null && !GORemoveList.Contains(obj))
                                    {
                                        //Add it to the list
                                        GORemoveList.Add(currentGrid.GetGridObject(gridCheckPositions[j]));
                                    }
                                }
                            }
                            j++;
                        }
                    }
                
                    //Checks the center position and does the same as above
                    if (!currentGrid.CheckIfOnGrid(placementCheckPosition))
                    {
                        GameObject obj = currentGrid.GetGridObject(placementCheckPosition);
                        if (obj != null && !GORemoveList.Contains(obj))
                        {
                            GORemoveList.Add(currentGrid.GetGridObject(placementCheckPosition));
                        }
                    }

                    //For every object in the list (in the way), remove it
                    for (int i = 0; i < GORemoveList.Count; i++)
                    {
                        if(objectRemover)
                        {
                            objectRemover.RemoveObject(GORemoveList[i], GORemoveList[i].GetComponent<GridObject>().data);
                        }

                    }

                    //Reselect the preview object
                    if (objectSelector && objectSelector.SelectorCanvas)
                    {
                        SetupCanvasOnPreview(previewObj, objectSize);
                    }
                }

                
                //These will be the blocked positions after placing the object
                List<Vector3> checkPositions = objectPlacer.GetSpecificCheckPositions(gridCheckPositions, objectSize);

                //Builds the GridObject to send and apply on the ObjectPlacer
                GridObject.Data gridObject = BuildGridObject(currentGrid, checkPositions);

                //Passes the GridObject plus the layer
                if (previewObj && previewObj.activeSelf)
                {
                    GameObject objInstance;

                    //Builds new object
                    if (!previewObj.GetComponent<GridObject>())
                    {
                        //Generate a new instance ID for the object
                        gridObject.InstanceId = Guid.NewGuid().ToString();

                        //Creates a new object
                        if (buildTime <= 0)
                        {
                            objInstance = objectPlacer.PlaceObject(selectedGameObjectToPlace, gridObject, startingLayer);
                            objectPlacer.GetSpecificCheckPositions(gridCheckPositions, objectSize);
                        }
                        //Delay place an object
                        else
                        {
                            objInstance = objectPlacer.DelayBuildStart(selectedGameObjectToPlace, gridObject, startingLayer, buildTime);
                            objectPlacer.GetSpecificCheckPositions(gridCheckPositions, objectSize);
                        }

                        if(previewObjFloorTiles && placeTilesWithObject)
                        {
                            GameObject floorTiles = Instantiate(previewObj.transform.Find("FloorTileParent").gameObject);

                            floorTiles.transform.position = objInstance.transform.position;

                            floorTiles.transform.rotation = Quaternion.Euler(0, rotate, 0);

                            floorTiles.transform.parent = objInstance.transform;

                            floorTiles.name = "FloorTileParent";
                        }

                    }
                    //Moves an already placed object
                    else
                    {
                        
                        if(previewObjFloorTiles)
                        {
                            if (!placeTilesWithObject)
                            {
                                Destroy(previewObj.transform.Find("FloorTileParent").gameObject);
                            }
                        }
                        objectPlacer.MoveObject(selectedGameObjectToPlace, gridObject, startingLayer);


                        //Makes the instance the same object as the moved object as we are only moving and not creating
                        objInstance = selectedGameObjectToPlace;

                        //Reenable the animation on the moved object
                        enableAnimation();

                        //Resets everything after moving an object
                        previewObj = null;
                        DeselectPreview();
                    }

                    //Blocks the cells the objectSize takes up
                    objectPlacer.BlockCells(checkPositions, objInstance, currentGrid);

                }
                else
                {
                    if (dragMove)
                    {
                        ReturnObjectToOriginalPlace();
                    }
                }

                if (objectPlacer.HideCellsUnderPlacedObj)
                {
                    //Calls the function to check if the cells need to be hidden under the object
                    objectPlacer.HidePlacedObjCells(currentGrid, checkPositions);
                }
            }
            else
            {
                Debug.Log("To place objects you must have ObjectPlacer component");
            }
        }
        else
        {
            if(dragMove)
            {
                ReturnObjectToOriginalPlace();
            }
        }
    }

    //This builds a new GridObject with various variables for the objects data
    private GridObject.Data BuildGridObject(GridSquare currentGrid, List<Vector3> blockedCells)
    {
        GridObject.Data gridObject = new GridObject.Data();
        gridObject.ObjName = selectedGameObjectToPlace.name;
        gridObject.PrefabId = prefabId;
        gridObject.InstanceId = instanceId;
        gridObject.Position = placementCheckPosition;
        gridObject.CheckPositions = blockedCells;
        gridObject.Offset = previewObjOffset;
        gridObject.OriginalOffset = originalOffset;
        gridObject.Rotation = rotate;
        gridObject.GridSquare = currentGrid;
        gridObject.ObjSize = objectSize;
        gridObject.MoveOnPoints = moveOnPoints;
        if (selectedGameObjectToPlace && selectedGameObjectToPlace.GetComponent<UpgradeData>())
        {
            gridObject.UpgradeLevel = selectedGameObjectToPlace.GetComponent<UpgradeData>().CurrentUpgradeLevel;
        }
        else
        {
            gridObject.UpgradeLevel = 0;
        }
        return gridObject;
    }

    //If right clicking or pressing escape when moving an already placed object,
    //this will return the object to its previous position and rotation
    private void ReturnObjectToOriginalPlace()
    {
        List<Vector3> blockedCells = new List<Vector3>();

        //Gets the objects GridObject data
        var objData = previewObj.GetComponent<GridObject>().data;
        blockedCells.Add(objData.Position);

        //Blocks the center position of the objects previous position
        objData.GridSquare.ChangeCellStatus(objData.Position, previewObj);

        //Loop through all blocked positions
        foreach(var block in objData.CheckPositions)
        {
            //Blocks all previously blocked positions except the center
            objData.GridSquare.ChangeCellStatus(block, previewObj);
            blockedCells.Add(block);
        }

        objectPlacer.MoveObject(selectedGameObjectToPlace, objData, startingLayer);

        //Resets
        previewObj.SetActive(true);
        rotate = (int)objData.Rotation;
        selectedGameObjectToPlace.GetComponent<GridObject>().data.ObjSize = selectedGameObjectToPlace.GetComponent<Building>().objSize;

        //Deselects the selected object when moved
        if(objectSelector)
        {
            objectSelector.DeselectObject();
            if(objectSelector.SelectorCanvas)
            {
                objectSelector.ResetCanvas();
            }
        }

        enableAnimation();
        previewObj = null;
        selectedGameObjectToPlace = null;

        //Calls the function to check if the cells need to be hidden under the object
        if (objectPlacer)
        {
            if(objectPlacer.HideCellsUnderPlacedObj)
            {
                objectPlacer.HidePlacedObjCells(objData.GridSquare, blockedCells);
            }
        }

        //Resets moveOnPoints after replacing so the center of each cell is now selected
        if(moveOnPoints)
        {
            moveOnPoints = false;
        }

        previewObjOffset = Vector3.zero;
    }

    //Creates a visible preview of the object you want to place, moves with the cursor, and changes the material if applied
    private void CreatePreviewObject()
    {
        if(selectedGameObjectToPlace == null)
        {
            return;
        }

        //If the object has already been placed and is being moved
        if (selectedGameObjectToPlace.GetComponent<GridObject>())
        {
            previewObj = SelectedGameObjectToPlace;

            IOnBuildingMoveStart[] onPlacedList = previewObj.GetComponents<IOnBuildingMoveStart>();
            for (int i = 0; i < onPlacedList.Length; i++)
            {
                onPlacedList[i].OnMoveStart(previewObj);
            }

            //Set the new placementPosition as the position of the placed object
            placementCheckPosition = previewObj.GetComponent<GridObject>().data.Position;

            if (previewObjFloorTiles && !placeTilesWithObject)
            {
                CreatePreviewObjFloorTiles();
            }
        }
        //If you are selecting a new object to place
        else
        {
            //Makes a new object at the grid selector position
            previewObj = Instantiate(selectedGameObjectToPlace, placementCheckPosition, Quaternion.identity);
            if (selectedGameObjectToPlace.GetComponent<UpgradeData>())
            {
                UpgradeData upgradeData = selectedGameObjectToPlace.GetComponent<UpgradeData>();

                //Only change preview mesh if it higher than level 1 and there is a prefab
                if (upgradeData.CurrentUpgradeLevel > 1 && upgradeData.UpgradePrefabs[upgradeData.CurrentUpgradeLevel - 2])
                {
                    //Changes mesh of prefab
                    previewObj.GetComponent<MeshFilter>().sharedMesh =
                        upgradeData.UpgradePrefabs[upgradeData.CurrentUpgradeLevel - 2].GetComponent<MeshFilter>().sharedMesh;

                    previewObj.GetComponent<MeshRenderer>().sharedMaterials =
                        upgradeData.UpgradePrefabs[upgradeData.CurrentUpgradeLevel - 2].GetComponent<MeshRenderer>().sharedMaterials;
                }
            }

            //Assigns this new building to object size
            objectSize = previewObj.GetComponent<Building>().objSize;

            if (previewObjFloorTiles)
            {
                CreatePreviewObjFloorTiles();
            }

            previewObj.name = "PreviewObject";

            //Resets the offset so UpdatePreviewObjOffset is off to a fresh start
            previewObjOffset = Vector3.zero;
            
            //If you are not using rotate, the prefabs rotation will be used
            if (!rotation)
            {
                previewObj.transform.rotation = Quaternion.Euler(0, selectedGameObjectToPlace.transform.rotation.eulerAngles.y, 0);
            }
            //Decides which offset to apply
            UpdatePreviewObjOffset();

            //Keeps the original offset, to send to the object placer, to store as GridObject data
            originalOffset = previewObjOffset;

        }

        previewObj.transform.position = placementCheckPosition;

        if (!objPreviewMat || !invalidPlacementMat)
        {
            StoreOriginalMats();
        }

        //Pauses animation on the preview object until placed
        if(disableAnimationForPreviewObj)
        {
            disableAnimation();
        }

        //Preview objects have their layer set to ignoreraycast so the gridSelector still goes through to the grid collider
        ChangePreviewObjLayer(previewObj, 2);

        IOnSetPreview[] onPreviewList = previewObj.GetComponents<IOnSetPreview>();
        for (int i = 0; i < onPreviewList.Length; i++)
        {
            onPreviewList[i].OnPreview(previewObj);
        }

        ShowHideSelectorAndPreview();
    }

    private void CreatePreviewObjFloorTiles()
    {
        //Gets the initial positions for the object
        GetObjectSizeCheckPositions();

        if(previewObj)
        {
            GameObject tileParent = new GameObject("FloorTileParent");
            tileParent.transform.position = previewObj.transform.position;
            tileParent.transform.parent = previewObj.transform;

            List<Vector3> objCellPositions = GetObjectSize(objectSize);

            if(moveOnPoints)
            {
                foreach (Vector3 pos in objCellPositions)
                {
                    GameObject tile = Instantiate(previewObjFloorTilePrefab, 
                        pos + new Vector3(selectedCellSize * 0.5f, hoverDistance, selectedCellSize * 0.5f), Quaternion.identity, 
                        tileParent.transform);
                    tile.name = "previewFloorTile";
                }
            }
            else
            {
                foreach (Vector3 pos in objCellPositions)
                {
                    GameObject tile = Instantiate(previewObjFloorTilePrefab, 
                        pos + new Vector3(0, hoverDistance, 0), 
                        Quaternion.identity, 
                        tileParent.transform);
                    tile.name = "previewFloorTile";
                }
            }
            
        }
    }

    private void StoreOriginalMats()
    {
        //Assign children all child objects and parent of previewObj that contain a Renderer
        children = previewObj.GetComponentsInChildren<Renderer>();

        //New object
        if (!previewObj.GetComponent<GridObject>())
        {
            previewObjOwnMats = new Material[children.Length];

            //Loops over and stores the original materials to reapply whenever needed
            for (int i = 0; i < children.Length; i++)
            {
                previewObjOwnMats[i] = children[i].sharedMaterial;
            }
        }
        //Moved object
        else
        {
            //Gets the original materials as they are stored on the gridObject class
            previewObjOwnMats = previewObj.GetComponent<GridObject>().OwnMats;
        }
    }

    //Reapply original materials to all child objects
    private void ReapplyOriginalMats()
    {
        for (int i = 0; i < children.Length; i++)
        {
            children[i].sharedMaterial = previewObjOwnMats[i];
        }
    }

    //Stops the animation of a preview obj
    private void disableAnimation()
    {
        if(previewObj && previewObj.GetComponent<Animator>())
        {
            previewObj.GetComponent<Animator>().enabled = false;
        }
        if(previewObj && previewObj.GetComponentInChildren<ParticleSystem>())
        {
            previewObj.GetComponentInChildren<ParticleSystem>().gameObject.SetActive(false);
        }
    }

    //Resumes the aniamtion of a preview obj
    private void enableAnimation()
    {
        if (previewObj && previewObj.GetComponent<Animator>())
        {
            previewObj.GetComponent<Animator>().enabled = true;
        }
        if (previewObj && previewObj.GetComponentInChildren<ParticleSystem>(true))
        {
            previewObj.GetComponentInChildren<ParticleSystem>(true).gameObject.SetActive(true);
        }
    }

    //Cycles through the given obj and all of its children, changing every material on each one to the given newMat
    public void ChangeObjMat(GameObject obj, Material newMat)
    {
        if (!changeMatColorNotMat && newMat)
        {
            Renderer[] children;
            children = obj.GetComponentsInChildren<Renderer>();

            if(usePreviewMatsForFloorTiles)
            {
                foreach (Renderer rend in children)
                {
                    if (!rend.gameObject.GetComponent<ParticleSystem>())
                    {
                        Material[] mats = new Material[rend.materials.Length];
                        for (int i = 0; i < rend.materials.Length; i++)
                        {
                            mats[i] = newMat;
                        }
                        rend.materials = mats;
                    }
                }
            }
            else
            {
                foreach (Renderer rend in children)
                {
                    if (!rend.gameObject.GetComponent<ParticleSystem>() && rend.gameObject.name != "previewFloorTile")
                    {
                        Material[] mats = new Material[rend.materials.Length];
                        for (int i = 0; i < rend.materials.Length; i++)
                        {
                            mats[i] = newMat;
                        }
                        rend.materials = mats;
                    }
                }
            } 
        }
        else
        {
            if (newMat)
            {
                Renderer[] matChildren;
                matChildren = obj.GetComponentsInChildren<Renderer>();
                foreach (Renderer rend in matChildren)
                {
                    Material[] mats = rend.materials;
                    for (int i = 0; i < rend.materials.Length; i++)
                    {
                        mats[i].SetColor("_Color", newMat.GetColor("_Color")); //newMat;
                    }
                    rend.materials = mats;
                }
            }
        }
    }

    //Changes the given obj and all of its children to the given newLayer
    public void ChangePreviewObjLayer(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        Transform[] objChildren = obj.GetComponentsInChildren<Transform>();
        foreach (Transform child in objChildren)
        {
            child.gameObject.layer = newLayer;
        }
    }

    //This is the core of the grid selector, this finds the center of each cell that the curser is inside
    //This also goes on to find the positions for each true boolean in SelectObject.ObjectSize such as topLeft etc
    private void GetCellPositions()
    {
        float newXPos;
        float newZPos;
        
        if(moveOnPoints)
        {
            //Finds the center of the cell you are inside, on each axis
            newXPos = (RoundTo((hit.point.x - hit.transform.position.x), selectedCellSize));
            //float newYPos = (RoundTo((hit.point.y - hit.transform.position.y), selectedCellSize) - selectedCellSize / 2);
            newZPos = (RoundTo((hit.point.z - hit.transform.position.z), selectedCellSize));
            placementCheckPosition = new Vector3(newXPos + (selectedCellSize * 0.5f), 0, newZPos + (selectedCellSize * 0.5f)) + hit.transform.position;
        }
        else
        {
            //Finds the center of the cell you are inside, on each axis
            newXPos = (RoundTo((hit.point.x - hit.transform.position.x), selectedCellSize) - selectedCellSize * 0.5f);
            //float newYPos = (RoundTo((hit.point.y - hit.transform.position.y), selectedCellSize) - selectedCellSize / 2);
            newZPos = (RoundTo((hit.point.z - hit.transform.position.z), selectedCellSize) - selectedCellSize * 0.5f);
            placementCheckPosition = new Vector3(newXPos, 0, newZPos) + hit.transform.position;
        }

        //Assigns the new position
        gridSelectorPos = new Vector3(newXPos, hoverDistance, newZPos) + hit.transform.position;

        //Trims the vector3 to 3 decimal places to fix rounding errors
        gridSelectorPos = TrimToThreeDP(gridSelectorPos);
        placementCheckPosition = TrimToThreeDP(placementCheckPosition);
    }

    //Works out all other positions from the gridSelectorPos
    //These are check positions so do not include the hoverDistance from the grid selector
    private void GetObjectSizeCheckPositions()
    {
        if (selectedGameObjectToPlace)
        {
            int i = 0;

            //1st row positions********************************************************
            //pos_m2_p2 
            calcPos(-selectedCellSize * 2, +selectedCellSize * 2);
            //pos_m1_p2
            calcPos(-selectedCellSize, +selectedCellSize * 2);
            //pos_0_p2
            calcPos(0, +selectedCellSize * 2);
            //pos_p1_p2
            calcPos(+selectedCellSize, +selectedCellSize * 2);
            //pos_p2_p2
            calcPos(+selectedCellSize * 2, +selectedCellSize * 2);

            //2nd row positions********************************************************
            //pos_m2_p1
            calcPos(-selectedCellSize * 2, +selectedCellSize);
            //pos_m1_p1
            calcPos(-selectedCellSize, +selectedCellSize);
            //pos_0_p1
            calcPos(0, +selectedCellSize);
            //pos_p1_p1
            calcPos(+selectedCellSize, +selectedCellSize);
            //pos_p2_p1
            calcPos(+selectedCellSize * 2, +selectedCellSize);

            //3rd row positions********************************************************
            //pos_m2_0
            calcPos(-selectedCellSize * 2, 0);
            //pos_m1_0
            calcPos(-selectedCellSize, 0);
            //Pos_0_0
            gridCheckPositions[i] = placementCheckPosition;
            i++;
            //pos_p1_0
            calcPos(+selectedCellSize, 0);
            //pos_p2_0
            calcPos(+selectedCellSize * 2, 0);

            //4th row positions********************************************************
            //pos_m2_m1
            calcPos(-selectedCellSize * 2, -selectedCellSize);
            //pos_m1_m1
            calcPos(-selectedCellSize, -selectedCellSize);
            //pos_0_m1
            calcPos(0, -selectedCellSize);
            //pos_p1_m1
            calcPos(+selectedCellSize, -selectedCellSize);
            //pos_p2_m1
            calcPos(+selectedCellSize * 2, -selectedCellSize);

            //5th row positions********************************************************
            //pos_m2_m2
            calcPos(-selectedCellSize * 2, -selectedCellSize * 2);
            //pos_m1_m2
            calcPos(-selectedCellSize, -selectedCellSize * 2);
            //pos_0_m2
            calcPos(0, -selectedCellSize * 2);
            //pos_p1_m2
            calcPos(+selectedCellSize, -selectedCellSize * 2);
            //pos_p2_m2
            calcPos(+selectedCellSize * 2, -selectedCellSize * 2);

            Vector3 calcPos(float x, float z)
            {
                Vector3 pos;
                pos = placementCheckPosition + new Vector3(x, 0, z);
                gridCheckPositions[i] = TrimToThreeDP(pos);
                pos = gridCheckPositions[i];
                i++;
                return pos;
            }
        }
    }

    //Takes a vector3 and then parses a string trimmed to 3 decimal places for x, y and z back into a vector3
    private Vector3 TrimToThreeDP(Vector3 num)
    {
        num.x = float.Parse(num.x.ToString("F3"));
        num.y = float.Parse(num.y.ToString("F3"));
        num.z = float.Parse(num.z.ToString("F3"));
        return num;
    }

    //Finds the middle point of our value
    float RoundTo(float value, float multipleOf)
    {
        float roundedNumber;
        if (moveOnPoints)
        {
            roundedNumber = Mathf.Round(value / multipleOf) * multipleOf;
        }
        else
        {
            roundedNumber = Mathf.Ceil(value / multipleOf) * multipleOf;
        }
        //Prevents rounding up above grid width or height
        if (roundedNumber > gridsquare.GridHeight|| roundedNumber > gridsquare.GridWidth)
        {
            value -= 0.01f;
        }
        //Prevents the 0 rounding becoming negative number
        if(value == 0)
        {
            value += 0.01f;
        }
        if (moveOnPoints)
        {
            roundedNumber = Mathf.Round(value / multipleOf) * multipleOf;
        }
        else
        {
            roundedNumber = Mathf.Ceil(value / multipleOf) * multipleOf;
        }
        return roundedNumber;

        
    }
    
    //This starts the whole object placement procedure by assigning the selectedGameObjectToPlace a Building 
    public void SetGameObjectToPlace(Building building)
    {
        //Check to see if to use objectSelector or not
        if(objectSelector && objectSelector.HideCanvasOnPreview)
        {
            objectSelector.HideCanvas();
        }

        selectedGameObjectToPlace = building.gameObject;

        startingLayer = selectedGameObjectToPlace.layer;
        prefabId = building.PrefabId;

        if(building.gameObject.GetComponent<GridObject>())
        {
            GridObject.Data gridObjData = building.gameObject.GetComponent<GridObject>().data;
            this.instanceId = gridObjData.InstanceId;
            building.objSize = CreateObjectSizeInstance(gridObjData);
            objectSize = gridObjData.ObjSize;
        }
        else
        {
            //objectSize = building.objSize;
            this.instanceId = null;
        }
        this.moveOnPoints = building.MoveOnPoints;
        this.buildTime = building.BuildTime;
        


        CreatePreviewObject();

        //This is saying that the GameObject given does not contain a GridObject component,
        //therefor it is a new object and not an already placed one
        if (!building.gameObject.GetComponent<GridObject>())
        {
            rotate = 0;
        }
        //This is an already placed object, we are just moving it
        else
        {
            UpdatePreviewObjOffset();
            rotate = (int)building.gameObject.GetComponent<GridObject>().data.Rotation;
            originalOffset = building.gameObject.GetComponent<GridObject>().data.OriginalOffset;
        }

        if (objectSelector && objectSelector.SelectorCanvas)
        {
            if (building.gameObject.GetComponent<GridObject>())
            {
                SetupCanvasOnPreview(building.gameObject, building.gameObject.GetComponent<GridObject>().data.ObjSize);
            }
            else
            {
                SetupCanvasOnPreview(building.gameObject, building.objSize);
            }
        }
    }

    //Creates a unique ObjectSize object with the correct booleans set to true
    public Building.ObjectSize CreateObjectSizeInstance(GridObject.Data gridObjectData)
    {
        Building.ObjectSize clonedObjSize = new Building.ObjectSize();
        System.Reflection.FieldInfo[] clonedFields;
        clonedFields = clonedObjSize.GetType().GetFields();
        int i = 0;
        if (gridObjectData.ObjSize != null)
        {
            foreach (System.Reflection.FieldInfo prop in gridObjectData.ObjSize.GetType().GetFields())
            {
                clonedFields[i].SetValue(clonedObjSize, prop.GetValue(gridObjectData.ObjSize));
                i++;
            }
        }
        return clonedObjSize;
    }

    private List<Vector3> GetObjectSize(Building.ObjectSize objSize)
    {
        List<Vector3> checkPositions = new List<Vector3>();

        int i = 0;

        if (objSize != null)
        {
            foreach (System.Reflection.FieldInfo prop in objSize.GetType().GetFields())
            {
                if ((bool)prop.GetValue(objSize))
                {
                    checkPositions.Add(gridCheckPositions[i]);
                }

                i++;
            }
        }
        return checkPositions;
    }

    //Removes any previewObj from the scene
    public void DeselectPreview()
    {
        selectedGameObjectToPlace = null;
        if(previewObj)
        {
            if (!previewObj.GetComponent<GridObject>())
            {
                Destroy(previewObj);
                previewObj = null;
            }
            ShowHideSelectorAndPreview();
            if (objectSelector && objectSelector.SelectorCanvas)
            {
                //This resets the selector position to the center of the cell
                objectSelector.SelectorCanvas.transform.position =
                    placementCheckPosition + new Vector3(0, objectSelector.SelectorCanvasHeight, 0);
                snap = false;
            }
        }

        rotate = 0;
        moveOnPoints = false;
        previewObjOffset = Vector3.zero;
        if(objectSelector && objectSelector.SelectorCanvas)
        {
            objectSelector.ResetCanvas();
        }
    }

    //This sets up the canvas to be on the preview object as soon as its created
    private void SetupCanvasOnPreview(GameObject obj, Building.ObjectSize objSize)
    {
        objectSelector.HoveredObject = obj;
        objectSelector.PreviewObj = true;
        objectSelector.GetObjectWidthHeight(GetObjectSize(objSize));
        objectSelector.AdjustCanvasImage();
    }

    //This may needed in future development
    private void DefaultPlacementPos()
    {
        placementCheckPosition = new Vector3(0 + (selectedCellSize * 0.5f), 0, 0 + (selectedCellSize * 0.5f));
        placementCheckPosition = TrimToThreeDP(placementCheckPosition);
        GetObjectSizeCheckPositions();
    }
}

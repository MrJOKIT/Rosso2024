using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
//Strips at build time
#if UNITY_EDITOR
using UnityEditor;
#endif
/*************This class is the base of the system where all other components relate to**************/
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GridSquare : MonoBehaviour
{
    //Enums
    public enum GridType
    {
        SingleCell,
        Chequered,
        Simple,
        Points,
        Lines
    }
    public enum BlockType
    {
        BlockBoth,
        BlockAbove,
        BlockBelow
    }

    /*******Grid private variables********/
    //Data variables
    Vector3[] gridPoints;
    Vector3[] cells;
    int numCells;
    public Dictionary<Vector3, GameObject> gridCellsStatus;
    public Dictionary<Vector3, GameObject> gridCells;
    static List<MeshRenderer> freeCells = new List<MeshRenderer>();
    static List<MeshRenderer> blockedCells = new List<MeshRenderer>();

    //Simple mesh creation variables
    Mesh mesh;
    Vector3[] meshVertices;
    int[] meshTriangles;
    BoxCollider meshCollider;
    float colliderThickness = 0.01f;
    Vector2[] uvs;
    Renderer rend;

    //Containers
    GameObject cellContainer;
    GameObject pointsContainer;
    GameObject gridObjContainer;

    //Line variables
    Mesh linesMesh;
    Vector3[] linesVertices;
    int[] linesTriangles;
    GameObject line;
    GameObject linesContainer;

    //Odd
    Vector3 checkBoxSize;
    Transform pointsArr;

    //Cached components
    GridSelector gridSelector;
    ObjectPlacer objectPlacer;
    GridObjectOptions gridObjectOptions;

    //Save and loading 
    GridCreationData gridCreationData;
    bool loaded = false;
    bool created;
    bool createInEditor = false;
    bool autoCellBlockPreview = false;
    /*****************Serialised inspector variables**********************/
    //General settings
    [SerializeField] bool editorExtension = false;
    [SerializeField] string id;
    [SerializeField] bool visualOnly = false;
    [Min(1)]
    [SerializeField] int gridWidth = 10;
    [Min(1)]
    [SerializeField] int gridHeight = 10;
    [Min(0.001f)]
    [SerializeField] float cellSize = 1;
    [SerializeField] GridType gridType;
    [SerializeField] bool drawSimple;
    [Min(0)]
    [SerializeField] int autoSaveInterval;
    [SerializeField] bool saveGridOnExit;
    [SerializeField] bool loadSaveOnStart;
    [SerializeField] bool loadConfigOnStart;
    [SerializeField] bool checkMatRuntime;
    [SerializeField] float tileX = 2;
    [SerializeField] float tileY = 2;

    [Range(0, 1f)]
    //Point creation variables
    [SerializeField] float pointRadius;

    //LinesMeshVariables
    [Range(0, 0.5f)]
    [SerializeField] float linesThickness;

    //Auto cell blocking variables
    [SerializeField] bool autoCellBlocking = false;
    [SerializeField] BlockType blocktype;
    [SerializeField] LayerMask ignoreLayers;
    [SerializeField] int groundLayer;
    [SerializeField] float groundDistance = 0.05f;
    [SerializeField] float aboveCheckBoxSize = 1f;
    [SerializeField] float aboveCheckBoxHeight = 1f;
    [SerializeField] float checkBoxOffset;
    [SerializeField] bool showAboveBoxColliders = false;
    [SerializeField] bool showBelowRays = false;
    [SerializeField] bool checkGroundHits = false;

    //Prefabs
    [SerializeField] GameObject gridCellPrefab;
    [SerializeField] GameObject secondGridCellPrefab;
    [SerializeField] GameObject blockedAboveCellPrefab;
    [SerializeField] GameObject blockedBelowCellPrefab;
    [SerializeField] GameObject pointsPrefab;

    //Debug
    [SerializeField] bool drawGridPositions = false;
    [SerializeField] bool drawCellPositions = false;

    //Accessors
    public string Id
    {
        get { return id; }
        set { id = value; }
    }
    public Transform PointsArr
    {
        get { return pointsArr; }
    }
    public bool VisualOnly
    {
        get { return visualOnly; }
    }
    public GridType GetGridType
    {
        get { return gridType; }
    }
    public int GridWidth
    {
        get { return gridWidth; }
    }
    public int GridHeight
    {
        get { return gridHeight; }
    }
    public float LinesThickness
    {
        get { return linesThickness; }
    }
    public float PointRadius
    {
        get { return pointRadius; }
    }
    public bool Created
    {
        get { return created; }
        set { created = value; }
    }
    public GameObject GridObjectContainer
    {
        get { return gridObjContainer; }
    }
    public float CellSize
    {
        get { return cellSize; }
    }
    public bool Loaded
    {
        set { loaded = value; }
    }
    public int GroundLayer
    {
        get { return groundLayer; }
    }
    public Vector3[] Cells
    {
        get { return cells; }
        set { cells = value; }
    }
    public GameObject CellContainer
    {
        get => cellContainer;
        set => cellContainer = value;
    }
    public List<MeshRenderer> FreeCells
    {
        get { return freeCells; }
        set { freeCells = value; }
    }
    public List<MeshRenderer> BlockedCells
    {
        get { return blockedCells; }
        set { blockedCells = value; }
    }
    public float GroundDistance
    {
        get => groundDistance;
        set => groundDistance = value;
    }
    public BlockType Blocktype
    {
        get => blocktype;
        set => blocktype = value;
    }
    public bool EditorExtension { get => editorExtension; set => editorExtension = value; }
    public bool UseFixedUpdate { get => createInEditor; set => createInEditor = value; }
    public GridCreationData GridCreationData { get => gridCreationData; set => gridCreationData = value; }
    public Vector3[] GridPoints { get => gridPoints; set => gridPoints = value; }
    public bool AutoCellBlockPreview { get => autoCellBlockPreview; set => autoCellBlockPreview = value; }

#if UNITY_EDITOR
    private void Reset()
    {
        SetNewId();
        CreateGrid();
        CreateGridCells();
    }

    public void SetNewId()
    {
        id = GUID.Generate().ToString().Substring(0, 8);
    }
#endif
    private void Awake()
    {
        if (GridBuilder2Manager.Instance != null)
        {
            //Cache objects
            objectPlacer = GridBuilder2Manager.Instance.ObjectPlacer;
            gridObjectOptions = GridBuilder2Manager.Instance.GridObjectOptions;
            gridSelector = GridBuilder2Manager.Instance.GridSelector;
        }
        LoadGridCreationData();

        createInEditor = false;

        if(gridCreationData != null)
        {
            created = true;
        }

    }

    public void LoadGridCreationData()
    {
        string loadPath = $"GridCreationData/{gameObject.scene.name}/{this.name}-{id}".Replace(" ", string.Empty);

        gridCreationData = Resources.Load<GridCreationData>(loadPath);

        if (gridCreationData != null)
        {
            gridCreationData.RebuildData(this);
        }
    }

    public void StartGridCR()
    {
        createInEditor = true;
        StartCoroutine(Start());
    }

    IEnumerator Start()
    {
        if (!created)
        {

#if UNITY_EDITOR
            if(createInEditor)
            {
                EditorUtility.DisplayProgressBar("Grid Creation Progress", "Creating data structures...", 0.1f);
            }
#endif

            RemoveAutoCellBlockPreview();

            if(createInEditor || !editorExtension)
            {
                InitialiseDataStructures();
            }

            
            //The grid system needs the physics to calculate so we have to wait for an update frame and then we can build the rest of the grid
            if(!createInEditor)
            {
                yield return new WaitForFixedUpdate();
            }

            yield return null;

#if UNITY_EDITOR
            if (createInEditor)
            {
                EditorUtility.DisplayProgressBar("Grid Creation Progress", "Creating grid nodes...", 0.3f);
            }
#endif
            /************Keep these creation types above the collider creation otherwise the grid will detect itself when building.*************/
            //Creates the relevent containers and builds the grid by its grid type
            switch (gridType)
            {
                case GridType.Simple:
                    {
                        mesh = new Mesh();
                        GetComponent<MeshFilter>().mesh = mesh;
                        rend = GetComponent<Renderer>();
                        CreateSimpleGrid();
                        break;
                    }

                case GridType.SingleCell:
                case GridType.Chequered:
                    {
                        cellContainer = new GameObject("CellContainer");
                        cellContainer.transform.parent = transform;
                        if (FindObjectOfType<BuildMode>())
                        {
                            cellContainer.SetActive(false);
                        }
                        CreateSingleCellGrid();
                        break;
                    }

                case GridType.Points:
                    {
                        pointsContainer = new GameObject("PointsContainer");
                        pointsContainer.transform.parent = transform;
                        if (FindObjectOfType<BuildMode>())
                        {
                            pointsContainer.SetActive(false);
                        }
                        rend = GetComponent<Renderer>();
                        CreatePointGrid();
 
                        pointsArr = pointsContainer.GetComponentInChildren<Transform>();
                        break;
                    }

                case GridType.Lines:
                    {
                        linesContainer = new GameObject("LinesContainer");
                        linesContainer.transform.parent = transform;
                        if (FindObjectOfType<BuildMode>())
                        {
                            linesContainer.SetActive(false);
                        }
                        linesMesh = new Mesh();
                        CreateLinesGrid();
                        break;
                    }
            }

#if UNITY_EDITOR
            if (createInEditor)
            {
                EditorUtility.DisplayProgressBar("Grid Creation Progress", "Saving Grid Creation Data...", 0.6f);
            }
#endif
            //New parent for the placed objects to go under
            gridObjContainer = new GameObject("GridObjContainer");
            gridObjContainer.transform.parent = transform;

            //Keep this last
            if (!visualOnly)
            {
                meshCollider = gameObject.AddComponent<BoxCollider>();
                SetColliderSize();
            }

            if (createInEditor)
            {
                gridCreationData = SaveLoadGrid.CreateGridCreationData(this);
            }

            created = true;
        }
        else
        {
            gridObjContainer = gameObject.transform.Find("GridObjContainer").gameObject;
        }

#if UNITY_EDITOR
        if (createInEditor)
        {
            EditorUtility.DisplayProgressBar("Grid Creation Progress", "Loading options...", 0.9f);
        }
#endif

        //This will load the grid at beginning if enabled and if it finds a save file
        if (loadSaveOnStart)
        {
            LoadGrid();
        }

        if (loadConfigOnStart)
        {
            LoadPreconfiguration();
        }
        //Sets up repeating the save command after X minutes
        if (autoSaveInterval > 0)
        {
            InvokeRepeating("SaveGrid", autoSaveInterval * 60, autoSaveInterval * 60);
        }

#if UNITY_EDITOR
        if (createInEditor)
        {
            EditorUtility.ClearProgressBar();
        }
#endif
    }

    private void InitialiseDataStructures()
    {
        //Initialises the data structures for the grid system
        CreateGrid();
        CreateGridCells();
        CreateGridStatus();
        CreateGridCellStatus();
    }

    private void RemoveAutoCellBlockPreview()
    {
        //Checks to see on play if there is a preview of the autoCellBlock on, and if so, remove it
        if (gameObject.transform.childCount > 0)
        {
            Transform[] children = gameObject.transform.GetComponentsInChildren<Transform>();
            for (int i = 0; i < children.Length; i++)
            {
                if (!children[i].GetComponent<GridSquare>())
                {
                    Destroy(children[i].gameObject);
                }
            }
        }
    }

    private void Update()
    {
        //Simple way to see the material tiling at runtime
        if (gridType == GridType.Simple)
        {
            if (checkMatRuntime)
            {
                UpdateMaterial();
            }
        }
    }

    //This will save the grid on application quit if enabled
    private void OnApplicationQuit()
    {
        if (saveGridOnExit)
        {
            SaveGrid();
        }
    }

    //Sends the grid to be saved with all of its placed objects
    public void SaveGrid()
    {
        if(gameObject.activeSelf)
        {
            SaveLoadGrid.SaveCurrentGrid(this);
        }
    }



    //Loads the grid with all objects previously saved
    public void LoadGrid()
    {
        //If it is already loaded, do not load again
        if (loaded)
        {
            return;
        }

        //If you are loading a preconfig you cannot load a save until after saving again
        if (loadConfigOnStart)
        {
            return;
        }

        //Clears storage for initiating objects again
        ObjectStorage.GOTypeList.Clear();
        ObjectStorage.GOInstanceList.Clear();

        List<GameObject> destroyGOList = new List<GameObject>();
        List<Vector3> changeStatusList = new List<Vector3>();

        if(gridCellsStatus != null)
        foreach (KeyValuePair<Vector3, GameObject> entry in gridCellsStatus)
        {
            if (entry.Value != null && entry.Value != this.gameObject)
            {
                destroyGOList.Add(entry.Value);
                changeStatusList.Add(entry.Key);
            }
        }

        //This is when you try to load after making changes on the current session without saving
        if (destroyGOList.Count > 0)
        {
            Debug.Log("There are unsaved changes, loading old save");

            //If using a confirmation UI, put the below for loop in a coroutine to wait until the user has clicked
            for (int i = 0; i < destroyGOList.Count; i++)
            {
                ChangeCellStatus(changeStatusList[i], null);
                Destroy(destroyGOList[i].gameObject);
            }
        }

        //Will not try to load if you have hidden the grid, the data needs the gridSqaure object to be active
        if (gameObject.activeSelf)
        {
            int counter = 0;
            int objSizeCounter = 0;
            int blockCellCounter = 0;

            //Gets the data
            GridData gridData = SaveLoadGrid.LoadCurrentGrid(this);

            if (gridData != null)
            {
                //This loops through every placed object that was saved
                for (int i = 0; i < gridData.objectName.Count; i++)
                {
                    //Outputs the gridData to all the initial variables the GridObject class needs
                    /**************YOU MUST PUT YOUR PLACEABLE OBJECTS IN "Resources/Prefabs/PlaceableObjects/"*******************/
                    //These folders can be anywhere, but must contain the above directory ^^^^^^
                    GameObject resourceObject = (GameObject)Resources.Load($"PlaceableObjects/{gridData.objectName[i]}");

                    Vector3 position = new Vector3(gridData.xPositions[i], gridData.yPositions[i], gridData.zPositions[i]);
                    Vector3 offset = new Vector3(gridData.xOffsetPositions[i], gridData.yOffsetPositions[i], gridData.zOffsetPositions[i]);
                    Vector3 originalOffset = new Vector3(gridData.xOriginalOffsetPos[i], gridData.yOriginalOffsetPos[i], gridData.zOriginalOffsetPos[i]);
                    float rotation = gridData.rotationY[i];
                    Building.ObjectSize objSize = new Building.ObjectSize();
                    bool moveOnPoints = gridData.moveOnPoints[i];
                    List<Vector3> checkPositions = new List<Vector3>();
                    int buildTime = gridData.buildTime[i];
                    int buildTimeRemaining = gridData.buildTimeRemaining[i];
                    int upgradeLevel = gridData.upgradeLevel[i];

                    //Checks to see if the object exists in the placeable objects folder
                    if (resourceObject)
                    {
                        GameObject clonedObj;


                        //Gets check positions
                        for (int j = 0; j < gridData.amountOfCheckPositions[i]; j++)
                        {
                            Vector3 checkPosition = new Vector3(
                                gridData.xcheckPositions[counter],
                                gridData.ycheckPositions[counter],
                                gridData.zcheckPositions[counter]);
                            checkPositions.Add(checkPosition);

                            counter++;
                        }

                        //Finds the correct object size booleans
                        int k = 0;
                        System.Reflection.FieldInfo[] clonedFields;
                        clonedFields = objSize.GetType().GetFields();
                        foreach (System.Reflection.FieldInfo prop in objSize.GetType().GetFields())
                        {
                            clonedFields[k].SetValue(objSize, gridData.selectChecks[k + objSizeCounter]);
                            k++;
                        }
                        objSizeCounter += objSize.GetNum();

                        //Builds the GridObject data class
                        GridObject.Data gridObjectData = new GridObject.Data();
                        gridObjectData.ObjName = gridData.objectName[i];
                        gridObjectData.PrefabId = gridData.prefabID[i];
                        gridObjectData.InstanceId = gridData.instanceID[i];
                        gridObjectData.Position = position;
                        gridObjectData.CheckPositions = checkPositions;
                        gridObjectData.Offset = offset;
                        gridObjectData.OriginalOffset = originalOffset;
                        gridObjectData.Rotation = rotation;
                        gridObjectData.GridSquare = this;
                        gridObjectData.ObjSize = objSize;
                        gridObjectData.MoveOnPoints = moveOnPoints;
                        gridObjectData.BuildTime = buildTime;
                        gridObjectData.BuildTimeRemaining = buildTimeRemaining;
                        gridObjectData.UpgradeLevel = upgradeLevel;

                        //Rebuilds the objects back onto the grid
                        if (objectPlacer)
                        {
                            //Has an upgrade timer remaining
                            if(buildTimeRemaining > 0)
                            {
                                //In the middle of an upgrade with time remaining
                                if(upgradeLevel >= 1)
                                {
                                    clonedObj = objectPlacer.PlaceObject(resourceObject, gridObjectData, resourceObject.layer);
                                }
                                //Being built on a timer
                                else
                                {
                                    clonedObj = objectPlacer.DelayBuildStart(resourceObject, gridObjectData, resourceObject.layer, gridObjectData.BuildTime);
                                }

                            }
                            else
                            {
                                clonedObj = objectPlacer.PlaceObject(resourceObject, gridObjectData, resourceObject.layer);
                            }

                            //If the object has an upgrade, rebuild the currentUgpradeLevel
                            if (clonedObj.GetComponent<UpgradeData>())
                            {
                                clonedObj.GetComponent<UpgradeData>().CurrentUpgradeLevel = upgradeLevel;
                                if (buildTimeRemaining > 0 && (upgradeLevel >= 1))
                                {
                                    if (gridObjectOptions)
                                    {
                                        gridObjectOptions.SelectedObject = clonedObj;
                                        gridObjectOptions.StartUpgrade();
                                    }
                                }
                            }

                            if (gridSelector.PreviewObjFloorTiles && gridSelector.PlaceTilesWithObject && gridSelector.PreviewObjFloorTilePrefab)
                            {
                                //Rebuilds the floor tiles if enabled
                                GameObject tileParent = new GameObject("FloorTileParent");
                                tileParent.transform.position = clonedObj.transform.position;
                                tileParent.transform.parent = clonedObj.transform;

                                foreach (Vector3 pos in checkPositions)
                                {
                                    GameObject tile = Instantiate(gridSelector.PreviewObjFloorTilePrefab,
                                        pos + new Vector3(0, gridSelector.HoverDistance, 0),
                                        Quaternion.identity,
                                        tileParent.transform);
                                    tile.name = "previewFloorTile";
                                }

                                tileParent.transform.localRotation = Quaternion.Euler(0, -rotation, 0); 
                            }

                            //Re hides the cells underneath the loaded object if enabled
                            if (objectPlacer.HideCellsUnderPlacedObj)
                            {
                                objectPlacer.HidePlacedObjCells(this, checkPositions);
                            }
                        }
                        else
                        {
                            clonedObj = null;
                            Debug.Log("Cannot load objects, need an object placer in the scene");
                        }


                        //Blocks the cells 
                        ChangeCellStatus(position, clonedObj);

                        for (int j = 0; j < gridData.amountOfCheckPositions[i]; j++)
                        {
                            Vector3 checkPosition = new Vector3(
                            gridData.xcheckPositions[blockCellCounter],
                            gridData.ycheckPositions[blockCellCounter],
                            gridData.zcheckPositions[blockCellCounter]);

                            ChangeCellStatus(checkPosition, clonedObj);

                            blockCellCounter++;
                        }
                    }
                    //Resource object does not exist
                    else
                    {
                        //This adds one to the internal loop counters skipping one objectsSize if no resource is found
                        counter++;
                        objSizeCounter++;
                        blockCellCounter++;
                        Debug.Log($"{gridData.objectName[i]} not found in Resources/PlaceableObjects/");
                    }
                }
            }
            loaded = true;
        }
    }

    //Removes this grids save file, careful using this, this is the ONLY copy
    //Would normally be used for something like users manually deleting old save files
    public void DeleteGridSave()
    {
        SaveLoadGrid.DeleteCurrentGridSaveData(this);
    }

    public void LoadPreconfiguration()
    {
        //If it is already loaded, do not load again
        if (loaded)
        {
            return;
        }

        //Cannot load preconfig and save 
        if(loadSaveOnStart)
        {
            return;
        }

        //Clears storage for initiating objects again
        ObjectStorage.GOTypeList.Clear();
        ObjectStorage.GOInstanceList.Clear();

        List<GameObject> destroyGOList = new List<GameObject>();
        List<Vector3> changeStatusList = new List<Vector3>();

        if (gridCellsStatus != null)
            foreach (KeyValuePair<Vector3, GameObject> entry in gridCellsStatus)
            {
                if (entry.Value != null && entry.Value != this.gameObject)
                {
                    destroyGOList.Add(entry.Value);
                    changeStatusList.Add(entry.Key);
                }
            }

        //This is when you try to load after making changes on the current session without saving
        if (destroyGOList.Count > 0)
        {
            Debug.Log("There are unsaved changes, loading old save");

            //If using a confirmation UI, put the below for loop in a coroutine to wait until the user has clicked
            for (int i = 0; i < destroyGOList.Count; i++)
            {
                ChangeCellStatus(changeStatusList[i], null);
                Destroy(destroyGOList[i].gameObject);
            }
        }

        //Will not try to load if you have hidden the grid, the data needs the gridSqaure object to be active
        if (gameObject.activeSelf)
        {
            int counter = 0;
            int objSizeCounter = 0;
            int blockCellCounter = 0;

            //Gets the data
            GridData gridData = SaveLoadGrid.LoadPreconfiguration(this);

            if (gridData != null)
            {
                //This loops through every placed object that was saved
                for (int i = 0; i < gridData.objectName.Count; i++)
                {
                    //Outputs the gridData to all the initial variables the GridObject class needs
                    /**************YOU MUST PUT YOUR PLACEABLE OBJECTS IN "Resources/PlaceableObjects/"*******************/
                    //These folders can be anywhere, but must contain the above directory ^^^^^^
                    GameObject resourceObject = (GameObject)Resources.Load($"PlaceableObjects/{gridData.objectName[i]}");

                    Vector3 position = new Vector3(gridData.xPositions[i], gridData.yPositions[i], gridData.zPositions[i]);
                    Vector3 offset = new Vector3(gridData.xOffsetPositions[i], gridData.yOffsetPositions[i], gridData.zOffsetPositions[i]);
                    Vector3 originalOffset = new Vector3(gridData.xOriginalOffsetPos[i], gridData.yOriginalOffsetPos[i], gridData.zOriginalOffsetPos[i]);
                    float rotation = gridData.rotationY[i];
                    Building.ObjectSize objSize = new Building.ObjectSize();
                    bool moveOnPoints = gridData.moveOnPoints[i];
                    List<Vector3> checkPositions = new List<Vector3>();
                    int buildTime = gridData.buildTime[i];
                    int buildTimeRemaining = gridData.buildTimeRemaining[i];
                    int upgradeLevel = gridData.upgradeLevel[i];

                    //Checks to see if the object exists in the placeable objects folder
                    if (resourceObject)
                    {
                        GameObject clonedObj;


                        //Gets check positions
                        for (int j = 0; j < gridData.amountOfCheckPositions[i]; j++)
                        {
                            Vector3 checkPosition = new Vector3(
                                gridData.xcheckPositions[counter],
                                gridData.ycheckPositions[counter],
                                gridData.zcheckPositions[counter]);
                            checkPositions.Add(checkPosition);

                            counter++;
                        }

                        //Finds the correct object size booleans
                        int k = 0;
                        System.Reflection.FieldInfo[] clonedFields;
                        clonedFields = objSize.GetType().GetFields();
                        foreach (System.Reflection.FieldInfo prop in objSize.GetType().GetFields())
                        {
                            clonedFields[k].SetValue(objSize, gridData.selectChecks[k + objSizeCounter]);
                            k++;
                        }
                        objSizeCounter += objSize.GetNum();

                        //Builds the GridObject data class
                        GridObject.Data gridObjectData = new GridObject.Data();
                        gridObjectData.ObjName = gridData.objectName[i];
                        gridObjectData.PrefabId = gridData.prefabID[i];
                        gridObjectData.InstanceId = gridData.instanceID[i];
                        gridObjectData.Position = position;
                        gridObjectData.CheckPositions = checkPositions;
                        gridObjectData.Offset = offset;
                        gridObjectData.OriginalOffset = originalOffset;
                        gridObjectData.Rotation = rotation;
                        gridObjectData.GridSquare = this;
                        gridObjectData.ObjSize = objSize;
                        gridObjectData.MoveOnPoints = moveOnPoints;
                        gridObjectData.BuildTime = buildTime;
                        gridObjectData.BuildTimeRemaining = buildTimeRemaining;
                        gridObjectData.UpgradeLevel = upgradeLevel;

                        //Rebuilds the objects back onto the grid
                        if (objectPlacer)
                        {
                            if (buildTimeRemaining > 0)
                            {
                                //In the middle of an upgrade with time remaining
                                if (upgradeLevel >= 1)
                                {
                                    clonedObj = objectPlacer.PlaceObject(resourceObject, gridObjectData, resourceObject.layer);
                                }
                                //Being built on a timer
                                else
                                {
                                    clonedObj = objectPlacer.DelayBuildStart(resourceObject, gridObjectData, resourceObject.layer, gridObjectData.BuildTime);
                                }
                            }
                            else
                            {
                                clonedObj = objectPlacer.PlaceObject(resourceObject, gridObjectData, resourceObject.layer);
                            }

                            //If the object has an upgrade, rebuild the currentUgpradeLevel
                            if (clonedObj.GetComponent<UpgradeData>())
                            {
                                clonedObj.GetComponent<UpgradeData>().CurrentUpgradeLevel = upgradeLevel;
                                if (buildTimeRemaining > 0 && (upgradeLevel >= 1))
                                {
                                    if (gridObjectOptions)
                                    {
                                        gridObjectOptions.SelectedObject = clonedObj;
                                        gridObjectOptions.StartUpgrade();
                                    }
                                }
                            }

                            if (gridSelector.PreviewObjFloorTiles && gridSelector.PlaceTilesWithObject && gridSelector.PreviewObjFloorTilePrefab)
                            {
                                //Rebuilds the floor tiles if enabled
                                GameObject tileParent = new GameObject("FloorTileParent");
                                tileParent.transform.position = clonedObj.transform.position;
                                tileParent.transform.parent = clonedObj.transform;

                                foreach (Vector3 pos in checkPositions)
                                {
                                    GameObject tile = Instantiate(gridSelector.PreviewObjFloorTilePrefab,
                                        pos + new Vector3(0, gridSelector.HoverDistance, 0),
                                        Quaternion.identity,
                                        tileParent.transform);
                                    tile.name = "previewFloorTile";
                                }

                                tileParent.transform.localRotation = Quaternion.Euler(0, -rotation, 0);
                            }

                            //Re hides the cells underneath the loaded object if enabled
                            if (objectPlacer.HideCellsUnderPlacedObj)
                            {
                                objectPlacer.HidePlacedObjCells(this, checkPositions);
                            }
                        }
                        else
                        {
                            clonedObj = null;
                            Debug.Log("Cannot load objects, need an object placer in the scene");
                        }


                        //Blocks the cells 
                        ChangeCellStatus(position, clonedObj);

                        for (int j = 0; j < gridData.amountOfCheckPositions[i]; j++)
                        {
                            Vector3 checkPosition = new Vector3(
                            gridData.xcheckPositions[blockCellCounter],
                            gridData.ycheckPositions[blockCellCounter],
                            gridData.zcheckPositions[blockCellCounter]);

                            ChangeCellStatus(checkPosition, clonedObj);

                            blockCellCounter++;
                        }
                    }
                    //Resource object does not exist
                    else
                    {
                        //This adds one to the internal loop counters skipping one objectsSize if no resource is found
                        counter++;
                        objSizeCounter++;
                        blockCellCounter++;
                        Debug.Log($"{gridData.objectName[i]} not found in Resources/PlaceableObjects/");
                    }
                }
            }
            loaded = true;
        }
    }


    //If the grid type is set to points, this calculates if a point should exist and where it should be
    //This function includes the autoCellBlock calculations
    private void CreatePointObjects(GameObject prefab, int i)
    {
        //Finds the vector3 position of each point on a cell
        float halfCell = cellSize * 0.5f;
        Vector3 point1 = gridPoints[i] + new Vector3(-halfCell, 0, halfCell);
        point1 = TrimToThreeDP(point1);
        Vector3 point2 = gridPoints[i] + new Vector3(halfCell, 0, halfCell);
        point2 = TrimToThreeDP(point2);
        Vector3 point3 = gridPoints[i] + new Vector3(halfCell, 0, -halfCell);
        point3 = TrimToThreeDP(point3);
        Vector3 point4 = gridPoints[i] + new Vector3(-halfCell, 0, -halfCell);
        point4 = TrimToThreeDP(point4);
        RaycastHit hit;

        //Depending on the blocktype selected, the following functions if statements
        //calculate if a point should exist or not

        switch(blocktype)
        {
            case (BlockType.BlockBoth):
                {
                    if (Physics.Raycast(gridPoints[i], Vector3.down, out hit, GroundDistance))
                    {
                        //This is the check for below, if it does not hit something, continue to check above
                        if (hit.distance < GroundDistance)
                        {
                            if (checkGroundHits)
                            {
                                Debug.Log(hit.distance);
                            }

                            //This is the check for above, if it is clear, create a point
                            if (CheckCellStatus(point1) || CheckCellStatus(point2) || CheckCellStatus(point3) || CheckCellStatus(point4))
                            {
                                createPhysicalGridCellObject(prefab, i, false);
                            }
                        }
                    }
                    break;
                }

            //This is the check for above, if clear, create a point
            case (BlockType.BlockAbove):
                {
                    if (CheckCellStatus(point1) || CheckCellStatus(point2) || CheckCellStatus(point3) || CheckCellStatus(point4))
                    {
                        createPhysicalGridCellObject(prefab, i, false);
                    }
                    break;
                }

            //This is the check for below, if clear, create a point
            case (BlockType.BlockBelow):
                {
                    if (Physics.Raycast(gridPoints[i], Vector3.down, out hit, GroundDistance))
                    {
                        if (hit.distance < GroundDistance)
                        {
                            if (checkGroundHits)
                            {
                                Debug.Log(hit.distance);
                            }
                            createPhysicalGridCellObject(prefab, i, false);
                        }
                    }
                    break;
                }
        }
    }

    //This function trims any numbers longer than 3 decimal points, fixing any rounding errors
    private Vector3 TrimToThreeDP(Vector3 num)
    {
        num.x = float.Parse(num.x.ToString("F3"));
        num.y = float.Parse(num.y.ToString("F3"));
        num.z = float.Parse(num.z.ToString("F3"));
        return num;
    }

    //Filters the cells to whether a cell block should be placed or not
    public void CreateAutoCellBlock(GameObject prefab, int i, bool preview)
    {
        RaycastHit hit;

        //Create four points from the centre of each cell
        float halfCell = cellSize * 0.5f;
        Vector3 point1 = new Vector3(-halfCell, 0, halfCell);
        Vector3 point2 = new Vector3(halfCell, 0, halfCell);
        Vector3 point3 = new Vector3(halfCell, 0, -halfCell);
        Vector3 point4 = new Vector3(-halfCell, 0, -halfCell);

        //Checks the points
        bool point1Clear = CheckPoint(point1);
        bool point2Clear = CheckPoint(point2);
        bool point3Clear = CheckPoint(point3);
        bool point4Clear = CheckPoint(point4);

        ClearAbove(i);

        //This function filters for any open space below the grid, thus not creating a cell there
        bool CheckPoint(Vector3 cellPoint)
        {
            if (Physics.Raycast(cells[i] + cellPoint, Vector3.down, out hit, GroundDistance, 1 << groundLayer))
            {
                //Need to stop the underlaying grid having effect
                if (hit.distance < GroundDistance)
                {
                    if (checkGroundHits)
                    {
                        Debug.Log(hit.distance);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        switch (blocktype)
        {
            //Only creates cells if not blocked above or below
            case (BlockType.BlockBoth):
                {
                    //Cells are both clear above and below
                    if (point1Clear && point2Clear && point3Clear && point4Clear && ClearAbove(i))
                    {
                        if (gridType != GridType.Points)
                        {
                            createPhysicalGridCellObject(prefab, i, false);

                            if (autoCellBlockPreview)
                            {
                                freeCells.Add(prefab.GetComponent<MeshRenderer>());
                            }
                        }
                        else
                        {
                            if (autoCellBlockPreview)
                            {
                                createPhysicalGridCellObject(prefab, i, false);
                                freeCells.Add(prefab.GetComponent<MeshRenderer>());
                            }
                        }
                    }
                    //Cells are blocked either above or below
                    else
                    {
                        if (blockedAboveCellPrefab != null && point1Clear && point2Clear && point3Clear && point4Clear)
                        {
                            createPhysicalGridCellObject(blockedAboveCellPrefab, i, true);
                        }
                        if (blockedBelowCellPrefab != null && ClearAbove(i))
                        {
                            createPhysicalGridCellObject(blockedBelowCellPrefab, i, true);
                        }
                        if (!autoCellBlockPreview)
                        {
                            //Marks the cells with something in them
                            //ChangeGridCellStatus(cells[i], null);
                            ChangeCellStatus(cells[i], gameObject);
                        }
                        else
                        {
                            //Displays the blocked cells for the preview
                            if (preview)
                            {
                                createPhysicalGridCellObject(prefab, i, true);
                            }
                            else
                            {
                                //ChangeGridCellStatus(cells[i], null);
                                ChangeCellStatus(cells[i], gameObject);
                            }
                            if (autoCellBlockPreview)
                            {
                                blockedCells.Add(prefab.GetComponent<MeshRenderer>());
                            }
                        }
                    }
                    break;
                }

            //Only creates cells if not blocked above
            case (BlockType.BlockAbove):
                {
                    if (ClearAbove(i))
                    {
                        if (gridType != GridType.Points)
                        {
                            createPhysicalGridCellObject(prefab, i, false);
                            if (autoCellBlockPreview)
                            {
                                freeCells.Add(prefab.GetComponent<MeshRenderer>());
                            }
                        }
                        else
                        {
                            if (autoCellBlockPreview)
                            {
                                createPhysicalGridCellObject(prefab, i, false);
                                freeCells.Add(prefab.GetComponent<MeshRenderer>());
                            }
                        }
                    }

                    else
                    {
                        //If a cell is blocked, place a prefab instead of leaving it empty
                        if (blockedAboveCellPrefab != null)
                        {
                            createPhysicalGridCellObject(blockedAboveCellPrefab, i, true);
                        }
                        //Marks the cells with something in them
                        if (Application.isPlaying)
                        {
                            //Marks the cells with something in them
                            //ChangeGridCellStatus(cells[i], null);
                            ChangeCellStatus(cells[i], gameObject);
                        }
                        else
                        {
                            //Displays the blocked cells for the preview
                            if (preview)
                            {
                                createPhysicalGridCellObject(prefab, i, true);
                            }
                            else
                            {
                                //ChangeGridCellStatus(cells[i], null);
                                ChangeCellStatus(cells[i], gameObject);
                            }
                            if(autoCellBlockPreview)
                            {
                                blockedCells.Add(prefab.GetComponent<MeshRenderer>());
                            }

                        }
                    }
                    break;
                }

            //Only creates cells if there is no empty space below
            case (BlockType.BlockBelow):
                {
                    if (point1Clear && point2Clear && point3Clear && point4Clear)
                    {
                        if (gridType != GridType.Points)
                        {
                            createPhysicalGridCellObject(prefab, i, false);
                            if (autoCellBlockPreview)
                            {
                                freeCells.Add(prefab.GetComponent<MeshRenderer>());
                            }
                        }
                        else
                        {
                            if (autoCellBlockPreview)
                            {
                                createPhysicalGridCellObject(prefab, i, false);
                                freeCells.Add(prefab.GetComponent<MeshRenderer>());
                            }
                        }
                    }

                    else
                    {
                        //If a cell is blocked, place a prefab instead of leaving it empty
                        if (blockedBelowCellPrefab != null)
                        {
                            createPhysicalGridCellObject(blockedBelowCellPrefab, i, true);
                        }
                        //Marks the cells with something in them
                        if (Application.isPlaying)
                        {
                            //Marks the cells with something in them
                            //ChangeGridCellStatus(cells[i], null);
                            ChangeCellStatus(cells[i], gameObject);
                        }
                        else
                        {
                            //Displays the blocked cells for the preview
                            if (preview)
                            {
                                createPhysicalGridCellObject(prefab, i, true);
                            }
                            else
                            {
                                //ChangeGridCellStatus(cells[i], null);
                                ChangeCellStatus(cells[i], gameObject);
                            }
                            if (autoCellBlockPreview)
                            {
                                blockedCells.Add(prefab.GetComponent<MeshRenderer>());
                            }
                        }
                    }
                    break;
                }
        }
    }

    private bool ClearAbove(int i)
    {
        //This section filters for anything blocking above the grid
        bool aboveClear = true;
        Collider[] colliders = Physics.OverlapBox(cells[i] + new Vector3(0, checkBoxOffset + (aboveCheckBoxHeight * 0.5f), 0), GetCheckBoxSize());
        foreach (var item in colliders)
        {
            //If the hit is not a gridsquare component
            if (!item.GetComponent<GridSquare>())
            {
                int layerInt = item.gameObject.layer;
                if (ignoreLayers != (ignoreLayers | (1 << layerInt)))
                {
                    aboveClear = false;
                }
            }
        }
        return aboveClear;
    }

    //This function creates the physical cell object using the given prefab
    private void createPhysicalGridCellObject(GameObject prefab, int i, bool blocked)
    {
        //Checks to see if there is a pointsPrefab to use
        if(gridType == GridType.Points && !pointsPrefab)
        {
            return;
        }
        //Checks to see if the grid type matches one that uses grid cells 
        if (gridType != GridType.Points && gridType != GridType.Lines)
        {
            //Checks to see if a prefab is assigned in the inspector if application is in play mode
            if (Application.isPlaying && gridCellPrefab == null)
            {
                return;
            }
        }

        GameObject gridCellInstance;

        switch(gridType)
        {
            case (GridType.SingleCell):
            case (GridType.Chequered):
            case (GridType.Simple):
                {
                    CreateSingleGridCellInstance(prefab, i, blocked);
                    break;
                }

            case (GridType.Points):
                {
                    if (Application.isPlaying || createInEditor && !autoCellBlockPreview)
                    {
                        gridCellInstance = Instantiate(prefab, gridPoints[i], Quaternion.identity);
                        gridCellInstance.transform.parent = pointsContainer.transform;

                        //Scales the points
                        Vector3 gridCellTransform = gridCellInstance.transform.localScale;
                        gridCellTransform.x = pointRadius;
                        gridCellTransform.z = pointRadius;
                        gridCellInstance.transform.localScale = gridCellTransform;

                        //Names the points
                        gridCellInstance.name = "GridPoint: " + gridPoints[i];
                    }
                    else
                    {
                        CreateSingleGridCellInstance(prefab, i, blocked);
                    }
                    break;
                }

            case (GridType.Lines):
                {
                    if (Application.isPlaying || createInEditor && !autoCellBlockPreview)
                    {
                        gridCellInstance = Instantiate(prefab, cells[i], Quaternion.identity);
                        gridCellInstance.transform.parent = linesContainer.transform;
                        //gridCells.Add(cells[i], gridCellInstance);

                        //Names the lines
                        gridCellInstance.name = "GridLine";
                    }
                    else
                    {
                        CreateSingleGridCellInstance(prefab, i, blocked);
                    }
                    break;
                }
        }
    }

    private GameObject CreateSingleGridCellInstance(GameObject prefab, int i, bool blocked)
    {
        GameObject gridCellInstance;

        if (prefab != null)
        {
            gridCellInstance  = Instantiate(prefab, cells[i], Quaternion.identity);
        }
        else
        {
            gridCellInstance = new GameObject();
        }

        gridCellInstance.transform.parent = cellContainer.transform;

        //Scales the grid cells based on cell size
        Vector3 scale = gridCellInstance.transform.localScale;
        scale.x *= cellSize;
        scale.z *= cellSize;
        gridCellInstance.transform.localScale = scale;
        //Names the cells
        if (!blocked)
        {
            gridCellInstance.name = "GridCell: " + cells[i];
        }
        else
        {
            gridCellInstance.name = "BlockedCell: " + cells[i];
        }
        if (!autoCellBlockPreview)
        {
            ChangeGridCellStatus(cells[i], gridCellInstance);
        }
        else
        {
            //This adds the cells to relevent Lists to use them later for the Grid autocellblock preview in the editor
            if (!blocked)
            {
                freeCells.Add(gridCellInstance.GetComponent<MeshRenderer>());
            }
            else
            {
                blockedCells.Add(gridCellInstance.GetComponent<MeshRenderer>());
            }
        }

        return gridCellInstance;
    }

    //Determines the size of the checkbox used in the physics calculations
    public Vector3 GetCheckBoxSize()
    {
        checkBoxSize = new Vector3((cellSize * aboveCheckBoxSize) * 0.5f, aboveCheckBoxHeight * 0.5f, (cellSize * aboveCheckBoxSize) * 0.5f);
        return checkBoxSize;
    }

    //This is the core function creating all of the grid points in 3D space
    public void CreateGrid()
    {
        //Creates the initial grid of points
        if (gridWidth > 0 && gridHeight > 0)
        {
            //Double loop to go over grid width and grid height
            gridPoints = new Vector3[(gridWidth + 1) * (gridHeight + 1)];
            for (int i = 0, x = 0; x <= gridWidth; x++)
            {
                for (int z = 0; z <= gridHeight; z++)
                {
                    //Add the points into the data structure
                    gridPoints[i] =
                    new Vector3(
                        transform.position.x + x * cellSize,
                        transform.position.y,
                        transform.position.z + z * cellSize);
                    i++;
                }
            }
        }
    }



    //This uses the core function above to create the cells for the grid
    public void CreateGridCells()
    {
        //Calculates the total number of cells
        numCells = gridHeight * gridWidth;
        cells = new Vector3[numCells];
        if (gridPoints == null)
            return;

        //Calculates the center of the grid cells from the grid points
        for (int i = 1, j = 0; i < gridPoints.Length - gridHeight; i++)
        {
            if (i % (gridHeight + 1) != 0)
            {
                cells[j] = gridPoints[i - 1] + new Vector3(cellSize, 0, cellSize) * 0.5f;

                float trimmedX = float.Parse(cells[j].x.ToString("F3"));
                float trimmedY = float.Parse(cells[j].y.ToString("F3"));
                float trimmedZ = float.Parse(cells[j].z.ToString("F3"));

                //Add the vectors into the data structure
                cells[j] = new Vector3(trimmedX, trimmedY, trimmedZ);

                j++;
            }
        }
    }

    //Initialises each cell with a value of null to begin with, or in other words, its empty.
    public void CreateGridStatus()
    {
        gridCellsStatus = new Dictionary<Vector3, GameObject>();
        
        for (int i = 0; i < cells.Length; i++)
        {
            gridCellsStatus.Add(cells[i], null);
        };
    }

    public void CreateGridCellStatus()
    {
        gridCells = new Dictionary<Vector3, GameObject>();

        for (int i = 0; i < cells.Length; i++)
        {
            gridCells.Add(cells[i], null);
        };
    }

    //This function checks to see if a cell is empty. 
    public bool CheckCellStatus(Vector3 cellPos)
    {
        bool isEmpty;
        GameObject value;
        isEmpty = gridCellsStatus.TryGetValue(cellPos, out value);
        //If a given cell position is empty and exists it will return true otherwise it will return false
        if (value == null && isEmpty)
        {
            isEmpty = true;
        }
        else
        {
            isEmpty = false;
        }
        return isEmpty;
    }

    //This function checks to see if a cell is on the visibly created cells only.
    //This is used by the ObjectPlacer Overwrite field.
    public bool CheckIfOnGrid(Vector3 cellPos)
    {
        bool isEmpty;
        GameObject value;
        gridCellsStatus.TryGetValue(cellPos, out value);

        //If a given cell was created 
        if (value != this.gameObject)
        {
            isEmpty = false;
        }
        else
        {
            isEmpty = true;
        }
        return isEmpty;
    }

    //This simply changes the value of the given cell position to something other than null, so the cell is not empty
    //You can pass null here to empty it
    public void ChangeCellStatus(Vector3 cellPos, GameObject obj)
    {
        gridCellsStatus[cellPos] = obj;
    }

    //This adds the given cell prefab to the cells List at its position
    public void ChangeGridCellStatus(Vector3 cellPrefabPos, GameObject obj)
    {
        gridCells[cellPrefabPos] = obj;
    }

    //Returns the cell GameObject occupied in the gridCells dictionary at the given position
    //If nothing is found or position does not exist, it will return null
    public GameObject GetCellObject(Vector3 pos)
    {
        GameObject obj;
        gridCells.TryGetValue(pos, out obj);
        return obj;
    }

    //Returns the placed GameObject occupied in the gridCellsStatus dictionary at the given position
    //If nothing is found or position does not exist, it will return null
    public GameObject GetGridObject(Vector3 pos)
    {
        GameObject obj;
        gridCellsStatus.TryGetValue(pos, out obj);
        return obj;
    }

    /***********The following functions create initialise each grid type*******************/

    //Initialises the single cell and chequered grid types
    private void CreateSingleCellGrid()
    {
        //Chequered type with autocellblocking
        if(gridType == GridType.Chequered)
        {
            if(autoCellBlocking)
            {
                int c = 0;
                //Loops through all the cells
                for (int i = 0; i < gridWidth; i++)
                {
                    for (int j = 0; j < gridHeight; j++)
                    {
                        //This creates the checkered effect
                        if ((i + j) % 2 == 0)
                        {
                            CreateAutoCellBlock(gridCellPrefab, c, false);
                        }
                        else
                        {
                            if(secondGridCellPrefab)
                            {
                                CreateAutoCellBlock(secondGridCellPrefab, c, false);
                            }
                            else
                            {
                                ChangeGridCellStatus(cells[c], gameObject);
                                Debug.Log("Assign a second prefab in inspector for checkered pattern");
                            }
                        }
                        c++;
                    }
                }
            }
            //Chequered type without autocellblocking
            else
            {
                int c = 0;
                for (int i = 0; i < gridWidth; i++)
                {
                    for (int j = 0; j < gridHeight; j++)
                    {
                        if ((i + j) % 2 == 0)
                        {
                            createPhysicalGridCellObject(gridCellPrefab, c, false);
                        }
                        else
                        {
                            if(secondGridCellPrefab)
                            {
                                createPhysicalGridCellObject(secondGridCellPrefab, c, false);
                            }
                            else
                            {
                                Debug.Log("Assign a second prefab in inspector for checkered pattern");
                            }
                        }
                        c++;
                    }
                }
            }   
        }
        if(gridType == GridType.SingleCell)
        {
            //Single cell type with autocellblocking
            if(autoCellBlocking)
            {    
                for (int i = 0; i < cells.Length; i++)
                {
                    CreateAutoCellBlock(gridCellPrefab, i, false);
                }
            }
            //Single cell type without autocellblocking
            else
            {
                for (int i = 0; i < cells.Length; i++)
                {
                    createPhysicalGridCellObject(gridCellPrefab, i, false);
                }
            }
        }
    }

    //Creates the simple type, creating a new uv'd mesh plane
    private void CreateSimpleGrid()
    {
        //Creates 4 vertices from the gridPoints array
        meshVertices = new Vector3[]
        {
            gridPoints[0] - transform.position,
            gridPoints[gridHeight] - transform.position,
            gridPoints[gridWidth * (gridHeight + 1)] - transform.position,
            gridPoints[gridPoints.Length - 1] - transform.position
        };
        //Creates the 2 triangles
        meshTriangles = new int[]
        {
            0, 1, 2,
            1, 3, 2
        };

        CreateMesh();
    }

    //Builds and assigns the vertices and triangles to the mesh
    //Also updates the normals
    private void CreateMesh()
    {
        mesh.Clear();
        mesh.vertices = meshVertices;
        mesh.triangles = meshTriangles;
        mesh.name = "SimpleGridMesh";

        CreateUVs();
        mesh.uv = uvs;
        UpdateMaterial();

        mesh.RecalculateNormals();
    }

    //Creates the UV's for the simple grid type
    private void CreateUVs()
    {
        uvs = new Vector2[meshVertices.Length];

        uvs[0] = new Vector2(0, 0);
        uvs[1] = new Vector2(0, 1);
        uvs[2] = new Vector2(1, 0);
        uvs[3] = new Vector2(1, 1);
    }

    //Updates the simple grid types material with tiling equivalent to the grid height and width
    private void UpdateMaterial()
    {
        if(rend) rend.sharedMaterial.mainTextureScale = new Vector2((float)gridWidth / tileX, (float)gridHeight / tileY);
    }

    //Calculates the Point grid type
    private void CreatePointGrid()
    {
        //With autocellblock
        if(autoCellBlocking)
        {
            for (int i = 0; i < cells.Length; i++)
            {
                //Does the normal cell blocking without physically creating anything
                CreateAutoCellBlock(pointsPrefab, i, false);
            }
            for (int i = 0; i < gridPoints.Length; i++)
            {
                //This is the function that goes on to create the points using the already 'available' cells specified in the last for loop
                CreatePointObjects(pointsPrefab, i);
            }
        }
        //Without autocellblock
        else
        {
            for (int i = 0; i < gridPoints.Length; i++)
            {
                createPhysicalGridCellObject(pointsPrefab, i, false);
            }
        }  
    }

    //Creates the lines type grid
    private void CreateLinesGrid()
    {
        //Offset so the center of the lines is central on the cell, not on grid point
        Vector3 offset = new Vector3((-cellSize * 0.5f), 0, (-cellSize * 0.5f));

        //Calculates each vertex position of the lines
        linesVertices = new Vector3[]
        {
            new Vector3(linesThickness, 0, linesThickness) + offset,
            new Vector3(-linesThickness, 0, -linesThickness) + offset,
            new Vector3(-linesThickness, 0, (cellSize + linesThickness)) + offset,
            new Vector3(linesThickness, 0, (cellSize - linesThickness)) + offset,
            new Vector3((cellSize + LinesThickness), 0, (cellSize + LinesThickness)) + offset,
            new Vector3((cellSize - LinesThickness), 0, (cellSize - LinesThickness)) + offset,
            new Vector3((cellSize + linesThickness), 0, -linesThickness) + offset,
            new Vector3((cellSize - linesThickness), 0, linesThickness) + offset
        };

        //Triangle vertex order
        linesTriangles = new int[]
        {
            0, 1, 2,
            0, 2, 3,
            2, 4, 3,
            3, 4, 5,
            4, 6, 5,
            5, 6, 7,
            7, 6, 1,
            0, 7, 1
        };

        //Appends everything to the mesh
        linesMesh.Clear();
        linesMesh.vertices = linesVertices;
        linesMesh.triangles = linesTriangles;
        linesMesh.name = "LinesMesh";
        line = new GameObject("Line");

        line.AddComponent<MeshFilter>().mesh = linesMesh;
        MeshRenderer ownMR = GetComponent<MeshRenderer>();
        MeshRenderer lineMR = line.AddComponent<MeshRenderer>();
        lineMR.sharedMaterial = ownMR.sharedMaterial;
        lineMR.shadowCastingMode = ownMR.shadowCastingMode;
        lineMR.receiveShadows = ownMR.receiveShadows;
        lineMR.allowOcclusionWhenDynamic = ownMR.allowOcclusionWhenDynamic;

        //Creates the lines only on available cells
        for (int i = 0; i < cells.Length; i++)
        {
            if (autoCellBlocking)
            {
                CreateAutoCellBlock(line, i, false);
            }
            //Creates the lines on all cells
            else
            {
                createPhysicalGridCellObject(line, i, false);
            }
        }
        //Removes the original line created for instantiating
        if(createInEditor)
        {
            DestroyImmediate(line);
        }
        else
        {
            Destroy(line);
        }

    }
 
    //Changes the size and position of the collider based on cellSize and the grid width and height
    private void SetColliderSize()
    {
        meshCollider.size = new Vector3(gridWidth * cellSize, colliderThickness, gridHeight * cellSize);
        meshCollider.center = new Vector3((float)gridWidth * cellSize / 2, (colliderThickness / 2), (float)gridHeight * cellSize / 2);
    }

    //These functions initialise all the data for creating the grids points, so they can then be previewed in the viewport
    private void OnValidate()
    {
        if(!Application.isPlaying)
        {
            CreateGrid();
            CreateGridCells();
        }

    }

    //Creates the viewport preview of some of the basic settings
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //If you move the grid, reinitialise its points
        if (transform.hasChanged)
        {
            CreateGrid();
            CreateGridCells();
            transform.hasChanged = false;
        }

//Wont run at build time
#if UNITY_EDITOR
        //These two functions are used for debugging, they will impact performance if used on large grids
        if (drawGridPositions)
        {
            //Draw the points the grid is made up from
            DrawPointPositions();
        }
        if(drawCellPositions)
        {
            //Draw the cell coordinates in world space
            DrawCellPositions();
        }

        //Previews the autocellblocking setup features
        if (autoCellBlocking)
        {
            if (showAboveBoxColliders)
            {
                //Draw the autocellblock physics check boxes
                DrawAutoCellBlockingBoxes();
            }
            if(showBelowRays)
            {
                //Draw the rays to check for the ground object
                DrawAutoCellBlockingRays();
            }
        }   
        //Draws the grid gizmo
        if(!drawSimple)
        {
            DrawLines();
        }
        else
        {
            DrawSimpleLines();
        }
#endif
    }

    //Draws only the outside lines of the grid, useful if your grid is extremely large to speed up scene view
    private void DrawSimpleLines()
    {
        Gizmos.DrawLine(gridPoints[0], gridPoints[gridHeight]);
        Gizmos.DrawLine(gridPoints[gridHeight], gridPoints[gridPoints.Length - 1]);
        Gizmos.DrawLine(gridPoints[gridPoints.Length - 1], gridPoints[gridPoints.Length - 1 - gridHeight]);
        Gizmos.DrawLine(gridPoints[0], gridPoints[gridPoints.Length - 1 - gridHeight]);
    }

    //Draws a line from each grid point extending as far as the ground distance, this is how far the physics check goes
    private void DrawAutoCellBlockingRays()
    {
        for (int i = 0; i < gridPoints.Length; i++)
        {
            Gizmos.DrawLine(gridPoints[i], gridPoints[i] + new Vector3(0, -GroundDistance, 0));
        }
    }

    //Draws a box from the center of each cell
    private void DrawAutoCellBlockingBoxes()
    {
        if (cells != null)
        {
            for (int i = 0; i < cells.Length; i++)
            {
                Gizmos.DrawWireCube(cells[i] + new Vector3(0, checkBoxOffset + (aboveCheckBoxHeight * 0.5f), 0), new Vector3(cellSize * aboveCheckBoxSize, aboveCheckBoxHeight, cellSize * aboveCheckBoxSize));
            }
        }
    }

//Wont run at build time
#if UNITY_EDITOR
    //Draws the cell position as a Vector3
    private void DrawCellPositions()
    {

        for (int i = 0; i < numCells; i++)
        {
            Handles.Label(cells[i], cells[i].ToString("F3"));
        }
    }

    //Draws the point positions as a Vector3
    private void DrawPointPositions()
    {
        for (int i = 0; i < gridPoints.Length; i++)
        {
            Handles.Label(gridPoints[i], new Vector2(gridPoints[i].x, gridPoints[i].z).ToString("F3"));
        }
    }
#endif

    //Draws the Lines of the grid
    private void DrawLines()
    {
        if (gridHeight > 0 && gridWidth > 0)
        {
            //Z lines
            for (int x = 0; x < gridWidth + 1; x++)
            {
                for (int z = 0; z < gridHeight; z++)
                {
                    Gizmos.DrawLine(gridPoints[x * (gridHeight + 1) + z], gridPoints[x * (gridHeight + 1) + z + 1]);
                }
            }
            //X lines
            for (int z = 0; z < gridHeight + 1; z++)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    Gizmos.DrawLine(gridPoints[z * gridWidth + x], gridPoints[z * gridWidth + (gridHeight + 1) + x]);
                }
            }
        }
    }

    //Useful function to call to get a list of all of the currently placed objects on this grid
    public List<GameObject> GetCellsDictionary()
    {
        List<GameObject> allPlacedObjects = new List<GameObject>();
        foreach (KeyValuePair<Vector3, GameObject> entry in gridCellsStatus)
        {
            //Debug.Log(entry);
            allPlacedObjects.Add(entry.Value);
        }
        return allPlacedObjects;
    }

    //Useful function to call to get a list of all of the currently placed objects on this grid
    public List<GameObject> GetGridCellsDictionary()
    {
        List<GameObject> allPlacedObjects = new List<GameObject>();
        foreach (KeyValuePair<Vector3, GameObject> entry in gridCells)
        {
            //Debug.Log(entry);
            allPlacedObjects.Add(entry.Value);
        }
        return allPlacedObjects;
    }
}

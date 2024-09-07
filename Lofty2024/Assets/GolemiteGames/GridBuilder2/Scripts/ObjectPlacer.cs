using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;
using System;

/*************This class handles all placement of objects and blocking of cells**************/
public class ObjectPlacer : MonoBehaviour
{
    public enum PlacementType
    {
        PlaceMultiple,
        PlaceOne,
        DragBuild,
        DragAndDrop
    }

    //Only serialized variable to hide cells under the placed object
    [SerializeField] PlacementType placementType;
    [SerializeField] float placementHeight;
    [SerializeField] bool overwrite;
    [SerializeField] bool hideCellsUnderPlacedObj;
    [SerializeField] Material buildMaterial;
    [SerializeField] GameObject timerCanvas;
    [SerializeField] float canvasHeight;
    //Unity event variable 
    UnityEvent movedObjEvent = new UnityEvent();

    //Build time preview variables
    GridSelector gridSelector;

    //Accessors
    public UnityEvent MovedObjEvent
    {
        get { return movedObjEvent; }
    }
    public bool HideCellsUnderPlacedObj
    {
        get { return hideCellsUnderPlacedObj; }
    }
    public bool Overwrite
    {
        get { return overwrite; }
    }
    public GameObject TimerCanvas
    {
        get { return timerCanvas; }
    }
    public float PlacementHeight
    {
        get { return placementHeight; }
    }
    public bool PlaceOne
    {
        get
        {
            if (placementType == PlacementType.PlaceOne)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public bool PlaceMultiple
    {
        get
        {
            if (placementType == PlacementType.PlaceMultiple)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public bool DragBuild
    {
        get 
        { 
            if (placementType == PlacementType.DragBuild)
            {
                return true;
            }
            else 
            {
                return false;
            }
        }
    }
    public bool DragAndDrop
    {
        get
        {
            if (placementType == PlacementType.DragAndDrop)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    private void Awake()
    {
        gridSelector = GridBuilder2Manager.Instance.GridSelector;
    }
        
    private void Start()
    {
    }


    //Places an object on the grid
    public GameObject PlaceObject(GameObject obj, GridObject.Data gridObjectData, int layer)
    {
        GameObject clonedObj;

        //Create object
        if (gridObjectData.MoveOnPoints)
        {
            clonedObj = Instantiate(obj,
            gridObjectData.Position -
            new Vector3((gridSelector.SelectedCellSize * 0.5f), 0, (gridSelector.SelectedCellSize * 0.5f)) +
            new Vector3(0, placementHeight, 0),
            Quaternion.Euler(0, GetObjectRotation(gridObjectData, obj),
            0));
        }
        else
        {
            clonedObj = Instantiate(obj,
            gridObjectData.Position + new Vector3(0, placementHeight, 0),
            Quaternion.Euler(0, GetObjectRotation(gridObjectData, obj),
            0));
        }
        if (obj.GetComponent<UpgradeData>())
        {
            UpgradeData upgradeData = obj.GetComponent<UpgradeData>();
            if (gridObjectData.UpgradeLevel > 1 && upgradeData.UpgradePrefabs[upgradeData.CurrentUpgradeLevel - 1])
            {
                //Changes mesh of prefab
                clonedObj.GetComponent<MeshFilter>().sharedMesh =
                    upgradeData.UpgradePrefabs[gridObjectData.UpgradeLevel - 2].GetComponent<MeshFilter>().sharedMesh;

                clonedObj.GetComponent<MeshRenderer>().sharedMaterials =
                    upgradeData.UpgradePrefabs[gridObjectData.UpgradeLevel - 2].GetComponent<MeshRenderer>().sharedMaterials;
            }
        }
        clonedObj.name = obj.name;

        //Gives the object its own unique SelectObject.ObjectSize so it remembers what cells it takes up
        Building.ObjectSize clonedObjSize = gridSelector.CreateObjectSizeInstance(gridObjectData);

        //Adds the GridObject class to the placed object, this contains all the object data needed for multiple functions
        GridObject clonedGridObject = clonedObj.AddComponent<GridObject>();

        //Copies the sent class data to the new object
        clonedGridObject.data = gridObjectData;
        clonedGridObject.data.ObjSize = clonedObjSize;

        //Changes object and children ignoreraycast layer to original layer
        clonedObj.layer = layer;
        Transform[] clonedObjChildren = clonedObj.GetComponentsInChildren<Transform>();
        foreach (Transform child in clonedObjChildren)
        {
            child.gameObject.layer = layer;
        }

        //Adds the placed object under the gridobjectcontainer under the grid object for easy tracking
        clonedObj.transform.parent = gridObjectData.GridSquare.GridObjectContainer.transform;

        //Resets the loaded status so if you load again, the grid will assume its previous position
        gridObjectData.GridSquare.Loaded = false;

        //Add it to Dictionaries for easier locating
        ObjectStorage.AddTypeObject(gridObjectData.PrefabId, clonedObj);
        ObjectStorage.GOInstanceList.Add(gridObjectData.InstanceId, clonedObj);

        CallOnBuildingPlaceInt(clonedObj);

        return clonedObj;
    }

    public GameObject PlaceObject(GameObject obj, Vector3 pos, GridSquare grid, int layer)
    {
        GameObject clonedObj;

        //Create object
        clonedObj = Instantiate(obj,
        pos + new Vector3(0, placementHeight, 0),
        Quaternion.Euler(0, obj.transform.rotation.eulerAngles.y,
        0));
        
        clonedObj.name = obj.name;

        //Adds the GridObject class to the placed object, this contains all the object data needed for multiple functions
        GridObject clonedGridObject = clonedObj.AddComponent<GridObject>();
        clonedGridObject.data = new GridObject.Data();
        clonedGridObject.data.Position = new Vector3(clonedObj.transform.position.x, 0, clonedObj.transform.position.z);
        clonedGridObject.data.GridSquare = grid;

        //Changes object and children ignoreraycast layer to original layer
        clonedObj.layer = layer;
        Transform[] clonedObjChildren = clonedObj.GetComponentsInChildren<Transform>();
        foreach (Transform child in clonedObjChildren)
        {
            child.gameObject.layer = layer;
        }

        //Adds the placed object under the gridobjectcontainer under the grid object for easy tracking
        clonedObj.transform.parent = grid.GridObjectContainer.transform;

        //Resets the loaded status so if you load again, the grid will assume its previous position
        grid.Loaded = false;



        return clonedObj;
    }

    public GameObject DelayBuildStart(GameObject obj, GridObject.Data gridObjectData, int layer, int buildTime)
    {
        GameObject objInstance = null;

        //Return object here is our object instance that is placed temporarily before building the actual object
        StartCoroutine(DelayBuild(obj, gridObjectData, layer, buildTime, returnObject => {
            if(returnObject)
            {
                objInstance = returnObject;
            }
        }));
        return objInstance;
    }

    private IEnumerator DelayBuild(GameObject obj, GridObject.Data gridObjectData, int layer, int buildTime, System.Action<GameObject> clone)
    {
        GameObject clonedObj;
        //Create a temp timed clone
        if (gridObjectData.MoveOnPoints)
        {
            clonedObj = Instantiate(obj,
            gridObjectData.Position -
            new Vector3((gridSelector.SelectedCellSize * 0.5f), 0, (gridSelector.SelectedCellSize * 0.5f)) +
            new Vector3(0, placementHeight, 0),
            Quaternion.Euler(0, GetObjectRotation(gridObjectData, obj),
            0));
        }
        else
        {
            clonedObj = Instantiate(obj,
            gridObjectData.Position + new Vector3(0, placementHeight, 0),
            Quaternion.Euler(0, GetObjectRotation(gridObjectData, obj),
            0));
        }


        //Assign parent
        clonedObj.transform.parent = gridObjectData.GridSquare.gameObject.transform.GetChild(1).transform;

        //Create new ObjectSize instance
        Building.ObjectSize clonedObjSize = gridSelector.CreateObjectSizeInstance(gridObjectData);

        if (gridSelector)
        {
            gridSelector.ChangeObjMat(clonedObj, buildMaterial);
        }
        else
        {
            Debug.Log("You need a grid selector to change materials");
        }


        //Always add the GridObject class after changing the material
        GridObject clonedObjData =  clonedObj.AddComponent<GridObject>();
        clonedObjData.data = gridObjectData;
        clonedObjData.data.ObjSize = clonedObjSize;
        clonedObjData.data.BuildTime = buildTime;
        clone(clonedObj);

        //If building on a timer to begin with
        if(clonedObjData.data.BuildTimeRemaining == 0)
        {
            clonedObjData.data.BuildTimeRemaining = buildTime; 
        }

        //If you have added a canvas with a Timer component, this will display your Timer with the seconds set in the SelectObject component
        if (timerCanvas)
        {
            CreateTimerCanvas(buildTime, clonedObj, clonedObjData);
        }
        else
        {
            //This object has a Time to build set, but no canvas to display under Timer Canvas
        }

        CallOnBuildingTimerStartInt(clonedObj);

        //This will now wait for the Timer to reach 0 before continuing
        yield return new WaitForSeconds(gridObjectData.BuildTimeRemaining);

        CallOnBuildingTimerEndInt(clonedObj);

        //This line prevents the actual object being built by the PlaceObject function if you have removed the temp object
        if (!clonedObj)
        {
            yield break;
        }


        gridObjectData.BuildTimeRemaining = -1;
        
        //Place the actual object
        GameObject builtObject = PlaceObject(obj, gridObjectData, layer);

        if(gridSelector.PreviewObjFloorTiles && gridSelector.PlaceTilesWithObject)
        {
            clonedObj.transform.Find("FloorTileParent").gameObject.transform.parent = builtObject.transform;
        }


        //Destroy the timed object
        Destroy(clonedObj);


        //Block the cells with the actual object returned built object
        foreach (Vector3 position in gridObjectData.CheckPositions)
        {
            gridObjectData.GridSquare.ChangeCellStatus(position, builtObject);
        }
    }

    public GameObject CreateTimerCanvas(int buildTime, GameObject clonedObj, GridObject clonedObjData)
    {
        //Create a new Canvas
        GameObject clonedCanvas =
            Instantiate(timerCanvas,
            clonedObj.transform.position + new Vector3(0, GetObjectHeight(clonedObj) + canvasHeight, 0),
            timerCanvas.transform.rotation);
        clonedCanvas.transform.SetParent(clonedObj.transform, true);
        clonedCanvas.name = "Build Timer Canvas";

        if (clonedCanvas.GetComponentInChildren<Timer>())
        {
            //Sets the Timer with the seconds set in the SelectObject component
            Timer[] clonedTimers = clonedCanvas.transform.GetComponentsInChildren<Timer>();
            int hours = buildTime / 60 / 60;
            int minutes = (buildTime / 60) - (hours * 60);
            int seconds = buildTime - (hours * 60 * 60) - (minutes * 60);

            for (int i = 0; i < clonedTimers.Length; i++)
            {
                //Adds the variables to the timers object
                clonedTimers[i].hours = hours;
                clonedTimers[i].minutes = minutes;
                clonedTimers[i].seconds = seconds;

                //Starts the timers
                clonedTimers[i].StartTimer(clonedObjData.data.BuildTimeRemaining);
            }
        }

        //Displays the Canvas
        clonedCanvas.gameObject.SetActive(true);

        return clonedCanvas;
    }




    //Changes an objects position and rotation to the given position and rotation
    public void MoveObject(GameObject obj, GridObject.Data gridObjectData, int layer)
    {
        //Checks to see if you are hovering over a canvas, if so, ignore
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            //Instead of instantiating a new object, we just ammend the position and rotation of the given object
            Vector3 newPos = gridObjectData.Position + new Vector3(0, placementHeight, 0);
            Quaternion newRot = Quaternion.Euler(0, GetObjectRotation(gridObjectData, obj), 0);
            //Create object
            if (gridObjectData.MoveOnPoints)
            {
                obj.transform.position = newPos -
                    new Vector3((gridSelector.SelectedCellSize * 0.5f), 0, (gridSelector.SelectedCellSize * 0.5f));
            }
            else
            {
                obj.transform.position = newPos;
            }

            obj.transform.rotation = newRot;

            //Adds the GridObject class to the placed object, this contains all the object data needed for multiple functions
            GridObject clonedObjData = obj.GetComponent<GridObject>();

            //Adds all of the object data to the class
            clonedObjData.data = gridObjectData;

            //Changes object and children ignoreraycast layer to original layer
            obj.layer = layer;
            Transform[] clonedObjChildren = obj.GetComponentsInChildren<Transform>();
            foreach (Transform child in clonedObjChildren)
            {
                child.gameObject.layer = layer;
            }

            //Adds the placed object under the gridobjectcontainer under the grid object for easy tracking
            obj.transform.parent = gridObjectData.GridSquare.GridObjectContainer.transform;

            //Resets the loaded status so if you load again, the grid will assume its previous position
            gridObjectData.GridSquare.Loaded = false;

            CallOnBuildingMoveEndInt(obj);

            //This fires the event after moving an already placed object
            movedObjEvent.Invoke();
        }
    }

    public void MoveObject(GameObject obj, Vector3 pos, GridSquare grid, int layer)
    {
        //Checks to see if you are hovering over a canvas, if so, ignore
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            //Instead of instantiating a new object, we just ammend the position and rotation of the given object
            Vector3 newPos = pos + new Vector3(0, placementHeight, 0);
            Quaternion newRot = Quaternion.Euler(0, obj.transform.rotation.eulerAngles.y, 0);
            obj.transform.position = newPos;
            obj.transform.rotation = newRot;

            //Adds the GridObject class to the placed object, this contains all the object data needed for multiple functions
            GridObject clonedObjData = obj.GetComponent<GridObject>();

            //Adds all of the object data to the class
            clonedObjData.data.Position = newPos;
            clonedObjData.data.Rotation = newRot.y;

            //Changes object and children ignoreraycast layer to original layer
            obj.layer = layer;
            Transform[] clonedObjChildren = obj.GetComponentsInChildren<Transform>();
            foreach (Transform child in clonedObjChildren)
            {
                child.gameObject.layer = layer;
            }

            //Adds the placed object under the gridobjectcontainer under the grid object for easy tracking
            obj.transform.parent = grid.GridObjectContainer.transform;

            //Resets the loaded status so if you load again, the grid will assume its previous position
            grid.Loaded = false;

            CallOnBuildingMoveEndInt(obj);

            //This fires the event after moving an already placed object
            movedObjEvent.Invoke();
        }
    }

    //Formulates and returns a List of only the blocked cells
    public List<Vector3> GetSpecificCheckPositions(Vector3[] checkPositions, Building.ObjectSize objectSize)
    {
        //Create a new List that we can return at the end
        List<Vector3> specificCheckPositions = new List<Vector3>();
        
        if (objectSize != null)
        {
            int i = 0;

            //Loops over all the potential object positions
            foreach (System.Reflection.FieldInfo prop in objectSize.GetType().GetFields())
            {
                //The bool is true
                if((bool)prop.GetValue(objectSize))
                {
                    //Add the check position to the List
                    specificCheckPositions.Add(checkPositions[i]);
                }
                i++;
            }

        }
        return specificCheckPositions;
    }

    //Blocks the specific check positions on the grid
    public void BlockCells(List<Vector3> checkPositions, GameObject obj, GridSquare grid)
    {
        foreach(Vector3 pos in checkPositions)
        {
            //Change the cell status
            grid.ChangeCellStatus(pos, obj);
        }
    }


    //If checked, hides the cells that are taken up from the placed object
    public void HidePlacedObjCells(GridSquare gridSquare, List<Vector3> checkPositions)
    {
        //This function only works for single and checkered types
        if (gridSquare.GetGridType == GridSquare.GridType.Simple ||
            gridSquare.GetGridType == GridSquare.GridType.Points ||
            gridSquare.GetGridType == GridSquare.GridType.Lines)
        {
            return;
        }

        //This will hold all of the cell GameObjects under the placed object
        List<GameObject> cellList = new List<GameObject>();

        foreach (Vector3 pos in checkPositions)
        {
            //If there is a cell created
            if (gridSquare.GetCellObject(pos) != null)
            {
                //Add it to the cellList
                cellList.Add(gridSquare.GetCellObject(pos));
            }
        }

        //For every cell in the cellList, hide it 
        foreach (GameObject cell in cellList)
        {
            cell.SetActive(false);
        }
    }

    //Shows the cells underneath a placed object if then removed
    public void ShowRemovedObjCells(GridSquare gridSquare, List<Vector3> checkPositions)
    {
        //This function only works for single and checkered types
        if (gridSquare.GetGridType == GridSquare.GridType.Simple ||
            gridSquare.GetGridType == GridSquare.GridType.Points ||
            gridSquare.GetGridType == GridSquare.GridType.Lines)
        {
            return;
        }

        //This will hold all of the cell GameObjects under the placed object
        List<GameObject> cellList = new List<GameObject>();

        //If there is an object occupying the cell
        foreach (Vector3 pos in checkPositions)
        {
            if (gridSquare.GetCellObject(pos) != null)
            {
                //Add it to the cellList
                cellList.Add(gridSquare.GetCellObject(pos));
            }
        }

        //For every cell in the cellList, show it
        foreach (GameObject cell in cellList)
        {
            cell.SetActive(true);
        }     
    }

    //Returns the objects height if it has a collider
    public float GetObjectHeight(GameObject obj)
    {
        if(obj.GetComponent<Collider>())
        {
            return obj.GetComponent<Collider>().bounds.size.y;
        }
        else
        {
            return 0;
        }
    }

    //If you are using rotate, that is the rotate that will be placed, if not, it will use the prefabs rotation
    private float GetObjectRotation(GridObject.Data gridObject, GameObject obj)
    {
        if(gridSelector && gridSelector.Rotation)
        {
            return gridObject.Rotation;
        }
        else
        {
            return obj.transform.rotation.eulerAngles.y;
        }
    }

    private static void CallOnBuildingPlaceInt(GameObject placedObj)
    {
        IOnBuildingPlace[] onPlacedList = placedObj.GetComponents<IOnBuildingPlace>();
        for (int i = 0; i < onPlacedList.Length; i++)
        {
            onPlacedList[i].OnPlace(placedObj);
        }
    }

    private static void CallOnBuildingMoveEndInt(GameObject movedObj)
    {
        IOnBuildingMoveEnd[] onMovedList = movedObj.GetComponents<IOnBuildingMoveEnd>();
        for (int i = 0; i < onMovedList.Length; i++)
        {
            onMovedList[i].OnMoveEnd(movedObj);
        }
    }

    private static void CallOnBuildingTimerStartInt(GameObject timedObj)
    {
        IOnBuildingTimedStart[] onTimerStartList = timedObj.GetComponents<IOnBuildingTimedStart>();
        for (int i = 0; i < onTimerStartList.Length; i++)
        {
            onTimerStartList[i].OnTimerStart(timedObj);
        }
    }

    private static void CallOnBuildingTimerEndInt(GameObject timedObj)
    {
        IOnBuildingTimedEnd[] onTimerStartList = timedObj.GetComponents<IOnBuildingTimedEnd>();
        for (int i = 0; i < onTimerStartList.Length; i++)
        {
            onTimerStartList[i].OnTimerEnd();
        }
    }
}

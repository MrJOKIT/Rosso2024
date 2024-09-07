using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*************This class handles all removal of objects and clearing of cells**************/
public class ObjectRemover : MonoBehaviour
{
    //Private variables
    Ray ray;
    RaycastHit hit;
    GameObject removeObj;
    GameObject previousHoveredObject;
    Material[] hoveredObjectsMaterial;
    bool matChanged = false;

    //Cached variables
    GridSelector gridSelector;
    ObjectSelector objectSelector;
    ObjectPlacer objectPlacer;

    //Serialized variables
    [SerializeField] Material removePreviewMat;
    [SerializeField] bool dragRemove;

    void Awake()
    {

    }
    private void Start()
    {
        //Caches objects
        gridSelector = GridBuilder2Manager.Instance.GridSelector;
        objectPlacer = GridBuilder2Manager.Instance.ObjectPlacer;
        objectSelector = GridBuilder2Manager.Instance.ObjectSelector;
    }
    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            removeObj = hit.collider.gameObject;
            //This will only run if the object has the GridObject component, this gets added by the object placer at placement time 
            if(removeObj.GetComponent<GridObject>()) {

                if(!matChanged)
                {
                    //Sets the previously selected object to change it back when not hovering over it
                    previousHoveredObject = hit.collider.gameObject;

                    //Gets the hovered objects material for replacement later
                    Renderer[] children = previousHoveredObject.GetComponentsInChildren<Renderer>();
                    hoveredObjectsMaterial = new Material[children.Length];
                    for (int i = 0; i < children.Length; i++)
                    {
                        hoveredObjectsMaterial[i] = children[i].sharedMaterial;
                    }

                    //Changes the material for visual confirmation of deletion
                    if(removePreviewMat)
                    {
                        gridSelector.ChangeObjMat(removeObj, removePreviewMat);
                    }

                    //So the code runs once for each object
                    matChanged = true;
                }
                
                //This replaces the objects material if it still finds a GridObject without touching something else, 
                //hence not reaching the else statement further down
                if (previousHoveredObject != removeObj)
                {
                    ChangeMatBack();
                }

                //Checks to see if the cursor is hovered over a canvas element
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    //Click remove
                    if (Input.GetMouseButtonUp(0) && !dragRemove)
                    {
                        RemoveObject(removeObj, removeObj.GetComponent<GridObject>().data);

                    }

                    //Drag remove
                    if (Input.GetMouseButton(0) && dragRemove) 
                    {
                        if(hit.collider.gameObject.GetComponent<GridObject>())
                        {
                            RemoveObject(removeObj, removeObj.GetComponent<GridObject>().data);
                        }
                        
                    }
                }
            }
            //Changes the material to the original if not touching a GridObject component
            else
            {
                if(previousHoveredObject != null)
                {
                    if(matChanged)
                    {
                        ChangeMatBack();
                    }
                }
            }
        }
        //Changes the material back to the original if the ray hits nothing
        else
        {
            if (previousHoveredObject != null)
            {
                if (matChanged)
                {
                    ChangeMatBack();
                }
            }
        }
    }

    public void RemoveObject(GameObject objToRemove, GridObject.Data gridObjectData)
    {

        //Removes the object from the GridSquare object freeing up the cell for placement
        UnblockCells(gridObjectData.CheckPositions, gridObjectData.GridSquare);

        //Shows the grid cells that were hidden
        if(objectPlacer)
        {
            if(objectPlacer.HideCellsUnderPlacedObj)
            {
                objectPlacer.ShowRemovedObjCells(gridObjectData.GridSquare, gridObjectData.CheckPositions);
            }
        }

        matChanged = false;

        //Remove the object from the dictionaries as it is being deleted
        ObjectStorage.RemoveTypeObject(gridObjectData.PrefabId, objToRemove);
        ObjectStorage.GOInstanceList.Remove(gridObjectData.InstanceId);
        CallOnBuildingRemoveInt(objToRemove);
        Destroy(objToRemove);
        if(objectSelector)
        {
            objectSelector.ResetCanvas();
        }
    }

    //More generic variation of the RemoveObject method
    public void RemoveObject(GameObject objToRemove, List<Vector3> checkPositions, GridSquare grid)
    {
        //Removes the object from the GridSquare object freeing up the cell for placement
        UnblockCells(checkPositions, grid);

        //Shows the grid cells that were hidden
        if (objectPlacer)
        {
            if (objectPlacer.HideCellsUnderPlacedObj)
            {
                objectPlacer.ShowRemovedObjCells(grid, checkPositions);
            }
        }

        matChanged = false;
        CallOnBuildingRemoveInt(objToRemove);
        Destroy(objToRemove);
        if(objectSelector)
        {
            objectSelector.ResetCanvas();
        }
    }

    //Only used as a public helper function to delete all objects of a given type via the SelectObject btn
    public void RemoveObjectsOfType(int prefabId = 0)
    {
        List<GameObject> GOList = ObjectStorage.GetObjectsOfType(prefabId);
        if (GOList != null)
        {
            for (int i = 0; i < GOList.Count; i++)
            {
                CallOnBuildingRemoveInt(GOList[i]);
                Destroy(GOList[i]);
            }
        }
    }

    //Remove individual instance as long as it has an instance ID
    public void RemoveObjectByInstanceId(string id)
    {
        GameObject GO;
        if(ObjectStorage.GOInstanceList.TryGetValue(id, out GO))
        {
            CallOnBuildingRemoveInt(GO);
            Destroy(GO);
        }
    }

    //Reverts the previously hovered objects material back to what it was
    private void ChangeMatBack()
    {
        if(previousHoveredObject)
        {
            previousHoveredObject.GetComponent<GridObject>().ReapplyOriginalMaterials();
        }
        else
        {
            objectSelector.ResetCanvas();
        }
        matChanged = false;
    }

    //Loops and calls the gridsquare function to change the cell status, freeing up the cells
    public void UnblockCells(List<Vector3> checkPositions, GridSquare grid)
    {
        foreach (Vector3 pos in checkPositions)
        {
            grid.ChangeCellStatus(pos, null);
        }
    }

    //This is called from another script when having to exit remove mode
    public void CancelRemove()
    {
        if(previousHoveredObject)
        {
            ChangeMatBack();
        }
    }

    private static void CallOnBuildingRemoveInt(GameObject removedObj)
    {
        IOnBuildingRemove[] onRemovedList = removedObj.GetComponents<IOnBuildingRemove>();
        for (int i = 0; i < onRemovedList.Length; i++)
        {
            onRemovedList[i].OnRemove(removedObj);
        }
    }
}



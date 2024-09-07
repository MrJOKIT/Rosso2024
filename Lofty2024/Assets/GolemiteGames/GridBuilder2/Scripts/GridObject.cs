using System.Collections.Generic;
using UnityEngine;

/*************This class is added to every placed object and contains all data related to the object for use at various points**************/
/*************It also stores its own material and layer to it can always revert back to it when needed**************/
public class GridObject : MonoBehaviour
{
    //Cached 
    GridSelector gridSelector;

    //Own materials
    Material[] ownMats;
    Renderer[] children;

    int layer;
    bool runOneTime = true;

    public Material[] OwnMats
    {
        get { return ownMats; }
    }

    [System.Serializable]
    public class Data
    {
        string objName;
        int prefabId;
        string instanceId;
        Vector3 position;
        Vector3 offset;
        Vector3 originalOffset;
        List<Vector3> checkPositions;
        float rotation;
        GridSquare gridSquare;
        Building.ObjectSize objSize;
        bool moveOnPoints;
        int buildTime;
        int buildTimeRemaining;
        int upgradeLevel;
        //Accessors to all of the data contained by the this class
        public string ObjName
        {
            get { return objName; }
            set { objName = value; }
        }
        public int PrefabId
        {
            get { return prefabId; }
            set { prefabId = value; }
        }
        public string InstanceId
        {
            get { return instanceId; }
            set { instanceId = value; }
        }
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }
        public Vector3 Offset
        {
            get { return offset; }
            set { offset = value; }
        }
        public Vector3 OriginalOffset
        {
            get { return originalOffset; }
            set { originalOffset = value; }
        }
        public List<Vector3> CheckPositions
        {
            get { return checkPositions; }
            set { checkPositions = value; }
        }
        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }
        public GridSquare GridSquare
        {
            get { return gridSquare; }
            set { gridSquare = value; }
        }
        public Building.ObjectSize ObjSize
        {
            get { return objSize; }
            set { objSize = value; }
        }
        public bool MoveOnPoints
        {
            get { return moveOnPoints; }
            set { moveOnPoints = value; }
        }
        public int BuildTime
        {
            get { return buildTime; }
            set { buildTime = value; }
        }
        public int BuildTimeRemaining
        {
            get { return buildTimeRemaining; }
            set { buildTimeRemaining = value; }
        }
        public int UpgradeLevel
        {
            get { return upgradeLevel; }
            set { upgradeLevel = value; }
        }
    }

    public Data data;

    private void Awake()
    {
        layer = gameObject.layer;
        InitObject();
    }
    private void Start()
    {
        gridSelector = GridBuilder2Manager.Instance.GridSelector;
    }
    private void InitObject()
    {
        children = GetComponentsInChildren<Renderer>();
        ownMats = new Material[children.Length];
        for (int i = 0; i < children.Length; i++)
        {
            ownMats[i] = children[i].sharedMaterial;
        }
    }

    private void Update()
    {
        //Reapplies the original layer back to the objects once
        if(gridSelector && !gridSelector.SelectedGameObjectToPlace)
        {
            if(gameObject.layer == LayerMask.NameToLayer("Ignore Raycast"))
            {
                if(!runOneTime)
                {
                    gameObject.layer = layer;
                    runOneTime = true;
                }
            }
        }
    }

    public void ReapplyOriginalMaterials()
    {
        for (int i = 0; i < children.Length; i++)
        {
            children[i].sharedMaterial = ownMats[i];
        }
    }
    private void OnMouseEnter()
    {
        if(gridSelector && gridSelector.SelectedGameObjectToPlace)
        {
            IgnoreRaycastChange();     
        }   
    }
    private void IgnoreRaycastChange()
    {
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        runOneTime = false;
    }
}

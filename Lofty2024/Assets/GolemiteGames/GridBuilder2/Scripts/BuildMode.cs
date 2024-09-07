using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*************This class should be on a Button that hides any grids and any functionality until the Button is pressed**************/
public class BuildMode : MonoBehaviour
{
    //Private variables
    Button btn;
    RemoveMode removeMode;
    GridSelector gridSelector;
    ObjectSelector objectSelector;
    GameObject[] gridsCellContainers;
    GameObject[] gridsObjectContainers;
    bool buildMode = false;

    //Serialized variables
    [Tooltip("You would place a menu GameObject here that holds all of your buildable objects(SelectObject.cs)")]
    [SerializeField] GameObject buildMenu;
    [SerializeField] bool hideGridObjectsWithCells;
    [Tooltip("If the Grid Selector has place tiles with building enabled, this will also hide them as well")]
    [SerializeField] bool hideTilesWithCells;

    private void Awake()
    {
        //Gets all the relevent components
        btn = GetComponent<Button>();
        removeMode = FindObjectOfType<RemoveMode>();
        gridSelector = FindObjectOfType<GridSelector>();
        objectSelector = FindObjectOfType<ObjectSelector>();
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetGridContainers());

        btn.onClick.AddListener(() => {
            if(buildMode)
            {
                //Exiting build mode
                BuildModeOff();
            }
            else
            {
                //Entering build mode
                BuildModeOn();
            }
        });
    }

    //This will get all the cell and object containers(if checked) of all active grids
    private IEnumerator GetGridContainers()
    {
        //Declare the arrays
        GridSquare[] grids = FindObjectsOfType<GridSquare>();
        gridsCellContainers = new GameObject[grids.Length];

        gridsObjectContainers = new GameObject[grids.Length];
        

        //Loops through all active grids
        for (int i = 0; i < grids.Length; i++)
        {
            //Waiting until the GridSquare objects have been created
            yield return new WaitUntil(() => grids[i].Created);

            //Fill up the arrays
            gridsObjectContainers[i] = grids[i].transform.GetChild(1).gameObject;
            

            gridsCellContainers[i] = grids[i].transform.GetChild(0).gameObject;
        }
        
        //Disables build mode by default 
        BuildModeOff();
    }

    //Hides all grid cell objects and prevents any placement logic
    private void BuildModeOff()
    {
        //Disables anything that was previously active
        if (gridSelector)
        {
            //Removes the preview obj
            gridSelector.DeselectPreview();
        }
        if (removeMode)
        {
            //Stops remove mode operations
            removeMode.DisableRemoveMode();
            removeMode.ChangeText();
        }
        if(objectSelector)
        {
            //Stops user from selecting objects
            objectSelector.enabled = false;
            if(objectSelector.SelectorCanvas)
            {
                objectSelector.SelectorCanvas.gameObject.SetActive(false);
            }
        }

        //Hides the physical grids
        foreach (GameObject container in gridsCellContainers)
        {
            container.SetActive(false);
        }

        //Hides the floor tiles if enabled
        if (gridSelector.PlaceTilesWithObject && hideTilesWithCells)
        {
            foreach (GameObject objContainer in gridsObjectContainers)
            {
                foreach (Transform building in objContainer.GetComponentsInChildren<Transform>(true))
                {
                    building.transform.Find("FloorTileParent")?.gameObject.SetActive(false);
                }
            }
        }

        //Hides the grid objects if checked
        if (hideGridObjectsWithCells)
        {
            foreach (GameObject objContainer in gridsObjectContainers)
            {
                objContainer.SetActive(false);
            }
        }

        //Disables grid selector
        if (gridSelector)
        {
            gridSelector.enabled = false;
            gridSelector.gameObject.SetActive(false);
        }

        //Disables canvas build menu
        buildMenu.SetActive(false);

        buildMode = false;
    }

    private void BuildModeOn()
    {
        //Shows the physical grids
        foreach (GameObject container in gridsCellContainers)
        {
            container.SetActive(true);
        }

        //Shows the physical grids if checked
        if (hideGridObjectsWithCells)
        {
            foreach (GameObject objContainer in gridsObjectContainers)
            {
                objContainer.SetActive(true);
            }
        }

        //Enables selection of objects
        if (objectSelector)
        {
            objectSelector.enabled = true;
            if (objectSelector.SelectorCanvas)
            {
                objectSelector.SelectorCanvas.gameObject.SetActive(true);
            }
        }

        //Shows the floor tiles if enabled
        if (gridSelector.PlaceTilesWithObject && hideTilesWithCells)
        {
            foreach (GameObject objContainer in gridsObjectContainers)
            {
                foreach (Transform building in objContainer.GetComponentsInChildren<Transform>(true))
                {
                    building.transform.Find("FloorTileParent")?.gameObject.SetActive(true);
                }
            }
        }

        //Enables grid selector
        if (gridSelector)
        {
            gridSelector.enabled = true;
            gridSelector.gameObject.SetActive(true);
        }

        //Enables canvas build menu
        buildMenu.SetActive(true);

        buildMode = true;
    }
}

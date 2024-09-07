using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PickBuildingBtn : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    //Cached variables
    GridSelector gridSelector;
    RemoveMode removeMode;
    ObjectPlacer objectPlacer;
    ObjectSelector objectSelector;

    bool dragAndDrop = false;
    Button btn;

    [SerializeField] Building buildingPrefab;

    private void Awake()
    {
        removeMode = FindObjectOfType<RemoveMode>();
        btn = GetComponent<Button>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //Caches objects
        objectPlacer = GridBuilder2Manager.Instance.ObjectPlacer;
        if (objectPlacer)
        {
            dragAndDrop = objectPlacer.DragAndDrop;
        }
        gridSelector = GridBuilder2Manager.Instance.GridSelector;
        objectSelector = GridBuilder2Manager.Instance.ObjectSelector;


        if (buildingPrefab)
        {
            buildingPrefab.SaveStartChecks();
        }

        if (!dragAndDrop)
        {
            btn.onClick.AddListener(() =>
            {
                SetBuildObject();
            });
        }
    }

    //Sets up the functions to place the prefab attached to this component
    private void SetBuildObject()
    {
        if (removeMode)
        {
            removeMode.DisableRemoveMode();
            removeMode.ChangeText();
        }
        if (objectSelector)
        {
            if (objectSelector.SelectedObj)
            {
                objectSelector.DeselectObject();
            }
        }
        if (gridSelector)
        {
            gridSelector.DeselectPreview();

            if (buildingPrefab)
            {
                buildingPrefab.ResetStartChecks();
                gridSelector.DragMove = false;
                gridSelector.SetGameObjectToPlace(buildingPrefab);
            }
        }
    }

    //This are used for the drag and drop objectPlacer method
    public void OnPointerDown(PointerEventData pointerData)
    {
        if (objectPlacer.DragAndDrop)
        {
            if (pointerData.button == PointerEventData.InputButton.Left)
            {
                gridSelector.Dragging = true;
                SetBuildObject();
            }
        }
    }

    //This are used for the drag and drop objectPlacer method
    public void OnPointerUp(PointerEventData pointerData)
    {
        if (pointerData.button == PointerEventData.InputButton.Left)
        {
            if (gridSelector.Dragging && !gridSelector.PreviewObj)
            {
                gridSelector.DeselectPreview();
                gridSelector.Dragging = false;
            }
            if (gridSelector.Dragging && gridSelector.PreviewObj)
            {
                if (!gridSelector.PreviewObj.activeSelf)
                {
                    gridSelector.DeselectPreview();
                    gridSelector.Dragging = false;
                }
            }
        }
    }
}


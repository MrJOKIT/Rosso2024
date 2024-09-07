using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GridBuilder2_Core
{
    public class PickBuildingWithKey : MonoBehaviour
    {
        //Cached variables
        GridSelector gridSelector;
        RemoveMode removeMode;
        ObjectSelector objectSelector;

        [SerializeField] Building building;
        [SerializeField] KeyCode keyToPickObject;

        private void Awake()
        {
            removeMode = FindObjectOfType<RemoveMode>();
        }

        private void Start()
        {
            gridSelector = GridBuilder2Manager.Instance.GridSelector;
            objectSelector = GridBuilder2Manager.Instance.ObjectSelector;

            if (building)
            {
                building.SaveStartChecks();
            }
        }

        void Update()
        {
            if(Input.GetKeyDown(keyToPickObject))
            {
                SetBuildObject();
            }
        }

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

                if (building)
                {
                    building.ResetStartChecks();
                    gridSelector.DragMove = false;
                    gridSelector.SetGameObjectToPlace(building);
                }
            }
        }
    } 
}

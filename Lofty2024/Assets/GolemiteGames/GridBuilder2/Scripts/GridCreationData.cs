using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class GridCreationData : ScriptableObject
{
    [SerializeReference]
    Vector3[] gridPoints;

    [SerializeReference]
    Vector3[] cells;

    [SerializeReference]
    List<Vector3> gridCellStatusPos;

    [SerializeReference]
    List<bool> gridCellStatusEmpty;

    private void OnEnable()
    {
        hideFlags = HideFlags.DontUnloadUnusedAsset;
    }
    public void Initialise(Vector3[] gridPoints, Vector3[] cells, Dictionary<Vector3, GameObject> gridCellsStatus)
    {
        this.gridPoints = gridPoints;
        this.cells = cells;

        //Lists for cell status
        this.gridCellStatusPos = gridCellsStatus.Keys.ToList();
        this.gridCellStatusEmpty = new List<bool>();

        foreach (GameObject GO in gridCellsStatus.Values.ToList())
        {
            if (GO == null)
            {
                gridCellStatusEmpty.Add(true);
            }
            else
            {
                gridCellStatusEmpty.Add(false);
            }
        }
    }

    public void RebuildData(GridSquare grid)
    {
        //Sets the initial data structures to the saved ones
        grid.GridPoints = gridPoints;
        grid.Cells = cells;

        //Initialises the dictionaries with null
        grid.CreateGridStatus();
        grid.CreateGridCellStatus();

        //Rebuilds the gridCellStatus dictionary using two lists 
        for (int i = 0; i < gridCellStatusPos.Count; i++)
        {
            if(gridCellStatusEmpty[i])
            {
                grid.ChangeCellStatus(gridCellStatusPos[i], null);
            }
            else
            {
                grid.ChangeCellStatus(gridCellStatusPos[i], grid.gameObject);
            }
        }

        //Rebuilds the gridCells dictionary using two lists 
        if (grid.GetGridType == GridSquare.GridType.SingleCell || grid.GetGridType == GridSquare.GridType.Chequered)
        {
            //Gets all children cells of grid, will only work with cells and not lines or points
            List<Transform> _gridCells = grid.transform.Find("CellContainer").GetComponentsInChildren<Transform>().ToList();

            //Remove the parent
            Transform container = grid.transform.Find("CellContainer");
            _gridCells.Remove(container);

            //Change the status of that position with found Gameobject
            for (int i = 0; i < _gridCells.Count; i++)
            {
                grid.ChangeGridCellStatus(_gridCells[i].position, _gridCells[i].gameObject);
            }
        }
    }
}


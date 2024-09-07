using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EditorAttributes;
using UnityEngine;

public class GridSpawnManager : Singeleton<GridSpawnManager>
{
    [Header("Material")] 
    public Material attackMat;
    public Material movableMat;

    [Header("Current Gird")] 
    public List<GridMover> currentGrid;
    
    
    [Button("Clear Mover")]
    public void ClearMover()
    {
        foreach (GridMover grid in currentGrid)
        {
            grid.ClearGrid();
        }
    }
    
    public void AddGridList(GridMover gridObject)
    {
        currentGrid.Add(gridObject);
    }
}

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
    public Material trapMat;

    [Header("Current Gird")] 
    public List<GridMover> currentGrid;

    public bool useWarp;
    
    
    [Button("Clear Mover")]
    public void ClearMover()
    {
        foreach (GridMover grid in currentGrid)
        {
            grid.enemyActive = false;
            grid.ClearGrid();
        }
    }
    
    public void AddGridList(GridMover gridObject)
    {
        currentGrid.Add(gridObject);
    }

    public void RemoveGrid(GridMover grid)
    {
        currentGrid.Remove(grid);
    }

    public IEnumerator WarpSelector()
    {
        yield return new WaitForSeconds(0.25f);
        if (!useWarp)
        {
            foreach (GridMover grid in currentGrid)
            {
                if (grid.gridState == GridState.Empty)
                {
                    grid.gridState = GridState.OnMove;
                }
            }

            useWarp = true;
        }
    }
}

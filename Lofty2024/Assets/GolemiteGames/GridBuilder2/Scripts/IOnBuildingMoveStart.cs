using UnityEngine;

public interface IOnBuildingMoveStart 
{
    /// <summary>
    /// Called at the start of moving an already placed object via the GridObjectOptions/ObjectPlacer class
    /// </summary>
    /// <param name="gameObject"></param>
    public void OnMoveStart(GameObject gameObject);
}


using UnityEngine;

public interface IOnBuildingMoveEnd
{
    /// <summary>
    /// Called at the end of moving an already placed object via the GridObjectOptions/ObjectPlacer class
    /// </summary>
    /// <param name="gameObject"></param>
    public void OnMoveEnd(GameObject gameObject);
}


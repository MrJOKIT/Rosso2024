using UnityEngine;

public interface IOnBuildingPlace
{
    /// <summary>
    /// Called when an object is placed via the ObjectPlacer class
    /// </summary>
    /// <param name="gameObject"></param>
    public void OnPlace(GameObject gameObject);
}


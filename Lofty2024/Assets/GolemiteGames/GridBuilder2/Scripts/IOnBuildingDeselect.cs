using UnityEngine;

public interface IOnBuildingDeselect 
{
    /// <summary>
    /// Called when an object is deselected via the ObjectSelector/GridObjectOptions class
    /// </summary>
    /// <param name="gameObject"></param>
    public void OnDeselect(GameObject gameObject);
}


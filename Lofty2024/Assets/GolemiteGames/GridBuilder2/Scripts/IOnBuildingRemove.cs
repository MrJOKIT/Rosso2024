using UnityEngine;

public interface IOnBuildingRemove 
{
    /// <summary>
    /// Called when an object is removed from the grid via the ObjectRemover class
    /// </summary>
    /// <param name="gameObject"></param>
    public void OnRemove(GameObject gameObject);
}


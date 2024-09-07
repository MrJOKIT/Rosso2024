using UnityEngine;

public interface IOnBuildingSelect 
{
    /// <summary>
    /// Called when an object is selected via the ObjectSelector/GridObjectOptions class
    /// </summary>
    /// <param name="gameObject"></param>
    public void OnSelect(GameObject gameObject);
}


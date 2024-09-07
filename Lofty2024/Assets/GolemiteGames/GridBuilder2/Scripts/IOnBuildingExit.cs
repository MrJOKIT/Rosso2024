using UnityEngine;
public interface IOnBuildingExit
{
    /// <summary>
    /// Called when the building exits being hovered via the ObjectSelector class
    /// </summary>
    /// <param name="gameObject"></param>
    public void OnBuildingExit(GameObject gameObject);
}

using UnityEngine;

public interface IOnBuildingOver
{
    /// <summary>
    /// Called when the building is hovered over via the ObjectSelector class
    /// </summary>
    /// <param name="gameObject"></param>
    public void OnBuildingOver(GameObject gameObject);
}

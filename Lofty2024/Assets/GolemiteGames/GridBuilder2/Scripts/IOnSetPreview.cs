using UnityEngine;

public interface IOnSetPreview
{
    /// <summary>
    /// Called when you set a preview object via the GridSelector class - Note this is only a preview and is not the same object that is placed on the grid
    /// </summary>
    /// <param name="prefab"></param>
    public void OnPreview(GameObject prefab);
}

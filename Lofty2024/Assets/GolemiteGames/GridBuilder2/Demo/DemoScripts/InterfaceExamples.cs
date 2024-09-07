using UnityEngine;

public class InterfaceExamples : MonoBehaviour, 
    IOnBuildingPlace, 
    IOnBuildingRemove, 
    IOnBuildingMoveStart, 
    IOnBuildingMoveEnd,  
    IOnBuildingSelect, 
    IOnBuildingDeselect,
    IOnBuildingOver,
    IOnBuildingExit,
    IOnBuildingTimedStart,
    IOnBuildingTimedEnd,
    IOnSetPreview
{
    public void OnPlace(GameObject gameObject)
    {
        Debug.Log($"{gameObject.name} has been placed");
    }
    public void OnRemove(GameObject gameObject)
    {
        Debug.Log($"{gameObject.name} has been removed");
    }
    public void OnMoveStart(GameObject gameObject)
    {
        Debug.Log($"Started moving {gameObject.name}");
    }
    public void OnMoveEnd(GameObject gameObject)
    {
        Debug.Log($"Finished moving {gameObject.name}");
    }
    public void OnSelect(GameObject gameObject)
    {
        Debug.Log($"{gameObject.name} selected");
    }
    public void OnDeselect(GameObject gameObject)
    {
        Debug.Log($"{gameObject.name} deselected");
    }
    public void OnBuildingOver(GameObject gameObject)
    {
        Debug.Log($"{gameObject.name} over");
    }
    public void OnBuildingExit(GameObject gameObject)
    {
        Debug.Log($"{gameObject.name} exit");
    }
    public void OnTimerStart(GameObject gameObject)
    {
        Debug.Log($"Timer started on temp object {gameObject.name}");
    }
    public void OnTimerEnd()
    {
        Debug.Log("Timer ended");
    }
    public void OnPreview(GameObject prefab)
    {
        Debug.Log($"This {prefab} holds a copy of the prefab to place");
    }
}

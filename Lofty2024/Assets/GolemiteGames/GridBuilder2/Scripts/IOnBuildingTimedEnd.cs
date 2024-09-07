using UnityEngine;

public interface IOnBuildingTimedEnd
{
    /// <summary>
    /// Called when a building with a timer has finished via the ObjectPlacer class
    /// Note: The timed object at this point is now destroyed. The IOnBuildingPlace OnPlace() method is now also called at this point
    /// </summary>
    public void OnTimerEnd();
}

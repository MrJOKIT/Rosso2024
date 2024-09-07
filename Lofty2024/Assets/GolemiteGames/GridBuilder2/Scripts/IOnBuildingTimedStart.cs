using UnityEngine;

public interface IOnBuildingTimedStart 
{
    /// <summary>
    /// Called when a building with a timer is placed via the ObjectPlacer class - 
    /// Note: Should be used sparingly as the timed object gets destroyed once the timer is finished
    /// </summary>
    /// <param name="gameObject"></param>
    public void OnTimerStart(GameObject gameObject);
}


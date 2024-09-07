using System.Collections.Generic;
using UnityEngine;

/*************This class is essientially a data class for storing a GridSquares placed object information. It does this per grid.**************/
[System.Serializable]
public class GridData
{
    public List<string> GridId = new List<string>();

    //Initialises a lot of lists containing data per placed object
    public List<string> objectName = new List<string>();
    public List<int> prefabID = new List<int>();
    public List<string> instanceID = new List<string>();

    public List<float> xPositions = new List<float>();
    public List<float> yPositions = new List<float>();
    public List<float> zPositions = new List<float>();
    public List<float> xOffsetPositions = new List<float>();
    public List<float> yOffsetPositions = new List<float>();
    public List<float> zOffsetPositions = new List<float>();
    public List<float> xOriginalOffsetPos = new List<float>();
    public List<float> yOriginalOffsetPos = new List<float>();
    public List<float> zOriginalOffsetPos = new List<float>();

    public List<float> rotationY = new List<float>();

    public List<int> amountOfCheckPositions = new List<int>();
    public List<float> xcheckPositions = new List<float>();
    public List<float> ycheckPositions = new List<float>();
    public List<float> zcheckPositions = new List<float>();

    public List<bool> selectChecks = new List<bool>();

    public List<bool> moveOnPoints = new List<bool>();
    public List<int> buildTime = new List<int>();
    public List<int> buildTimeRemaining = new List<int>();
    public List<int> upgradeLevel = new List<int>();

    public GridData (GridSquare gridSquare)
    {
        Transform objContainer = gridSquare.transform.Find("GridObjContainer");
        foreach (Transform child in objContainer)
        {
            GridObject.Data objData = child.GetComponent<GridObject>().data;

            objectName.Add(objData.ObjName);
            prefabID.Add(objData.PrefabId);
            instanceID.Add(objData.InstanceId);
            xPositions.Add(objData.Position.x);
            yPositions.Add(objData.Position.y);
            zPositions.Add(objData.Position.z);
            xOffsetPositions.Add(objData.Offset.x);
            yOffsetPositions.Add(objData.Offset.y);
            zOffsetPositions.Add(objData.Offset.z);
            xOriginalOffsetPos.Add(objData.OriginalOffset.x);
            yOriginalOffsetPos.Add(objData.OriginalOffset.y);
            zOriginalOffsetPos.Add(objData.OriginalOffset.z);
            rotationY.Add(objData.Rotation);

            foreach(Vector3 pos in objData.CheckPositions)
            {
                xcheckPositions.Add(pos.x);
                ycheckPositions.Add(pos.y);
                zcheckPositions.Add(pos.z);
            }

            amountOfCheckPositions.Add(objData.CheckPositions.Count);

            foreach (System.Reflection.FieldInfo prop in objData.ObjSize.GetType().GetFields())
            {
                selectChecks.Add((bool)prop.GetValue(objData.ObjSize));
            }

            moveOnPoints.Add(objData.MoveOnPoints);

            buildTime.Add(objData.BuildTime);

            upgradeLevel.Add(objData.UpgradeLevel);



            //If the object has remaining time, get it and add it to the data for use on load
            Timer timer = child.transform.GetComponentInChildren<Timer>();
            if(timer)
            {
                //We send the same time remaining in, but have to use different methods because of timeRemaining works in Timer.cs
                if(timer.countMethod == Timer.CountMethod.CountDown)
                {
                    buildTimeRemaining.Add((int)timer.timeRemaining);
                }
                else
                {
                    buildTimeRemaining.Add((int)timer.ReturnTotalSeconds() - (int)timer.timeRemaining);
                }
            }
            else
            {
                //Already fully built
                buildTimeRemaining.Add(0);
            }
        }      
    }
}

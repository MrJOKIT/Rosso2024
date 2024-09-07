using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/*************This class contains the prefab data you are going to place such as its size, build time and type ID**************/
public class Building : MonoBehaviour
{
    //1st row
    bool tempPos_m2_p2;
    bool tempPos_m1_p2;
    bool tempPos_0_p2; 
    bool tempPos_p1_p2;
    bool tempPos_p2_p2;

    //2nd row
    bool tempPos_m2_p1;
    bool tempPos_m1_p1;
    bool tempPos_0_p1; 
    bool tempPos_p1_p1;
    bool tempPos_p2_p1;

    //3rd row
    bool tempPos_m2_0;
    bool tempPos_m1_0;
    bool tempPos_0_0;  
    bool tempPos_p1_0;
    bool tempPos_p2_0; 

    //4th row
    bool tempPos_m2_m1;
    bool tempPos_m1_m1;
    bool tempPos_0_m1; 
    bool tempPos_p1_m1;
    bool tempPos_p2_m1;

    //5th row
    bool tempPos_m2_m2;
    bool tempPos_m1_m2;
    bool tempPos_0_m2; 
    bool tempPos_p1_m2;
    bool tempPos_p2_m2;

    [SerializeField] int prefabId;
    [SerializeField] bool moveOnPoints;
    [Min(0)]
    [SerializeField] int buildTime = 0;

    //The naming of these variables is important as it relates to the position from the center. pos = position, m = -, p = +. 
    //For example pos_m1_p1 is 1 cell left from center on the x axis and 1 cell up on the z axis. 
    [System.Serializable]
    public class ObjectSize
    {
        //1st row
        [SerializeField] public bool pos_m2_p2 = false;
        [SerializeField] public bool pos_m1_p2 = false;
        [SerializeField] public bool pos_0_p2 = false;
        [SerializeField] public bool pos_p1_p2 = false;
        [SerializeField] public bool pos_p2_p2 = false;

        //2nd row
        [SerializeField] public bool pos_m2_p1 = false;
        [SerializeField] public bool pos_m1_p1 = false;
        [SerializeField] public bool pos_0_p1 = false;
        [SerializeField] public bool pos_p1_p1 = false;
        [SerializeField] public bool pos_p2_p1 = false;

        //3rd row
        [SerializeField] public bool pos_m2_0 = false;
        [SerializeField] public bool pos_m1_0 = false;
        [SerializeField] public bool pos_0_0 = true;
        [SerializeField] public bool pos_p1_0 = false;
        [SerializeField] public bool pos_p2_0 = false;

        //4th row
        [SerializeField] public bool pos_m2_m1 = false;
        [SerializeField] public bool pos_m1_m1 = false;
        [SerializeField] public bool pos_0_m1 = false;
        [SerializeField] public bool pos_p1_m1 = false;
        [SerializeField] public bool pos_p2_m1 = false;

        //5th row
        [SerializeField] public bool pos_m2_m2 = false;
        [SerializeField] public bool pos_m1_m2 = false;
        [SerializeField] public bool pos_0_m2 = false;
        [SerializeField] public bool pos_p1_m2 = false;
        [SerializeField] public bool pos_p2_m2 = false;

        //Returns the total number of bools for the object
        public int GetNum()
        {
            int num = this.GetType().GetFields().Length;
            return num;
        }
    }
    [SerializeField] private ObjectSize objectSize;
    public ObjectSize objSize
    {
        get { return objectSize; }
        set { objectSize = value; }
    }
    public int PrefabId
    {
        get { return prefabId; }
    }
    public bool MoveOnPoints
    {
        get { return moveOnPoints; }
    }
    public int BuildTime
    {
        get { return buildTime; }
    }

    //Saves all the original start positions to revert back to after placement
    public void SaveStartChecks()
    {
        //1st row
        tempPos_m2_p2 = objectSize.pos_m2_p2;
        tempPos_m1_p2 = objectSize.pos_m1_p2;
        tempPos_0_p2 = objectSize.pos_0_p2;
        tempPos_p1_p2 = objectSize.pos_p1_p2;
        tempPos_p2_p2 = objectSize.pos_p2_p2;

        //2nd row
        tempPos_m2_p1 = objectSize.pos_m2_p1;
        tempPos_m1_p1 = objectSize.pos_m1_p1;
        tempPos_0_p1 = objectSize.pos_0_p1;
        tempPos_p1_p1 = objectSize.pos_p1_p1;
        tempPos_p2_p1 = objectSize.pos_p2_p1;

        //3rd row
        tempPos_m2_0 = objectSize.pos_m2_0;
        tempPos_m1_0 = objectSize.pos_m1_0;
        tempPos_0_0 = objectSize.pos_0_0;
        tempPos_p1_0 = objectSize.pos_p1_0;
        tempPos_p2_0 = objectSize.pos_p2_0;

        //4th row
        tempPos_m2_m1 = objectSize.pos_m2_m1;
        tempPos_m1_m1 = objectSize.pos_m1_m1;
        tempPos_0_m1 = objectSize.pos_0_m1;
        tempPos_p1_m1 = objectSize.pos_p1_m1;
        tempPos_p2_m1 = objectSize.pos_p2_m1;

        //5th row
        tempPos_m2_m2 = objectSize.pos_m2_m2;
        tempPos_m1_m2 = objectSize.pos_m1_m2;
        tempPos_0_m2 = objectSize.pos_0_m2;
        tempPos_p1_m2 = objectSize.pos_p1_m2;
        tempPos_p2_m2 = objectSize.pos_p2_m2;
    }

    //Resets bools back to the saved values
    public void ResetStartChecks()
    {
        //1st row
        objectSize.pos_m2_p2 = tempPos_m2_p2;
        objectSize.pos_m1_p2 = tempPos_m1_p2;
        objectSize.pos_0_p2 = tempPos_0_p2;
        objectSize.pos_p1_p2 = tempPos_p1_p2;
        objectSize.pos_p2_p2 = tempPos_p2_p2;

        //2nd row
        objectSize.pos_m2_p1 = tempPos_m2_p1;
        objectSize.pos_m1_p1 = tempPos_m1_p1;
        objectSize.pos_0_p1 = tempPos_0_p1;
        objectSize.pos_p1_p1 = tempPos_p1_p1;
        objectSize.pos_p2_p1 = tempPos_p2_p1;

        //3rd row
        objectSize.pos_m2_0 = tempPos_m2_0;
        objectSize.pos_m1_0 = tempPos_m1_0;
        objectSize.pos_0_0 = tempPos_0_0;
        objectSize.pos_p1_0 = tempPos_p1_0;
        objectSize.pos_p2_0 = tempPos_p2_0;

        //4th row
        objectSize.pos_m2_m1 = tempPos_m2_m1;
        objectSize.pos_m1_m1 = tempPos_m1_m1;
        objectSize.pos_0_m1 = tempPos_0_m1;
        objectSize.pos_p1_m1 = tempPos_p1_m1;
        objectSize.pos_p2_m1 = tempPos_p2_m1;

        //5th row
        objectSize.pos_m2_m2 = tempPos_m2_m2;
        objectSize.pos_m1_m2 = tempPos_m1_m2;
        objectSize.pos_0_m2 = tempPos_0_m2;
        objectSize.pos_p1_m2 = tempPos_p1_m2;
        objectSize.pos_p2_m2 = tempPos_p2_m2;
    }


}


using System.Collections.Generic;
using UnityEngine;

/*************This class can be called from anywhere to retrieve all objects of a certain type(set by prefab id) 
 * or any single object by string instance ID**************/
public class ObjectStorage
{
    //Initiates two collections in which to find all of the placed GameObjects across all grids
    static public Dictionary<int, List<GameObject>> GOTypeList = new Dictionary<int, List<GameObject>>();
    static public Dictionary<string, GameObject> GOInstanceList = new Dictionary<string, GameObject>();

    public static void AddTypeObject(int prefabId, GameObject obj)
    {
        List<GameObject> GOList;
        //Finds a gameobject already in the list
        if(GOTypeList.TryGetValue(prefabId, out GOList))
        {
            GOList.Add(obj);
        }
        //Creates a new list
        else
        {
            List<GameObject> newList = new List<GameObject>();
            newList.Add(obj);
            GOTypeList.Add(prefabId, newList);
        }
    }
    public static void RemoveTypeObject(int prefabId, GameObject obj)
    {
        List<GameObject> GOList;
        //Finds a gameobject already in the list
        if (GOTypeList.TryGetValue(prefabId, out GOList))
        {
            GOList.Remove(obj);
        }
    }

    //After setting a prefab ID on the SelectObject component you can then filter and find all of those objects placed
    public static List<GameObject> GetObjectsOfType(int prefabId)
    {
        List<GameObject> GOList;
        GOTypeList.TryGetValue(prefabId, out GOList);
        return GOList;
    } 
}

using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using UnityEditor;
/*************This class is the basic save and load system, these functions are used from the GridSquare component**************/
public static class SaveLoadGrid
{
   //Saves the data to the devices 'save' location
   public static void SaveCurrentGrid (GridSquare gridSquare)
    {
        string id = gridSquare.Id;
        BinaryFormatter bf = new BinaryFormatter();

        string path = $"{Application.persistentDataPath}/{gridSquare.gameObject.scene.name}";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string filePath = $"{Application.persistentDataPath}/{gridSquare.gameObject.scene.name}/{gridSquare.name.Replace(" ", string.Empty)}-{id}.data";

        FileStream fileStream = new FileStream(filePath, FileMode.Create);

        GridData gridData = new GridData(gridSquare);

        bf.Serialize(fileStream, gridData);
        fileStream.Close();

        Debug.Log("Save succesful");
    }

    //Loads the data from the devices 'save' location
    public static GridData LoadCurrentGrid(GridSquare gridSquare)
    {
        string id = gridSquare.Id;
        string filePath = $"{Application.persistentDataPath}/{gridSquare.gameObject.scene.name}/{gridSquare.name.Replace(" ", string.Empty)}-{id}.data";
        string configFilePath = $"{Application.dataPath}/GolemiteGames/GridBuilder2/PreConfig/{gridSquare.gameObject.scene.name}/{gridSquare.name.Replace(" ", string.Empty)}-{id}.preconfig";

        GridData gridData;

        //If there is saved data, load it
        if (File.Exists(filePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fileStream = new FileStream(filePath, FileMode.Open);

            gridData = bf.Deserialize(fileStream) as GridData;
            fileStream.Close();

            return gridData;
        }
        //If there is no saved data
        //Look for a config file
        else if(File.Exists(configFilePath))
        {
            //Debug.Log("File does not exist in " + filePath);
            //Load it
            gridData = LoadPreconfiguration(gridSquare);

            return gridData;
        }
        else
        {
            return null;
        }
    }
    
    //If data exists, delete it
    public static void DeleteCurrentGridSaveData(GridSquare gridSquare)
    {
        string id = gridSquare.Id;
        string filePath = $"{Application.persistentDataPath}/{gridSquare.gameObject.scene.name}/{gridSquare.name.Replace(" ", string.Empty)}-{id}.data";

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        else
        {
            Debug.Log("No save data exists for " + gridSquare);
        }
    }

    public static void SavePreconfiguration(GridSquare gridSquare)
    {
        string id = gridSquare.Id;
        if (!Application.isPlaying)
        {
            Debug.Log("You need to be in play mode to save configuration files");
            return;
        }

        BinaryFormatter bf = new BinaryFormatter();

        string path = $"{Application.dataPath}/GolemiteGames/GridBuilder2/PreConfig/{gridSquare.gameObject.scene.name}";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string filePath = $"{Application.dataPath}/GolemiteGames/GridBuilder2/PreConfig/{gridSquare.gameObject.scene.name}/{gridSquare.name.Replace(" ", string.Empty)}-{id}.preconfig";
        Debug.Log(filePath);
        FileStream fileStream = new FileStream(filePath, FileMode.Create);

        GridData gridData = new GridData(gridSquare);

        bf.Serialize(fileStream, gridData);
        
        fileStream.Close();

        SaveScreenShot(gridSquare);

        Debug.Log("Configuration saved");
    }

    public static GridData LoadPreconfiguration(GridSquare gridSquare)
    {
        string id = gridSquare.Id;

        if (!Application.isPlaying)
        {
            Debug.Log("You need to be in play mode to load configuration files");
            return null;
        }
        string configFilePath = $"{Application.dataPath}/GolemiteGames/GridBuilder2/PreConfig/{gridSquare.gameObject.scene.name}/{gridSquare.name.Replace(" ", string.Empty)}-{id}.preconfig";

        GridData gridData;

        if (File.Exists(configFilePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fileStream = new FileStream(configFilePath, FileMode.Open);

            gridData = bf.Deserialize(fileStream) as GridData;
            fileStream.Close();

            return gridData;
        }
        else
        {
            Debug.Log($"No config file found at {configFilePath}");
            return null;
        }
    }

    //Delete the config for the grid for that scene if it exists
    public static void DeletePreconfiguration(GridSquare gridSquare)
    {
        string id = gridSquare.Id;

        string filePath = $"{Application.dataPath}/GolemiteGames/GridBuilder2/PreConfig/{gridSquare.gameObject.scene.name}/{gridSquare.name.Replace(" ", string.Empty)}-{id}.preconfig";
        string meta = $"{Application.dataPath}/GolemiteGames/GridBuilder2/PreConfig/{gridSquare.gameObject.scene.name}/{gridSquare.name.Replace(" ", string.Empty)}-{id}.preconfig.meta";

        if (File.Exists(filePath))
        {
            Debug.Log($"Deleted {filePath}");
            File.Delete(filePath);
            File.Delete(meta);
        }
        else
        {
            Debug.Log("No config data exists for " + gridSquare);
        }
    }

    public static void SaveScreenShot(GridSquare gridSquare)
    {
        string id = gridSquare.Id;

        string path = $"{Application.dataPath}/GolemiteGames/GridBuilder2/Editor/Resources/PreConfigScreenshots";

        if(!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string filePath = $"{Application.dataPath}/GolemiteGames/GridBuilder2/Editor/Resources/PreConfigScreenshots/{gridSquare.name.Replace(" ", string.Empty)}-{id}.png";

        ScreenCapture.CaptureScreenshot(filePath);
    }

    public static void RemoveScreenShot(GridSquare gridSquare)
    {
        string id = gridSquare.Id;

        string filePath = $"{Application.dataPath}/GolemiteGames/GridBuilder2/Editor/Resources/PreConfigScreenshots/{gridSquare.name.Replace(" ", string.Empty)}-{id}.png";
        string meta = $"{Application.dataPath}/GolemiteGames/GridBuilder2/Editor/Resources/PreConfigScreenshots/{gridSquare.name.Replace(" ", string.Empty)}-{id}.png.meta";
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            File.Delete(meta);
        }
    }

    public static GridCreationData CreateGridCreationData(GridSquare gridSquare)
    {
        GridCreationData data = ScriptableObject.CreateInstance<GridCreationData>();
        data.Initialise(gridSquare.GridPoints, gridSquare.Cells, gridSquare.gridCellsStatus);

        string path = $"Assets/GolemiteGames/GridBuilder2/Resources/GridCreationData/{gridSquare.gameObject.scene.name}/{gridSquare.name}-{gridSquare.Id}".Replace(" ", string.Empty);
        string folderPath = $"Assets/GolemiteGames/GridBuilder2/Resources/GridCreationData/{gridSquare.gameObject.scene.name}".Replace(" ", string.Empty);
        if (string.IsNullOrEmpty(path)) return null;

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        path += ".asset";

#if UNITY_EDITOR
        AssetDatabase.CreateAsset(data, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
#endif
        if (File.Exists(path))
        {
            Debug.Log("Created Grid Creation Data");
        }

        string loadPath = $"GridCreationData/{gridSquare.gameObject.scene.name}/{gridSquare.name}-{gridSquare.Id}".Replace(" ", string.Empty);
        GridCreationData gridCreationData = Resources.Load<GridCreationData>(loadPath);

        return gridCreationData;
    }
}

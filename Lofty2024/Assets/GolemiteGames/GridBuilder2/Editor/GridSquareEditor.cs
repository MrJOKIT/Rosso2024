using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

[CanEditMultipleObjects, CustomEditor(typeof(GridSquare))]
public class GridSquareEditor : Editor
{
    GUIStyle configImageStyle;
    float configImageSize = 200f;
    string path;
    string savePath; 
    string configPath; 
    Texture2D configImage;
    GUIContent content = new GUIContent();

    Vector3[] meshVertices;
    int[] meshTriangles;
    Mesh mesh;
    float currentRadius;

    int lastNode;

    GameObject container;
    GameObject tile;

    Material greenMat;
    Material redMat;
    Material standardMat;

    GridSquare grid;

    static float transparency = 120f;
    static bool created;
    static bool generalSettings = true;
    static bool saveSettings = true;
    static bool autoCellBlock = true;
    static bool autoUpdate = false;
    static bool prevAutoUpdate;
    static bool prevCreated;
    static bool prefabs = true;
    static bool debug = false;
    static bool configImageCreated = false;
    static bool playing = false;

    public SerializedProperty
        editorExtension_Prop,
        visualOnly_Prop,
        gridWidth_Prop,
        gridHeight_Prop,
        cellSize_Prop,
        gridType_Prop,
        pointRadius_Prop,
        linesThickness_Prop,
        id_Prop,
        autoSaveInterval_Prop,
        saveGridOnExit_Prop,
        loadSaveOnStart_Prop,
        loadConfigOnStart_Prop,
        drawSimple_Prop,
        checkMatRuntime_Prop,
        tileX_Prop,
        tileY_Prop,
        autoCellBlocking_Prop,
        blocktype_Prop,
        showAboveBoxColliders_Prop,
        showBelowRays_Prop,
        checkGroundHits_Prop,
        aboveCheckBoxSize_Prop,
        aboveCheckBoxHeight_Prop,
        checkBoxOffset_Prop,
        groundLayer_Prop,
        ignoreLayers_Prop,
        groundDistance_Prop,
        gridCellPrefab_Prop,
        secondGridCellPrefab_Prop,
        blockedAboveCellPrefab_Prop,
        blockedBelowCellPrefab_Prop,
        pointsPrefab_Prop,
        drawGridPositions_Prop,
        drawCellPositions_Prop;

    public void OnEnable()
    {
        editorExtension_Prop = serializedObject.FindProperty("editorExtension");
        visualOnly_Prop = serializedObject.FindProperty("visualOnly");
        gridWidth_Prop = serializedObject.FindProperty("gridWidth");
        gridHeight_Prop = serializedObject.FindProperty("gridHeight");
        cellSize_Prop = serializedObject.FindProperty("cellSize");
        gridType_Prop = serializedObject.FindProperty("gridType");
        pointRadius_Prop = serializedObject.FindProperty("pointRadius");
        linesThickness_Prop = serializedObject.FindProperty("linesThickness");
        autoSaveInterval_Prop = serializedObject.FindProperty("autoSaveInterval");
        saveGridOnExit_Prop = serializedObject.FindProperty("saveGridOnExit");
        loadSaveOnStart_Prop = serializedObject.FindProperty("loadSaveOnStart");
        id_Prop = serializedObject.FindProperty("id");
        loadConfigOnStart_Prop = serializedObject.FindProperty("loadConfigOnStart");
        drawSimple_Prop = serializedObject.FindProperty("drawSimple");
        checkMatRuntime_Prop = serializedObject.FindProperty("checkMatRuntime");
        tileX_Prop = serializedObject.FindProperty("tileX");
        tileY_Prop = serializedObject.FindProperty("tileY");
        autoCellBlocking_Prop = serializedObject.FindProperty("autoCellBlocking");
        blocktype_Prop = serializedObject.FindProperty("blocktype");
        ignoreLayers_Prop = serializedObject.FindProperty("ignoreLayers");
        groundLayer_Prop = serializedObject.FindProperty("groundLayer");
        groundDistance_Prop = serializedObject.FindProperty("groundDistance");
        aboveCheckBoxSize_Prop = serializedObject.FindProperty("aboveCheckBoxSize");
        aboveCheckBoxHeight_Prop = serializedObject.FindProperty("aboveCheckBoxHeight");
        checkBoxOffset_Prop = serializedObject.FindProperty("checkBoxOffset");
        showAboveBoxColliders_Prop = serializedObject.FindProperty("showAboveBoxColliders");
        showBelowRays_Prop = serializedObject.FindProperty("showBelowRays");
        checkGroundHits_Prop = serializedObject.FindProperty("checkGroundHits");
        gridCellPrefab_Prop = serializedObject.FindProperty("gridCellPrefab");
        secondGridCellPrefab_Prop = serializedObject.FindProperty("secondGridCellPrefab");
        blockedAboveCellPrefab_Prop = serializedObject.FindProperty("blockedAboveCellPrefab");
        blockedBelowCellPrefab_Prop = serializedObject.FindProperty("blockedBelowCellPrefab");
        pointsPrefab_Prop = serializedObject.FindProperty("pointsPrefab");
        drawGridPositions_Prop = serializedObject.FindProperty("drawGridPositions");
        drawCellPositions_Prop = serializedObject.FindProperty("drawCellPositions");
        currentRadius = pointRadius_Prop.floatValue;

        grid = target as GridSquare;

        CalcGridCreationStatus();

        savePath = $"{Application.persistentDataPath}/{SceneManager.GetActiveScene().name}/{grid.name.Replace(" ", string.Empty)}-{grid.Id}.data";
        configPath = $"{Application.dataPath}/GolemiteGames/GridBuilder2/PreConfig/{SceneManager.GetActiveScene().name}/{grid.name.Replace(" ", string.Empty)}-{grid.Id}.preconfig";

        if (prevAutoUpdate)
        {
            autoUpdate = true;
        }

        if (grid.gameObject.activeSelf)
        {
            if (!Application.isPlaying)
            {
                if (prevCreated)
                {
                    CreateMaterials();
                }
            }
        }

        path = $"PreConfigScreenshots/{grid.name.Replace(" ", string.Empty)}-{grid.Id}";
        configImage = (Texture2D)Resources.Load(path);
        if (configImage)
        {
            content.image = configImage;
        }
    }

    //Causing issues when on selecting the grid and then playing
    private void CalcGridCreationStatus()
    {
        if(grid)
        if (grid.GetComponentsInChildren<Transform>().Length > 1)
        {
            grid.Created = true;
        }
        else
        {
            grid.Created = false;
        }
    }

#if UNITY_EDITOR
    private void OnDisable()
    {
        if (!Application.isPlaying && !playing)
        {
            prevAutoUpdate = autoUpdate;
            prevCreated = created;
        }
        EditorUtility.ClearProgressBar();

        CalcGridCreationStatus();
    }
#endif
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        configImageStyle = new GUIStyle();
        configImageStyle.alignment = TextAnchor.UpperCenter;

        GUIStyle textLeftStyle = new GUIStyle();
        textLeftStyle.alignment = TextAnchor.MiddleLeft;

        if (Application.isPlaying && !playing)
        {
            if (autoUpdate)
            {
                prevAutoUpdate = true;
            }
            prevCreated = created;
            autoUpdate = false;
            RemovePreviewGrid();
            playing = true;
        }
        if(!Application.isPlaying && playing)
        {
            autoUpdate = prevAutoUpdate;
            playing = false;
            if (prevCreated)
            {
                created = true;
                if(grid.transform.childCount > 0)
                container = grid.transform.GetChild(0).gameObject;
                RemovePreviewGrid();
                CreateMaterials();
                CreateNewGridPreview();
            } 
        }
        if (autoUpdate && !playing)
        {
            UpdateExistingPreview();
        }

        generalSettings = EditorGUILayout.BeginFoldoutHeaderGroup(generalSettings, "General Settings");
        if (generalSettings)
        {
            EditorGUILayout.HelpBox("General settings for setting the size of the grid. \n" +
                "Set in Metres", MessageType.None);
            EditorGUILayout.PropertyField(editorExtension_Prop);
            if(editorExtension_Prop.boolValue)
            {
                
                bool cancelled = false;

                if (created)
                {
                    GUI.enabled = false;
                }
                EditorGUILayout.BeginHorizontal();
                if(grid.Created)
                {
                    GUI.enabled = false;
                }
                if(!grid.Created)
                {
                    if (GUILayout.Button("Create Grid", GUILayout.MinHeight(30)))
                    {
                        grid.StartGridCR();

                        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                    }
                }
                else
                {
                    if (GUILayout.Button("Grid Created", GUILayout.MinHeight(30)))
                    {
                    }
                }


                if(cancelled)
                {
                    DeleteCreatedGrid();
                }
                if (grid.Created)
                {
                    //EditorUtility.ClearProgressBar();
                }
                if (!created)
                {
                    GUI.enabled = true;
                }
                if (!grid.Created || Application.isPlaying)
                {
                    GUI.enabled = false;
                }
                if (GUILayout.Button("Delete Grid", GUILayout.MinHeight(30)))
                {
                    DeleteCreatedGrid();
                }
                if (!grid.Created)
                {
                    GUI.enabled = true;
                }
                if (grid.Created)
                {
                    GUI.enabled = false;
                }
                EditorGUILayout.EndHorizontal();
                GUI.enabled = false;




                GUI.enabled = true;
               
            }
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(visualOnly_Prop);
            if(EditorGUI.EndChangeCheck())
            {
                visualOnly_Prop.serializedObject.ApplyModifiedProperties();
            }
            if (grid.Created)
            {
                GUI.enabled = false;
            }
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(gridWidth_Prop);
            EditorGUILayout.PropertyField(gridHeight_Prop);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Node count");
            GUI.enabled = false;
            EditorGUILayout.IntField(gridWidth_Prop.intValue * gridHeight_Prop.intValue);
            GUI.enabled = true;
            if (grid.Created)
            {
                GUI.enabled = false;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(cellSize_Prop);
            if (EditorGUI.EndChangeCheck())
            {
                cellSize_Prop.serializedObject.ApplyModifiedProperties();
                gridWidth_Prop.serializedObject.ApplyModifiedProperties();
                gridHeight_Prop.serializedObject.ApplyModifiedProperties();
                gridType_Prop.serializedObject.ApplyModifiedProperties();
                if (autoUpdate && autoCellBlocking_Prop.boolValue)
                {
                    //Applies the latest changes to the properties before rebuilding the grid
                    RemovePreviewGrid();
                    CreateNewGridPreview();
                }
            }
            EditorGUILayout.PropertyField(gridType_Prop);
            GUI.enabled = true;
            if (gridType_Prop.enumValueIndex == 3)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Points Radius");
                EditorGUI.BeginChangeCheck();
                pointRadius_Prop.floatValue = EditorGUILayout.Slider(pointRadius_Prop.floatValue, 0, grid.CellSize * 4);
                //EditorGUILayout.PropertyField(pointRadius_Prop);
                if (EditorGUI.EndChangeCheck())
                {
                    pointRadius_Prop.serializedObject.ApplyModifiedProperties();
                    UpdatePointsRadius();
                }
                EditorGUILayout.EndHorizontal();
            }
            if (gridType_Prop.enumValueIndex == 4)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Lines Thickness");
                EditorGUI.BeginChangeCheck();
                linesThickness_Prop.floatValue = EditorGUILayout.Slider(linesThickness_Prop.floatValue, 0, grid.CellSize * 0.5f);
                //EditorGUILayout.PropertyField(linesThickness_Prop);
                if(EditorGUI.EndChangeCheck())
                {
                    linesThickness_Prop.serializedObject.ApplyModifiedProperties();
                    UpdateLinesThickness();
                }
                EditorGUILayout.EndHorizontal();
            }
            if(grid.Created)
            {
                GUI.enabled = false;
            }
            if (gridType_Prop.enumValueIndex == 2)
            {
                EditorGUILayout.HelpBox("Use this to tune the tiling of the material on the Simple grid type in play mode. \n" +
                    "Uncheck for final build", MessageType.None);
                EditorGUILayout.PropertyField(checkMatRuntime_Prop);
                EditorGUILayout.PropertyField(tileX_Prop);
                EditorGUILayout.PropertyField(tileY_Prop);
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space();

        saveSettings = EditorGUILayout.BeginFoldoutHeaderGroup(saveSettings, "Save/Load/Preconfiguration");
        if (saveSettings)
        {

            EditorGUILayout.HelpBox("This is what identifies each grid with the save file and also the preconfig file", MessageType.None);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("New", GUILayout.MaxWidth(50)))
            {
                id_Prop.stringValue = GUID.Generate().ToString().Substring(0, 8);
                id_Prop.serializedObject.ApplyModifiedProperties();
            }
            if (GUILayout.Button("Copy", GUILayout.MaxWidth(50)))
            {
                GUIUtility.systemCopyBuffer = id_Prop.stringValue;
            }
            if (GUILayout.Button("Paste", GUILayout.MaxWidth(50)))
            {
                id_Prop.stringValue = GUIUtility.systemCopyBuffer;
                id_Prop.serializedObject.ApplyModifiedProperties();
            }
            GUILayout.Label($"ID: {id_Prop.stringValue}");
            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
           
            EditorGUILayout.HelpBox("Autosave in minutes, if 0, does not perform any autosaving", MessageType.None);
            EditorGUILayout.PropertyField(autoSaveInterval_Prop);
            EditorGUILayout.HelpBox("If you save over a preconfig file, it will then always load the save, not the preconfig", MessageType.None);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(saveGridOnExit_Prop);
            if(File.Exists(savePath))
            {
                GUI.enabled = true;
            }
            else
            {
                GUI.enabled = false;
            }

            if (GUILayout.Button("Delete Save"))
            {
                SaveLoadGrid.DeleteCurrentGridSaveData(grid);
            }
            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();


            EditorGUILayout.HelpBox("You can either load a save, or a pre config at runtime", MessageType.None);
            if (loadConfigOnStart_Prop.boolValue)
            {
                GUI.enabled = false;
                loadSaveOnStart_Prop.boolValue = false;
            }
            EditorGUILayout.PropertyField(loadSaveOnStart_Prop);
            GUI.enabled = true;

            if (loadSaveOnStart_Prop.boolValue)
            {
                GUI.enabled = false;
                loadConfigOnStart_Prop.boolValue = false;
            }
            EditorGUILayout.PropertyField(loadConfigOnStart_Prop);
            GUI.enabled = true;

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            if (!Application.isPlaying)
            {
                GUI.enabled = false;
            }
            if (GUILayout.Button("Create Preconfiguration"))
            {
                SaveLoadGrid.RemoveScreenShot(grid);
                SaveLoadGrid.SavePreconfiguration(grid);
                SaveLoadGrid.SaveScreenShot(grid);

                configImageCreated = true;
            }
            if (configImageCreated)
            {
                AssetDatabase.Refresh();
                path = $"PreConfigScreenshots/{grid.name.Replace(" ", string.Empty)}-{grid.Id}";
                configImage = (Texture2D)Resources.Load(path);

                if (configImage)
                {
                    content.image = configImage;
                    configImageCreated = false;
                }
            }
            GUI.enabled = true;

            if (File.Exists(configPath))
            {
                GUI.enabled = true;
            } 
            else
            {
                GUI.enabled = false;
            }

            if (GUILayout.Button("Delete Preconfiguration"))
            {
                SaveLoadGrid.DeletePreconfiguration(grid);
                SaveLoadGrid.RemoveScreenShot(grid);
                AssetDatabase.Refresh();
                content.image = null;
            }
            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            if (content.image != null)
            {
                GUILayout.Label(content, configImageStyle, GUILayout.MaxHeight(configImageSize));
            }
            GUILayout.FlexibleSpace();
        }   
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space();

        if(grid.Created)
        {
            GUI.enabled = false;
        }

        if(gridType_Prop.enumValueIndex != 2)
        {
            autoCellBlock = EditorGUILayout.BeginFoldoutHeaderGroup(autoCellBlock, "Auto Cell Block");
            if (autoCellBlock)
            {
                EditorGUILayout.HelpBox("If enabled this will automatically block cells depending on settings", MessageType.None);
                EditorGUILayout.PropertyField(autoCellBlocking_Prop);

                if (autoCellBlocking_Prop.boolValue)
                {
                    GUI.enabled = true;
                    if (grid.Created)
                    {
                        GUI.enabled = false;
                    }
                    EditorGUILayout.HelpBox("Runs update continuously, careful with performance for large grids", MessageType.None);
                    EditorGUILayout.BeginHorizontal();
                    if (created)
                    {
                        GUI.enabled = true;
                        if (grid.Created)
                        {
                            GUI.enabled = false;
                        }
                        autoUpdate = EditorGUILayout.Toggle("Auto Update", autoUpdate);
                    }
                    else
                    {
                        GUI.enabled = false;
                        autoUpdate = EditorGUILayout.Toggle("Auto Update", autoUpdate);
                        GUI.enabled = true;
                    }
                    if (grid.Created)
                    {
                        GUI.enabled = false;
                    }
                    GUILayout.Label("Transparency");
                    EditorGUI.BeginChangeCheck();
                    transparency = EditorGUILayout.Slider(transparency, 0, 255f);

                    if (EditorGUI.EndChangeCheck()) 
                    {
                        //Debug.Log(greenMat);
                        Color32 tempGreenColor = greenMat.color;
                        tempGreenColor.a = (byte)transparency;
                        greenMat.color = tempGreenColor;
                        Color32 tempRedColor = redMat.color;
                        tempRedColor.a = (byte)transparency;
                        redMat.color = tempRedColor;
                    };
                    EditorGUILayout.EndHorizontal();


                    EditorGUILayout.BeginHorizontal();

                    if (Application.isPlaying) {
                        GUI.enabled = false;
                    }
                    if (prevCreated)
                    {
                        created = true;
                    }
                    if(grid.Created)
                    {
                        GUI.enabled = false;
                    }
                    if (!created)
                    {
                        if (GUILayout.Button("Create"))
                        {
                            CreateNewGridPreview();
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Update"))
                        {
                            RemovePreviewGrid();
                            CreateNewGridPreview();
                        }
                    }

                    if (GUILayout.Button("Remove"))
                    {
                        autoUpdate = false;
                        prevCreated = false;
                        RemovePreviewGrid();
                    }

                    GUI.enabled = true;
                    if (grid.Created)
                    {
                        GUI.enabled = false;
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(blocktype_Prop);
                    EditorGUILayout.PropertyField(ignoreLayers_Prop);
                    EditorGUILayout.PropertyField(groundLayer_Prop);
                    EditorGUILayout.PropertyField(groundDistance_Prop);
                    EditorGUILayout.PropertyField(aboveCheckBoxSize_Prop);
                    EditorGUILayout.PropertyField(aboveCheckBoxHeight_Prop);
                    EditorGUILayout.PropertyField(checkBoxOffset_Prop);
                    if (EditorGUI.EndChangeCheck())
                    {
                        blocktype_Prop.serializedObject.ApplyModifiedProperties();
                        ignoreLayers_Prop.serializedObject.ApplyModifiedProperties();
                        groundLayer_Prop.serializedObject.ApplyModifiedProperties();
                        groundDistance_Prop.serializedObject.ApplyModifiedProperties();
                        aboveCheckBoxSize_Prop.serializedObject.ApplyModifiedProperties();
                        aboveCheckBoxHeight_Prop.serializedObject.ApplyModifiedProperties();
                        checkBoxOffset_Prop.serializedObject.ApplyModifiedProperties();

                        if (autoUpdate)
                        {
                            UpdateExistingPreview();
                        }
                    }
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("Use these tools to get a visual look at what is being done, " +
                        "and to adjust your settings accordingly", MessageType.None);
                    EditorGUILayout.PropertyField(showAboveBoxColliders_Prop);
                    EditorGUILayout.PropertyField(showBelowRays_Prop);
                    EditorGUILayout.PropertyField(checkGroundHits_Prop);

                    GUI.enabled = true;
                    if (grid.Created)
                    {
                        GUI.enabled = false;
                    }
                }

            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        EditorGUILayout.Space();

        if (gridType_Prop.enumValueIndex != 2)
        {
            prefabs = EditorGUILayout.BeginFoldoutHeaderGroup(prefabs, "Prefabs");
            if (prefabs)
            {
                EditorGUILayout.HelpBox("Prefabs for creating the Single and Checkered grid types", MessageType.None);
                if(gridType_Prop.enumValueIndex != 3 && gridType_Prop.enumValueIndex != 4)
                {
                    EditorGUILayout.PropertyField(gridCellPrefab_Prop);
                    if (gridType_Prop.enumValueIndex == 1)
                    {
                        GUI.enabled = true;
                    }
                    else
                    {
                        GUI.enabled = false;
                    }
                    if (grid.Created)
                    {
                        GUI.enabled = false;
                    }
                    EditorGUILayout.PropertyField(secondGridCellPrefab_Prop);
                    GUI.enabled = true;
                    if (grid.Created)
                    {
                        GUI.enabled = false;
                    }
                    EditorGUILayout.PropertyField(blockedAboveCellPrefab_Prop);
                    EditorGUILayout.PropertyField(blockedBelowCellPrefab_Prop);
                }
                if(gridType_Prop.enumValueIndex == 3)
                {
                    EditorGUILayout.PropertyField(pointsPrefab_Prop);
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        EditorGUILayout.Space();

        debug = EditorGUILayout.BeginFoldoutHeaderGroup(debug, "Debug");
        if (debug)
        {
            EditorGUILayout.HelpBox("Shows the cell positions and point positions as they are stored, " +
                "turning this on for large grids will significantly impact performance", MessageType.None);
            EditorGUILayout.PropertyField(drawCellPositions_Prop);
            EditorGUILayout.PropertyField(drawGridPositions_Prop);
            EditorGUILayout.PropertyField(drawSimple_Prop);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        serializedObject.ApplyModifiedProperties();
    }

    private void UpdatePointsRadius()
    {
        if(grid.Created)
        {

            List<Transform> children = grid.transform.Find("PointsContainer").GetComponentsInChildren<Transform>().ToList();

            children.Remove(grid.transform.Find("PointsContainer"));

            foreach (Transform child in children)
            {
                Vector3 pointScale = child.transform.localScale;
                pointScale.x = pointRadius_Prop.floatValue;
                pointScale.z = pointRadius_Prop.floatValue;
                child.transform.localScale = pointScale;
                currentRadius = grid.PointRadius;
            }

        }
    }

    private void UpdateLinesThickness()
    {
        if(grid.Created)
        {
            List<MeshFilter> children = grid.transform.Find("LinesContainer").GetComponentsInChildren<MeshFilter>().ToList();

            for (int i = 0; i < children.Count; i++)
            {
                //Offset so the center of the lines is central on the cell, not on grid point
                Vector3 offset = new Vector3((-grid.CellSize * 0.5f), 0, (-grid.CellSize * 0.5f));

                //Calculates each vertex position of the lines
                Vector3[] newVertices = new Vector3[]
                {
                    new Vector3(linesThickness_Prop.floatValue, 0, linesThickness_Prop.floatValue) + offset,
                    new Vector3(-linesThickness_Prop.floatValue, 0, -linesThickness_Prop.floatValue) + offset,
                    new Vector3(-linesThickness_Prop.floatValue, 0, (grid.CellSize + linesThickness_Prop.floatValue)) + offset,
                    new Vector3(linesThickness_Prop.floatValue, 0, (grid.CellSize - linesThickness_Prop.floatValue)) + offset,
                    new Vector3((grid.CellSize + linesThickness_Prop.floatValue), 0, (grid.CellSize + linesThickness_Prop.floatValue)) + offset,
                    new Vector3((grid.CellSize - linesThickness_Prop.floatValue), 0, (grid.CellSize - linesThickness_Prop.floatValue)) + offset,
                    new Vector3((grid.CellSize + linesThickness_Prop.floatValue), 0, -linesThickness_Prop.floatValue) + offset,
                    new Vector3((grid.CellSize - linesThickness_Prop.floatValue), 0, linesThickness_Prop.floatValue) + offset
                };

                children[i].sharedMesh.vertices = newVertices;

            }
        }
    }

    private void DeleteCreatedGrid()
    {
        foreach (Transform child in grid.GetComponentsInChildren<Transform>(false))
        {
            if (child != null)
            {
                if (child.gameObject == grid.gameObject)
                {
                    continue;
                }
                DestroyImmediate(child.gameObject);
                autoUpdate = false;
                prevCreated = false;
                grid.Created = false;
                RemovePreviewGrid();
            }
        }

        if(grid.GetGridType == GridSquare.GridType.Simple)
        {
            grid.GetComponent<MeshFilter>().mesh = null;
        }

        grid.gridCellsStatus = null;
        grid.gridCells = null;
        grid.Cells = null;

        EditorUtility.ClearProgressBar();
        DestroyImmediate(grid.GetComponent<BoxCollider>());

        grid.GridCreationData = null;
        string loadPath = $"Assets/GolemiteGames/GridBuilder2/Resources/GridCreationData/{UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}/{grid.name}-{grid.Id}.asset".Replace(" ", string.Empty);
        if (System.IO.File.Exists(loadPath))
        {
            if (AssetDatabase.DeleteAsset(loadPath))
            {
                Debug.Log("Removed Grid Creation Data");
            }
            else
            {
                Debug.LogError($"Cannot delete, unknown error");
            }
        }
        else
        {
            Debug.LogError($"File does not exist in {loadPath}");
        }



        AssetDatabase.Refresh();
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());



        EditorGUIUtility.ExitGUI();
    }

    private void CreateNewGridPreview()
    {
        grid = target as GridSquare;
        grid.AutoCellBlockPreview = true;
        if (!created)
        {
            CreateAutoCellBlockPreview();
            created = true;
        }
    }

    private void UpdateExistingPreview()
    {
        grid = target as GridSquare;
        List<MeshRenderer> tempFreeTiles = new List<MeshRenderer>();
        List<MeshRenderer> tempBlockedTiles = new List<MeshRenderer>();

        if(grid.FreeCells.Count > 0)
        foreach (MeshRenderer tile in grid.FreeCells)
        {
                if(tile)
            if (RecheckCellBlock(tile.gameObject))
            {
                tempFreeTiles.Add(tile);
            }
            else
            {
                tempBlockedTiles.Add(tile);
            }
        }
        if(grid.BlockedCells.Count > 0)
        foreach (MeshRenderer tile in grid.BlockedCells)
        {
                if(tile)
            if (RecheckCellBlock(tile.gameObject))
            {
                tempFreeTiles.Add(tile);
            }
            else
            {
                tempBlockedTiles.Add(tile);
            }
        }

        grid.FreeCells = tempFreeTiles;
        grid.BlockedCells = tempBlockedTiles;


        foreach (MeshRenderer tile in grid.FreeCells)
        {
            tile.sharedMaterial = greenMat;
        }
        foreach (MeshRenderer tile in grid.BlockedCells)
        {
            tile.sharedMaterial = redMat;
        }
    }

    private void RemovePreviewGrid()
    {
        GridSquare grid = target as GridSquare;
        grid.AutoCellBlockPreview = false;
        if (grid)
        {
            if (grid.FreeCells.Count > 0)
            {
                grid.FreeCells.Clear();
            }
            if (grid.BlockedCells.Count > 0)
            {
                grid.BlockedCells.Clear();
            }
            if (grid.transform.childCount > 0 && grid.gameObject.transform.GetChild(0).gameObject.name == "PreviewContainer")
            {
                DestroyImmediate(grid.gameObject.transform.GetChild(0).gameObject);
            }
            else
            {
                //Nothing to remove
            }
        }
        created = false;
    }

  
    private void CreateAutoCellBlockPreview()
    {
        grid = target as GridSquare;

        grid.CreateGrid();
        grid.CreateGridCells();
        container = new GameObject("PreviewContainer");
        container.transform.parent = grid.transform;
        grid.CellContainer = container;

        //If you want to see the preview tiles, remove this line
        //container.hideFlags = HideFlags.HideInHierarchy;

        GameObject tile = CreatePreviewTile();

        for (int i = 0; i < grid.Cells.Length; i++)
        {
            grid.CreateAutoCellBlock(tile, i, true);
        }
        UpdateMaterial();

        DestroyImmediate(tile);
    }

    private GameObject CreatePreviewTile()
    {
        grid = target as GridSquare;

        tile = new GameObject("Tile");
        var tileMF = tile.AddComponent<MeshFilter>();
        var tileMR = tile.AddComponent<MeshRenderer>();
        tileMR.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        tileMR.receiveShadows = false;


        CreateMesh();
        tileMF.mesh = mesh;

        CreateMaterials();
        tileMR.sharedMaterial = standardMat;
        return tile;
    }

    private void CreateMaterials()
    {
        greenMat = new Material(Resources.Load("GridPreviewMat") as Material);
        greenMat.name = "AvailableMat";
        redMat = new Material(Resources.Load("GridPreviewMat") as Material);
        redMat.name = "BlockedMat";
        standardMat = new Material(Resources.Load("GridPreviewMat") as Material);

        Color32 green = new Color32(0, 255, 0, (byte)transparency);
        Color32 red = new Color32(255, 0, 0, (byte)transparency);

        greenMat.color = green;
        redMat.color = red;
        standardMat.color = Color.white - new Color32(0, 0, 0, (byte)transparency);

        UpdateRenderMode(greenMat);
        UpdateRenderMode(redMat);
        UpdateRenderMode(standardMat);
    }

    private void UpdateRenderMode(Material mat)
    {
        mat.SetFloat("_Mode", 3f);
        mat.SetOverrideTag("RenderType", "Transparent");
        mat.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.One);
        mat.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetFloat("_ZWrite", 0.0f);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.DisableKeyword("_ALPHABLEND_ON");
        mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
    }

    private void UpdateMaterial()
    {
        foreach (MeshRenderer tile in grid.FreeCells)
        {
            tile.sharedMaterial = greenMat;
        }
        foreach (MeshRenderer tile in grid.BlockedCells)
        {
            tile.sharedMaterial = redMat;
        }
    }

    //Builds and assigns the vertices and triangles to the mesh
    //Also updates the normals
    private void CreateMesh()
    {
        //Creates 4 vertices from your cellSize
        Vector3 offset = new Vector3(0.5f, 0, 0.5f);
        meshVertices = new Vector3[]
        {
            Vector3.zero - offset,
            new Vector3(0, 0, 1) - offset,
            new Vector3(1, 0, 1) - offset,
            new Vector3(1, 0, 0) - offset
        };
        //Creates the 2 triangles
        meshTriangles = new int[]
        {
            0, 1, 2,
            2, 3, 0
        };

        mesh = new Mesh();
        mesh.Clear();
        mesh.vertices = meshVertices;
        mesh.triangles = meshTriangles;
        mesh.name = "PreviewAutoCellTile";
        mesh.RecalculateNormals();
    }

    public bool RecheckCellBlock(GameObject prefab)
    {
        bool allClear = false;

        RaycastHit hit;
        Collider[] colliders;

        //Create four points from the centre of each cell
        float halfCell = grid.CellSize * 0.5f;
        Vector3 point1 = new Vector3(-halfCell, 0, halfCell);
        Vector3 point2 = new Vector3(halfCell, 0, halfCell);
        Vector3 point3 = new Vector3(halfCell, 0, -halfCell);
        Vector3 point4 = new Vector3(-halfCell, 0, -halfCell);

        //Checks the points
        bool point1Clear = CheckPoint(point1);
        bool point2Clear = CheckPoint(point2);
        bool point3Clear = CheckPoint(point3);
        bool point4Clear = CheckPoint(point4);
        bool boxClear = true;

        //This section filters for anything blocking above the grid
        Vector3 aboveCheckPos = prefab.transform.position + new Vector3(0, checkBoxOffset_Prop.floatValue + (aboveCheckBoxHeight_Prop.floatValue * 0.5f), 0);
        colliders = Physics.OverlapBox(aboveCheckPos, grid.GetCheckBoxSize());
        foreach (var item in colliders)
        {
            //Say this and if it is not this gridsquare
            if (!item.GetComponent<GridSquare>())
            {
                int layerInt = item.gameObject.layer;
                if (ignoreLayers_Prop.intValue != (ignoreLayers_Prop.intValue | (1 << layerInt)))
                {
                    boxClear = false;
                }
            }
        }

        //This function filters for any open space below the grid, thus not creating a cell there
        bool CheckPoint(Vector3 cellPoint)
        {
            if (Physics.Raycast(prefab.transform.position + cellPoint, Vector3.down, out hit, grid.GroundDistance, 1 << grid.GroundLayer))
            {
                //Need to stop the underlaying grid having effect
                if (hit.distance < grid.GroundDistance)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        //Only creates cells if not blocked above or below
        if (grid.Blocktype == GridSquare.BlockType.BlockBoth)
        {
            if (grid.GetGridType != GridSquare.GridType.Points && grid.GetGridType != GridSquare.GridType.Lines)
            {
                if (point1Clear && point2Clear && point3Clear && point4Clear && boxClear)
                {
                    allClear = true;
                }
                else
                {
                    allClear = false;
                }
            }
        }

        //Only creates cells if not blocked above
        if (grid.Blocktype == GridSquare.BlockType.BlockAbove)
        {
            if (grid.GetGridType != GridSquare.GridType.Points && grid.GetGridType != GridSquare.GridType.Lines)
            {
                if (boxClear)
                {
                    allClear = true;
                }
                else
                {
                    allClear = false;
                }
            }
        }

        //Only creates cells if there is no empty space below
        if (grid.Blocktype == GridSquare.BlockType.BlockBelow)
        {
            if (grid.GetGridType != GridSquare.GridType.Points && grid.GetGridType != GridSquare.GridType.Lines)
            {
                if (point1Clear && point2Clear && point3Clear && point4Clear)
                {
                    allClear = true;
                }
                else
                {
                    allClear = false;
                }
            }
        }
        return allClear;
    }
}

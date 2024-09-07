using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ObjectPlacer))]
public class ObjectPlacerEditor : Editor
{
    static bool buildTimeOptions;
    static bool generalSettings = true;

    public SerializedProperty
        placementType_Prop,
        placementHeight_Prop,
        overwrite_Prop,
        hideCellsUnderPlacedObj_Prop,
        buildMaterial_Prop,
        timerCanvas_Prop,
        canvasHeight_Prop;


    
    public void OnEnable()
    {
        placementType_Prop = serializedObject.FindProperty("placementType");
        placementHeight_Prop = serializedObject.FindProperty("placementHeight");
        overwrite_Prop = serializedObject.FindProperty("overwrite");
        hideCellsUnderPlacedObj_Prop = serializedObject.FindProperty("hideCellsUnderPlacedObj");
        buildMaterial_Prop = serializedObject.FindProperty("buildMaterial");
        timerCanvas_Prop = serializedObject.FindProperty("timerCanvas");
        canvasHeight_Prop = serializedObject.FindProperty("canvasHeight");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        generalSettings = EditorGUILayout.BeginFoldoutHeaderGroup(generalSettings, "General Settings");
        if (generalSettings)
        {
            EditorGUILayout.HelpBox("Method of object placement", MessageType.None);
            EditorGUILayout.HelpBox("For drag and drop you will need to use SelectObject buttons", MessageType.None);
            EditorGUILayout.PropertyField(placementType_Prop);
            EditorGUILayout.HelpBox("Height above your ground object, if using the grid as ground, how high above the grid", MessageType.None);
            EditorGUILayout.PropertyField(placementHeight_Prop);
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("This will allow you to place objects on top of one another, removing the object it was placed on", MessageType.None);
            EditorGUILayout.HelpBox("The placement type cannot be drag build and you will need an object remover", MessageType.None);
            if (!FindObjectOfType<ObjectRemover>() || placementType_Prop.enumValueIndex == 2)
            {
                GUI.enabled = false;
                overwrite_Prop.boolValue = false;
            }
            EditorGUILayout.PropertyField(overwrite_Prop);
            GUI.enabled = true;
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("This will not work for Simple, Lines, Points or Visual only grids", MessageType.None);
            EditorGUILayout.PropertyField(hideCellsUnderPlacedObj_Prop);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space();

        buildTimeOptions = EditorGUILayout.BeginFoldoutHeaderGroup(buildTimeOptions, "Building Time Settings");
        if (buildTimeOptions)
        {
            EditorGUILayout.HelpBox("Insert a canvas with a timer or something to display while building", MessageType.None);
            EditorGUILayout.PropertyField(timerCanvas_Prop);
            EditorGUILayout.PropertyField(buildMaterial_Prop);
            EditorGUILayout.PropertyField(canvasHeight_Prop);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        serializedObject.ApplyModifiedProperties();
    }
}


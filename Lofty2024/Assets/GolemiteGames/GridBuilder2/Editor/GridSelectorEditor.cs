using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridSelector))]
public class GridSelectorEditor : Editor
{
    static bool General = true;
    static bool GridSelectorTile = false;
    static bool InvalidPlacementFeedback = false;

    public SerializedProperty
        smoothMove_Prop,
        moveSpeed_Prop,
        rotation_Prop,
        smoothRotate_Prop,
        rotateSpeed_Prop,
        previewObjHoverDistance_Prop,
        hideSelectorOnPreview_Prop,
        previewObjFloorTiles_Prop,
        previewObjFloorTilePrefab_Prop,
        placeTilesWithObject_Prop,
        usePreviewMatsForFloorTiles_Prop,
        disableAnimationForPreviewObj_Prop,
        hoverDistance_Prop,
        dragBuild_Prop,
        invalidPlacementFeedback_Prop,
        showInvalidPreviewObj_Prop,
        invalidPlacementMat_Prop,
        objPreviewMat_Prop,
        changeMatColorNotMat_Prop;

    public void OnEnable()
    {
        smoothMove_Prop = serializedObject.FindProperty("smoothMove");
        moveSpeed_Prop = serializedObject.FindProperty("moveSpeed");
        rotation_Prop = serializedObject.FindProperty("rotation");
        smoothRotate_Prop = serializedObject.FindProperty("smoothRotate");
        rotateSpeed_Prop = serializedObject.FindProperty("rotateSpeed");
        previewObjHoverDistance_Prop = serializedObject.FindProperty("previewObjHoverDistance");
        previewObjFloorTiles_Prop = serializedObject.FindProperty("previewObjFloorTiles");
        previewObjFloorTilePrefab_Prop = serializedObject.FindProperty("previewObjFloorTilePrefab");
        placeTilesWithObject_Prop = serializedObject.FindProperty("placeTilesWithObject");
        usePreviewMatsForFloorTiles_Prop = serializedObject.FindProperty("usePreviewMatsForFloorTiles");
        hideSelectorOnPreview_Prop = serializedObject.FindProperty("hideSelectorOnPreview");
        disableAnimationForPreviewObj_Prop = serializedObject.FindProperty("disableAnimationForPreviewObj");
        hoverDistance_Prop = serializedObject.FindProperty("hoverDistance");
        dragBuild_Prop = serializedObject.FindProperty("dragBuild");
        invalidPlacementFeedback_Prop = serializedObject.FindProperty("invalidPlacementFeedback");
        showInvalidPreviewObj_Prop = serializedObject.FindProperty("showInvalidPreviewObj");
        invalidPlacementMat_Prop = serializedObject.FindProperty("invalidPlacementMat");
        objPreviewMat_Prop = serializedObject.FindProperty("objPreviewMat");
        changeMatColorNotMat_Prop = serializedObject.FindProperty("changeMatColorNotMat");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        General = EditorGUILayout.BeginFoldoutHeaderGroup(General, "General");
        if (General)
        {
            EditorGUILayout.HelpBox("General settings for the behavior of the cell selector", MessageType.None);
            EditorGUILayout.PropertyField(smoothMove_Prop);
            if (smoothMove_Prop.boolValue)
            {
                GUI.enabled = true;
            }
            else
            {
                GUI.enabled = false;
            }
            EditorGUILayout.PropertyField(moveSpeed_Prop);
            GUI.enabled = true;

            EditorGUILayout.PropertyField(rotation_Prop);
            if (rotation_Prop.boolValue)
            {
                GUI.enabled = true;
            }
            else
            {
                GUI.enabled = false;
                smoothRotate_Prop.boolValue = false;
            }
            EditorGUILayout.PropertyField(smoothRotate_Prop);
            if (smoothRotate_Prop.boolValue)
            {
                GUI.enabled = true;
            }
            else
            {
                GUI.enabled = false;
            }
            EditorGUILayout.PropertyField(rotateSpeed_Prop);
            GUI.enabled = true;
            EditorGUILayout.PropertyField(objPreviewMat_Prop);
            EditorGUILayout.PropertyField(previewObjHoverDistance_Prop);
            EditorGUILayout.PropertyField(changeMatColorNotMat_Prop);
            EditorGUILayout.PropertyField(disableAnimationForPreviewObj_Prop);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        GridSelectorTile = EditorGUILayout.BeginFoldoutHeaderGroup(GridSelectorTile, "Grid Selector Tile");
        if (GridSelectorTile)
        {
            EditorGUILayout.HelpBox("Settings related to the physical Grid Selector Mesh", MessageType.None);
            EditorGUILayout.PropertyField(hideSelectorOnPreview_Prop);
            EditorGUILayout.PropertyField(hoverDistance_Prop);
            EditorGUILayout.PropertyField(previewObjFloorTiles_Prop);
            EditorGUILayout.PropertyField(previewObjFloorTilePrefab_Prop);
            EditorGUILayout.PropertyField(placeTilesWithObject_Prop);
            EditorGUILayout.PropertyField(usePreviewMatsForFloorTiles_Prop);

        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        InvalidPlacementFeedback = EditorGUILayout.BeginFoldoutHeaderGroup(InvalidPlacementFeedback, "Invalid Placement Feedback");
        if (InvalidPlacementFeedback)
        {
            EditorGUILayout.HelpBox("Settings related to hiding or showing the grid selector tile and the preview object", MessageType.None);
            EditorGUILayout.PropertyField(invalidPlacementFeedback_Prop);
            if (invalidPlacementFeedback_Prop.boolValue)
            {
                GUI.enabled = true;
            }
            else
            {
                GUI.enabled = false;
                showInvalidPreviewObj_Prop.boolValue = false;
            }
            EditorGUILayout.PropertyField(showInvalidPreviewObj_Prop);
            EditorGUILayout.PropertyField(invalidPlacementMat_Prop);
            GUI.enabled = true;
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        
            serializedObject.ApplyModifiedProperties();
    }
}



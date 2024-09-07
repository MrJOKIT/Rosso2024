using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Building)), CanEditMultipleObjects]
public class BuildingEditor : Editor
{
    float boxSize = 35f;
    float gridGap = 4f;

    Texture2D image;
    GUIContent content = new GUIContent();

    public SerializedProperty
        prefabId_Prop,
        moveOnPoints_Prop, 
        buildTime_Prop,

        //1st row
        pos_m2_p2,
        pos_m1_p2,
        pos_0_p2,
        pos_p1_p2,
        pos_p2_p2,

        //2nd row
        pos_m2_p1,
        pos_m1_p1,
        pos_0_p1,
        pos_p1_p1,
        pos_p2_p1,

        //3rd row
        pos_m2_0,
        pos_m1_0,
        pos_0_0,
        pos_p1_0,
        pos_p2_0,

        //4rd row
        pos_m2_m1,
        pos_m1_m1,
        pos_0_m1,
        pos_p1_m1,
        pos_p2_m1,

        //5th row
        pos_m2_m2,
        pos_m1_m2,
        pos_0_m2,
        pos_p1_m2,
        pos_p2_m2,

        objectSize_Prop;

    public void OnEnable()
    {
        prefabId_Prop = serializedObject.FindProperty("prefabId");
        moveOnPoints_Prop = serializedObject.FindProperty("moveOnPoints");
        buildTime_Prop = serializedObject.FindProperty("buildTime");

        objectSize_Prop = serializedObject.FindProperty("objectSize");

        //1st row
        pos_m2_p2 = serializedObject.FindProperty("objectSize").FindPropertyRelative("pos_m2_p2");
        pos_m1_p2 = serializedObject.FindProperty("objectSize").FindPropertyRelative("pos_m1_p2");
        pos_0_p2 = serializedObject.FindProperty("objectSize").FindPropertyRelative("pos_0_p2");
        pos_p1_p2 = serializedObject.FindProperty("objectSize").FindPropertyRelative("pos_p1_p2");
        pos_p2_p2 = serializedObject.FindProperty("objectSize").FindPropertyRelative("pos_p2_p2");

        //2nd row
        pos_m2_p1 = serializedObject.FindProperty("objectSize").FindPropertyRelative("pos_m2_p1");
        pos_m1_p1 = serializedObject.FindProperty("objectSize").FindPropertyRelative("pos_m1_p1");
        pos_0_p1 = serializedObject.FindProperty("objectSize").FindPropertyRelative("pos_0_p1");
        pos_p1_p1 = serializedObject.FindProperty("objectSize").FindPropertyRelative("pos_p1_p1");
        pos_p2_p1 = serializedObject.FindProperty("objectSize").FindPropertyRelative("pos_p2_p1");

        //3rd row
        pos_m2_0 = serializedObject.FindProperty("objectSize").FindPropertyRelative("pos_m2_0");
        pos_m1_0 = serializedObject.FindProperty("objectSize").FindPropertyRelative("pos_m1_0");

        if (!moveOnPoints_Prop.boolValue)
        {
            image = (Texture2D)Resources.Load("RotationPivot0");
            content.image = image;
        }
        else
        {
            image = (Texture2D)Resources.Load("RotationPivot1");
            content.image = image;
        }
        pos_0_0 = serializedObject.FindProperty("objectSize").FindPropertyRelative("pos_0_0");
        pos_p1_0 = serializedObject.FindProperty("objectSize").FindPropertyRelative("pos_p1_0");
        pos_p2_0 = serializedObject.FindProperty("objectSize").FindPropertyRelative("pos_p2_0");

        //4th row
        pos_m2_m1 = serializedObject.FindProperty("objectSize").FindPropertyRelative("pos_m2_m1");
        pos_m1_m1 = serializedObject.FindProperty("objectSize").FindPropertyRelative("pos_m1_m1");
        pos_0_m1 = serializedObject.FindProperty("objectSize").FindPropertyRelative("pos_0_m1");
        pos_p1_m1 = serializedObject.FindProperty("objectSize").FindPropertyRelative("pos_p1_m1");
        pos_p2_m1 = serializedObject.FindProperty("objectSize").FindPropertyRelative("pos_p2_m1");

        //5th row
        pos_m2_m2 = serializedObject.FindProperty("objectSize").FindPropertyRelative("pos_m2_m2");
        pos_m1_m2 = serializedObject.FindProperty("objectSize").FindPropertyRelative("pos_m1_m2");
        pos_0_m2 = serializedObject.FindProperty("objectSize").FindPropertyRelative("pos_0_m2");
        pos_p1_m2 = serializedObject.FindProperty("objectSize").FindPropertyRelative("pos_p1_m2");
        pos_p2_m2 = serializedObject.FindProperty("objectSize").FindPropertyRelative("pos_p2_m2");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUIStyle skin = new GUIStyle()
        {
            normal = new GUIStyleState()
            {
                background = Texture2D.grayTexture
            },
            active = new GUIStyleState()
            {
                background = Texture2D.grayTexture
            },
            onNormal = new GUIStyleState()
            {
                background = Texture2D.whiteTexture
            } 
        };
        GUIStyle stylePress = new GUIStyle()
        {
            active = new GUIStyleState()
            {
                background = Texture2D.grayTexture
            }
        };

        EditorGUILayout.HelpBox("ID for all placed objects of this prefab", MessageType.None);
        EditorGUILayout.PropertyField(prefabId_Prop);
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("If checked, this will move this object on the grid points, and not on the center of the grid cells." +
            "It also changes how the check positions rotate, so they now rotate around the yellow dotted pivot. It is ideal for even square objects.", MessageType.None);
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(moveOnPoints_Prop);
        if(EditorGUI.EndChangeCheck())
        {
            if (!moveOnPoints_Prop.boolValue)
            {
                image = (Texture2D)Resources.Load("RotationPivot0");
                content.image = image;
            }
            else
            {
                image = (Texture2D)Resources.Load("RotationPivot1");
                content.image = image;
            }
        }
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("If this value is more than 0, this will build a temporary timed object by the indicated seconds, more features for this are on the objectPlacer", MessageType.None);
        EditorGUILayout.HelpBox("You may wish to add a build Timer, you can assign a Canvas with one on under the ObjectPlacer components building time settings", MessageType.None);
        EditorGUILayout.PropertyField(buildTime_Prop);

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Select the size of the object, this is the cells it will take up, center is always on and the pivot is displayed as the yellow dot for rotation and movement", MessageType.None);
        //1st row
        GUILayout.BeginHorizontal();
        if (moveOnPoints_Prop.boolValue)
        {
            GUI.enabled = false;
            pos_m2_p2.boolValue = false; 
            pos_m1_p2.boolValue = false;
            pos_0_p2.boolValue = false; 
            pos_p1_p2.boolValue = false;
            pos_p2_p2.boolValue = false;
            pos_p2_p1.boolValue = false;
            pos_p2_0.boolValue = false;
            pos_p2_m1.boolValue = false;
            pos_p2_m2.boolValue = false;
        }
        else
        {
            GUI.enabled = true;
        }
        GUILayout.FlexibleSpace();
        pos_m2_p2.boolValue = GUILayout.Toggle(pos_m2_p2.boolValue, "", skin, GUILayout.ExpandWidth(false), GUILayout.MaxHeight(boxSize), GUILayout.MaxWidth(boxSize));
        GUILayout.Space(gridGap);
        pos_m1_p2.boolValue = GUILayout.Toggle(pos_m1_p2.boolValue, "", skin, GUILayout.ExpandWidth(false), GUILayout.MaxHeight(boxSize), GUILayout.MaxWidth(boxSize));
        GUILayout.Space(gridGap);
        pos_0_p2.boolValue = GUILayout.Toggle(pos_0_p2.boolValue, "", skin, GUILayout.ExpandWidth(false), GUILayout.MaxHeight(boxSize), GUILayout.MaxWidth(boxSize));
        GUILayout.Space(gridGap);
        pos_p1_p2.boolValue = GUILayout.Toggle(pos_p1_p2.boolValue, "", skin, GUILayout.ExpandWidth(false), GUILayout.MaxHeight(boxSize), GUILayout.MaxWidth(boxSize));
        GUILayout.Space(gridGap);
        pos_p2_p2.boolValue = GUILayout.Toggle(pos_p2_p2.boolValue, "", skin, GUILayout.ExpandWidth(false), GUILayout.MaxHeight(boxSize), GUILayout.MaxWidth(boxSize));
        GUILayout.FlexibleSpace();
        if (moveOnPoints_Prop.boolValue)
        {
            GUI.enabled = true;
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(gridGap);

        //2nd row
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        pos_m2_p1.boolValue = GUILayout.Toggle(pos_m2_p1.boolValue, "", skin, GUILayout.ExpandWidth(false), GUILayout.MaxHeight(boxSize), GUILayout.MaxWidth(boxSize));
        GUILayout.Space(gridGap);
        pos_m1_p1.boolValue = GUILayout.Toggle(pos_m1_p1.boolValue, "", skin, GUILayout.ExpandWidth(false), GUILayout.MaxHeight(boxSize), GUILayout.MaxWidth(boxSize));
        GUILayout.Space(gridGap);
        pos_0_p1.boolValue = GUILayout.Toggle(pos_0_p1.boolValue, "", skin, GUILayout.ExpandWidth(false), GUILayout.MaxHeight(boxSize), GUILayout.MaxWidth(boxSize));
        GUILayout.Space(gridGap);
        pos_p1_p1.boolValue = GUILayout.Toggle(pos_p1_p1.boolValue, "", skin, GUILayout.ExpandWidth(false), GUILayout.MaxHeight(boxSize), GUILayout.MaxWidth(boxSize));
        GUILayout.Space(gridGap);
        if (moveOnPoints_Prop.boolValue)
        {
            GUI.enabled = false;
        }
        else
        {
            GUI.enabled = true;
        }
        pos_p2_p1.boolValue = GUILayout.Toggle(pos_p2_p1.boolValue, "", skin, GUILayout.ExpandWidth(false), GUILayout.MaxHeight(boxSize), GUILayout.MaxWidth(boxSize));
        if (moveOnPoints_Prop.boolValue)
        {
            GUI.enabled = true;
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.Space(gridGap);

        //3rd row
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        pos_m2_0.boolValue = GUILayout.Toggle(pos_m2_0.boolValue, "", skin, GUILayout.ExpandWidth(false), GUILayout.MaxHeight(boxSize), GUILayout.MaxWidth(boxSize));
        GUILayout.Space(gridGap);
        pos_m1_0.boolValue = GUILayout.Toggle(pos_m1_0.boolValue, "", skin, GUILayout.ExpandWidth(false), GUILayout.MaxHeight(boxSize), GUILayout.MaxWidth(boxSize));
        GUILayout.Space(gridGap);
        //GUI.enabled = false;
        if(content != null)
        pos_0_0.boolValue = GUILayout.Toggle(true, content, skin, GUILayout.ExpandWidth(false), GUILayout.MaxHeight(boxSize), GUILayout.MaxWidth(boxSize));
        GUILayout.Space(gridGap);
        //GUI.enabled = true;
        pos_p1_0.boolValue = GUILayout.Toggle(pos_p1_0.boolValue, "", skin, GUILayout.ExpandWidth(false), GUILayout.MaxHeight(boxSize), GUILayout.MaxWidth(boxSize));
        GUILayout.Space(gridGap);
        if (moveOnPoints_Prop.boolValue)
        {
            GUI.enabled = false;
        }
        else
        {
            GUI.enabled = true;
        }
        pos_p2_0.boolValue = GUILayout.Toggle(pos_p2_0.boolValue, "", skin, GUILayout.ExpandWidth(false), GUILayout.MaxHeight(boxSize), GUILayout.MaxWidth(boxSize));
        if (moveOnPoints_Prop.boolValue)
        {
            GUI.enabled = true;
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.Space(gridGap);

        //4th row
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        pos_m2_m1.boolValue = GUILayout.Toggle(pos_m2_m1.boolValue, "", skin, GUILayout.ExpandWidth(false), GUILayout.MaxHeight(boxSize), GUILayout.MaxWidth(boxSize));
        GUILayout.Space(gridGap);
        pos_m1_m1.boolValue = GUILayout.Toggle(pos_m1_m1.boolValue, "", skin, GUILayout.ExpandWidth(false), GUILayout.MaxHeight(boxSize), GUILayout.MaxWidth(boxSize));
        GUILayout.Space(gridGap);
        pos_0_m1.boolValue = GUILayout.Toggle(pos_0_m1.boolValue, "", skin, GUILayout.ExpandWidth(false), GUILayout.MaxHeight(boxSize), GUILayout.MaxWidth(boxSize));
        GUILayout.Space(gridGap);
        pos_p1_m1.boolValue = GUILayout.Toggle(pos_p1_m1.boolValue, "", skin, GUILayout.ExpandWidth(false), GUILayout.MaxHeight(boxSize), GUILayout.MaxWidth(boxSize));
        GUILayout.Space(gridGap);
        if (moveOnPoints_Prop.boolValue)
        {
            GUI.enabled = false;
        }
        else
        {
            GUI.enabled = true;
        }
        pos_p2_m1.boolValue = GUILayout.Toggle(pos_p2_m1.boolValue, "", skin, GUILayout.ExpandWidth(false), GUILayout.MaxHeight(boxSize), GUILayout.MaxWidth(boxSize));
        if (moveOnPoints_Prop.boolValue)
        {
            GUI.enabled = true;
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.Space(gridGap);

        //5th row
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        pos_m2_m2.boolValue = GUILayout.Toggle(pos_m2_m2.boolValue, "", skin, GUILayout.ExpandWidth(false), GUILayout.MaxHeight(boxSize), GUILayout.MaxWidth(boxSize));
        GUILayout.Space(gridGap);
        pos_m1_m2.boolValue = GUILayout.Toggle(pos_m1_m2.boolValue, "", skin, GUILayout.ExpandWidth(false), GUILayout.MaxHeight(boxSize), GUILayout.MaxWidth(boxSize));
        GUILayout.Space(gridGap);
        pos_0_m2.boolValue = GUILayout.Toggle(pos_0_m2.boolValue, "", skin, GUILayout.ExpandWidth(false), GUILayout.MaxHeight(boxSize), GUILayout.MaxWidth(boxSize));
        GUILayout.Space(gridGap);
        pos_p1_m2.boolValue = GUILayout.Toggle(pos_p1_m2.boolValue, "", skin, GUILayout.ExpandWidth(false), GUILayout.MaxHeight(boxSize), GUILayout.MaxWidth(boxSize));
        GUILayout.Space(gridGap);
        if (moveOnPoints_Prop.boolValue)
        {
            GUI.enabled = false;
        }
        else
        {
            GUI.enabled = true;
        }
        pos_p2_m2.boolValue = GUILayout.Toggle(pos_p2_m2.boolValue, "", skin, GUILayout.ExpandWidth(false), GUILayout.MaxHeight(boxSize), GUILayout.MaxWidth(boxSize));
        if (moveOnPoints_Prop.boolValue)
        {
            GUI.enabled = true;
        } 
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }
}

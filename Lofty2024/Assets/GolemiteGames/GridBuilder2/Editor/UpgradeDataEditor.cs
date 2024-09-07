using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UpgradeData))]
public class UpgradeDataEditor : Editor
{
    public SerializedProperty
        upgradePrefabs_Prop,
        upgradeTimes_Prop,
        currentUpgradeLevel_Prop;

    private void OnEnable()
    {
        upgradePrefabs_Prop = serializedObject.FindProperty("upgradePrefabs");
        upgradeTimes_Prop = serializedObject.FindProperty("upgradeTimes");
        currentUpgradeLevel_Prop = serializedObject.FindProperty("currentUpgradeLevel");
        
    }

    public override void OnInspectorGUI()
    {

        //Buttons
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Upgrade"))
        {
            upgradePrefabs_Prop.arraySize++;
            upgradeTimes_Prop.arraySize++;
        }
        if (GUILayout.Button("Remove Upgrade"))
        {
            upgradePrefabs_Prop.arraySize--;
            upgradeTimes_Prop.arraySize--;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();

        if(upgradePrefabs_Prop.arraySize > 0)
        {
            EditorGUILayout.LabelField("Upgrade Prefabs", GUILayout.MinWidth(200f));
            EditorGUILayout.LabelField("Upgrade Time", GUILayout.MaxWidth(100f));
        }

        EditorGUILayout.EndHorizontal();
        for (int i = 0; i < upgradePrefabs_Prop.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(upgradePrefabs_Prop.GetArrayElementAtIndex(i), GUIContent.none, GUILayout.MinWidth(200f));
            EditorGUILayout.PropertyField(upgradeTimes_Prop.GetArrayElementAtIndex(i), GUIContent.none, GUILayout.MaxWidth(100f));
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Current Upgrade Level", GUILayout.MinWidth(200f));
        int userInt = EditorGUILayout.IntField(currentUpgradeLevel_Prop.intValue, GUILayout.MaxWidth(100f));
        if(userInt != currentUpgradeLevel_Prop.intValue)
        {
            if (userInt < upgradePrefabs_Prop.arraySize + 1)
            {
                currentUpgradeLevel_Prop.intValue = userInt;
            }
        }
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }
}

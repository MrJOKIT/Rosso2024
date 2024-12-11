using System;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

public enum AnimationDirections
{
    Horizontal,
    Vertical,
    Both
}

public enum AnimationHorizontalDirections
{
    LeftToRightOnly,
    RightToLeftOnly,
    Randomize
}

public enum AnimationVerticalDirections
{
    TopToBottomOnly,
    BottomToTopOnly,
    Randomize
}

#if UNITY_EDITOR
[CustomEditor(typeof(TextRevealer))]
public class TextRevealerEditor : Editor
{
    bool showRevealOptions = false;
    bool showUnrevealOptions = false;

    MonoBehaviour textComponent = null;

    public override void OnInspectorGUI()
    {
        TextRevealer myScript = (TextRevealer)target;

        myScript.textComponent = (MonoBehaviour)EditorGUILayout.ObjectField(new GUIContent("Target Text"), myScript.textComponent, typeof(MonoBehaviour), true);

        if (myScript.textComponent  != null &&
            (myScript.textComponent is Text
            || myScript.textComponent is TextMeshProUGUI
            || myScript.textComponent is TextMeshPro))
        {
            if (textComponent != myScript.textComponent)
            {
                myScript.UpdateTextComponent(false);
            }

            textComponent = myScript.textComponent;

            myScript.AutomaticUnreveal = EditorGUILayout.Toggle(new GUIContent("Auto Unreveal"), myScript.AutomaticUnreveal);
            myScript.RevealOnEnabled = EditorGUILayout.Toggle(new GUIContent("Reveal On Enabled"), myScript.RevealOnEnabled);
            myScript.UnrevealOnDisabled = EditorGUILayout.Toggle(new GUIContent("Unreveal On Disabled"), myScript.UnrevealOnDisabled);

            if (myScript.AutomaticUnreveal)
            {
                myScript.AutomaticUnrevealAfterSeconds = EditorGUILayout.FloatField(new GUIContent("Unreveal after (seconds)"), myScript.AutomaticUnrevealAfterSeconds);
                myScript.AutomaticUnrevealAfterSeconds = Math.Max(myScript.AutomaticUnrevealAfterSeconds, 0);
            }

            showRevealOptions = EditorGUILayout.Foldout(showRevealOptions, "Reveal Options", true);
            if (showRevealOptions)
            {
                EditorGUILayout.LabelField("Character Options", new GUIStyle() { fontStyle = FontStyle.Bold });
                myScript.RevealSmoothOverlap = EditorGUILayout.Toggle(new GUIContent("Smooth Overlap"), myScript.RevealSmoothOverlap);

                if (myScript.RevealSmoothOverlap)
                {
                    myScript.RevealSmoothOverlapSmoothness = EditorGUILayout.Slider(new GUIContent("Smooth Overlap Smoothness"), myScript.RevealSmoothOverlapSmoothness, 0.1f, 1f);
                }

                myScript.RevealAnimationDirection = (AnimationDirections)EditorGUILayout.EnumPopup(new GUIContent("Animation Direction"), myScript.RevealAnimationDirection);

                if (myScript.RevealAnimationDirection == AnimationDirections.Both || myScript.RevealAnimationDirection == AnimationDirections.Horizontal)
                {
                    myScript.RevealAnimationHorizontalDirection = (AnimationHorizontalDirections)EditorGUILayout.EnumPopup(new GUIContent("Horizontal Direction"), myScript.RevealAnimationHorizontalDirection);

                    myScript.revealMinHorizontalDistance = EditorGUILayout.FloatField(new GUIContent("Min Horizontal Starting Distance:"), myScript.revealMinHorizontalDistance);
                    myScript.revealMaxHorizontalDistance = EditorGUILayout.FloatField(new GUIContent("Max Horizontal Starting Distance:"), myScript.revealMaxHorizontalDistance);
                    EditorGUILayout.MinMaxSlider(new GUIContent("Horizontal Starting Distance Range"), ref myScript.revealMinHorizontalDistance, ref myScript.revealMaxHorizontalDistance, 0f, 5000);
                }

                if (myScript.RevealAnimationDirection == AnimationDirections.Both || myScript.RevealAnimationDirection == AnimationDirections.Vertical)
                {
                    myScript.RevealAnimationVerticalDirection = (AnimationVerticalDirections)EditorGUILayout.EnumPopup(new GUIContent("Vertical Direction"), myScript.RevealAnimationVerticalDirection);

                    myScript.revealMinVerticalDistance = EditorGUILayout.FloatField(new GUIContent("Min Vertical Starting Distance:"), myScript.revealMinVerticalDistance);
                    myScript.revealMaxVerticalDistance = EditorGUILayout.FloatField(new GUIContent("Max Vertical Starting Distance:"), myScript.revealMaxVerticalDistance);
                    EditorGUILayout.MinMaxSlider(new GUIContent("Vertical Starting Distance Range"), ref myScript.revealMinVerticalDistance, ref myScript.revealMaxVerticalDistance, 0f, 5000);
                }

                EditorGUILayout.LabelField("Text Options", new GUIStyle() { fontStyle = FontStyle.Bold });
                myScript.RevealTime = EditorGUILayout.FloatField(new GUIContent("Reveal Duration (Seconds)"), myScript.RevealTime);

                myScript.RevealLetterOpacityBegin = EditorGUILayout.FloatField(new GUIContent("Initial Opacity"), myScript.RevealLetterOpacityBegin);
                myScript.RevealLetterOpacityEnd = EditorGUILayout.FloatField(new GUIContent("Final Opacity"), myScript.RevealLetterOpacityEnd);
                myScript.RevealRandomRotation = EditorGUILayout.Toggle(new GUIContent("Apply Random Rotation"), myScript.RevealRandomRotation);
                myScript.RevealReverse = EditorGUILayout.Toggle(new GUIContent("Reverse"), myScript.RevealReverse);

                myScript.RevealTime = Math.Max(myScript.RevealTime, 0f);

                myScript.RevealLetterOpacityBegin = Math.Min(myScript.RevealLetterOpacityBegin, 1);
                myScript.RevealLetterOpacityBegin = Math.Max(myScript.RevealLetterOpacityBegin, 0);

                myScript.RevealLetterOpacityEnd = Math.Min(myScript.RevealLetterOpacityEnd, 1);
                myScript.RevealLetterOpacityEnd = Math.Max(myScript.RevealLetterOpacityEnd, 0);

                EditorGUILayout.LabelField("Events", new GUIStyle() { fontStyle = FontStyle.Bold });
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnRevealStarted"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnRevealCompleted"));
            }

            showUnrevealOptions = EditorGUILayout.Foldout(showUnrevealOptions, "Unreveal Options", true);
            if (showUnrevealOptions)
            {
                EditorGUILayout.LabelField("Character Options", new GUIStyle() { fontStyle = FontStyle.Bold });
                myScript.UnrevealSmoothOverlap = EditorGUILayout.Toggle(new GUIContent("Smooth Overlap"), myScript.UnrevealSmoothOverlap);

                if (myScript.UnrevealSmoothOverlap)
                {
                    myScript.UnrevealSmoothOverlapSmoothness = EditorGUILayout.Slider(new GUIContent("Smooth Overlap Smoothness"), myScript.UnrevealSmoothOverlapSmoothness, 0.1f, 1f);
                }
                
                myScript.UnrevealAnimationDirection = (AnimationDirections)EditorGUILayout.EnumPopup(new GUIContent("Animation Direction"), myScript.UnrevealAnimationDirection);

                if (myScript.UnrevealAnimationDirection == AnimationDirections.Both || myScript.UnrevealAnimationDirection == AnimationDirections.Horizontal)
                {
                    myScript.UnrevealAnimationHorizontalDirection = (AnimationHorizontalDirections)EditorGUILayout.EnumPopup(new GUIContent("Horizontal Direction"), myScript.UnrevealAnimationHorizontalDirection);
                    myScript.unrevealMinHorizontalDistance = EditorGUILayout.FloatField(new GUIContent("Min Ending Horizontal Distance:"), myScript.unrevealMinHorizontalDistance);
                    myScript.unrevealMaxHorizontalDistance = EditorGUILayout.FloatField(new GUIContent("Max Ending Horizontal Distance:"), myScript.unrevealMaxHorizontalDistance);
                    EditorGUILayout.MinMaxSlider(new GUIContent("Horizontal Ending Distance Range"), ref myScript.unrevealMinHorizontalDistance, ref myScript.unrevealMaxHorizontalDistance, 0f, 5000);
                }

                if (myScript.UnrevealAnimationDirection == AnimationDirections.Both || myScript.UnrevealAnimationDirection == AnimationDirections.Vertical)
                {
                    myScript.UnrevealAnimationVerticalDirection = (AnimationVerticalDirections)EditorGUILayout.EnumPopup(new GUIContent("Vertical Direction"), myScript.UnrevealAnimationVerticalDirection);
                    myScript.unrevealMinVerticalDistance = EditorGUILayout.FloatField(new GUIContent("Min Ending Vertical Distance:"), myScript.unrevealMinVerticalDistance);
                    myScript.unrevealMaxVerticalDistance = EditorGUILayout.FloatField(new GUIContent("Max Ending Vertical Distance:"), myScript.unrevealMaxVerticalDistance);
                    EditorGUILayout.MinMaxSlider(new GUIContent("Vertical Ending Distance Range"), ref myScript.unrevealMinVerticalDistance, ref myScript.unrevealMaxVerticalDistance, 0f, 5000);
                }

                EditorGUILayout.LabelField("Text Options", new GUIStyle() { fontStyle = FontStyle.Bold });
                myScript.UnrevealTime = EditorGUILayout.FloatField(new GUIContent("Unreveal Duration (Seconds)"), myScript.UnrevealTime);

                myScript.UnrevealLetterOpacityBegin = EditorGUILayout.FloatField(new GUIContent("Initial Opacity"), myScript.UnrevealLetterOpacityBegin);
                myScript.UnrevealLetterOpacityEnd = EditorGUILayout.FloatField(new GUIContent("Final Opacity"), myScript.UnrevealLetterOpacityEnd);
                myScript.UnrevealRandomRotation = EditorGUILayout.Toggle(new GUIContent("Apply Random Rotation"), myScript.UnrevealRandomRotation);
                myScript.UnrevealReverse = EditorGUILayout.Toggle(new GUIContent("Reverse"), myScript.UnrevealReverse);

                myScript.UnrevealTime = Math.Max(myScript.UnrevealTime, 0.0f);

                myScript.UnrevealLetterOpacityBegin = Math.Min(myScript.UnrevealLetterOpacityBegin, 1f);
                myScript.UnrevealLetterOpacityBegin = Math.Max(myScript.UnrevealLetterOpacityBegin, 0f);

                myScript.UnrevealLetterOpacityEnd = Math.Min(myScript.UnrevealLetterOpacityEnd, 1f);
                myScript.UnrevealLetterOpacityEnd = Math.Max(myScript.UnrevealLetterOpacityEnd, 0f);

                EditorGUILayout.LabelField("Events", new GUIStyle() { fontStyle = FontStyle.Bold });
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnUnrevealStarted"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnUnrevealCompleted"));
            }

            using (new EditorGUI.DisabledScope(myScript.IsEditorPlayingAnimation))
            {
                if (GUILayout.Button("Preview Reveal"))
                {
                    myScript.Reveal();
                }

                if (GUILayout.Button("Preview Unreveal"))
                {
                    myScript.Unreveal();
                }
            }

            using (new EditorGUI.DisabledScope(!myScript.IsEditorPlayingAnimation))
            {
                if (GUILayout.Button("Stop Preview"))
                {
                    myScript.StopAnimation();
                }
            }

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
        else
        {
            EditorGUILayout.LabelField("Target text must be of type Text, TextMeshPro or TextMeshProUGUI");
        }
    }
}
#endif
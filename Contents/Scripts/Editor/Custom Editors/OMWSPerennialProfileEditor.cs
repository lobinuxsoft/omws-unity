using CryingOnion.OhMy.WeatherSystem.Data;
using CryingOnion.OhMy.WeatherSystem.Utility;
using UnityEditor;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.EditorScript
{
    [CustomEditor(typeof(OMWSPerennialProfile))]
    [CanEditMultipleObjects]
    public class OMWSPerennialProfileEditor : Editor
    {
        SerializedProperty tickSpeedMultiplier;
        SerializedProperty standardYear;
        SerializedProperty leapYear;
        OMWSPerennialProfile prof;

        void OnEnable()
        {
            tickSpeedMultiplier = serializedObject.FindProperty("tickSpeedMultiplier");
            standardYear = serializedObject.FindProperty("standardYear");
            leapYear = serializedObject.FindProperty("leapYear");
            prof = (OMWSPerennialProfile)target;
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            Undo.RecordObject(prof, prof.name + " Profile Changes");

            EditorGUILayout.LabelField("Current Settings", EditorStyles.boldLabel);
            prof.currentTicks = EditorGUILayout.Slider("Current Ticks", prof.currentTicks, 0, prof.ticksPerDay);
            prof.currentDay = EditorGUILayout.IntSlider("Current Day", prof.currentDay, 0, prof.daysPerYear);
            prof.currentYear = EditorGUILayout.IntField("Current Year", prof.currentYear);
            EditorGUILayout.Space(20);

            EditorGUILayout.LabelField("Time Movement", EditorStyles.boldLabel);
            prof.pauseTime = EditorGUILayout.Toggle("Pause Time", prof.pauseTime);

            if (!prof.pauseTime)
            {
                prof.tickSpeed = EditorGUILayout.FloatField("Tick Speed", prof.tickSpeed);
                EditorGUILayout.PropertyField(tickSpeedMultiplier);
            }

            EditorGUILayout.Space(20);

            EditorGUILayout.LabelField("Length Settings", EditorStyles.boldLabel);
            prof.ticksPerDay = EditorGUILayout.FloatField("Ticks Per Day", prof.ticksPerDay);
            EditorGUILayout.Space(10);
            prof.realisticYear = EditorGUILayout.Toggle("Realistic Year", prof.realisticYear);

            if (prof.realisticYear)
            {
                prof.useLeapYear = EditorGUILayout.Toggle("Use Leap Year", prof.useLeapYear);

                EditorGUILayout.Space(10);
                EditorGUILayout.PropertyField(standardYear);
                if (prof.useLeapYear)
                    EditorGUILayout.PropertyField(leapYear);
            }
            else
            {
                prof.daysPerYear = EditorGUILayout.IntField("Days Per Year", prof.daysPerYear);
            }

            EditorGUILayout.Space(20);

            EditorGUILayout.LabelField("Sun Movement Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("timeCurveSettings"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("sunriseWeight"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("dayWeight"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("sunsetWeight"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("nightWeight"));
            prof.GetModifiedDayPercent();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("displayCurve"));

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(prof);
        }

        public void OnStaticMeasureGUI(GUIStyle style, ref bool lengthWindow, ref bool movementWindow, ref bool curveWindow)
        {
            serializedObject.Update();

            movementWindow = EditorGUILayout.BeginFoldoutHeaderGroup(movementWindow,
                new GUIContent("    Movement Settings"), OMWSEditorUtilities.FoldoutStyle());

            if (movementWindow)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(serializedObject.FindProperty("pauseTime"));

                if (!serializedObject.FindProperty("pauseTime").boolValue)
                {
                    EditorGUILayout.Space();
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("resetTicksOnStart"));

                    if (serializedObject.FindProperty("resetTicksOnStart").boolValue)
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("startTicks"));

                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("tickSpeed"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("tickSpeedMultiplier"));
                    EditorGUI.indentLevel--;
                }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            lengthWindow = EditorGUILayout.BeginFoldoutHeaderGroup(lengthWindow,
                new GUIContent("    Length Settings"), OMWSEditorUtilities.FoldoutStyle());

            EditorGUILayout.EndFoldoutHeaderGroup();

            if (lengthWindow)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(serializedObject.FindProperty("ticksPerDay"));
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(serializedObject.FindProperty("realisticYear"));

                if (serializedObject.FindProperty("realisticYear").boolValue)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("useLeapYear"));
                    EditorGUILayout.Space();

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("standardYear"));

                    if (serializedObject.FindProperty("useLeapYear").boolValue)
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("leapYear"));
                }
                else
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("daysPerYear"));

                EditorGUI.indentLevel--;
            }

            curveWindow = EditorGUILayout.BeginFoldoutHeaderGroup(curveWindow,
                new GUIContent("    Curve Settings"), OMWSEditorUtilities.FoldoutStyle());

            EditorGUILayout.EndFoldoutHeaderGroup();

            if (curveWindow)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(serializedObject.FindProperty("timeCurveSettings"));
                EditorGUILayout.Space();
                prof.GetModifiedDayPercent();

                EditorGUI.indentLevel++;

                switch ((OMWSPerennialProfile.TimeCurveSettings)serializedObject.FindProperty("timeCurveSettings").enumValueIndex)
                {
                    case (OMWSPerennialProfile.TimeCurveSettings.linearDay):
                        break;
                    case (OMWSPerennialProfile.TimeCurveSettings.simpleCurve):
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("sunriseWeight").FindPropertyRelative("weight"), new GUIContent("Sunrise Weight"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("dayWeight").FindPropertyRelative("weight"), new GUIContent("Day Weight"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("sunsetWeight").FindPropertyRelative("weight"), new GUIContent("Sunset Weight"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("nightWeight").FindPropertyRelative("weight"), new GUIContent("Night Weight"));
                        break;
                    case (OMWSPerennialProfile.TimeCurveSettings.advancedCurve):
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("sunriseWeight"), new GUIContent("Sunrise Settings"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("dayWeight"), new GUIContent("Day Settings"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("sunsetWeight"), new GUIContent("Sunset Settings"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("nightWeight"), new GUIContent("Night Settings"));
                        break;
                }

                EditorGUI.indentLevel--;

                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("displayCurve"));

                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }

        public void OnRuntimeMeasureGUI()
        {
            serializedObject.Update();

            serializedObject.FindProperty("currentTicks").floatValue = EditorGUILayout.Slider("Current Ticks", serializedObject.FindProperty("currentTicks").floatValue, 0, serializedObject.FindProperty("ticksPerDay").floatValue);
            serializedObject.FindProperty("currentDay").intValue = EditorGUILayout.IntSlider(new GUIContent("Current Day"), serializedObject.FindProperty("currentDay").intValue, 0, serializedObject.FindProperty("daysPerYear").intValue);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("currentYear"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}
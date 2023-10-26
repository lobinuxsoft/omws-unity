using CryingOnion.OhMy.WeatherSystem.Data;
using CryingOnion.OhMy.WeatherSystem.Utility;
using UnityEditor;

namespace CryingOnion.OhMy.WeatherSystem.EditorScript
{
    [CustomEditor(typeof(OMWSSatelliteProfile))]
    [CanEditMultipleObjects]
    public class OMWSSatelliteProfileEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("satelliteReference"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("size"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("distance"));

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("useLight"));

            EditorGUI.BeginDisabledGroup(!serializedObject.FindProperty("useLight").boolValue);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("flare"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("lightColorMultiplier"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("castShadows"));
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("satelliteRotateSpeed"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("satelliteRotateAxis"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("initialRotation"));

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("orbitOffset"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("satelliteDirection"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("satellitePitch"));

            if (serializedObject.hasModifiedProperties)
                serializedObject.FindProperty("changedLastFrame").boolValue = true;

            serializedObject.ApplyModifiedProperties();
        }

        public void NestedGUI()
        {
            serializedObject.Update();

            serializedObject.FindProperty("open").boolValue = EditorGUILayout.BeginFoldoutHeaderGroup(serializedObject.FindProperty("open").boolValue, $"    {target.name}", OMWSEditorUtilities.FoldoutStyle());
            EditorGUILayout.EndFoldoutHeaderGroup();

            if (serializedObject.FindProperty("open").boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("satelliteReference"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("size"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("distance"));
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("useLight"));
                EditorGUI.indentLevel++;
                EditorGUI.BeginDisabledGroup(!serializedObject.FindProperty("useLight").boolValue);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("flare"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("lightColorMultiplier"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("castShadows"));
                EditorGUI.indentLevel--;
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("satelliteRotateSpeed"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("satelliteRotateAxis"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("initialRotation"));
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("orbitOffset"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("satelliteDirection"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("satellitePitch"));

                if (serializedObject.hasModifiedProperties)
                    serializedObject.FindProperty("changedLastFrame").boolValue = true;

                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
using CryingOnion.OhMy.WeatherSystem.Module;
using UnityEditor;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.EditorScript
{
    [CustomEditor(typeof(OMWSMicrosplatModule))]
    [CanEditMultipleObjects]
    public class OMWSMicrosplatModuleEditor : OMWSModuleEditor
    {
        SerializedProperty updateFrequency;
        OMWSMicrosplatModule module;

        void OnEnable() { }

        public override GUIContent GetGUIContent() =>
            new GUIContent("", (Texture)Resources.Load("Integration"), "MicroSplat Control: Links the OMWS with MicroSplat.");

        public override void DisplayInOMWSWindow()
        {
            serializedObject.Update();

            if (module == null)
                module = (OMWSMicrosplatModule)target;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("updateWetness"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("minWetness"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxWetness"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("updateRainRipples"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("updatePuddles"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("updateStreams"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("updateSnow"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("updateWindStrength"));
            EditorGUILayout.Space(20);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("updateFrequency"));
            serializedObject.ApplyModifiedProperties();
        }
    }
}
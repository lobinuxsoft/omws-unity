using CryingOnion.OhMy.WeatherSystem.Core;
using CryingOnion.OhMy.WeatherSystem.Data;
using CryingOnion.OhMy.WeatherSystem.Module;
using CryingOnion.OhMy.WeatherSystem.Utility;
using UnityEditor;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.EditorScript
{
    [CustomEditor(typeof(OMWSSatelliteManager))]
    [CanEditMultipleObjects]
    public class OMWSSatelliteManagerEditor : OMWSModuleEditor
    {
        OMWSSatelliteManager t;
        static bool manageSatellites;

        private void OnEnable() => t = (OMWSSatelliteManager)target;

        public override GUIContent GetGUIContent() =>
            new GUIContent("", (Texture)Resources.Load("OMWSMoon"), "Satellites: Manage satellites and moons within the OMWS system.");

        public override void OnInspectorGUI() { }

        public override void DisplayInOMWSWindow()
        {
            serializedObject.Update();
            manageSatellites = EditorGUILayout.BeginFoldoutHeaderGroup(manageSatellites, new GUIContent("    Manage Satellites"), OMWSEditorUtilities.FoldoutStyle());
            EditorGUILayout.EndFoldoutHeaderGroup();

            if (manageSatellites)
            {
                EditorGUILayout.Space();
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("satellites"));
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("hideInHierarchy"));
                EditorGUI.indentLevel--;
            }

            EditorGUI.indentLevel++;

            if (t.satellites != null)
            {
                foreach (OMWSSatelliteProfile i in t.satellites)
                {
                    if (i)
                        (CreateEditor(i) as OMWSSatelliteProfileEditor).NestedGUI();
                }
            }

            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            if (GUILayout.Button("Refresh Satellites"))
                ((OMWSSatelliteManager)target).UpdateSatellites();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
using CryingOnion.OhMy.WeatherSystem.Module;
using UnityEditor;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.EditorScript
{
    [CustomEditor(typeof(OMWSSaveLoad))]
    public class OMWSSaveLoadEditor : OMWSModuleEditor
    {
        OMWSSaveLoad saveLoad;

        void OnEnable() => saveLoad = (OMWSSaveLoad)target;

        public override GUIContent GetGUIContent() => new GUIContent("", (Texture)Resources.Load("Save"), "Save & Load: Manage save and load commands within the OMWS system.");

        public override void OnInspectorGUI() { }

        public override void DisplayInOMWSWindow()
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Save"))
                saveLoad.Save();
            if (GUILayout.Button("Load"))
                saveLoad.Load();

            EditorGUILayout.EndHorizontal();
        }
    }
}
using CryingOnion.OhMy.WeatherSystem.Core;
using CryingOnion.OhMy.WeatherSystem.Module;
using CryingOnion.OhMy.WeatherSystem.Utility;
using UnityEditor;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.EditorScript
{
    [CustomEditor(typeof(OMWSAmbienceManager))]
    [CanEditMultipleObjects]
    public class OMWSAmbienceManagerEditor : OMWSModuleEditor
    {
        protected static bool profileSettings;
        protected static bool currentInfo;
        OMWSAmbienceManager ambienceManager;

        public override GUIContent GetGUIContent() =>
            new GUIContent("", (Texture)Resources.Load("Ambience Profile"), "Ambience: Controls a secondary weather system that runs parallel to the main system allowing for ambient noises and FX.");

        void OnEnable()
        {
            if (target)
                ambienceManager = (OMWSAmbienceManager)target;
        }

        public override void OnInspectorGUI() { }

        public override void DisplayInOMWSWindow()
        {
            serializedObject.Update();

            if (ambienceManager == null)
                if (target)
                    ambienceManager = (OMWSAmbienceManager)target;
                else
                    return;

            profileSettings = EditorGUILayout.BeginFoldoutHeaderGroup(profileSettings, "    Forecast Settings", OMWSEditorUtilities.FoldoutStyle());
            EditorGUI.EndFoldoutHeaderGroup();

            if (profileSettings)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ambienceProfiles"));
                EditorGUILayout.Space();

                if (GUILayout.Button("Add all ambience profiles"))
                    ambienceManager.FindAllAmbiences();

                EditorGUI.indentLevel--;
            }


            currentInfo = EditorGUILayout.BeginFoldoutHeaderGroup(currentInfo, "    Current Information", OMWSEditorUtilities.FoldoutStyle());

            EditorGUI.EndFoldoutHeaderGroup();

            if (currentInfo)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("currentAmbienceProfile"));

                if (Application.isPlaying)
                    EditorGUILayout.HelpBox(ambienceManager.currentAmbienceProfile.name + " will be playing for the next " + Mathf.Round(ambienceManager.GetTimeTillNextAmbience()) + " ticks.", MessageType.None, true);

                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
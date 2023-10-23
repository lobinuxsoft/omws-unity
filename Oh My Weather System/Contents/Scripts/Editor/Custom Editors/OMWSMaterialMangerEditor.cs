using UnityEditor;
using UnityEngine;
using CryingOnion.OhMy.WeatherSystem.Module;
using CryingOnion.OhMy.WeatherSystem.Core;
using CryingOnion.OhMy.WeatherSystem.Utility;
using CryingOnion.OhMy.WeatherSystem.Data;

namespace CryingOnion.OhMy.WeatherSystem.EditorScript
{
    [CustomEditor(typeof(OMWSMaterialManager))]
    [CanEditMultipleObjects]
    public class OMWSMaterialMangerEditor : OMWSModuleEditor
    {
        OMWSMaterialManager materialManager;
        protected static bool profileSettings;
        protected static bool settings;
        SerializedObject so;

        void OnEnable() { }

        public override void OnInspectorGUI() { }

        public override GUIContent GetGUIContent() => new GUIContent("", (Texture)Resources.Load("MaterialManager"), "Materials: Manages the materials that are affected by the OMWS system.");

        public override void DisplayInOMWSWindow()
        {
            serializedObject.Update();

            if (materialManager == null)
            {
                if (target)
                {
                    materialManager = (OMWSMaterialManager)target;
                    so = new SerializedObject(materialManager.profile);
                }
                else
                    return;
            }

            materialManager = (OMWSMaterialManager)target;

            if (serializedObject.FindProperty("profile").objectReferenceValue == null)
                EditorGUILayout.HelpBox("Make sure that you have all of the necessary profile references!", MessageType.Error);

            profileSettings = EditorGUILayout.BeginFoldoutHeaderGroup(profileSettings, "    Profile Settings", OMWSEditorUtilities.FoldoutStyle());

            EditorGUI.EndFoldoutHeaderGroup();

            if (profileSettings)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("profile"));
                EditorGUILayout.Space();
                so.ApplyModifiedProperties();
                EditorGUI.indentLevel--;
            }

            if (materialManager.profile)
                (CreateEditor(materialManager.profile) as OMWSMaterialProfileEditor).DisplayInOMWSWindow();

            settings = EditorGUILayout.BeginFoldoutHeaderGroup(settings, "    Options", OMWSEditorUtilities.FoldoutStyle());

            EditorGUILayout.EndFoldoutHeaderGroup();

            if (settings)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_SnowAmount"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_SnowMeltSpeed"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Wetness"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_DryingSpeed"));
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
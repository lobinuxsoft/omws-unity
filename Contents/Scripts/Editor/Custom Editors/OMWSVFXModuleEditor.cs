using CryingOnion.OhMy.WeatherSystem.Module;
using CryingOnion.OhMy.WeatherSystem.Utility;
using UnityEditor;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.EditorScript
{
    [CustomEditor(typeof(OMWSVFXModule))]
    [CanEditMultipleObjects]
    public class OMWSVFXModuleEditor : OMWSModuleEditor
    {
        OMWSVFXModule t;

        protected static bool thunderTab;
        protected static bool windTab;
        protected static bool ParticlesTab;
        protected static bool FXTab;

        public override GUIContent GetGUIContent() =>
            new GUIContent("", (Texture)Resources.Load("FX Module"), "VFX: Manage FX types, particles, and other VFX related options.");

        void OnEnable() => t = (OMWSVFXModule)target;

        public override void DisplayInOMWSWindow()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("particleManager"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("audioManager"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("postFXManager"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("precipitationManager"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("filterManager"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("thunderManager"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("windManager"), true);

            ParticlesTab = EditorGUILayout.BeginFoldoutHeaderGroup(ParticlesTab, new GUIContent("    Miscellaneous Options"), OMWSEditorUtilities.FoldoutStyle());
            EditorGUILayout.EndFoldoutHeaderGroup();

            if (ParticlesTab)
            {

                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("Linked Particles");
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Stars"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_CloudParticles"));
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("hideFXInHierarchy"));
                if (EditorGUI.EndChangeCheck())
                    if (t.parent)
                        DestroyImmediate(t.parent.gameObject);

                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
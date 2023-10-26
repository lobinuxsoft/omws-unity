using CryingOnion.OhMy.WeatherSystem.Data;
using UnityEditor;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.EditorScript
{
    [CustomEditor(typeof(OMWSFilterFX))]
    [CanEditMultipleObjects]
    public class OMWSFilterFXEditor : OMWSFXProfileEditor
    {
        void OnEnable() { }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("filterSaturation"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("filterValue"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("filterColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("sunFilter"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudFilter"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("transitionTimeModifier"));

            serializedObject.ApplyModifiedProperties();
        }

        public override void RenderInWindow(Rect pos)
        {
            float space = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            var propPosA = new Rect(pos.x, pos.y + space, pos.width, EditorGUIUtility.singleLineHeight);
            var propPosB = new Rect(pos.x, pos.y + space * 2, pos.width, EditorGUIUtility.singleLineHeight);
            var propPosC = new Rect(pos.x, pos.y + space * 3, pos.width, EditorGUIUtility.singleLineHeight);
            var propPosD = new Rect(pos.x, pos.y + space * 4, pos.width, EditorGUIUtility.singleLineHeight);
            var propPosE = new Rect(pos.x, pos.y + space * 5, pos.width, EditorGUIUtility.singleLineHeight);
            var propPosF = new Rect(pos.x, pos.y + space * 6, pos.width, EditorGUIUtility.singleLineHeight);

            serializedObject.Update();

            EditorGUI.PropertyField(propPosA, serializedObject.FindProperty("filterSaturation"));
            EditorGUI.PropertyField(propPosB, serializedObject.FindProperty("filterValue"));
            EditorGUI.PropertyField(propPosC, serializedObject.FindProperty("filterColor"));
            EditorGUI.PropertyField(propPosD, serializedObject.FindProperty("sunFilter"));
            EditorGUI.PropertyField(propPosE, serializedObject.FindProperty("cloudFilter"));
            EditorGUI.PropertyField(propPosF, serializedObject.FindProperty("transitionTimeModifier"));

            serializedObject.ApplyModifiedProperties();
        }

        public override float GetLineHeight() => 6;
    }
}
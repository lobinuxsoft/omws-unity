using CryingOnion.OhMy.WeatherSystem.Data;
using UnityEditor;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.EditorScript
{
    [CustomEditor(typeof(OMWSMultiFXProfile))]
    [CanEditMultipleObjects]
    public class OMWSMultiFXProfileEditor : OMWSFXProfileEditor
    {
        void OnEnable() { }

        public override void RenderInWindow(Rect pos)
        {
            float space = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            var propPosA = new Rect(pos.x, pos.y + space, pos.width, EditorGUIUtility.singleLineHeight);

            serializedObject.Update();

            EditorGUI.PropertyField(propPosA, serializedObject.FindProperty("multiFX"));

            serializedObject.ApplyModifiedProperties();
        }

        public override float GetLineHeight() => 1 + (serializedObject.FindProperty("multiFX").isExpanded ? serializedObject.FindProperty("multiFX").arraySize : 0);

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("multiFX"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}
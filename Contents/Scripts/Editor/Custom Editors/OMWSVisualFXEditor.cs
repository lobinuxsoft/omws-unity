using CryingOnion.OhMy.WeatherSystem.Data;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;
#elif OMWS_WEATHER_URP
using UnityEngine.Rendering;
#endif
using UnityEngine;
using UnityEditor;

namespace CryingOnion.OhMy.WeatherSystem.EditorScript
{
    [CustomEditor(typeof(OMWSVisualFX))]
    [CanEditMultipleObjects]
    public class OMWSVisualFXEditor : OMWSFXProfileEditor
    {
        void OnEnable() { }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LayerField(new GUIContent("Volume Layer"), serializedObject.FindProperty("layer").intValue);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("priority"), new GUIContent("Priority"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("transitionTimeModifier"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("effectSettings"), new GUIContent("Post Processing Profile"));
            EditorGUILayout.Space();

            if (serializedObject.FindProperty("effectSettings").objectReferenceValue)
                CreateEditor(serializedObject.FindProperty("effectSettings").objectReferenceValue).OnInspectorGUI();

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

            EditorGUI.LayerField(propPosA, new GUIContent("Volume Layer"), serializedObject.FindProperty("layer").intValue);
            EditorGUI.PropertyField(propPosB, serializedObject.FindProperty("priority"));
            EditorGUI.PropertyField(propPosC, serializedObject.FindProperty("effectSettings"));

            EditorGUI.PropertyField(propPosD, serializedObject.FindProperty("transitionTimeModifier"));

            serializedObject.ApplyModifiedProperties();
        }

        public override float GetLineHeight() => 4;
    }
}
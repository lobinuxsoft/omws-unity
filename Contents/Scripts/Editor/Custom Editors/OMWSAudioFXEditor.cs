using CryingOnion.OhMy.WeatherSystem.Data;
using UnityEditor;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.EditorScript
{
    [CustomEditor(typeof(OMWSAudioFX))]
    [CanEditMultipleObjects]
    public class OMWSAudioFXEditor : OMWSFXProfileEditor
    {
        void OnEnable() { }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

#if OMWS_WEATHER_FMOD
            EditorGUILayout.PropertyField(serializedObject.FindProperty("eventRef"));
#else
            EditorGUILayout.PropertyField(serializedObject.FindProperty("clip"));
#endif
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maximumVolume"));
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("transitionTimeModifier"));

            serializedObject.ApplyModifiedProperties();
        }

        public override void RenderInWindow(Rect pos)
        {
            float space = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            var propPosA = new Rect(pos.x, pos.y + space, pos.width, EditorGUIUtility.singleLineHeight);
            var propPosB = new Rect(pos.x, pos.y + space * 2, pos.width, EditorGUIUtility.singleLineHeight);

            var propPosD = new Rect(pos.x, pos.y + space * 3, pos.width, EditorGUIUtility.singleLineHeight);

            serializedObject.Update();

#if OMWS_WEATHER_FMOD
            EditorGUI.PropertyField(propPosA, serializedObject.FindProperty("eventRef"));
#else
            EditorGUI.PropertyField(propPosA, serializedObject.FindProperty("clip"));
#endif

            EditorGUI.PropertyField(propPosB, serializedObject.FindProperty("maximumVolume"));
            EditorGUI.PropertyField(propPosD, serializedObject.FindProperty("transitionTimeModifier"));

            serializedObject.ApplyModifiedProperties();
        }

        public override float GetLineHeight() => 3;
    }
}
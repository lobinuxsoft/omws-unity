using CryingOnion.OhMy.WeatherSystem.Data;
using UnityEditor;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.EditorScript
{
    [CustomEditor(typeof(OMWSEventFX))]
    [CanEditMultipleObjects]
    public class OMWSEventFXEditor : OMWSFXProfileEditor
    {
        void OnEnable() { }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.HelpBox("No other properties to adjust! Set events in the OMWS Event Module!", MessageType.Info);

            serializedObject.ApplyModifiedProperties();
        }

        public override void RenderInWindow(Rect pos) { }

        public override float GetLineHeight() => 0;
    }
}
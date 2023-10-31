using UnityEditor;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.EditorScript
{
    public class OMWSModuleEditor : Editor
    {
        void OnEnable() { }

        public virtual GUIContent GetGUIContent() => new GUIContent();

        public override void OnInspectorGUI() { }

        public virtual void DisplayInOMWSWindow()
        {
            serializedObject.Update();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
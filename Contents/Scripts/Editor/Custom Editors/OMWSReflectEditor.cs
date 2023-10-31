using CryingOnion.OhMy.WeatherSystem.Module;
using UnityEditor;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.EditorScript
{
    [CustomEditor(typeof(OMWSReflect))]
    [CanEditMultipleObjects]
    public class OMWSReflectEditor : OMWSModuleEditor
    {
        OMWSReflect reflect;

        public override GUIContent GetGUIContent() =>
            new GUIContent("", (Texture)Resources.Load("Reflections"), "Reflections: Sets up a cubemap for reflections with OMWS.");

        void OnEnable() { }

        public override void DisplayInOMWSWindow()
        {
            if (reflect == null)
                reflect = (OMWSReflect)target;

            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("framesBetweenRenders"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("reflectionCubemap"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("layerMask"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}
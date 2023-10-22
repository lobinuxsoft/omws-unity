using CryingOnion.OhMy.WeatherSystem.Core;
using CryingOnion.OhMy.WeatherSystem.Module;
using UnityEditor;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.EditorScript
{
    [CustomEditor(typeof(OMWSTVEModule))]
    [CanEditMultipleObjects]
    public class OMWSTVEIntegrationEditor : OMWSModuleEditor
    {
        SerializedProperty updateFrequency;
        OMWSTVEModule module;


        void OnEnable() { }

        public override GUIContent GetGUIContent() =>
            new GUIContent("    TVE Control", (Texture)Resources.Load("Integration"), "Links the OMWS system with the vegetation engine by BOXOPHOBIC.");

        public override void DisplayInOMWSWindow()
        {
            serializedObject.Update();

            if (module == null)
                module = (OMWSTVEModule)target;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("updateFrequency"));
            serializedObject.ApplyModifiedProperties();

#if THE_VEGETATION_ENGINE
            if (!module.globalControl || !module.globalMotion)
            {
                EditorGUILayout.Space(20);
                EditorGUILayout.HelpBox("Make sure that you have active TVE Global Motion and TVE Global Control objects in your scene!", MessageType.Warning);

            }
#else
            EditorGUILayout.Space(20);
            EditorGUILayout.HelpBox("The Vegetation Engine is not currently in this project! Please make sure that it has been properly downloaded before using this module.", MessageType.Warning);
#endif
        }
    }
}
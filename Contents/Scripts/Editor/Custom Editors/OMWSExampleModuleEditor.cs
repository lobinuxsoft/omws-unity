using CryingOnion.OhMy.WeatherSystem.Module;
using UnityEditor;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.EditorScript
{
    [CustomEditor(typeof(OMWSExampleModule))]
    [CanEditMultipleObjects]
    public class OMWSExampleModuleEditor : OMWSModuleEditor
    {
        public override GUIContent GetGUIContent()
        {
            //Place your module's GUI content here.
            return new GUIContent("", (Texture)Resources.Load("MoreOptions"), "Example Module: Empty module to be used as a base for custom modules.");
        }

        void OnEnable() { }

        public override void DisplayInOMWSWindow()
        {
            serializedObject.Update();

            //Place custom inspector code here.

            serializedObject.ApplyModifiedProperties();
        }
    }
}
using UnityEngine;
using CryingOnion.OhMy.WeatherSystem.Core;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CryingOnion.OhMy.WeatherSystem.Module
{
    public class OMWSExampleModule : OMWSModule
    {
        /*
        __________________________________________________________________

        This script shows an example of an empty module that you can use as a
        base for creating your own custom modules!
        _____________________________________________________________________
        */
    }

#if UNITY_EDITOR
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
#endif
}
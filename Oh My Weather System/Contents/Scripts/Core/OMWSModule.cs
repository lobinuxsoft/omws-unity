using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CryingOnion.OhMy.WeatherSystem.Core
{
    /// <summary>
    /// Weather Module that contains all necessary references and is used as a base class for all subsequent modules.
    /// </summary>
    public abstract class OMWSModule : MonoBehaviour
    {
        [HideInInspector]
        public OMWSWeather weatherSphere;

        public virtual void SetupModule()
        {
            if (!enabled)
                return;
            weatherSphere = OMWSWeather.instance;
        }

        private void OnDisable() => DisableModule();

        public virtual void DisableModule() { }
    }

#if UNITY_EDITOR
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
#endif
}
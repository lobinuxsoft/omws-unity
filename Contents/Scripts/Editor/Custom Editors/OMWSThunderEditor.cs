using CryingOnion.OhMy.WeatherSystem.Utility;
using UnityEditor;

namespace CryingOnion.OhMy.WeatherSystem.EditorScript
{
    [CustomEditor(typeof(OMWSThunder))]
    [CanEditMultipleObjects]
    public class OMWSThunderEditor : Editor
    {
        OMWSThunder omwsThunder;

        void OnEnable() => omwsThunder = (OMWSThunder)target;

        public override void OnInspectorGUI()
        {
            if (omwsThunder == null)
            {
                if (target)
                    omwsThunder = (OMWSThunder)target;
                else
                    return;
            }

            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ThunderSounds"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ThunderDelayRange"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_LightIntensity"));
            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
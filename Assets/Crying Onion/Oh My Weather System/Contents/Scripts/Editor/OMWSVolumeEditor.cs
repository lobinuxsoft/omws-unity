using CryingOnion.OhMy.WeatherSystem.Utility;
using UnityEditor;

namespace CryingOnion.OhMy.WeatherSystem.EditorScript
{
    [CustomEditor(typeof(OMWSVolume))]
    public class OMWSVolumeEditor : Editor
    {
        SerializedProperty triggerState;
        SerializedProperty triggerType;
        SerializedProperty setType;
        SerializedProperty tag;

        SerializedProperty weatherProfile;
        SerializedProperty atmosphereProfile;
        SerializedProperty eventRef;
        SerializedProperty ticks;
        SerializedProperty day;

        void OnEnable()
        {
            triggerState = serializedObject.FindProperty("m_TriggerState");
            triggerType = serializedObject.FindProperty("m_TriggerType");
            setType = serializedObject.FindProperty("m_SetType");
            tag = serializedObject.FindProperty("m_Tag");

            eventRef = serializedObject.FindProperty("m_Event");
            weatherProfile = serializedObject.FindProperty("m_WeatherProfile");
            atmosphereProfile = serializedObject.FindProperty("m_AtmosphereProfile");
            ticks = serializedObject.FindProperty("ticks");
            day = serializedObject.FindProperty("day");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(triggerState);
            tag.stringValue = EditorGUILayout.TagField("Collides With", tag.stringValue);
            EditorGUILayout.Space(20);
            EditorGUILayout.PropertyField(triggerType);

            EditorGUI.indentLevel++;

            switch (triggerType.enumValueIndex)
            {
                case (int)OMWSVolume.TriggerType.setWeather:

                    EditorGUILayout.PropertyField(weatherProfile);
                    EditorGUILayout.Space(10);
                    EditorGUILayout.PropertyField(setType);

                    if (setType.intValue == 1)
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_TransitionTime"));

                    break;

                case (int)OMWSVolume.TriggerType.triggerEvent:

                    EditorGUILayout.Space(10);
                    EditorGUILayout.PropertyField(eventRef);

                    break;

                case (int)OMWSVolume.TriggerType.setAtmosphere:

                    EditorGUILayout.PropertyField(atmosphereProfile);
                    EditorGUILayout.Space(10);
                    EditorGUILayout.PropertyField(setType);

                    if (setType.intValue == 1)
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_TransitionTime"));

                    break;

                case (int)OMWSVolume.TriggerType.setTicks:

                    EditorGUILayout.PropertyField(ticks);
                    EditorGUILayout.Space(10);
                    EditorGUILayout.PropertyField(setType);

                    if (setType.intValue == 1)
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_TransitionTime"));

                    break;

                case (int)OMWSVolume.TriggerType.setDay:

                    EditorGUILayout.PropertyField(day);
                    EditorGUILayout.Space(10);
                    EditorGUILayout.PropertyField(setType);

                    if (setType.intValue == 1)
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_TransitionTime"));

                    break;

                case (int)OMWSVolume.TriggerType.setAmbience:

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_AmbienceProfile"));
                    EditorGUILayout.Space(10);
                    EditorGUILayout.PropertyField(setType);

                    if (setType.intValue == 1)
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_TransitionTime"));

                    break;
            }

            EditorGUI.indentLevel--;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
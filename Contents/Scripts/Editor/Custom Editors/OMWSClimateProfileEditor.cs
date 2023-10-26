using CryingOnion.OhMy.WeatherSystem.Data;
using UnityEditor;

namespace CryingOnion.OhMy.WeatherSystem.EditorScript
{
    [CustomEditor(typeof(OMWSClimateProfile))]
    [CanEditMultipleObjects]
    public class OMWSClimateProfileEditor : Editor
    {
        SerializedProperty tempratureOverYear;
        SerializedProperty precipitationOverYear;
        SerializedProperty tempratureOverDay;
        SerializedProperty precipitationOverDay;
        SerializedProperty tempratureFilter;
        SerializedProperty precipitationFilter;
        OMWSClimateProfile prof;

        void OnEnable()
        {
            tempratureOverYear = serializedObject.FindProperty("tempratureOverYear");
            precipitationOverYear = serializedObject.FindProperty("precipitationOverYear");
            tempratureOverDay = serializedObject.FindProperty("tempratureOverDay");
            precipitationOverDay = serializedObject.FindProperty("precipitationOverDay");
            tempratureFilter = serializedObject.FindProperty("tempratureFilter");
            precipitationFilter = serializedObject.FindProperty("precipitationFilter");
            prof = (OMWSClimateProfile)target;
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            Undo.RecordObject(prof, prof.name + " Profile Changes");

            EditorGUILayout.LabelField("Global Curves", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(tempratureOverYear);
            EditorGUILayout.PropertyField(precipitationOverYear);
            EditorGUILayout.PropertyField(tempratureOverDay);
            EditorGUILayout.PropertyField(precipitationOverDay);
            EditorGUILayout.Space(20);

            EditorGUILayout.LabelField("Global Filters", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(tempratureFilter);
            EditorGUILayout.PropertyField(precipitationFilter);

            EditorGUILayout.Space();
            EditorUtility.SetDirty(prof);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
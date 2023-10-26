using CryingOnion.OhMy.WeatherSystem.Data;
using UnityEditor;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.EditorScript
{
    [CustomEditor(typeof(OMWSForecastProfile))]
    [CanEditMultipleObjects]
    public class OMWSForecastProfileEditor : Editor
    {
        SerializedProperty profilesToForecast;
        SerializedProperty forecastLength;
        SerializedProperty startWeatherWith;
        SerializedProperty startWithRandomWeather;
        CryingOnion.OhMy.WeatherSystem.Data.OMWSForecastProfile prof;
        Vector2 scrollPos;

        void OnEnable()
        {
            profilesToForecast = serializedObject.FindProperty("profilesToForecast");
            forecastLength = serializedObject.FindProperty("forecastLength");
            startWithRandomWeather = serializedObject.FindProperty("startWeatherWith");
            prof = (CryingOnion.OhMy.WeatherSystem.Data.OMWSForecastProfile)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(profilesToForecast);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(startWithRandomWeather);
            if (startWithRandomWeather.enumValueIndex == (int)CryingOnion.OhMy.WeatherSystem.Data.OMWSForecastProfile.StartWeatherWith.initialProfile)
                EditorGUILayout.PropertyField(serializedObject.FindProperty("initialProfile"));
            if (startWithRandomWeather.enumValueIndex == (int)CryingOnion.OhMy.WeatherSystem.Data.OMWSForecastProfile.StartWeatherWith.initialForecast)
                EditorGUILayout.PropertyField(serializedObject.FindProperty("initialForecast"), true);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(forecastLength, new GUIContent("Profiles to Forecast Ahead"));
            serializedObject.ApplyModifiedProperties();
        }
    }
}
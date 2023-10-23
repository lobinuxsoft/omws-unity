using System.Collections.Generic;
using CryingOnion.OhMy.WeatherSystem.Core;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.Data
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "CryingOnion/Oh My Weather System/Profile/Forecast Profile", order = 361)]
    public class OMWSForecastProfile : ScriptableObject
    {
        [Tooltip("The weather profiles that this profile will forecast.")]
        public List<OMWSWeatherProfile> profilesToForecast;

        [Tooltip("The weather profile that this profile will forecast initially.")]
        public OMWSWeatherProfile initialProfile;

        [Tooltip("The weather profiles that this profile will forecast initially.")]
        public List<OMWSEcosystem.OMWSWeatherPattern> initialForecast;

        public enum StartWeatherWith { random, initialProfile, initialForecast }
        public StartWeatherWith startWeatherWith;

        [Tooltip("The amount of weather profiles to forecast ahead.")]
        public int forecastLength;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(CryingOnion.OhMy.WeatherSystem.Data.OMWSForecastProfile))]
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
#endif
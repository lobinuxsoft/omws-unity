using UnityEngine;
using CryingOnion.OhMy.WeatherSystem.Core;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CryingOnion.OhMy.WeatherSystem.Data
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "CryingOnion/Oh My Weather System/Climate Profile", order = 361)]
    public class OMWSClimateProfile : ScriptableObject
    {
        [Tooltip("The global temprature during the year. the x-axis is the current day over the days in the year and the y axis is the temprature in farenheit.")]
        public AnimationCurve tempratureOverYear;

        [Tooltip("The global precipitation during the year. the x-axis is the current day over the days in the year and the y axis is the precipitation.")]
        public AnimationCurve precipitationOverYear;

        [Tooltip("The local temprature during the day. the x-axis is the current ticks over 360 and the y axis is the temprature change in farenheit from the global temprature.")]
        public AnimationCurve tempratureOverDay;

        [Tooltip("The local precipitation during the day. the x-axis is the current ticks over 360 and the y axis is the precipitation change from the global precipitation.")]
        public AnimationCurve precipitationOverDay;


        [Tooltip("Adds an offset to the global temprature. Useful for adding biomes or climate change by location or elevation")]
        public float tempratureFilter;

        [Tooltip("Adds an offset to the global precipitation. Useful for adding biomes or climate change by location or elevation")]
        public float precipitationFilter;

        public float GetTemperature(bool celsius)
        {
            OMWSWeather weather = OMWSWeather.instance;

            float i = (tempratureOverYear.Evaluate(weather.GetCurrentYearPercentage()) * tempratureOverDay.Evaluate(weather.GetCurrentDayPercentage())) + tempratureFilter;

            if (celsius)
                i = (i - 32) * 5 / 9;

            return i;
        }

        public float GetTemperature(bool celsius, OMWSWeather weather)
        {
            float i = (tempratureOverYear.Evaluate(weather.GetCurrentYearPercentage()) * tempratureOverDay.Evaluate(weather.GetCurrentDayPercentage())) + tempratureFilter;

            if (celsius)
                i = (i - 32) * 5 / 9;

            return i;
        }

        public float GetTemperature(bool celsius, OMWSWeather weather, float inTicks)
        {
            float nextDays = inTicks / weather.perennialProfile.ticksPerDay;

            float i = (tempratureOverYear.Evaluate((weather.DayAndTime() + nextDays) / weather.perennialProfile.daysPerYear) * tempratureOverDay.Evaluate(weather.GetCurrentDayPercentage())) + tempratureFilter;

            if (celsius)
                i = (i - 32) * 5 / 9;

            return i;
        }

        public float GetHumidity()
        {
            OMWSWeather weather = OMWSWeather.instance;

            float i = (precipitationOverYear.Evaluate(weather.GetCurrentYearPercentage()) * precipitationOverDay.Evaluate(weather.GetCurrentDayPercentage())) + precipitationFilter;

            return i;
        }

        public float GetHumidity(OMWSWeather weather)
        {
            float i = (precipitationOverYear.Evaluate(weather.GetCurrentYearPercentage()) * precipitationOverDay.Evaluate(weather.GetCurrentDayPercentage())) + precipitationFilter;

            return i;
        }

        public float GetHumidity(OMWSWeather weather, float inTicks)
        {
            float nextDays = inTicks / weather.perennialProfile.ticksPerDay;

            float i = (precipitationOverYear.Evaluate((weather.DayAndTime() + nextDays) / weather.perennialProfile.daysPerYear) * precipitationOverDay.Evaluate(weather.GetCurrentDayPercentage())) + precipitationFilter;

            return i;
        }
    }

#if UNITY_EDITOR
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
#endif
}
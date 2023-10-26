using CryingOnion.OhMy.WeatherSystem.Core;
using CryingOnion.OhMy.WeatherSystem.Data;
using UnityEditor;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.EditorScript
{
    [CustomEditor(typeof(OMWSWeatherProfile))]
    [CanEditMultipleObjects]
    public class OMWSWeatherProfileEditor : Editor
    {
        OMWSWeatherProfile prof;

        void OnEnable() => prof = (OMWSWeatherProfile)target;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Forecasting Behaviours", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            Rect position = EditorGUILayout.GetControlRect();
            float startPos = position.width / 2.75f;
            var titleRect = new Rect(position.x, position.y, 70, position.height);
            EditorGUI.PrefixLabel(titleRect, new GUIContent("Weather Length"));
            float min = serializedObject.FindProperty("weatherTime").vector2Value.x;
            float max = serializedObject.FindProperty("weatherTime").vector2Value.y;
            var label1Rect = new Rect();
            var label2Rect = new Rect();
            var sliderRect = new Rect();

            if (position.width > 359)
            {
                label1Rect = new Rect(startPos, position.y, 64, position.height);
                label2Rect = new Rect(position.width - 47, position.y, 64, position.height);
                sliderRect = new Rect(startPos + 56, position.y, (position.width - startPos) - 95, position.height);
                EditorGUI.MinMaxSlider(sliderRect, ref min, ref max, 0, 180);
            }
            else
            {
                label1Rect = new Rect(position.width - 110, position.y, 50, position.height);
                label2Rect = new Rect(position.width - 72, position.y, 50, position.height);
            }

            min = EditorGUI.FloatField(label1Rect, (Mathf.Round(min * 100) / 100));
            max = EditorGUI.FloatField(label2Rect, (Mathf.Round(max * 100) / 100));

            if (min > max)
                min = max;

            serializedObject.FindProperty("weatherTime").vector2Value = new Vector2(min, max);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("likelihood"));

            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("forecastModifierMethod"), true);

            switch ((OMWSWeatherProfile.ForecastModifierMethod)serializedObject.FindProperty("forecastModifierMethod").intValue)
            {
                default:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("forecastNext"), new GUIContent("Forecast Modifiers", "Modifies the weather profiles that follow this in the forecast. Use the dropdown to force the forecast to either choose only one of the included profiles to forecast next, or to avoid the selected profiles entirely."), true);
                    break;
                case (OMWSWeatherProfile.ForecastModifierMethod.DontForecastNext):
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("forecastNext"), new GUIContent("Don't Forecast Next", "The forecast module will not select any of these weather profiles to immediately follow this profile in the forecast."), true);
                    break;
                case (OMWSWeatherProfile.ForecastModifierMethod.forecastNext):
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("forecastNext"), new GUIContent("Forecast Next", "The forecast module will only select one of these weather profiles to immediately follow this profile in the forecast."), true);
                    break;
                case (OMWSWeatherProfile.ForecastModifierMethod.forecastAnyProfileNext):
                    break;

            }

            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("chances"), new GUIContent("Chance Effectors"), true);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space(20);
            EditorGUILayout.LabelField("Cloud Settings", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudSettings").FindPropertyRelative("cumulusCoverage"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudSettings").FindPropertyRelative("altocumulusCoverage"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudSettings").FindPropertyRelative("chemtrailCoverage"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudSettings").FindPropertyRelative("cirrostratusCoverage"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudSettings").FindPropertyRelative("nimbusCoverage"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudSettings").FindPropertyRelative("nimbusVariation"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudSettings").FindPropertyRelative("nimbusHeightEffect"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudSettings").FindPropertyRelative("borderHeight"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudSettings").FindPropertyRelative("borderVariation"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudSettings").FindPropertyRelative("borderEffect"));
            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("fogDensity"));

            EditorGUILayout.Space(20);
            EditorGUI.indentLevel--;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("FX"), new GUIContent("Weather Effects"));

            serializedObject.ApplyModifiedProperties();
        }

        public void DisplayInOMWSWindow(OMWSWeather t)
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Forecasting Behaviours", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            Rect position = EditorGUILayout.GetControlRect();
            float startPos = position.width / 2.75f;
            var titleRect = new Rect(position.x, position.y, 70, position.height);
            EditorGUI.PrefixLabel(titleRect, new GUIContent("Weather Length"));
            float min = serializedObject.FindProperty("weatherTime").vector2Value.x;
            float max = serializedObject.FindProperty("weatherTime").vector2Value.y;
            var label1Rect = new Rect();
            var label2Rect = new Rect();
            var sliderRect = new Rect();

            if (position.width > 359)
            {
                label1Rect = new Rect(startPos, position.y, 64, position.height);
                label2Rect = new Rect(position.width - 47, position.y, 64, position.height);
                sliderRect = new Rect(startPos + 56, position.y, (position.width - startPos) - 95, position.height);
                EditorGUI.MinMaxSlider(sliderRect, ref min, ref max, 0, 180);
            }
            else
            {
                label1Rect = new Rect(position.width - 110, position.y, 50, position.height);
                label2Rect = new Rect(position.width - 72, position.y, 50, position.height);
            }

            min = EditorGUI.FloatField(label1Rect, (Mathf.Round(min * 100) / 100));
            max = EditorGUI.FloatField(label2Rect, (Mathf.Round(max * 100) / 100));

            if (min > max)
                min = max;

            serializedObject.FindProperty("weatherTime").vector2Value = new Vector2(min, max);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("likelihood"));

            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("forecastModifierMethod"), true);
            switch ((OMWSWeatherProfile.ForecastModifierMethod)serializedObject.FindProperty("forecastModifierMethod").intValue)
            {
                default:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("forecastNext"), new GUIContent("Forecast Modifiers", "Modifies the weather profiles that follow this in the forecast. Use the dropdown to force the forecast to either choose only one of the included profiles to forecast next, or to avoid the selected profiles entirely."), true);
                    break;
                case (OMWSWeatherProfile.ForecastModifierMethod.DontForecastNext):
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("forecastNext"), new GUIContent("Don't Forecast Next", "The forecast module will not select any of these weather profiles to immediately follow this profile in the forecast."), true);
                    break;
                case (OMWSWeatherProfile.ForecastModifierMethod.forecastNext):
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("forecastNext"), new GUIContent("Forecast Next", "The forecast module will only select one of these weather profiles to immediately follow this profile in the forecast."), true);
                    break;
                case (OMWSWeatherProfile.ForecastModifierMethod.forecastAnyProfileNext):
                    break;
            }

            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("chances"), new GUIContent("Chance Effectors"), true);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space(20);
            EditorGUILayout.LabelField("Cloud Settings", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudSettings").FindPropertyRelative("cumulusCoverage"));

            if (t.cloudStyle == OMWSWeather.CloudStyle.omwsDesktop || t.cloudStyle == OMWSWeather.CloudStyle.paintedSkies || t.cloudStyle == OMWSWeather.CloudStyle.soft)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudSettings").FindPropertyRelative("altocumulusCoverage"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudSettings").FindPropertyRelative("chemtrailCoverage"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudSettings").FindPropertyRelative("cirrostratusCoverage"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudSettings").FindPropertyRelative("nimbusCoverage"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudSettings").FindPropertyRelative("nimbusVariation"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudSettings").FindPropertyRelative("nimbusHeightEffect"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudSettings").FindPropertyRelative("borderHeight"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudSettings").FindPropertyRelative("borderVariation"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudSettings").FindPropertyRelative("borderEffect"));
            }

            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("fogDensity"));

            EditorGUILayout.Space(20);
            EditorGUI.indentLevel--;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("FX"), new GUIContent("Weather Effects"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}
using CryingOnion.OhMy.WeatherSystem.Core;
using CryingOnion.OhMy.WeatherSystem.Utility;
using UnityEditor;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.EditorScript
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(OMWSBiome))]
    public class OMWSBiomeEditor : Editor
    {
        protected static bool weatherFoldout;
        protected static bool biomeFoldout;
        protected static bool boundsFoldout;
        protected static bool infoFoldout;
        OMWSBiome biome;
        OMWSWeather weatherSphere;

        private void OnEnable()
        {
            biome = (OMWSBiome)target;
            weatherSphere = OMWSWeather.instance;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.indentLevel = 0;

            GUIStyle foldoutStyle = new GUIStyle(GUI.skin.GetStyle("toolbarPopup"));
            foldoutStyle.fontStyle = FontStyle.Bold;
            foldoutStyle.margin = new RectOffset(30, 10, 5, 5);


            weatherFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(weatherFoldout, "   Weather and Forecast", foldoutStyle);
            EditorGUILayout.EndFoldoutHeaderGroup();

            if (weatherFoldout)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("weatherSelectionMode"));

                EditorGUILayout.Space();

                if ((OMWSEcosystem.EcosystemStyle)serializedObject.FindProperty("weatherSelectionMode").intValue == OMWSEcosystem.EcosystemStyle.forecast)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("currentWeather"));
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("forecastProfile"));

                    EditorGUILayout.Space();

                    EditorGUI.indentLevel++;

                    if (serializedObject.FindProperty("forecastProfile").objectReferenceValue)
                        CreateEditor(serializedObject.FindProperty("forecastProfile").objectReferenceValue).OnInspectorGUI();
                    else
                        EditorGUILayout.HelpBox("Assign a forecast profile!", MessageType.Error);

                    EditorGUI.indentLevel--;
                }
                else
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("currentWeather"));


                EditorGUILayout.Space();

                EditorGUI.indentLevel--;
            }

            biomeFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(biomeFoldout, "   Climate", foldoutStyle);
            EditorGUILayout.EndFoldoutHeaderGroup();

            if (biomeFoldout)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("climateProfile"));
                EditorGUILayout.Space();

                EditorGUI.indentLevel++;

                if (serializedObject.FindProperty("climateProfile").objectReferenceValue)
                    CreateEditor(serializedObject.FindProperty("climateProfile").objectReferenceValue).OnInspectorGUI();
                else
                    EditorGUILayout.HelpBox("Assign a climate profile!", MessageType.Error);

                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("localTemperatureFilter"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("localPrecipitationFilter"));
                EditorGUI.indentLevel--;
            }

            boundsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(boundsFoldout, "   Bounds and Trigger", foldoutStyle);
            EditorGUILayout.EndFoldoutHeaderGroup();

            if (boundsFoldout)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("bounds"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("height"));
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("displayColor"));
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("transitionDistance"));
                EditorGUI.indentLevel--;
            }

            infoFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(infoFoldout, "   Current Information", foldoutStyle);
            EditorGUILayout.EndFoldoutHeaderGroup();

            if (infoFoldout)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.HelpBox("Currently it is " + Mathf.Round(biome.currentTemperature) + "F or " + Mathf.Round(biome.currentTemperatureCelsius) + "C with a precipitation chance of " + Mathf.Round(biome.currentPrecipitation) + "%.\n" +
                "Temperatures will " + (biome.currentTemperature > biome.GetTemperature(false, weatherSphere.perennialProfile.ticksPerDay) ? "drop" : "rise") + " tomorrow, bringing the temprature to " + Mathf.Round(biome.GetTemperature(false, weatherSphere.perennialProfile.ticksPerDay)) + "F", MessageType.None);
                EditorGUILayout.Space();


                if (biome.currentForecast.Count == 0)
                    EditorGUILayout.HelpBox("No forecast information yet!", MessageType.None);
                else
                {
                    EditorGUILayout.HelpBox("Currently it is " + biome.weatherSphere.currentWeather.name, MessageType.None);

                    for (int i = 0; i < biome.currentForecast.Count; i++)
                    {
                        EditorGUILayout.HelpBox("Starting at " + biome.weatherSphere.perennialProfile.FormatTime(false, biome.currentForecast[i].startTicks) + " the weather will change to " +
                            biome.currentForecast[i].profile.name + " for " + Mathf.Round(biome.currentForecast[i].weatherProfileDuration) +
                            " ticks or unitl " + biome.weatherSphere.perennialProfile.FormatTime(false, biome.currentForecast[i].endTicks) + ".", MessageType.None, true);

                        EditorGUILayout.Space(2);
                    }
                }

                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
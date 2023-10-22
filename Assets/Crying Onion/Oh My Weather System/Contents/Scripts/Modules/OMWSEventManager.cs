using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using CryingOnion.OhMy.WeatherSystem.Core;
using CryingOnion.OhMy.WeatherSystem.Data;
using CryingOnion.OhMy.WeatherSystem.Utility;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CryingOnion.OhMy.WeatherSystem.Module
{
    public class OMWSEventManager : OMWSModule
    {
        public UnityEvent onMorning;
        public UnityEvent onNoon;
        public UnityEvent onEvening;
        public UnityEvent onMidnight;
        public UnityEvent onNewTick;
        public UnityEvent onNewHour;
        public UnityEvent onNewDay;
        public UnityEvent onNewYear;
        public UnityEvent onWeatherProfileChange;

        [System.Serializable]
        public class OMWSEvent
        {
            public OMWSEventFX fxReference;
            public UnityEvent onPlay;
            public UnityEvent onStop;
        }

        public List<OMWSEvent> omwsEvents;
        public static List<OMWSEventFX> allEventTypes;
        public static List<OMWSEventFX> allEventTypesRef;

        void Awake()
        {
            if (Application.isPlaying)
            {
                foreach (OMWSEvent i in omwsEvents)
                {
                    i.fxReference.playing = false;

                    if (i.fxReference)
                    {
                        i.fxReference.onCall += i.onPlay.Invoke;
                        i.fxReference.onEnd += i.onStop.Invoke;
                    }
                }
            }
        }


        private void OnEnable()
        {

            if (GetComponent<OMWSWeather>())
            {
                GetComponent<OMWSWeather>().IntitializeModule(typeof(OMWSEventManager));
                DestroyImmediate(this);
                Debug.LogWarning("Add modules in the settings tab in OMWS 2!");
                return;
            }

            StartCoroutine(Refresh());
        }

        public IEnumerator Refresh()
        {
            yield return new WaitForEndOfFrame();

            OMWSWeather.Events.onMorning += onMorning.Invoke;
            OMWSWeather.Events.onNoon += onNoon.Invoke;
            OMWSWeather.Events.onEvening += onEvening.Invoke;
            OMWSWeather.Events.onMidnight += onMidnight.Invoke;
            OMWSWeather.Events.onNewTick += onNewTick.Invoke;
            OMWSWeather.Events.onNewHour += onNewHour.Invoke;
            OMWSWeather.Events.onNewDay += onNewDay.Invoke;
            OMWSWeather.Events.onNewYear += onNewYear.Invoke;
            OMWSWeather.Events.onWeatherChange += onWeatherProfileChange.Invoke;

        }

        private void OnDisable()
        {
            OMWSWeather.Events.onMorning -= onMorning.Invoke;
            OMWSWeather.Events.onNoon -= onNoon.Invoke;
            OMWSWeather.Events.onEvening -= onEvening.Invoke;
            OMWSWeather.Events.onMidnight -= onMidnight.Invoke;
            OMWSWeather.Events.onNewTick -= onNewTick.Invoke;
            OMWSWeather.Events.onNewHour -= onNewHour.Invoke;
            OMWSWeather.Events.onNewDay -= onNewDay.Invoke;
            OMWSWeather.Events.onNewYear -= onNewYear.Invoke;
            OMWSWeather.Events.onWeatherChange -= onWeatherProfileChange.Invoke;
        }

        public void LogConsoleEvent() => Debug.Log("Test Event Passed.");

        public void LogConsoleEvent(string log) => Debug.Log($"Test Event Passed. Log: {log}");
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(OMWSEventManager))]
    [CanEditMultipleObjects]
    public class OMWSEventManagerEditor : OMWSModuleEditor
    {
        protected static bool todEvents;
        protected static bool teEvents;
        protected static bool weatherEvents;
        protected static bool eventSettings;

        public override GUIContent GetGUIContent() => new GUIContent("    Events", (Texture)Resources.Load("Events"), "Setup Unity events that directly integrate into the OMWS system.");

        void OnEnable() { }

        public override void DisplayInOMWSWindow()
        {
            serializedObject.Update();

            todEvents = EditorGUILayout.BeginFoldoutHeaderGroup(todEvents,
                    new GUIContent("    Time of Day Events"), OMWSEditorUtilities.FoldoutStyle());

            EditorGUILayout.EndFoldoutHeaderGroup();
            if (todEvents)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onMorning"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onNoon"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onEvening"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onMidnight"));
                EditorGUI.indentLevel--;
            }

            teEvents = EditorGUILayout.BeginFoldoutHeaderGroup(teEvents,
                new GUIContent("    Time Elapsed Events"), OMWSEditorUtilities.FoldoutStyle());

            EditorGUILayout.EndFoldoutHeaderGroup();
            if (teEvents)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onNewTick"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onNewHour"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onNewDay"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onNewYear"));
                EditorGUI.indentLevel--;
            }

            weatherEvents = EditorGUILayout.BeginFoldoutHeaderGroup(weatherEvents,
                new GUIContent("    Weather Events"), OMWSEditorUtilities.FoldoutStyle());

            EditorGUILayout.EndFoldoutHeaderGroup();
            if (weatherEvents)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onWeatherProfileChange"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("omwsEvents"), new GUIContent("Event FX"));

                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
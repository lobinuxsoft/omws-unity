using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using CryingOnion.OhMy.WeatherSystem.Core;
using CryingOnion.OhMy.WeatherSystem.Data;

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
                Debug.LogWarning("Add modules in the settings tab in OMWS!");
                return;
            }

            StartCoroutine(Refresh());
        }

        public IEnumerator Refresh()
        {
            yield return new WaitForEndOfFrame();

            OMWSWeather.OMWSEvents.onMorning += onMorning.Invoke;
            OMWSWeather.OMWSEvents.onNoon += onNoon.Invoke;
            OMWSWeather.OMWSEvents.onEvening += onEvening.Invoke;
            OMWSWeather.OMWSEvents.onMidnight += onMidnight.Invoke;
            OMWSWeather.OMWSEvents.onNewTick += onNewTick.Invoke;
            OMWSWeather.OMWSEvents.onNewHour += onNewHour.Invoke;
            OMWSWeather.OMWSEvents.onNewDay += onNewDay.Invoke;
            OMWSWeather.OMWSEvents.onNewYear += onNewYear.Invoke;
            OMWSWeather.OMWSEvents.onWeatherChange += onWeatherProfileChange.Invoke;
        }

        private void OnDisable()
        {
            OMWSWeather.OMWSEvents.onMorning -= onMorning.Invoke;
            OMWSWeather.OMWSEvents.onNoon -= onNoon.Invoke;
            OMWSWeather.OMWSEvents.onEvening -= onEvening.Invoke;
            OMWSWeather.OMWSEvents.onMidnight -= onMidnight.Invoke;
            OMWSWeather.OMWSEvents.onNewTick -= onNewTick.Invoke;
            OMWSWeather.OMWSEvents.onNewHour -= onNewHour.Invoke;
            OMWSWeather.OMWSEvents.onNewDay -= onNewDay.Invoke;
            OMWSWeather.OMWSEvents.onNewYear -= onNewYear.Invoke;
            OMWSWeather.OMWSEvents.onWeatherChange -= onWeatherProfileChange.Invoke;
        }

        public void LogConsoleEvent() => Debug.Log("Test Event Passed.");

        public void LogConsoleEvent(string log) => Debug.Log($"Test Event Passed. Log: {log}");
    }
}
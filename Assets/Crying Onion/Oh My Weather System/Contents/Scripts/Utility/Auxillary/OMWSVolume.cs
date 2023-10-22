﻿using CryingOnion.OhMy.WeatherSystem.Core;
using CryingOnion.OhMy.WeatherSystem.Data;
using CryingOnion.OhMy.WeatherSystem.Module;
using UnityEngine;
using UnityEngine.Events;


namespace CryingOnion.OhMy.WeatherSystem.Utility
{
    [RequireComponent(typeof(Collider))]
    public class OMWSVolume : MonoBehaviour
    {
        public enum TriggerType { setWeather, triggerEvent, setTicks, setDay, setAtmosphere, setAmbience }
        public enum SetType { setInstantly, transition }
        public enum TriggerState { onEnter, onStay, onExit }

        [SerializeField]
        private TriggerType m_TriggerType;
        [SerializeField]
        private TriggerState m_TriggerState;
        [SerializeField]
        private SetType m_SetType;
        [SerializeField]
        private string m_Tag = "Untagged";
        private OMWSWeather m_CozyWeather;

        [SerializeField] private OMWSWeatherProfile m_WeatherProfile;
        [SerializeField] private float m_TransitionTime;
        [SerializeField] private UnityEvent m_Event;
        [SerializeField] private OMWSAtmosphereProfile m_AtmosphereProfile;
        [SerializeField] private OMWSAmbienceProfile m_AmbienceProfile;
        [SerializeField] private float ticks;
        [SerializeField] private int day;
        [SerializeField] private float transitionTime;

        private void Awake() => m_CozyWeather = OMWSWeather.instance;

        public void Run()
        {
            if (m_SetType == SetType.setInstantly)
                Set();
            else
                Transition();
        }

        public void Transition()
        {
            switch (m_TriggerType)
            {
                case (TriggerType.setWeather):
                    m_CozyWeather.SetWeather(m_WeatherProfile, m_TransitionTime);
                    break;
                case (TriggerType.triggerEvent):
                    m_Event.Invoke();
                    break;
                case (TriggerType.setAtmosphere):
                    m_CozyWeather.ChangeAtmosphere(m_AtmosphereProfile, m_AtmosphereProfile, m_TransitionTime);
                    break;
                case (TriggerType.setDay):
                    m_CozyWeather.TransitionTime(ticks, day);
                    break;
                case (TriggerType.setTicks):
                    m_CozyWeather.TransitionTime(ticks, m_CozyWeather.currentDay);
                    break;
                case (TriggerType.setAmbience):
                    m_CozyWeather.GetModule<OMWSAmbienceManager>().SetAmbience(m_AmbienceProfile, m_TransitionTime);
                    break;
            }
        }

        public void Set()
        {
            switch (m_TriggerType)
            {
                case (TriggerType.setWeather):
                    m_CozyWeather.currentWeather = m_WeatherProfile;
                    break;
                case (TriggerType.triggerEvent):
                    m_Event.Invoke();
                    break;
                case (TriggerType.setAtmosphere):
                    m_CozyWeather.atmosphereProfile = m_AtmosphereProfile;
                    m_CozyWeather.ResetQuality();
                    break;
                case (TriggerType.setDay):
                    m_CozyWeather.currentDay = day;
                    break;
                case (TriggerType.setTicks):
                    m_CozyWeather.currentTicks = ticks;
                    break;
                case (TriggerType.setAmbience):
                    if (m_CozyWeather.GetModule<OMWSAmbienceManager>())
                        m_CozyWeather.GetModule<OMWSAmbienceManager>().SetAmbience(m_AmbienceProfile, 0);
                    break;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (m_TriggerState != TriggerState.onEnter)
                return;

            if (other.gameObject.tag == m_Tag)
                Run();
        }

        private void OnTriggerStay(Collider other)
        {
            if (m_TriggerState != TriggerState.onStay)
                return;

            if (other.gameObject.tag == m_Tag)
                Run();
        }

        private void OnTriggerExit(Collider other)
        {
            if (m_TriggerState != TriggerState.onExit)
                return;

            if (other.gameObject.tag == m_Tag)
                Run();
        }
    }
}
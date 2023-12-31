﻿using CryingOnion.OhMy.WeatherSystem.Core;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.Module
{
    [ExecuteAlways]
    public class OMWSMicrosplatModule : OMWSModule
    {
        public enum UpdateFrequency { everyFrame, onAwake, viaScripting }
        public UpdateFrequency updateFrequency;

        [Header("Wetness")]
        public bool updateWetness = true;

        [Range(0f, 1f)] public float minWetness = 0f;

        [Range(0f, 1f)] public float maxWetness = 1f;

        [Header("Rain Ripples")]
        public bool updateRainRipples = true;

        [Header("Puddle Settings")]
        public bool updatePuddles = true;

        [Header("Stream Settings")]
        public bool updateStreams = true;

        [Header("Snow Settings")]
        public bool updateSnow = true;

        [Header("Wind Settings")]
        public bool updateWindStrength = true;

        private static readonly int GlobalSnowLevel = Shader.PropertyToID("_Global_SnowLevel");
        private static readonly int GlobalWetnessParams = Shader.PropertyToID("_Global_WetnessParams");
        private static readonly int GlobalPuddleParams = Shader.PropertyToID("_Global_PuddleParams");
        private static readonly int GlobalRainIntensity = Shader.PropertyToID("_Global_RainIntensity");
        private static readonly int GlobalStreamMax = Shader.PropertyToID("_Global_StreamMax");
        private static readonly int GlobalWindParticulateStrength = Shader.PropertyToID("_Global_WindParticulateStrength");
        private static readonly int GlobalSnowParticulateStrength = Shader.PropertyToID("_Global_SnowParticulateStrength");

        // Start is called before the first frame update
        private void OnEnable()
        {
            if (GetComponent<OMWSWeather>())
            {
                GetComponent<OMWSWeather>().IntitializeModule(typeof(OMWSMicrosplatModule));
                DestroyImmediate(this);
                Debug.LogWarning("Add modules in the settings tab in OMWS!");
                return;
            }

            SetupModule();

            if (updateFrequency == UpdateFrequency.onAwake)
                UpdateShaderProperties();
        }

        // Update is called once per frame
        private void Update()
        {
            if (updateFrequency == UpdateFrequency.everyFrame)
                UpdateShaderProperties();
        }

        public void UpdateShaderProperties()
        {

            if (weatherSphere.omwsMaterials)
            {
                if (updateSnow)
                    Shader.SetGlobalFloat(GlobalSnowLevel, weatherSphere.omwsMaterials.m_SnowAmount);

                if (updateWetness)
                {
                    float currentWetness = Mathf.Clamp(weatherSphere.omwsMaterials.m_Wetness, minWetness, maxWetness);
                    Shader.SetGlobalVector(GlobalWetnessParams, new Vector2(minWetness, currentWetness));
                }

                if (updatePuddles)
                    Shader.SetGlobalFloat(GlobalPuddleParams, weatherSphere.omwsMaterials.m_Wetness);

                if (updateRainRipples)
                    Shader.SetGlobalFloat(GlobalRainIntensity, weatherSphere.omwsMaterials.m_Wetness);

                if (updateStreams)
                    Shader.SetGlobalFloat(GlobalStreamMax, weatherSphere.omwsMaterials.m_Wetness);
            }

            if (weatherSphere.VFX)
            {
                if (updateWindStrength)
                    Shader.SetGlobalFloat(GlobalWindParticulateStrength, weatherSphere.VFX.windManager.windSpeed);

                if (updateSnow && updateWindStrength)
                    Shader.SetGlobalFloat(GlobalSnowParticulateStrength, weatherSphere.VFX.windManager.windSpeed);
            }
        }
    }
}
using CryingOnion.OhMy.WeatherSystem.Core;
using CryingOnion.OhMy.WeatherSystem.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.Data
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Crying Onion/Oh My Weather System/Weather Profile", order = 361)]
    public class OMWSWeatherProfile : ScriptableObject
    {
        [Tooltip("Specifies the minimum (x) and maximum (y) length for this weather profile.")]
        public Vector2 weatherTime = new Vector2(120, 480);

        [Tooltip("Multiplier for the computational chance that this weather profile will play; 0 being never, and 2 being twice as likely as the average.")]
        [Range(0, 2)] public float likelihood = 1;

        [OMWSHideTitle]
        [Tooltip("Allow only these weather profiles to immediately follow this weather profile in a forecast.")]
        public OMWSWeatherProfile[] forecastNext;

        public enum ForecastModifierMethod { forecastNext, DontForecastNext, forecastAnyProfileNext }
        public ForecastModifierMethod forecastModifierMethod = ForecastModifierMethod.forecastAnyProfileNext;

        [Tooltip("Animation curves that increase or decrease weather chance based on time, temprature, etc.")]
        [OMWSChanceEffector] public List<OMWSChanceEffector> chances;

        public CloudSettings cloudSettings;

        [Tooltip("The density of fog for this weather profile.")]
        [Range(0.1f, 5)] public float fogDensity = 1;

        [OMWSFX] public OMWSFXProfile[] FX;

        [System.Serializable]
        public class CloudSettings
        {
            [Tooltip("Multiplier for cumulus clouds.")]
            [Range(0, 2)]
            public float cumulusCoverage = 1;
            [Space(5)]
            [Tooltip("Multiplier for altocumulus clouds.")]
            [Range(0, 2)]
            public float altocumulusCoverage = 0;
            [Tooltip("Multiplier for chemtrails.")]
            [Range(0, 2)]
            public float chemtrailCoverage = 0;
            [Tooltip("Multiplier for cirrostratus clouds.")]
            [Range(0, 2)]
            public float cirrostratusCoverage = 0;
            [Tooltip("Multiplier for cirrus clouds.")]
            [Range(0, 2)]
            public float cirrusCoverage = 0;
            [Tooltip("Multiplier for nimbus clouds.")]
            [Space(5)]
            [Range(0, 2)]
            public float nimbusCoverage = 0;
            [Tooltip("Variation for nimbus clouds.")]
            [Range(0, 1)]
            public float nimbusVariation = 0.9f;
            [Tooltip("Height mask effect for nimbus clouds.")]
            [Range(0, 1)]
            public float nimbusHeightEffect = 1;

            [Space(5)]
            [Tooltip("Starting height for cloud border.")]
            [Range(0, 1)]
            public float borderHeight = 0.5f;
            [Tooltip("Variation for cloud border.")]
            [Range(0, 1)]
            public float borderVariation = 0.9f;
            [Tooltip("Multiplier for the border. Values below zero clip the clouds whereas values above zero add clouds.")]
            [Range(-1, 1)]
            public float borderEffect = 1;
        }

        public float GetChance(float temp, float precip, float yearPercent, float time, float snow, float rain)
        {
            float i = likelihood;

            foreach (OMWSChanceEffector j in chances)
                i *= j.GetChance(temp, precip, yearPercent, time, snow, rain);

            return i > 0 ? i : 0;
        }

        public float GetChance(OMWSWeather weather)
        {
            float i = likelihood;

            foreach (OMWSChanceEffector j in chances)
                i *= j.GetChance(weather);

            return i > 0 ? i : 0;
        }


        public void SetWeatherWeight(float weightVal)
        {
            foreach (OMWSFXProfile fx in FX)
                if (fx != null)
                    fx.PlayEffect(weightVal);
        }

        public void StopWeather()
        {
            foreach (OMWSFXProfile fx in FX)
                fx.StopEffect();
        }
    }
}
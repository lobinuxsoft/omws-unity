using CryingOnion.OhMy.WeatherSystem.Core;
using System.Collections.Generic;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.Data
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Crying Onion/Oh My Weather System/Profile/Forecast Profile", order = 361)]
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
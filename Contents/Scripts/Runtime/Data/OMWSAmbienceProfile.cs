using CryingOnion.OhMy.WeatherSystem.Core;
using CryingOnion.OhMy.WeatherSystem.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.Data
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Crying Onion/Oh My Weather System/Ambience Profile", order = 361)]
    public class OMWSAmbienceProfile : ScriptableObject
    {
        [Tooltip("Specifies the minimum (x) and maximum (y) length for this ambience profile.")]
        public Vector2 playTime = new Vector2(30, 60);

        [Tooltip("Multiplier for the computational chance that this ambience profile will play; 0 being never, and 2 being twice as likely as the average.")]
        [Range(0, 2)] public float likelihood = 1;

        [OMWSHideTitle(2)]
        public OMWSWeatherProfile[] dontPlayDuring;

        [OMWSChanceEffector]
        public List<OMWSChanceEffector> chances;


        [OMWSFX]
        public OMWSFXProfile[] FX;

        [Range(0, 1)] public float FXVolume = 1;

        public bool useVFX;

        public float GetChance(OMWSWeather weather)
        {
            float i = likelihood;

            foreach (OMWSChanceEffector j in chances)
                i *= j.GetChance(weather);

            return Mathf.Clamp(i, 0, 1000000);
        }

        public void SetWeight(float weightVal)
        {
            foreach (OMWSFXProfile fx in FX)
                fx.PlayEffect(weightVal);
        }

        public void Stop()
        {
            foreach (OMWSFXProfile fx in FX)
                fx.StopEffect();
        }

    }
}
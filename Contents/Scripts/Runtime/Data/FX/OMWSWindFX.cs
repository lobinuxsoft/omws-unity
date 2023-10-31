using CryingOnion.OhMy.WeatherSystem.Core;
using CryingOnion.OhMy.WeatherSystem.Module;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.Data
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Crying Onion/Oh My Weather System/FX/Wind FX", order = 361)]
    public class OMWSWindFX : OMWSFXProfile
    {
        [Range(0, 2)] public float windAmount;
        [Range(0, 2)] public float windSpeed;
        public float weight;
        OMWSWeather weather;

        public override void PlayEffect() => weight = 1;

        public override void PlayEffect(float i) => weight = Mathf.Clamp01(transitionTimeModifier.Evaluate(i));

        public override void StopEffect() => weight = 0;

        public override bool InitializeEffect(OMWSVFXModule VFX)
        {
            if (VFX == null)
                VFX = OMWSWeather.instance.VFX;

            VFXMod = VFX;

            if (!VFX.filterManager.isEnabled)
                return false;

            if (VFX)
            {
                VFX.windManager.windFXes.Add(this);
                weather = VFX.weatherSphere;
            }

            return true;
        }
    }
}
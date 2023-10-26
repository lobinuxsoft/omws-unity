using CryingOnion.OhMy.WeatherSystem.Core;
using CryingOnion.OhMy.WeatherSystem.Module;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.Data
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Crying Onion/Oh My Weather System/FX/Precipitation FX", order = 361)]
    public class OMWSPrecipitationFX : OMWSFXProfile
    {
        [Range(0, 0.05f)] public float rainAmount;
        [Range(0, 0.05f)] public float snowAmount;
        public float weight;
        OMWSWeather weather;

        public override void PlayEffect()
        {
            if (!VFXMod)
                if (InitializeEffect(null) == false)
                    return;

            if (VFXMod.precipitationManager.isEnabled)
                weight = 1;
            else
                weight = 0;
        }

        public override void PlayEffect(float i)
        {
            if (!VFXMod)
                if (InitializeEffect(null) == false)
                    return;

            if (VFXMod.precipitationManager.isEnabled)
                weight = 1 * Mathf.Clamp01(transitionTimeModifier.Evaluate(i));
            else
                weight = 0;
        }

        public override void StopEffect() => weight = 0;

        public override bool InitializeEffect(OMWSVFXModule VFX)
        {
            if (VFX == null)
                VFX = OMWSWeather.instance.VFX;

            VFXMod = VFX;

            if (!VFX.precipitationManager.isEnabled)
                return false;

            VFX.precipitationManager.precipitationFXes.Add(this);
            weather = VFX.weatherSphere;

            return true;
        }
    }
}
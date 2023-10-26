using CryingOnion.OhMy.WeatherSystem.Core;
using CryingOnion.OhMy.WeatherSystem.Module;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.Data
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Crying Onion/Oh My Weather System/FX/Filter FX", order = 361)]
    public class OMWSFilterFX : OMWSFXProfile
    {
        [Range(-1, 1)] public float filterSaturation;
        [Range(-1, 1)] public float filterValue;
        [ColorUsage(false, true)] public Color filterColor = Color.white;
        [ColorUsage(false, true)] public Color sunFilter = Color.white;
        [ColorUsage(false, true)] public Color cloudFilter = Color.white;
        public float weight;
        OMWSWeather weather;

        public override void PlayEffect()
        {
            if (!VFXMod)
                if (InitializeEffect(null) == false)
                    return;

            if (VFXMod.filterManager.isEnabled)
                weight = 1;
            else
                weight = 0;
            weather.CalculateFilterColors();
        }

        public override void PlayEffect(float i)
        {
            if (!VFXMod)
                if (InitializeEffect(null) == false)
                    return;

            if (VFXMod.filterManager.isEnabled)
                weight = Mathf.Clamp01(transitionTimeModifier.Evaluate(i));
            else
                weight = 0;

            weather.CalculateFilterColors();
        }

        public override void StopEffect()
        {
            if (!VFXMod)
                if (InitializeEffect(null) == false)
                    return;

            weight = 0;
            weather.CalculateFilterColors();
        }

        public override bool InitializeEffect(OMWSVFXModule VFX)
        {
            if (VFX == null)
                VFX = OMWSWeather.instance.VFX;

            VFXMod = VFX;

            if (!VFX.filterManager.isEnabled)
                return false;

            VFX.weatherSphere.possibleFilters.Add(this);
            weather = VFX.weatherSphere;
            StopEffect();

            return true;
        }
    }
}
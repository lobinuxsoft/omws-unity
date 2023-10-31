using UnityEngine;
using System.Collections.Generic;
using CryingOnion.OhMy.WeatherSystem.Core;
using CryingOnion.OhMy.WeatherSystem.Utility;
using CryingOnion.OhMy.WeatherSystem.Module;

namespace CryingOnion.OhMy.WeatherSystem.Data
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Crying Onion/Oh My Weather System/FX/Multi FX", order = 361)]
    public class OMWSMultiFXProfile : OMWSFXProfile
    {
        public OMWSWeather weather;

        [System.Serializable]
        public class OMWSMultiFXType
        {
            public OMWSFXProfile FX;
            public AnimationCurve intensityCurve;
        }

        [OMWSMultiAudio] public List<OMWSMultiFXType> multiFX;

        public override void PlayEffect()
        {
            if (weather == null)
                weather = OMWSWeather.instance;

            foreach (OMWSMultiFXType i in multiFX)
                i.FX.PlayEffect();
        }

        public override void PlayEffect(float weight)
        {
            if (weather == null)
                weather = OMWSWeather.instance;

            foreach (OMWSMultiFXType i in multiFX)
                i.FX.PlayEffect(i.intensityCurve.Evaluate(weather.GetCurrentDayPercentage()) * weight);
        }

        public override void StopEffect()
        {
            if (weather == null)
                weather = OMWSWeather.instance;

            foreach (OMWSMultiFXType i in multiFX)
                i.FX.StopEffect();
        }

        public override bool InitializeEffect(OMWSVFXModule VFX)
        {
            if (VFX == null)
                VFX = OMWSWeather.instance.VFX;

            VFXMod = VFX;

            weather = VFX.weatherSphere;

            foreach (OMWSMultiFXType i in multiFX)
                i.FX.InitializeEffect(VFX);

            return true;
        }
    }
}
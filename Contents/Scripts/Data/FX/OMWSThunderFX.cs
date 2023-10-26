using CryingOnion.OhMy.WeatherSystem.Core;
using CryingOnion.OhMy.WeatherSystem.Module;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.Data
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Crying Onion/Oh My Weather System/FX/Thunder FX", order = 361)]
    public class OMWSThunderFX : OMWSFXProfile
    {
        public Vector2 timeBetweenStrikes;
        public float weight;
        OMWSThunderManager thunderManager;

        public override void PlayEffect()
        {
            if (!VFXMod)
                if (InitializeEffect(null) == false)
                    return;

            if (VFXMod.thunderManager.isEnabled)
                weight = 1;
            else
                weight = 0;
        }

        public override void PlayEffect(float i)
        {
            if (!VFXMod)
                if (InitializeEffect(null) == false)
                    return;

            if (VFXMod.thunderManager.isEnabled)
                weight = i;
            else
                weight = 0;
        }

        public override void StopEffect() => weight = 0;

        public override bool InitializeEffect(OMWSVFXModule VFX)
        {
            if (VFX == null)
                VFX = OMWSWeather.instance.VFX;

            VFXMod = VFX;

            if (!VFX.thunderManager.isEnabled)
                return false;

            thunderManager = VFX.thunderManager;
            thunderManager.thunderFX.Add(this);

            return true;
        }
    }
}
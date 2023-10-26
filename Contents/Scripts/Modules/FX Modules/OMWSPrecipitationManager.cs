using CryingOnion.OhMy.WeatherSystem.Data;
using System.Collections.Generic;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.Module
{
    [System.Serializable]
    public class OMWSPrecipitationManager : OMWSFXModule
    {
        [Range(0, 2)] public float accumulationMultiplier = 1;
        public List<OMWSPrecipitationFX> precipitationFXes = new List<OMWSPrecipitationFX>();

        public override void OnFXEnable()
        {
            if (!vfx.weatherSphere.omwsMaterials)
                vfx.weatherSphere.omwsMaterials = vfx.weatherSphere.GetModule<OMWSMaterialManager>();
        }

        public override void OnFXUpdate()
        {
            if (vfx.weatherSphere.omwsMaterials == null)
                return;

            float snowSpeed = 0;
            float rainSpeed = 0;

            foreach (OMWSPrecipitationFX j in precipitationFXes)
            {
                if (j.weight > 0)
                {
                    snowSpeed += j.snowAmount * j.weight * accumulationMultiplier;
                    rainSpeed += j.rainAmount * j.weight * accumulationMultiplier;
                }
            }

            vfx.weatherSphere.omwsMaterials.rainSpeed = rainSpeed;
            vfx.weatherSphere.omwsMaterials.snowSpeed = snowSpeed;
        }

        public override void OnFXDisable() { }

        public override void SetupFXParent() { }
    }
}
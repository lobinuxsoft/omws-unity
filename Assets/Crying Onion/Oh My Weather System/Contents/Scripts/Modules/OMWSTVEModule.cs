using CryingOnion.OhMy.WeatherSystem.Core;
using UnityEngine;
#if THE_VEGETATION_ENGINE
using TheVegetationEngine;
#endif

namespace CryingOnion.OhMy.WeatherSystem.Module
{
    [ExecuteAlways]
    public class OMWSTVEModule : OMWSModule
    {
        public enum UpdateFrequency { everyFrame, onAwake, viaScripting }
        public UpdateFrequency updateFrequency;

#if THE_VEGETATION_ENGINE

        public TVEGlobalControl globalControl;
        public TVEGlobalMotion globalMotion;

#endif

        void OnEnable()
        {
            if (GetComponent<OMWSWeather>())
            {
                GetComponent<OMWSWeather>().IntitializeModule(typeof(OMWSTVEModule));
                DestroyImmediate(this);
                Debug.LogWarning("Add modules in the settings tab in OMWS 2!");
                return;
            }
        }

        // Start is called before the first frame update
        void Awake()
        {
            SetupModule();

#if THE_VEGETATION_ENGINE

            if (updateFrequency == UpdateFrequency.onAwake)
                UpdateTVE();

#endif

        }

#if THE_VEGETATION_ENGINE

        public override void SetupModule()
        {
            if (!enabled)
                return;

            weatherSphere = OMWSWeather.instance;

            if (!weatherSphere)
            {
                enabled = false;
                return;
            }

            if (!globalControl)
                globalControl = FindObjectOfType<TVEGlobalControl>();

            if (!globalControl)
            {
                enabled = false;
                return;
            }

            if (!globalMotion)
                globalMotion = FindObjectOfType<TVEGlobalMotion>();

            if (!globalMotion)
            {
                enabled = false;
                return;
            }

            globalControl.mainLight = weatherSphere.sunLight;
        }


        // Update is called once per frame
        void Update()
        {
            if (updateFrequency == UpdateFrequency.everyFrame)
                UpdateTVE();
        }

        public void UpdateTVE()
        {
            if (weatherSphere.omwsMaterials)
            {
                globalControl.globalWetness = weatherSphere.cozyMaterials.m_Wetness;
                globalControl.globalOverlay = weatherSphere.cozyMaterials.m_SnowAmount;
            }

            globalControl.seasonControl = weatherSphere.GetCurrentYearPercentage() * 4;

            if (weatherSphere.VFX)
            {
                globalMotion.windPower = weatherSphere.VFX.windManager.windSpeed;
                globalMotion.transform.LookAt(globalMotion.transform.position + weatherSphere.VFX.windManager.windDirection, Vector3.up);
            }
        }

#endif
    }
}
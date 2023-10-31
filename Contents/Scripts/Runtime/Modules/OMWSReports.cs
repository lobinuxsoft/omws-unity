using CryingOnion.OhMy.WeatherSystem.Core;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.Module
{
    [ExecuteAlways]
    public class OMWSReports : OMWSModule
    {
        void OnEnable()
        {
            if (GetComponent<OMWSWeather>())
            {
                GetComponent<OMWSWeather>().IntitializeModule(typeof(OMWSReports));
                DestroyImmediate(this);
                Debug.LogWarning("Add modules in the settings tab in OMWS!");
                return;
            }
        }

        // Start is called before the first frame update
        void Awake()
        {
            if (!enabled)
                return;

            SetupModule();
        }
    }
}
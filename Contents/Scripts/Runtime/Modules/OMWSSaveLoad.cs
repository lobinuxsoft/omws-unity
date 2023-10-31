using CryingOnion.OhMy.WeatherSystem.Core;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.Module
{
    public class OMWSSaveLoad : OMWSModule
    {
        void OnEnable()
        {
            if (GetComponent<OMWSWeather>())
            {
                GetComponent<OMWSWeather>().IntitializeModule(typeof(OMWSSaveLoad));
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


        public void Save()
        {
            if (weatherSphere == null)
                SetupModule();

            string weatherJSON = JsonUtility.ToJson(weatherSphere);
            PlayerPrefs.SetString("OMWS_Properties", weatherJSON);
            PlayerPrefs.SetString("OMWS_Perennial", JsonUtility.ToJson(weatherSphere.perennialProfile));
        }

        public void Load()
        {
            if (weatherSphere == null)
                SetupModule();

            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString("OMWS_Properties"), weatherSphere);
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString("OMWS_Perennial"), weatherSphere.perennialProfile);

            weatherSphere.Update();
        }
    }
}
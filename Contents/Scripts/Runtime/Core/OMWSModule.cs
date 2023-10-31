using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.Core
{
    /// <summary>
    /// Weather Module that contains all necessary references and is used as a base class for all subsequent modules.
    /// </summary>
    public abstract class OMWSModule : MonoBehaviour
    {
        [HideInInspector]
        public OMWSWeather weatherSphere;

        public virtual void SetupModule()
        {
            if (!enabled)
                return;
            weatherSphere = OMWSWeather.instance;
        }

        private void OnDisable() => DisableModule();

        public virtual void DisableModule() { }
    }
}
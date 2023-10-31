using CryingOnion.OhMy.WeatherSystem.Core;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.Utility
{
    [ExecuteAlways]
    public class OMWSSatellite : MonoBehaviour
    {
        public float orbitOffset;
        public float satelliteRotateSpeed;
        public float satelliteDirection;
        private Transform m_Satellite;
        private OMWSWeather m_WeatherManager;

        // Start is called before the first frame update
        void Awake()
        {
            m_Satellite = transform.GetChild(0);
            m_WeatherManager = OMWSWeather.instance;
        }

        // Update is called once per frame
        void Update()
        {
            m_Satellite.localEulerAngles = m_Satellite.localEulerAngles + Vector3.up * Time.deltaTime * satelliteRotateSpeed;
            transform.localEulerAngles = new Vector3(-((m_WeatherManager.GetCurrentDayPercentage() * 360) - 90 + orbitOffset), satelliteDirection, 0);
        }
    }
}
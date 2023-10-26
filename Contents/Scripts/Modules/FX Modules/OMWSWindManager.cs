using CryingOnion.OhMy.WeatherSystem.Core;
using CryingOnion.OhMy.WeatherSystem.Data;
using CryingOnion.OhMy.WeatherSystem.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.Module
{
    [System.Serializable]
    public class OMWSWindManager : OMWSFXModule
    {
        public WindZone windZone;

        private float m_WindSpeed;
        private float m_WindAmount;
        private Vector3 m_WindDirection;
        private float m_Seed;
        [Tooltip("Multiplies the total wind power by a coefficient.")]
        [Range(0, 2)] public float windMultiplier = 1;
        public bool useWindzone = true;
        public bool generateWindzoneAtRuntime = true;
        public bool useShaderWind = true;
        private float m_WindTime;
        private OMWSWindFX defaultWind;
        public List<OMWSWindFX> windFXes = new List<OMWSWindFX>();

        public float windSpeed { get { return m_WindSpeed; } }

        public Vector3 windDirection
        {
            get { return m_WindDirection; }
            set { m_WindDirection = windDirection; }
        }

        public override void OnFXEnable()
        {
            defaultWind = (OMWSWindFX)Resources.Load("Default Wind");

            m_WindTime = 0;

            m_Seed = Random.value * 1000;

            CalculateWind();
        }

        public override void OnFXUpdate()
        {

            if (vfx == null)
                vfx = OMWSWeather.instance.GetModule<OMWSVFXModule>();

            if (!isEnabled)
                return;

            if (generateWindzoneAtRuntime)
                if (useWindzone)
                {
                    if (windZone == null) { SetupFXParent(); }

                    else if (windZone.transform.parent == null)
                        windZone.transform.parent = vfx.parent;

                    windZone.transform.localPosition = Vector3.zero;
                }
                else if (windZone)
                    MonoBehaviour.DestroyImmediate(windZone.gameObject);

            ManageWind();

            if (useShaderWind)
            {
                Shader.SetGlobalFloat("OMWS_WindTime", m_WindTime);
                Shader.SetGlobalVector("OMWS_WindDirection", m_WindDirection * m_WindAmount * windMultiplier);
            }
        }

        public void CalculateWind()
        {
            List<OMWSWindFX> k = new List<OMWSWindFX>();
            m_WindSpeed = defaultWind.windAmount;
            m_WindAmount = defaultWind.windSpeed;

            foreach (OMWSWindFX j in windFXes)
            {
                if (j.weight > 0)
                {
                    m_WindAmount += j.windAmount * j.weight;
                    m_WindSpeed += j.windSpeed * j.weight;
                }
            }
        }

        public void ManageWind()
        {
            if (!Application.isPlaying)
            {
                m_WindSpeed = defaultWind.windSpeed;
                m_WindAmount = defaultWind.windAmount;
            }

            float i = 360 * Mathf.PerlinNoise(m_Seed, Time.time / 100000);

            m_WindDirection = new Vector3(Mathf.Sin(i), 0, Mathf.Cos(i));

            if (useWindzone)
            {
                if (windZone)
                {
                    windZone.transform.LookAt(vfx.transform.position + m_WindDirection, Vector3.up);
                    windZone.windMain = m_WindAmount * windMultiplier;
                }
            }

            m_WindTime += Time.deltaTime * m_WindSpeed;
        }

        public override void SetupFXParent()
        {
            if (vfx.parent == null)
                return;

            if (useWindzone)
            {
                Transform wind = new GameObject().transform;
                wind.localPosition = Vector3.zero;
                wind.localEulerAngles = Vector3.zero;
                wind.localScale = Vector3.one;
                wind.name = "Wind Zone";
                wind.parent = vfx.parent;
                wind.gameObject.AddComponent<OMWSFXParent>();
                windZone = wind.gameObject.AddComponent<WindZone>();
            }
        }

        public override void OnFXDisable()
        {
            if (windZone)
                MonoBehaviour.DestroyImmediate(windZone.gameObject);

            Shader.SetGlobalFloat("OMWS_WindTime", 0);
            Shader.SetGlobalVector("OMWS_WindDirection", Vector3.zero);
        }
    }
}
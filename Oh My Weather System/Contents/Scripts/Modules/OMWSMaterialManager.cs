using CryingOnion.OhMy.WeatherSystem.Core;
using CryingOnion.OhMy.WeatherSystem.Data;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace CryingOnion.OhMy.WeatherSystem.Module
{
    [ExecuteAlways]
    public class OMWSMaterialManager : OMWSModule
    {
        [SerializeField, Range(0, 1)] public float m_SnowAmount;
        [SerializeField] public float m_SnowMeltSpeed = 0.35f;
        [SerializeField, Range(0, 1)] public float m_Wetness;
        [SerializeField] public float m_DryingSpeed = 0.5f;
        public float snowSpeed;
        public float rainSpeed;

        public OMWSMaterialManagerProfile profile;
        public List<OMWSPrecipitationFX> precipitationFXes = new List<OMWSPrecipitationFX>();

        void OnEnable()
        {
            if (GetComponent<OMWSWeather>())
            {
                GetComponent<OMWSWeather>().IntitializeModule(typeof(OMWSMaterialManager));
                DestroyImmediate(this);
                Debug.LogWarning("Add modules in the settings tab in OMWS 2!");
                return;
            }
        }


        // Start is called before the first frame update
        void Awake()
        {
            if (!enabled)
                return;
            base.SetupModule();

            if (profile == null)
                return;

            SetupStaticGlobalVariables();
        }

        // Update is called once per frame
        void Update()
        {
            if (weatherSphere == null)
                base.SetupModule();

            if (profile == null)
                return;

            m_SnowAmount += Time.deltaTime * snowSpeed;

            if (snowSpeed == 0)
                if (weatherSphere.currentTemperature > 32)
                    m_SnowAmount -= Time.deltaTime * m_SnowMeltSpeed * 0.03f;
                else
                    m_SnowAmount -= Time.deltaTime * m_SnowMeltSpeed * 0.001f;

            m_Wetness += (Time.deltaTime * rainSpeed) + (-1 * m_DryingSpeed * 0.001f);

            m_SnowAmount = Mathf.Clamp01(m_SnowAmount);
            m_Wetness = Mathf.Clamp01(m_Wetness);

            SetupStaticGlobalVariables();

            Shader.SetGlobalFloat("OMWS_SnowAmount", m_SnowAmount);
            Shader.SetGlobalFloat("OMWS_WetnessAmount", m_Wetness);

            foreach (OMWSMaterialManagerProfile.OMWSModulatedValue i in profile.modulatedValues)
            {
                switch (i.modulationTarget)
                {
                    case (OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationTarget.globalColor):
                        Shader.SetGlobalColor(i.targetVariableName, i.mappedGradient.Evaluate(GetPercentage(i.modulationSource)));
                        break;
                    case (OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationTarget.globalValue):
                        Shader.SetGlobalFloat(i.targetVariableName, i.mappedCurve.Evaluate(GetPercentage(i.modulationSource)));
                        break;
                    case (OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationTarget.materialColor):
                        if (i.targetMaterial)
                            i.targetMaterial.SetColor(i.targetVariableName, i.mappedGradient.Evaluate(GetPercentage(i.modulationSource)));
                        break;
                    case (OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationTarget.materialValue):
                        if (i.targetMaterial)
                            i.targetMaterial.SetFloat(i.targetVariableName, i.mappedCurve.Evaluate(GetPercentage(i.modulationSource)));
                        break;
                    case (OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationTarget.terrainLayerColor):
                        if (i.targetLayer)
                            i.targetLayer.specular = i.mappedGradient.Evaluate(GetPercentage(i.modulationSource));
                        break;
                }
            }
        }

        float GetPercentage(OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationSource modulationSource)
        {
            float i = 0;

            switch (modulationSource)
            {
                case (OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationSource.dayPercent):
                    i = weatherSphere.GetCurrentDayPercentage();
                    break;
                case (OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationSource.precipitation):
                    i = Mathf.Clamp01(weatherSphere.GetPrecipitation() / 100);
                    break;
                case (OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationSource.rainAmount):
                    i = m_Wetness;
                    break;
                case (OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationSource.snowAmount):
                    i = m_SnowAmount;
                    break;
                case (OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationSource.temperature):
                    i = Mathf.Clamp01(weatherSphere.GetTemperature(false) / 100);
                    break;
                case (OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationSource.yearPercent):
                    i = weatherSphere.GetCurrentYearPercentage();
                    break;
            }

            return i;
        }

        public void SetupStaticGlobalVariables()
        {
            Shader.SetGlobalFloat("OMWS_SnowScale", profile.snowNoiseSize);
            Shader.SetGlobalTexture("OMWS_SnowTexture", profile.snowTexture);
            Shader.SetGlobalColor("OMWS_SnowColor", profile.snowColor);
            Shader.SetGlobalFloat("OMWS_PuddleScale", profile.puddleScale);
        }
    }
}
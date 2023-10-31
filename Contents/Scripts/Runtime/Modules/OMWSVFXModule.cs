using CryingOnion.OhMy.WeatherSystem.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace CryingOnion.OhMy.WeatherSystem.Module
{
    [ExecuteAlways]
    public class OMWSVFXModule : OMWSModule
    {
        #region Particles

        [Tooltip("Set the color of these particle systems to the star color of the weather system.")]
        [SerializeField] private List<ParticleSystem> m_Stars = new List<ParticleSystem>();

        [Tooltip("Set the color of these particle systems to the cloud color of the weather system.")]
        [SerializeField] private List<ParticleSystem> m_CloudParticles = new List<ParticleSystem>();

        #endregion

        public Transform parent;
        public bool hideFXInHierarchy = true;

        public OMWSParticleManager particleManager = new OMWSParticleManager();
        public OMWSThunderManager thunderManager = new OMWSThunderManager();
        public OMWSFilterManager filterManager = new OMWSFilterManager();
        public OMWSAudioManager audioManager = new OMWSAudioManager();
        public OMWSPostFXManager postFXManager = new OMWSPostFXManager();
        public OMWSPrecipitationManager precipitationManager = new OMWSPrecipitationManager();
        public OMWSWindManager windManager = new OMWSWindManager();

        void OnEnable()
        {
            if (GetComponent<OMWSWeather>())
            {
                GetComponent<OMWSWeather>().IntitializeModule(typeof(OMWSSatelliteManager));
                DestroyImmediate(this);
                Debug.LogWarning("Add modules in the settings tab in OMWS!");
                return;
            }

            base.SetupModule();

            particleManager.vfx = this;
            thunderManager.vfx = this;
            windManager.vfx = this;
            audioManager.vfx = this;
            filterManager.vfx = this;
            postFXManager.vfx = this;
            precipitationManager.vfx = this;

            ResetFXHandler();
        }

        void Update()
        {
            if (weatherSphere == null)
                base.SetupModule();

            if (parent == null)
                ResetFXHandler();

            parent.position = transform.position;

            SetStarColors(weatherSphere.starColor);
            SetCloudColors(weatherSphere.cloudColor);

            thunderManager.OnFXUpdate();
            windManager.OnFXUpdate();
            particleManager.OnFXUpdate();
            audioManager.OnFXUpdate();
            filterManager.OnFXUpdate();
            postFXManager.OnFXUpdate();
            precipitationManager.OnFXUpdate();
        }

        public override void DisableModule()
        {
            if (parent)
                DestroyImmediate(parent.gameObject);

            thunderManager.OnFXDisable();
            windManager.OnFXDisable();
            particleManager.OnFXDisable();
            audioManager.OnFXDisable();
            postFXManager.OnFXDisable();
            precipitationManager.OnFXDisable();
            filterManager.OnFXDisable();
        }

        /// <summary>
        /// Sets the colors of the star particle systems.
        /// </summary> 
        private void SetStarColors(Color color)
        {
            if (m_Stars.Count == 0)
                return;

            foreach (ParticleSystem i in m_Stars)
            {
                if (i == null)
                    continue;

                ParticleSystem.MainModule j = i.main;
                j.startColor = color;
            }
        }

        /// <summary>
        /// Sets the colors of the cloud particle systems.
        /// </summary> 
        private void SetCloudColors(Color color)
        {
            if (m_CloudParticles.Count == 0)
                return;

            foreach (ParticleSystem i in m_CloudParticles)
            {
                if (i == null)
                    continue;

                ParticleSystem.MainModule j = i.main;
                j.startColor = color;

                ParticleSystem.TrailModule k = i.trails;
                k.colorOverLifetime = color;
            }
        }

        public void ResetFXHandler()
        {
            GameObject i = new GameObject();

            i.name = "OMWS FX";

            i.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild;

            if (hideFXInHierarchy)
                i.hideFlags = i.hideFlags | HideFlags.HideInHierarchy;

            parent = i.transform;

            thunderManager.OnFXEnable();
            windManager.OnFXEnable();
            particleManager.OnFXEnable();
            audioManager.OnFXEnable();
            precipitationManager.OnFXEnable();
            filterManager.OnFXEnable();
            postFXManager.OnFXEnable();
        }

        void Reset()
        {
            thunderManager.thunderPrefab = Resources.Load("OMWS Prefabs/Thunder And Lightning") as GameObject;
            audioManager.weatherFXMixer = (Resources.Load("OMWS Weather Mixer") as AudioMixer).FindMatchingGroups("Weather FX")[0];
        }
    }
}
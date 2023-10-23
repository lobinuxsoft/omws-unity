using CryingOnion.OhMy.WeatherSystem.Core;
using CryingOnion.OhMy.WeatherSystem.Module;
using System.Collections.Generic;
using UnityEngine;


namespace CryingOnion.OhMy.WeatherSystem.Utility
{
    public class OMWSParticles : MonoBehaviour
    {
        private OMWSWeather weatherSphere;

        [HideInInspector]
        public float weight;

        [HideInInspector]
        public OMWSParticleManager particleManager;

        [System.Serializable]
        public class OMWSParticleType
        {
            public ParticleSystem particleSystem;
            public float emissionAmount;
        }

        [HideInInspector]
        public List<OMWSParticleType> m_ParticleTypes;

        // Start is called before the first frame update
        void Awake()
        {
            weatherSphere = FindObjectOfType<OMWSWeather>();

            foreach (ParticleSystem i in GetComponentsInChildren<ParticleSystem>())
            {
                OMWSParticleType j = new OMWSParticleType();
                j.particleSystem = i;
                j.emissionAmount = i.emission.rateOverTime.constant;
                m_ParticleTypes.Add(j);
            }

            foreach (OMWSParticleType i in m_ParticleTypes)
            {
                ParticleSystem.EmissionModule k = i.particleSystem.emission;
                ParticleSystem.MinMaxCurve j = k.rateOverTime;

                j.constant = 0;
                k.rateOverTime = j;
            }
        }

        public void SetupTriggers()
        {
            foreach (OMWSParticleType particle in m_ParticleTypes)
            {
                ParticleSystem.TriggerModule triggers = particle.particleSystem.trigger;

                triggers.enter = ParticleSystemOverlapAction.Kill;
                triggers.inside = ParticleSystemOverlapAction.Kill;

                for (int j = 0; j < weatherSphere.omwsTriggers.Count; j++)
                    triggers.SetCollider(j, weatherSphere.omwsTriggers[j]);
            }
        }

        public void Play()
        {
            if (this == null)
                return;

            foreach (OMWSParticleType particle in m_ParticleTypes)
            {
                ParticleSystem.EmissionModule i = particle.particleSystem.emission;
                ParticleSystem.MinMaxCurve j = i.rateOverTime;

                j.constant = particle.emissionAmount * particleManager.multiplier;
                i.rateOverTime = j;

                if (particle.particleSystem.isStopped)
                    particle.particleSystem.Play();
            }
        }

        public void Stop()
        {
            if (m_ParticleTypes != null)
            {
                foreach (OMWSParticleType particle in m_ParticleTypes)
                {
                    if (particle.particleSystem != null)
                        if (particle.particleSystem.isPlaying)
                            particle.particleSystem.Stop();
                }
            }
        }

        public void Play(float weight)
        {
            if (this == null)
                return;

            foreach (OMWSParticleType particle in m_ParticleTypes)
            {
                ParticleSystem.EmissionModule i = particle.particleSystem.emission;
                ParticleSystem.MinMaxCurve j = i.rateOverTime;

                j.constant = Mathf.Lerp(0, particle.emissionAmount * particleManager.multiplier, weight);
                i.rateOverTime = j;

                if (particle.particleSystem.isStopped)
                    particle.particleSystem.Play();
            }
        }
    }
}
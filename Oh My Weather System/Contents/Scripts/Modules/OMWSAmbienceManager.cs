using CryingOnion.OhMy.WeatherSystem.Core;
using CryingOnion.OhMy.WeatherSystem.Data;
using CryingOnion.OhMy.WeatherSystem.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.Module
{
    [ExecuteAlways]
    public class OMWSAmbienceManager : OMWSModule
    {
        public List<OMWSAmbienceProfile> ambienceProfiles = new List<OMWSAmbienceProfile>();

        [System.Serializable]
        public class OMWSWeightedAmbience
        {
            public OMWSAmbienceProfile ambienceProfile;
            [Range(0, 1)] public float weight;
            public bool transitioning;

            public IEnumerator Transition(float value, float time)
            {
                transitioning = true;
                float t = 0;
                float start = weight;

                while (t < time)
                {
                    float div = (t / time);
                    yield return new WaitForEndOfFrame();

                    weight = Mathf.Lerp(start, value, div);
                    t += Time.deltaTime;
                }

                weight = value;
                ambienceProfile.SetWeight(weight);
                transitioning = false;
            }
        }

        public List<OMWSWeightedAmbience> weightedAmbience = new List<OMWSWeightedAmbience>();

        public OMWSAmbienceProfile currentAmbienceProfile;
        public OMWSAmbienceProfile ambienceChangeCheck;
        public float timeToChangeProfiles = 7;
        private float m_AmbienceTimer;

        void OnEnable()
        {
            if (GetComponent<OMWSWeather>())
            {
                GetComponent<OMWSWeather>().IntitializeModule(typeof(OMWSAmbienceManager));
                DestroyImmediate(this);
                Debug.LogWarning("Add modules in the settings tab in OMWS 2!");
                return;
            }
        }


        void Start()
        {
            if (!enabled)
                return;

            base.SetupModule();

            if (Application.isPlaying)
            {
                Transform t = weatherSphere.transform;

                SetNextAmbience();

                weightedAmbience = new List<OMWSWeightedAmbience>() { new OMWSWeightedAmbience() { weight = 1, ambienceProfile = currentAmbienceProfile } };
            }
        }

        public void FindAllAmbiences()
        {
            if (ambienceProfiles.Count > 0)
                ambienceProfiles.Clear();

            foreach (OMWSAmbienceProfile i in OMWSEditorUtilities.GetAllInstances<OMWSAmbienceProfile>())
                if (i.name != "Default Ambience")
                    ambienceProfiles.Add(i);
        }

        // Update is called once per frame
        void Update()
        {
            if (Application.isPlaying)
            {
                if (ambienceChangeCheck != currentAmbienceProfile)
                    SetAmbience(currentAmbienceProfile);

                m_AmbienceTimer -= Time.deltaTime * weatherSphere.perennialProfile.ModifiedTickSpeed();

                if (m_AmbienceTimer <= 0)
                    SetNextAmbience();

                foreach (OMWSWeightedAmbience i in weightedAmbience)
                    i.ambienceProfile.SetWeight(i.weight);

                weightedAmbience.RemoveAll(x => x.weight == 0 && x.transitioning == false);
            }
        }

        public void SetNextAmbience() => currentAmbienceProfile = WeightedRandom(ambienceProfiles.ToArray());

        public void SetAmbience(OMWSAmbienceProfile profile)
        {
            currentAmbienceProfile = profile;
            ambienceChangeCheck = currentAmbienceProfile;

            if (weightedAmbience.Find(x => x.ambienceProfile == profile) == null)
                weightedAmbience.Add(new OMWSWeightedAmbience() { weight = 0, ambienceProfile = profile, transitioning = true });

            foreach (OMWSWeightedAmbience j in weightedAmbience)
            {
                if (j.ambienceProfile == profile)
                    StartCoroutine(j.Transition(1, timeToChangeProfiles));
                else
                    StartCoroutine(j.Transition(0, timeToChangeProfiles));
            }

            m_AmbienceTimer += Random.Range(currentAmbienceProfile.playTime.x, currentAmbienceProfile.playTime.y);
        }
        public void SetAmbience(OMWSAmbienceProfile profile, float timeToChange)
        {
            currentAmbienceProfile = profile;
            ambienceChangeCheck = currentAmbienceProfile;

            if (weightedAmbience.Find(x => x.ambienceProfile == profile) == null)
                weightedAmbience.Add(new OMWSWeightedAmbience() { weight = 0, ambienceProfile = profile, transitioning = true });

            foreach (OMWSWeightedAmbience j in weightedAmbience)
            {
                if (j.ambienceProfile == profile)
                    StartCoroutine(j.Transition(1, timeToChange));
                else
                    StartCoroutine(j.Transition(0, timeToChange));
            }

            m_AmbienceTimer += Random.Range(currentAmbienceProfile.playTime.x, currentAmbienceProfile.playTime.y);
        }

        public void SkipTicks(float ticksToSkip) => m_AmbienceTimer -= ticksToSkip;

        public OMWSAmbienceProfile WeightedRandom(OMWSAmbienceProfile[] profiles)
        {
            OMWSAmbienceProfile i = null;
            List<float> floats = new List<float>();
            float totalChance = 0;

            foreach (OMWSAmbienceProfile k in profiles)
            {
                float chance;

                if (k.dontPlayDuring.Contains(weatherSphere.GetCurrentWeatherProfile()))
                    chance = 0;
                else
                    chance = k.GetChance(weatherSphere);

                floats.Add(chance);
                totalChance += chance;
            }

            if (totalChance == 0)
            {
                i = (OMWSAmbienceProfile)Resources.Load("Default Ambience");
                Debug.LogWarning("Could not find a suitable ambience given the current selected profiles and chance effectors. Defaulting to an empty ambience.");
                return i;
            }

            float selection = Random.Range(0, totalChance);

            int m = 0;
            float l = 0;

            while (l <= selection)
            {
                if (selection >= l && selection < l + floats[m])
                {
                    i = profiles[m];
                    break;
                }

                l += floats[m];
                m++;
            }

            if (!i)
                i = profiles[0];

            return i;
        }

        public float GetTimeTillNextAmbience() => m_AmbienceTimer;
    }
}
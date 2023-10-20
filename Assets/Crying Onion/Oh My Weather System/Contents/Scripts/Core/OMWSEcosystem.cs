using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CryingOnion.OhMy.WeatherSystem.Utility;
using CryingOnion.OhMy.WeatherSystem.Data;
using System.Linq;
using UnityEngine.Serialization;

namespace CryingOnion.OhMy.WeatherSystem.Core
{
    [ExecuteAlways]
    public class OMWSEcosystem : MonoBehaviour
    {
        public OMWSForecastProfile forecastProfile;
        public enum EcosystemStyle { manual, forecast }
        [Tooltip("How should this ecosystem manage weather selection? Manual allows you to set the current weather profiles and weights individually, " +
        "Automatic allows you to manually select the weather profile that this ecosystem will use and the weights will adjust accordingly," +
            " Forecast allows for dynamically changing weather based on a predetermined forecast that runs entirely on it's own.")]
        public EcosystemStyle weatherSelectionMode;

        public List<OMWSWeatherPattern> currentForecast;

        [System.Serializable]
        public class OMWSWeatherPattern
        {
            public OMWSWeatherProfile profile;
            public float weatherProfileDuration;
            public float startTicks;
            public float endTicks;
        }

        public float weatherTransitionTime = 15;

        public float weatherTimer;
        public OMWSWeather weatherSphere;
        [Range(0, 1)] public float weight;

        public OMWSClimateProfile climateProfile;

        [Tooltip("Adds an offset to the local temperature. Useful for adding biomes or climate change by location or elevation")]
        [FormerlySerializedAs("localTempratureFilter")]
        public float localTemperatureFilter;
        [Tooltip("Adds an offset to the local precipitation. Useful for adding biomes or climate change by location or elevation")]
        public float localPrecipitationFilter;

        [FormerlySerializedAs("currentTemprature")]
        public float currentTemperature;
        [FormerlySerializedAs("currentTempratureCelsius")]
        public float currentTemperatureCelsius;
        public float currentPrecipitation;

        public OMWSWeatherProfile currentWeather;
        public OMWSWeatherProfile weatherChangeCheck;

        [System.Serializable]
        public class OMWSWeightedWeather
        {
            [Range(0, 1)] public float weight; public OMWSWeatherProfile profile; public bool transitioning = true;

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
                transitioning = false;
            }
        }

        [OMWSWeightedWeather]
        public List<OMWSWeightedWeather> weightedWeatherProfiles;

        public OMWSWeightedWeather GetWeightedWeather(OMWSWeatherProfile profile, List<OMWSWeightedWeather> list)
        {
            OMWSWeightedWeather i = null;

            foreach (OMWSWeightedWeather j in list) { if (j.profile == profile) { i = j; return i; } }

            OMWSWeightedWeather k = new OMWSWeightedWeather();
            k.profile = profile;
            list.Add(k);
            i = list.Last();

            return i;
        }

        public void Awake()
        {
            if (!enabled)
                return;

            weatherSphere = OMWSWeather.instance;
            weatherTimer = 0;

            if (Application.isPlaying)
            {
                if (weatherSelectionMode == EcosystemStyle.forecast)
                {
                    switch (forecastProfile.startWeatherWith)
                    {
                        case (OMWSForecastProfile.StartWeatherWith.initialProfile):
                            {
                                if (forecastProfile.initialProfile == null)
                                {
                                    for (int i = 0; i < forecastProfile.forecastLength; i++)
                                        ForecastNewWeather();
                                    break;
                                }

                                ForecastNewWeather(forecastProfile.initialProfile);

                                for (int i = 1; i < forecastProfile.forecastLength; i++)
                                    ForecastNewWeather();

                                break;
                            }
                        case (OMWSForecastProfile.StartWeatherWith.initialForecast):
                            {
                                for (int i = 0; i < forecastProfile.initialForecast.Count; i++)
                                    ForecastNewWeather(forecastProfile.initialForecast[i].profile, forecastProfile.initialForecast[i].weatherProfileDuration);

                                for (int i = forecastProfile.initialForecast.Count; i < forecastProfile.forecastLength; i++)
                                    ForecastNewWeather();

                                break;
                            }
                        case (OMWSForecastProfile.StartWeatherWith.random):
                            {
                                for (int i = 0; i < forecastProfile.forecastLength; i++)
                                    ForecastNewWeather();

                                break;
                            }
                    }

                    SetupWeather();
                }
                else if (weatherSelectionMode == EcosystemStyle.manual)
                {
                    weightedWeatherProfiles = new List<OMWSWeightedWeather>() { new OMWSWeightedWeather() };
                    weightedWeatherProfiles[0].profile = currentWeather;
                    weightedWeatherProfiles[0].weight = 1;

                    weatherChangeCheck = currentWeather;
                }
            }
        }

        public void SetupWeather()
        {
            weightedWeatherProfiles = new List<OMWSWeightedWeather>();

            OMWSWeatherProfile i = currentForecast[0].profile;

            currentWeather = i;
            weatherTimer += currentForecast[0].weatherProfileDuration;
            GetWeightedWeather(i, weightedWeatherProfiles).weight = 1;

            currentForecast.RemoveAt(0);
            ForecastNewWeather();
        }

        public void SkipTicks(float ticksToSkip) => weatherTimer -= ticksToSkip;

        public void Update()
        {
            if (weatherSphere == null)
            {
                if (OMWSWeather.instance)
                    weatherSphere = OMWSWeather.instance;
                else
                {
                    Debug.LogError("Could not find an instance of OMWS. Make sure that your scene is properly setup!");
                    return;
                }
            }


            if (Application.isPlaying)
            {
                if (weatherSelectionMode == EcosystemStyle.forecast)
                {
                    ClampEcosystem();

                    weatherTimer -= Time.deltaTime * weatherSphere.perennialProfile.ModifiedTickSpeed();

                    if (weatherTimer <= 0)
                    {
                        while (weatherTimer <= 0)
                            SetNextWeather();
                    }
                }

                if (weatherChangeCheck != currentWeather)
                    SetWeather(currentWeather, weatherTransitionTime);

                weightedWeatherProfiles.RemoveAll(x => x.weight == 0 && x.transitioning == false);
            }
            else
            {
                weightedWeatherProfiles = new List<OMWSWeightedWeather>() { new OMWSWeightedWeather() { profile = currentWeather, weight = 1 } };
            }

            if (weatherSelectionMode == EcosystemStyle.manual)
                return;

            if (climateProfile == null)
            {
                Debug.LogError($"Assign a climate profile on {this.name}!");
                return;
            }

            currentTemperature = climateProfile.GetTemperature(false, weatherSphere) + localTemperatureFilter;
            currentTemperatureCelsius = climateProfile.GetTemperature(true, weatherSphere) + localTemperatureFilter;
            currentPrecipitation = climateProfile.GetHumidity(weatherSphere) + localPrecipitationFilter;
        }

        public void UpdateEcosystem()
        {
            if (weatherSphere.ecosystems.Count == 0)
                return;

            if (weatherSphere.ecosystems.Last() != this)
                return;
        }

        public void ClampEcosystem()
        {
            float j = 0;

            foreach (OMWSWeightedWeather i in weightedWeatherProfiles)
                j += i.weight;

            if (j == 0)
                j = 1;

            foreach (OMWSWeightedWeather i in weightedWeatherProfiles)
                i.weight /= j;
        }

        public void SetNextWeather()
        {
            if (currentForecast.Count == 0)
                ForecastNewWeather();

            SetWeather(currentForecast[0].profile, weatherTransitionTime);
            weatherTimer += currentForecast[0].weatherProfileDuration;

            currentForecast.RemoveAt(0);
            ForecastNewWeather();
        }

        /// <summary>
        /// Transitions the weather profile over the the course of the weather transition time and all of the impacted settings. 
        /// </summary>  
        public void SetWeather(OMWSWeatherProfile prof, float transitionTime)
        {
            currentWeather = prof;
            weatherChangeCheck = currentWeather;

            if (weightedWeatherProfiles.Find(x => x.profile == prof) == null)
                weightedWeatherProfiles.Add(new OMWSWeightedWeather() { profile = prof, weight = 0, transitioning = true });

            foreach (OMWSWeightedWeather j in weightedWeatherProfiles)
            {
                if (j.profile == prof)
                    StartCoroutine(j.Transition(1, transitionTime));
                else
                    StartCoroutine(j.Transition(0, transitionTime));
            }

            weatherTimer += Random.Range(prof.weatherTime.x, prof.weatherTime.y);
        }

        /// <summary>
        /// Transitions the weather profile using the default transition time. 
        /// </summary>  
        public void SetWeather(OMWSWeatherProfile prof)
        {
            currentWeather = prof;
            weatherChangeCheck = currentWeather;

            if (weightedWeatherProfiles.Find(x => x.profile == prof) == null)
                weightedWeatherProfiles.Add(new OMWSWeightedWeather() { profile = prof, weight = 0, transitioning = true });

            foreach (OMWSWeightedWeather j in weightedWeatherProfiles)
            {
                if (j.profile == prof)
                    StartCoroutine(j.Transition(1, weatherTransitionTime));
                else
                    StartCoroutine(j.Transition(0, weatherTransitionTime));
            }

            weatherTimer += Random.Range(prof.weatherTime.x, prof.weatherTime.y);
        }

        public void ForecastNewWeather()
        {
            OMWSWeatherPattern i = new OMWSWeatherPattern();

            if (currentForecast.Count > 0)
                i.profile = WeightedRandom(GetNextWeatherArray(forecastProfile.profilesToForecast.ToArray(), currentForecast.Last().profile.forecastNext, currentForecast.Last().profile.forecastModifierMethod));
            else
                i.profile = WeightedRandom(forecastProfile.profilesToForecast.ToArray());
            i.weatherProfileDuration = Random.Range(i.profile.weatherTime.x, i.profile.weatherTime.y);

            i.startTicks = weatherTimer + weatherSphere.calendar.currentTicks;

            foreach (OMWSWeatherPattern j in currentForecast)
                i.startTicks += j.weatherProfileDuration;

            while (i.startTicks > weatherSphere.perennialProfile.ticksPerDay)
                i.startTicks -= weatherSphere.perennialProfile.ticksPerDay;

            i.endTicks = i.startTicks + i.weatherProfileDuration;

            while (i.endTicks > weatherSphere.perennialProfile.ticksPerDay)
                i.endTicks -= weatherSphere.perennialProfile.ticksPerDay;

            currentForecast.Add(i);
        }

        public void ForecastNewWeather(OMWSWeatherProfile weatherProfile)
        {
            OMWSWeatherPattern i = new OMWSWeatherPattern();

            i.profile = weatherProfile;
            i.weatherProfileDuration = Random.Range(i.profile.weatherTime.x, i.profile.weatherTime.y);

            i.startTicks = weatherTimer + weatherSphere.calendar.currentTicks;

            foreach (OMWSWeatherPattern j in currentForecast)
                i.startTicks += j.weatherProfileDuration;

            while (i.startTicks > weatherSphere.perennialProfile.ticksPerDay)
                i.startTicks -= weatherSphere.perennialProfile.ticksPerDay;

            i.endTicks = i.startTicks + i.weatherProfileDuration;

            while (i.endTicks > weatherSphere.perennialProfile.ticksPerDay)
                i.endTicks -= weatherSphere.perennialProfile.ticksPerDay;

            currentForecast.Add(i);
        }

        public void ForecastNewWeather(OMWSWeatherProfile weatherProfile, float ticks)
        {
            OMWSWeatherPattern i = new OMWSWeatherPattern();

            i.profile = weatherProfile;
            i.weatherProfileDuration = ticks;

            i.startTicks = weatherTimer + weatherSphere.calendar.currentTicks;

            foreach (OMWSWeatherPattern j in currentForecast)
                i.startTicks += j.weatherProfileDuration;

            while (i.startTicks > weatherSphere.perennialProfile.ticksPerDay)
                i.startTicks -= weatherSphere.perennialProfile.ticksPerDay;

            i.endTicks = i.startTicks + i.weatherProfileDuration;

            while (i.endTicks > weatherSphere.perennialProfile.ticksPerDay)
                i.endTicks -= weatherSphere.perennialProfile.ticksPerDay;

            currentForecast.Add(i);
        }

        public OMWSWeatherProfile WeightedRandom(OMWSWeatherProfile[] profiles)
        {
            if (profiles.Count() == 0)
                profiles = forecastProfile.profilesToForecast.ToArray();

            OMWSWeatherProfile i = null;
            List<float> floats = new List<float>();
            float totalChance = 0;
            float inTicks = 0;

            foreach (OMWSWeatherPattern k in currentForecast)
                inTicks += k.weatherProfileDuration;

            foreach (OMWSWeatherProfile k in profiles)
            {
                float chance = k.GetChance(weatherSphere.GetTemperature(false, inTicks),
                    weatherSphere.GetPrecipitation(inTicks),
                    weatherSphere.GetCurrentYearPercentage(inTicks),
                    weatherSphere.calendar.currentTicks + (inTicks - Mathf.Floor(inTicks / weatherSphere.perennialProfile.ticksPerDay)), 0, 0);

                floats.Add(chance);
                totalChance += chance;
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

        OMWSWeatherProfile[] SubtractiveArray(OMWSWeatherProfile[] total, OMWSWeatherProfile[] subtraction) =>
            total.ToList().Except(subtraction.ToList()).ToArray();

        OMWSWeatherProfile[] IntersectionArray(OMWSWeatherProfile[] total, OMWSWeatherProfile[] intersection) =>
            intersection.ToList().Except(intersection.ToList().Except(total.ToList())).ToArray();

        OMWSWeatherProfile[] GetNextWeatherArray(OMWSWeatherProfile[] total, OMWSWeatherProfile[] exception, OMWSWeatherProfile.ForecastModifierMethod modifierMethod)
        {
            switch (modifierMethod)
            {
                case (OMWSWeatherProfile.ForecastModifierMethod.DontForecastNext):
                    return SubtractiveArray(total, exception);
                case (OMWSWeatherProfile.ForecastModifierMethod.forecastNext):
                    return IntersectionArray(total, exception);
                default:
                    return total;
            }
        }

        public float GetTemperature(bool celsius) =>
            climateProfile.GetTemperature(celsius, weatherSphere) + localTemperatureFilter;

        public float GetTemperature(bool celsius, float inTicks) =>
            climateProfile.GetTemperature(celsius, weatherSphere, inTicks) + localTemperatureFilter;

        public float GetPrecipitation() =>
            climateProfile.GetHumidity(weatherSphere) + localPrecipitationFilter;

        public float GetPrecipitation(float inTicks) =>
            climateProfile.GetHumidity(weatherSphere, inTicks) + localPrecipitationFilter;
    }
}
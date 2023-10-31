using CryingOnion.OhMy.WeatherSystem.Core;
using CryingOnion.OhMy.WeatherSystem.Data;
using CryingOnion.OhMy.WeatherSystem.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.Module
{
    [ExecuteAlways]
    public class OMWSSatelliteManager : OMWSModule
    {
        public List<OMWSSatelliteProfile> satellites = new List<OMWSSatelliteProfile>();
        [HideInInspector]
        public Transform satHolder = null;
        public bool hideInHierarchy = true;

        void OnEnable()
        {
            if (GetComponent<OMWSWeather>())
            {
                GetComponent<OMWSWeather>().IntitializeModule(typeof(OMWSSatelliteManager));
                DestroyImmediate(this);
                Debug.LogWarning("Add modules in the settings tab in OMWS!");
                return;
            }
        }

        // Start is called before the first frame update
        void Awake()
        {
            weatherSphere = OMWSWeather.instance;
            UpdateSatellites();
        }

        // Update is called once per frame
        void Update()
        {
            if (satHolder == null)
                UpdateSatellites();

            if (satHolder.hideFlags == (HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild) && hideInHierarchy)
                UpdateSatellites();

            if (satHolder.hideFlags != (HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild) && !hideInHierarchy)
                UpdateSatellites();

            satHolder.position = transform.position;

            if (satellites != null)
            {
                foreach (OMWSSatelliteProfile sat in satellites)
                {
                    if (!sat)
                        break;

                    if (sat.orbitRef == null)
                        UpdateSatellites();

                    if (sat.changedLastFrame == true)
                        UpdateSatellites();

                    sat.satelliteRotation += Time.deltaTime * sat.satelliteRotateSpeed;

                    sat.orbitRef.localEulerAngles = new Vector3(0, sat.satelliteDirection, sat.satellitePitch);
                    sat.orbitRef.GetChild(0).localEulerAngles = Vector3.right * ((360 * weatherSphere.GetCurrentDayPercentage()) + sat.orbitOffset);
                    sat.moonRef.localEulerAngles = sat.initialRotation + sat.satelliteRotateAxis.normalized * sat.satelliteRotation;

                    if (sat.useLight)
                        sat.lightRef.color = weatherSphere.moonlightColor * sat.lightColorMultiplier;
                }
            }
        }

        public void UpdateSatellites()
        {
            if (weatherSphere == null)
                weatherSphere = OMWSWeather.instance;

            Transform oldHolder = null;

            if (satHolder)
                oldHolder = satHolder;

            satHolder = new GameObject("OMWS Satellites").transform;
            if (hideInHierarchy)
                satHolder.gameObject.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild | HideFlags.HideInHierarchy;
            else
                satHolder.gameObject.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild;

            bool j = true;

            if (satellites != null)
            {
                foreach (OMWSSatelliteProfile i in satellites)
                {
                    InitializeSatellite(i, j);
                    j = false;
                }
            }

            if (oldHolder)
                DestroyImmediate(oldHolder.gameObject);
        }

        public void DestroySatellites()
        {
            if (satHolder)
                DestroyImmediate(satHolder.gameObject);
        }

        public void DestroySatellite(OMWSSatelliteProfile sat)
        {
            if (sat.orbitRef)
                DestroyImmediate(sat.orbitRef.gameObject);
        }

        private void OnDisable() => DestroySatellites();

        public void InitializeSatellite(OMWSSatelliteProfile sat, bool mainMoon)
        {
            float dist = 0;

            if (weatherSphere.lockToCamera != OMWSWeather.LockToCameraStyle.DontLockToCamera && weatherSphere.omwsCamera)
                dist = .92f * weatherSphere.omwsCamera.farClipPlane * sat.distance;
            else
                dist = .92f * 1000 * sat.distance * weatherSphere.transform.localScale.x;

            sat.orbitRef = new GameObject(sat.name).transform;
            sat.orbitRef.parent = satHolder;
            sat.orbitRef.transform.localPosition = Vector3.zero;
            var orbitArm = new GameObject("Orbit Arm");
            orbitArm.transform.parent = sat.orbitRef;
            orbitArm.transform.localPosition = Vector3.zero;
            orbitArm.transform.localEulerAngles = Vector3.zero;
            sat.moonRef = Instantiate(sat.satelliteReference, Vector3.forward * dist, Quaternion.identity, sat.orbitRef.GetChild(0)).transform;
            sat.moonRef.transform.localPosition = -Vector3.forward * dist;
            sat.moonRef.transform.localEulerAngles = sat.initialRotation;
            sat.moonRef.transform.localScale = sat.satelliteReference.transform.localScale * sat.size * dist / 1000;
            sat.orbitRef.localEulerAngles = new Vector3(0, sat.satelliteDirection, sat.satellitePitch);
            sat.orbitRef.GetChild(0).localEulerAngles = Vector3.right * ((360 * weatherSphere.GetCurrentDayPercentage()) + sat.orbitOffset);

            if (sat.useLight)
            {
                var obj = new GameObject("Light");
                obj.transform.parent = sat.orbitRef.GetChild(0);
                sat.lightRef = obj.AddComponent<Light>();
                sat.lightRef.transform.localEulerAngles = new Vector3(0, 0, 0);
                sat.lightRef.transform.localPosition = new Vector3(0, 0, 0);
                sat.lightRef.type = LightType.Directional;
                sat.lightRef.shadows = sat.castShadows;
                if (sat.flare)
                    sat.lightRef.flare = sat.flare;
            }

            if (mainMoon)
                sat.orbitRef.GetChild(0).gameObject.AddComponent<OMWSSetMoonDirection>();

            sat.changedLastFrame = false;
        }

        void Reset() => satellites = new List<OMWSSatelliteProfile> { Resources.Load("Profiles/Satellites/Moon") as OMWSSatelliteProfile };
    }
}
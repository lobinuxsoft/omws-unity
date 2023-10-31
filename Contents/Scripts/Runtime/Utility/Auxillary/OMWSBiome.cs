using CryingOnion.OhMy.WeatherSystem.Core;
using CryingOnion.OhMy.WeatherSystem.Data;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.Utility
{
    public class OMWSBiome : OMWSEcosystem
    {
        public List<Vector3> bounds = new List<Vector3>() { new Vector3(-5, 0, 5), new Vector3(5, 0, 5), new Vector3(5, 0, -5), new Vector3(-5, 0, -5) };
        public float height = 10;
        public float transitionDistance = 5;
        public Color displayColor = new Color(1, 1, 1, 1);
        MeshCollider trigger;
        Transform cam;

        // Start is called before the first frame update
        void Start()
        {
            weatherSphere = OMWSWeather.instance;
            cam = Camera.main.transform;

            if (Application.isPlaying)
            {
                trigger = gameObject.AddComponent<MeshCollider>();
                trigger.sharedMesh = BuildZoneCollider();
                trigger.convex = true;
                trigger.isTrigger = true;

                weatherSphere.ecosystems.Add(this);
            }
        }

        public Mesh BuildZoneCollider()
        {
            Mesh mesh = new Mesh();
            mesh.name = $"{name} Custom Trigger Mesh";

            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();

            foreach (Vector3 i in bounds)
            {
                verts.Add(i);
                verts.Add(new Vector3(i.x, height, i.z));
            }

            for (int i = 0; i < bounds.Count; i++)
            {
                if (i == 0)
                {
                    tris.Add(0);
                    tris.Add(verts.Count - 1);
                    tris.Add(verts.Count - 2);

                    tris.Add(0);
                    tris.Add(1);
                    tris.Add(verts.Count - 1);
                }
                else
                {
                    int start = i * 2;

                    tris.Add(start);
                    tris.Add(start - 1);
                    tris.Add(start - 2);

                    tris.Add(start);
                    tris.Add(start + 1);
                    tris.Add(start - 1);
                }
            }

            for (int i = 0; i < verts.Count - 4; i += 2)
            {
                tris.Add(0);
                tris.Add(i + 2);
                tris.Add(i + 4);

                tris.Add(1);
                tris.Add(i + 3);
                tris.Add(i + 5);
            }


            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0, true);
            mesh.RecalculateNormals();

            return mesh;
        }

        private new void Update()
        {
            base.Update();

            if (Application.isPlaying)
                weight = GetWeight();
        }

        public float GetWeight()
        {
            Vector3 point = cam.position;
            Vector3 closestPoint = trigger.ClosestPoint(point);

            if (point == closestPoint)
                return 1;

            float distToClosestPoint = Vector3.Distance(point, closestPoint);
            return 1 - (Mathf.Clamp(distToClosestPoint, 0, transitionDistance) / transitionDistance);
        }

        private void OnEnable()
        {
            if (Application.isPlaying)
                AddBiome();
        }

        private void OnDisable()
        {
            if (Application.isPlaying)
                RemoveBiome();
        }

        public void AddBiome() => weatherSphere.ecosystems.Add(this);

        public void RemoveBiome() => weatherSphere.ecosystems.Remove(this);

        private void OnDrawGizmos()
        {
            if (bounds.Count >= 3)
            {
                for (int i = 0; i < bounds.Count; i++)
                {
                    Gizmos.color = new Color(displayColor.r, displayColor.g, displayColor.b, 0.3f);
                    Gizmos.DrawSphere(TransformToLocalSpace(bounds[i]), 0.2f);

                    Vector3 point = Vector3.zero;

                    if (i == 0)
                        point = bounds.Last();
                    else
                        point = bounds[i - 1];

                    Gizmos.color = new Color(displayColor.r, displayColor.g, displayColor.b, 1);
                    Gizmos.DrawLine(TransformToLocalSpace(bounds[i]), TransformToLocalSpace(point));
                }

                for (int i = 0; i < bounds.Count; i++)
                {
                    Gizmos.color = new Color(displayColor.r, displayColor.g, displayColor.b, 0.5f);
                    Gizmos.DrawSphere(TransformToLocalSpace(bounds[i]) + Vector3.up * height, 0.2f);

                    Vector3 point = Vector3.zero;

                    if (i == 0)
                        point = bounds.Last();
                    else
                        point = bounds[i - 1];

                    Gizmos.color = new Color(displayColor.r, displayColor.g, displayColor.b, 1);
                    Gizmos.DrawLine(TransformToLocalSpace(bounds[i]) + Vector3.up * height, TransformToLocalSpace(point) + Vector3.up * height);
                    Gizmos.DrawLine(TransformToLocalSpace(bounds[i]), TransformToLocalSpace(bounds[i]) + Vector3.up * height);
                    Gizmos.color = new Color(displayColor.r, displayColor.g, displayColor.b, 0.3f);
                    Gizmos.DrawLine((TransformToLocalSpace(bounds[i]) + TransformToLocalSpace(point)) / 2,
                        (TransformToLocalSpace(bounds[i]) + TransformToLocalSpace(point)) / 2 + Vector3.up * height);
                }
            }
        }

        private Vector3 TransformToLocalSpace(Vector3 pos)
        {
            Vector3 i = pos.x * transform.right + pos.y * transform.up + pos.z * transform.forward;

            i += transform.position;

            return i;
        }

        void Reset()
        {
            forecastProfile = Resources.Load("Profiles/Forecast Profiles/Complex Forecast Profile") as OMWSForecastProfile;
            currentWeather = Resources.Load("Profiles/Weather Profiles/Partly Cloudy") as OMWSWeatherProfile;
            climateProfile = Resources.Load("Profiles/Climate Profiles/Default Climate") as OMWSClimateProfile;
        }
    }
}
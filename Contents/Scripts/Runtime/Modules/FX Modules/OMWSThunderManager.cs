using CryingOnion.OhMy.WeatherSystem.Core;
using CryingOnion.OhMy.WeatherSystem.Data;
using CryingOnion.OhMy.WeatherSystem.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.Module
{
    [System.Serializable]
    public class OMWSThunderManager : OMWSFXModule
    {
        public Vector2 thunderTime = new Vector2();
        public GameObject thunderPrefab = null;
        private float thunderTimer = 0;
        public bool active = false;
        public List<OMWSThunderFX> thunderFX = new List<OMWSThunderFX>();
        List<OMWSThunderFX> activeThunderFX = new List<OMWSThunderFX>();
        public OMWSThunderFX defaultFX = null;

        public override void OnFXEnable()
        {
            thunderTimer = Random.Range(thunderTime.x, thunderTime.y);
            defaultFX = (OMWSThunderFX)Resources.Load("Default Thunder");
        }

        public override void SetupFXParent()
        {
            if (vfx.parent == null)
                return;

            parent = new GameObject().transform;
            parent.parent = vfx.parent;
            parent.localPosition = Vector3.zero;
            parent.localScale = Vector3.one;
            parent.name = "Thunder FX";
            parent.gameObject.AddComponent<OMWSFXParent>();
        }

        public override void OnFXUpdate()
        {

            if (!isEnabled)
                return;

            if (vfx == null)
                vfx = OMWSWeather.instance.GetModule<OMWSVFXModule>();

            if (parent == null)
                SetupFXParent();
            else if (parent.parent == null)
                parent.parent = vfx.parent;

            parent.transform.localPosition = Vector3.zero;

            UpdateThunderTimes();

            if (active)
            {
                thunderTimer -= Time.deltaTime;

                if (thunderTimer <= 0)
                    Strike();
                if (thunderTimer > thunderTime.y)
                    thunderTimer = thunderTime.y;
            }
        }

        public void UpdateThunderTimes()
        {
            OMWSThunderFX j = defaultFX;
            activeThunderFX.Clear();
            float total = 0;

            foreach (OMWSThunderFX i in thunderFX)
            {
                if (i.weight > 0)
                {
                    activeThunderFX.Add(i);
                    total += i.weight;
                }
            }

            if (activeThunderFX.Count == 0)
            {
                active = false;
                thunderTime = new Vector2(61, 61);
                return;
            }

            j.weight = Mathf.Clamp01(1 - total);

            if (j.weight > 0)
            {
                activeThunderFX.Add(j);
                total += j.weight;
            }

            thunderTime = j.timeBetweenStrikes;

            foreach (OMWSThunderFX i in activeThunderFX)
                thunderTime = new Vector2(Mathf.Lerp(thunderTime.x, i.timeBetweenStrikes.x, i.weight),
                    Mathf.Lerp(thunderTime.y, i.timeBetweenStrikes.y, i.weight));

            if (thunderTime == new Vector2(61, 61)) { active = false; return; }

            active = true;
        }

        public void Strike()
        {
            Transform i = MonoBehaviour.Instantiate(thunderPrefab, parent).transform;
            i.eulerAngles = Vector3.up * Random.value * 360;

            thunderTimer = Random.Range(thunderTime.x, thunderTime.y);
        }

        public override void OnFXDisable()
        {
            if (parent)
                MonoBehaviour.DestroyImmediate(parent.gameObject);
        }
    }
}
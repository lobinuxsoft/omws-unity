using CryingOnion.OhMy.WeatherSystem.Core;
using CryingOnion.OhMy.WeatherSystem.Utility;
using System;
using UnityEngine;
using UnityEngine.Audio;

namespace CryingOnion.OhMy.WeatherSystem.Module
{
    [Serializable]
    public class OMWSAudioManager : OMWSFXModule
    {
        [Tooltip("Multiply audio volume by this number.")]
        [Range(0, 1)]
        public float volumeMultiplier = 1;

        [Tooltip("The audio mixer group that the OMWS weather audio FX will use.")]
        public AudioMixerGroup weatherFXMixer;

        public override void OnFXEnable() { }

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
        }

        public override void OnFXDisable()
        {
            if (parent)
                MonoBehaviour.DestroyImmediate(parent.gameObject);
        }

        public override void SetupFXParent()
        {
            if (vfx.parent == null)
                return;

            parent = new GameObject().transform;
            parent.parent = vfx.parent;
            parent.localPosition = Vector3.zero;
            parent.localScale = Vector3.one;
            parent.name = "Audio FX";
            parent.gameObject.AddComponent<OMWSFXParent>();
        }
    }
}
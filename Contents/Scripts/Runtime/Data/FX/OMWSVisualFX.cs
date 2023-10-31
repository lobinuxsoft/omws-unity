using CryingOnion.OhMy.WeatherSystem.Core;
using CryingOnion.OhMy.WeatherSystem.Module;
using UnityEngine;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;
#elif OMWS_WEATHER_URP
using UnityEngine.Rendering;
#endif

namespace CryingOnion.OhMy.WeatherSystem.Data
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Crying Onion/Oh My Weather System/FX/Post Processing FX", order = 361)]
    public class OMWSVisualFX : OMWSFXProfile
    {
        public int layer;
        public float priority = 100;
#if UNITY_POST_PROCESSING_STACK_V2
        public PostProcessProfile effectSettings;
        PostProcessVolume _volume;
#elif OMWS_WEATHER_URP 
        public VolumeProfile effectSettings;
        Volume _volume;
#endif

        public override void PlayEffect()
        {
#if UNITY_POST_PROCESSING_STACK_V2 || OMWS_WEATHER_URP

            if (!_volume)
                if (!InitializeEffect(VFXMod))
                    return;

            if (_volume.transform.parent == null)
            {
                _volume.transform.parent = VFXMod.postFXManager.parent;
                _volume.transform.localPosition = Vector3.zero;
            }

            _volume.weight = 1;

#endif
        }

        public override void PlayEffect(float i)
        {
#if UNITY_POST_PROCESSING_STACK_V2 || OMWS_WEATHER_URP

            if (!_volume)
                if (!InitializeEffect(VFXMod))
                    return;

            if (_volume.transform.parent == null)
            {
                _volume.transform.parent = VFXMod.postFXManager.parent;
                _volume.transform.localPosition = Vector3.zero;
            }

            _volume.weight = Mathf.Clamp01(transitionTimeModifier.Evaluate(i));

            if (i == 0)
            {
                Destroy(_volume.gameObject);
                return;
            }

#endif
        }

        public override void StopEffect()
        {
#if UNITY_POST_PROCESSING_STACK_V2 || OMWS_WEATHER_URP
            _volume.weight = 0;
            Destroy(_volume.gameObject); 
#endif
        }

        public override bool InitializeEffect(OMWSVFXModule VFX)
        {

            if (VFX == null)
                VFX = OMWSWeather.instance.VFX;

#if UNITY_POST_PROCESSING_STACK_V2 || OMWS_WEATHER_URP

            VFXMod = VFX;

            if (!VFX.postFXManager.isEnabled)
                return false;

#if UNITY_POST_PROCESSING_STACK_V2

            _volume = new GameObject().AddComponent<PostProcessVolume>();

#elif OMWS_WEATHER_URP

            _volume = new GameObject().AddComponent<Volume>();

#endif

            _volume.gameObject.layer = layer;
            _volume.profile = effectSettings;
            _volume.priority = priority;
            _volume.weight = 0;
            _volume.isGlobal = true;
            _volume.gameObject.name = name;
            _volume.transform.parent = VFX.postFXManager.parent;

#endif

            return true;
        }
    }
}
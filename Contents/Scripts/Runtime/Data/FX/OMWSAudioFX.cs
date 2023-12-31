using CryingOnion.OhMy.WeatherSystem.Core;
using CryingOnion.OhMy.WeatherSystem.Module;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.Data
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Crying Onion/Oh My Weather System/FX/Audio FX", order = 361)]
    public class OMWSAudioFX : OMWSFXProfile
    {

#if OMWS_WEATHER_FMOD

        [SerializeField] private FMODUnity.EventReference eventRef;
        private FMODUnity.StudioEventEmitter runtimeRef;

#else

        public AudioClip clip;
        private AudioSource runtimeRef;

#endif

        public float maximumVolume = 1;

        public override void PlayEffect()
        {
#if OMWS_WEATHER_FMOD

            if(eventRef.IsNull)
                return;

            if (!runtimeRef)
                if (InitializeEffect(VFXMod) == false)
                    return;

            runtimeRef.IsPlaying();

            if (!runtimeRef.IsPlaying())
                runtimeRef.Play();

            runtimeRef.EventInstance.setVolume(maximumVolume * VFXMod.audioManager.volumeMultiplier);

#else
            if (!runtimeRef)
                if (InitializeEffect(VFXMod) == false)
                    return;

            if (runtimeRef.transform.parent == null)
            {
                runtimeRef.transform.parent = VFXMod.particleManager.parent;
                runtimeRef.transform.localPosition = Vector3.zero;
            }

            if (!runtimeRef.isPlaying)
                runtimeRef.Play();

            runtimeRef.volume = maximumVolume * VFXMod.audioManager.volumeMultiplier;

#endif
        }

        public override void PlayEffect(float vol)
        {
#if OMWS_WEATHER_FMOD

            if (eventRef.IsNull)
                return;

            if (!runtimeRef)
                if (InitializeEffect(VFXMod) == false)
                    return;

            if (vol != 0)
            {
                if (!runtimeRef.IsPlaying() && runtimeRef.isActiveAndEnabled)
                    runtimeRef.Play();

                runtimeRef.EventInstance.setVolume(Mathf.Clamp01(transitionTimeModifier.Evaluate(vol)) * maximumVolume * VFXMod.audioManager.volumeMultiplier);
            }
            else
            {
                Destroy(runtimeRef.gameObject);
                return;
            }

#else

            if (!runtimeRef)
                if (InitializeEffect(VFXMod) == false)
                    return;

            if (runtimeRef.transform.parent == null)
            {
                runtimeRef.transform.parent = VFXMod.particleManager.parent;
                runtimeRef.transform.localPosition = Vector3.zero;
            }

            if (vol != 0)
            {
                if (!runtimeRef.isPlaying && runtimeRef.isActiveAndEnabled)
                    runtimeRef.Play();

                runtimeRef.volume = Mathf.Clamp01(transitionTimeModifier.Evaluate(vol)) * maximumVolume * VFXMod.audioManager.volumeMultiplier;
            }
            else
            {
                Destroy(runtimeRef.gameObject);
                return;
            }

#endif
        }

        public override void StopEffect()
        {
#if OMWS_WEATHER_FMOD

            if (eventRef.IsNull)
                return;

            if (!runtimeRef)
                return;

            if (runtimeRef.IsPlaying())
                runtimeRef.Stop();

            runtimeRef.EventInstance.setVolume(0);
            Destroy(runtimeRef.gameObject);

#else

            if (!runtimeRef)
                return;

            if (runtimeRef.isPlaying)
                runtimeRef.Stop();
            runtimeRef.volume = 0;
            Destroy(runtimeRef.gameObject);

#endif
        }


        public override bool InitializeEffect(OMWSVFXModule VFX)
        {

            if (VFX == null)
                VFX = OMWSWeather.instance.VFX;

            VFXMod = VFX;

            if (!VFX.audioManager.isEnabled)
                return false;

#if OMWS_WEATHER_FMOD

            if (eventRef.IsNull)
                return false;

            runtimeRef = new GameObject().AddComponent<FMODUnity.StudioEventEmitter>();
            runtimeRef.gameObject.name = name;
            runtimeRef.transform.parent = VFX.audioManager.parent;
            runtimeRef.EventReference = eventRef;
            runtimeRef.AllowFadeout = true;
            runtimeRef.Preload = true;
            runtimeRef.EventInstance.setVolume(0);

#else

            runtimeRef = new GameObject().AddComponent<AudioSource>();

            runtimeRef.gameObject.name = name;
            runtimeRef.transform.parent = VFX.audioManager.parent;
            runtimeRef.clip = clip;
            runtimeRef.outputAudioMixerGroup = VFX.audioManager.weatherFXMixer;
            runtimeRef.volume = 0;
            runtimeRef.loop = true;

#endif

            return true;
        }
    }
}
using CryingOnion.OhMy.WeatherSystem.Core;
using CryingOnion.OhMy.WeatherSystem.Module;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.Data
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Crying Onion/Oh My Weather System/FX/Audio FX", order = 361)]
    public class OMWSAudioFX : OMWSFXProfile
    {
        public AudioClip clip;
        private AudioSource runtimeRef;
        public float maximumVolume = 1;

        public override void PlayEffect()
        {
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
        }

        public override void PlayEffect(float vol)
        {
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
        }

        public override void StopEffect()
        {
            if (!runtimeRef)
                return;

            if (runtimeRef.isPlaying)
                runtimeRef.Stop();
            runtimeRef.volume = 0;
            Destroy(runtimeRef.gameObject);
        }

        public override bool InitializeEffect(OMWSVFXModule VFX)
        {

            if (VFX == null)
                VFX = OMWSWeather.instance.VFX;

            VFXMod = VFX;

            if (!VFX.audioManager.isEnabled)
                return false;

            runtimeRef = new GameObject().AddComponent<AudioSource>();

            runtimeRef.gameObject.name = name;
            runtimeRef.transform.parent = VFX.audioManager.parent;
            runtimeRef.clip = clip;
            runtimeRef.outputAudioMixerGroup = VFX.audioManager.weatherFXMixer;
            runtimeRef.volume = 0;
            runtimeRef.loop = true;

            return true;
        }
    }
}
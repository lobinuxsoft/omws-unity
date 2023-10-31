using UnityEngine;
using CryingOnion.OhMy.WeatherSystem.Utility;
using CryingOnion.OhMy.WeatherSystem.Module;

namespace CryingOnion.OhMy.WeatherSystem.Data
{
    [System.Serializable]
    public abstract class OMWSFXProfile : ScriptableObject
    {
        [OMWSTransitionTime]
        [Tooltip("A curve modifier that is used to impact the speed of the transition for this effect.")]
        public AnimationCurve transitionTimeModifier = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        public OMWSVFXModule VFXMod;

        /// <summary>
        /// Plays the effect at maximum intensity.
        /// </summary>  
        public abstract void PlayEffect();

        /// <summary>
        /// Plays the effect at a set intensity.
        /// </summary>      
        /// <param name="weight">The weight (or intensity percentage) that this effect will play at. From 0.0 to 1.0</param>
        public abstract void PlayEffect(float weight);

        /// <summary>
        /// Stops the effect completely..
        /// </summary>  
        public abstract void StopEffect();

        /// <summary>
        /// Called to instantiate the effect.
        /// </summary>                                                                                                          
        /// <param name="VFX">Holds a reference to the Weather VFX Module.</param>
        public abstract bool InitializeEffect(OMWSVFXModule VFX);
    }
}
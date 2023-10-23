using UnityEngine;
using CryingOnion.OhMy.WeatherSystem.Module;
using CryingOnion.OhMy.WeatherSystem.Core;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CryingOnion.OhMy.WeatherSystem.Data
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "CryingOnion/Oh My Weather System/FX/Event FX", order = 361)]
    public class OMWSEventFX : OMWSFXProfile
    {
        public OMWSEventManager events;
        public bool playing;

        public delegate void OnCall();
        public event OnCall onCall;

        public void RaiseOnCall()
        {
            if (onCall != null)
                onCall();
        }

        public delegate void OnEnd();
        public event OnEnd onEnd;

        public void RaiseOnEnd()
        {
            if (onEnd != null)
                onEnd();
        }

        public override void PlayEffect()
        {
            if (!playing)
            {
                playing = true;
                if (onCall != null)
                    onCall.Invoke();
            }
        }

        public override void PlayEffect(float intensity)
        {
            if (intensity > 0.5f)
                PlayEffect();
            else
                StopEffect();
        }

        public override void StopEffect()
        {
            if (playing)
            {
                playing = false;
                if (onEnd != null)
                    onEnd.Invoke();
            }
        }

        public override bool InitializeEffect(OMWSVFXModule VFX)
        {
            if (events == null)
                events = OMWSWeather.instance.GetModule<OMWSEventManager>();

            VFXMod = VFX;

            return true;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(OMWSEventFX))]
    [CanEditMultipleObjects]
    public class OMWSEventFXEditor : OMWSFXProfileEditor
    {
        void OnEnable() { }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.HelpBox("No other properties to adjust! Set events in the OMWS Event Module!", MessageType.Info);

            serializedObject.ApplyModifiedProperties();
        }

        public override void RenderInWindow(Rect pos) { }

        public override float GetLineHeight() => 0;
    }
#endif
}
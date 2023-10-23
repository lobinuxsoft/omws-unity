using UnityEngine;
using System.Collections.Generic;
using CryingOnion.OhMy.WeatherSystem.Core;
using CryingOnion.OhMy.WeatherSystem.Utility;
using CryingOnion.OhMy.WeatherSystem.Module;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CryingOnion.OhMy.WeatherSystem.Data
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "CryingOnion/Oh My Weather System/FX/Multi FX", order = 361)]
    public class OMWSMultiFXProfile : OMWSFXProfile
    {
        public OMWSWeather weather;

        [System.Serializable]
        public class OMWSMultiFXType
        {
            public OMWSFXProfile FX;
            public AnimationCurve intensityCurve;
        }

        [OMWSMultiAudio] public List<OMWSMultiFXType> multiFX;

        public override void PlayEffect()
        {
            if (weather == null)
                weather = OMWSWeather.instance;

            foreach (OMWSMultiFXType i in multiFX)
                i.FX.PlayEffect();
        }

        public override void PlayEffect(float weight)
        {
            if (weather == null)
                weather = OMWSWeather.instance;

            foreach (OMWSMultiFXType i in multiFX)
                i.FX.PlayEffect(i.intensityCurve.Evaluate(weather.GetCurrentDayPercentage()) * weight);
        }

        public override void StopEffect()
        {
            if (weather == null)
                weather = OMWSWeather.instance;

            foreach (OMWSMultiFXType i in multiFX)
                i.FX.StopEffect();
        }

        public override bool InitializeEffect(OMWSVFXModule VFX)
        {
            if (VFX == null)
                VFX = OMWSWeather.instance.VFX;

            VFXMod = VFX;

            weather = VFX.weatherSphere;

            foreach (OMWSMultiFXType i in multiFX)
                i.FX.InitializeEffect(VFX);

            return true;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(OMWSMultiFXProfile))]
    [CanEditMultipleObjects]
    public class OMWSMultiFXProfileEditor : OMWSFXProfileEditor
    {
        void OnEnable() { }

        public override void RenderInWindow(Rect pos)
        {
            float space = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            var propPosA = new Rect(pos.x, pos.y + space, pos.width, EditorGUIUtility.singleLineHeight);

            serializedObject.Update();

            EditorGUI.PropertyField(propPosA, serializedObject.FindProperty("multiFX"));

            serializedObject.ApplyModifiedProperties();
        }

        public override float GetLineHeight() => 1 + (serializedObject.FindProperty("multiFX").isExpanded ? serializedObject.FindProperty("multiFX").arraySize : 0);

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("multiFX"));

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
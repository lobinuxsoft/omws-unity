using UnityEngine;
using CryingOnion.OhMy.WeatherSystem.Core;
using CryingOnion.OhMy.WeatherSystem.Module;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CryingOnion.OhMy.WeatherSystem.Data
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "CryingOnion/Oh My Weather System/FX/Filter FX", order = 361)]
    public class OMWSFilterFX : OMWSFXProfile
    {
        [Range(-1, 1)] public float filterSaturation;
        [Range(-1, 1)] public float filterValue;
        [ColorUsage(false, true)] public Color filterColor = Color.white;
        [ColorUsage(false, true)] public Color sunFilter = Color.white;
        [ColorUsage(false, true)] public Color cloudFilter = Color.white;
        public float weight;
        OMWSWeather weather;

        public override void PlayEffect()
        {
            if (!VFXMod)
                if (InitializeEffect(null) == false)
                    return;

            if (VFXMod.filterManager.isEnabled)
                weight = 1;
            else
                weight = 0;
            weather.CalculateFilterColors();
        }

        public override void PlayEffect(float i)
        {
            if (!VFXMod)
                if (InitializeEffect(null) == false)
                    return;

            if (VFXMod.filterManager.isEnabled)
                weight = Mathf.Clamp01(transitionTimeModifier.Evaluate(i));
            else
                weight = 0;

            weather.CalculateFilterColors();
        }

        public override void StopEffect()
        {
            if (!VFXMod)
                if (InitializeEffect(null) == false)
                    return;

            weight = 0;
            weather.CalculateFilterColors();
        }

        public override bool InitializeEffect(OMWSVFXModule VFX)
        {
            if (VFX == null)
                VFX = OMWSWeather.instance.VFX;

            VFXMod = VFX;

            if (!VFX.filterManager.isEnabled)
                return false;

            VFX.weatherSphere.possibleFilters.Add(this);
            weather = VFX.weatherSphere;
            StopEffect();

            return true;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(OMWSFilterFX))]
    [CanEditMultipleObjects]
    public class OMWSFilterFXEditor : OMWSFXProfileEditor
    {
        void OnEnable() { }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("filterSaturation"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("filterValue"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("filterColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("sunFilter"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudFilter"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("transitionTimeModifier"));

            serializedObject.ApplyModifiedProperties();
        }

        public override void RenderInWindow(Rect pos)
        {
            float space = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            var propPosA = new Rect(pos.x, pos.y + space, pos.width, EditorGUIUtility.singleLineHeight);
            var propPosB = new Rect(pos.x, pos.y + space * 2, pos.width, EditorGUIUtility.singleLineHeight);
            var propPosC = new Rect(pos.x, pos.y + space * 3, pos.width, EditorGUIUtility.singleLineHeight);
            var propPosD = new Rect(pos.x, pos.y + space * 4, pos.width, EditorGUIUtility.singleLineHeight);
            var propPosE = new Rect(pos.x, pos.y + space * 5, pos.width, EditorGUIUtility.singleLineHeight);
            var propPosF = new Rect(pos.x, pos.y + space * 6, pos.width, EditorGUIUtility.singleLineHeight);

            serializedObject.Update();

            EditorGUI.PropertyField(propPosA, serializedObject.FindProperty("filterSaturation"));
            EditorGUI.PropertyField(propPosB, serializedObject.FindProperty("filterValue"));
            EditorGUI.PropertyField(propPosC, serializedObject.FindProperty("filterColor"));
            EditorGUI.PropertyField(propPosD, serializedObject.FindProperty("sunFilter"));
            EditorGUI.PropertyField(propPosE, serializedObject.FindProperty("cloudFilter"));
            EditorGUI.PropertyField(propPosF, serializedObject.FindProperty("transitionTimeModifier"));

            serializedObject.ApplyModifiedProperties();
        }

        public override float GetLineHeight() => 6;

    }
#endif
}
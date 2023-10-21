using UnityEngine;
using CryingOnion.OhMy.WeatherSystem.Core;
using CryingOnion.OhMy.WeatherSystem.Module;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CryingOnion.OhMy.WeatherSystem.Data
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "CryingOnion/Oh My Weather System/FX/Wind FX", order = 361)]
    public class OMWSWindFX : OMWSFXProfile
    {
        [Range(0, 2)]
        public float windAmount;
        [Range(0, 2)]
        public float windSpeed;
        public float weight;
        OMWSWeather weather;

        public override void PlayEffect() => weight = 1;

        public override void PlayEffect(float i) => weight = Mathf.Clamp01(transitionTimeModifier.Evaluate(i));

        public override void StopEffect()
        {
            weight = 0;
        }

        public override bool InitializeEffect(OMWSVFXModule VFX)
        {
            if (VFX == null)
                VFX = OMWSWeather.instance.VFX;

            VFXMod = VFX;

            if (!VFX.filterManager.isEnabled)
                return false;

            if (VFX)
            {
                VFX.windManager.windFXes.Add(this);
                weather = VFX.weatherSphere;
            }

            return true;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(OMWSWindFX))]
    [CanEditMultipleObjects]
    public class OMWSWindFXEditor : OMWSFXProfileEditor
    {
        void OnEnable() { }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("windAmount"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("windSpeed"));
            EditorGUILayout.Space();
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

            EditorGUI.PropertyField(propPosA, serializedObject.FindProperty("windAmount"));
            EditorGUI.PropertyField(propPosB, serializedObject.FindProperty("windSpeed"));

            EditorGUI.PropertyField(propPosC, serializedObject.FindProperty("transitionTimeModifier"));

            serializedObject.ApplyModifiedProperties();
        }

        public override float GetLineHeight() => 3;
    }
#endif
}
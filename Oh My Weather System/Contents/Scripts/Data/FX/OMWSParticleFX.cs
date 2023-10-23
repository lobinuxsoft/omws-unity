using UnityEngine;
using CryingOnion.OhMy.WeatherSystem.Module;
using CryingOnion.OhMy.WeatherSystem.Core;
using CryingOnion.OhMy.WeatherSystem.Utility;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CryingOnion.OhMy.WeatherSystem.Data
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "CryingOnion/Oh My Weather System/FX/Particle FX", order = 361)]
    public class OMWSParticleFX : OMWSFXProfile
    {
        public OMWSParticles particleSystem;
        private OMWSParticles runtimeRef;
        public bool autoScale;

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

            runtimeRef.Play();
        }

        public override void PlayEffect(float intensity)
        {
            if (!runtimeRef)
                if (InitializeEffect(VFXMod) == false)
                    return;

            if (runtimeRef.transform.parent == null)
            {
                runtimeRef.transform.parent = VFXMod.particleManager.parent;
                runtimeRef.transform.localPosition = Vector3.zero;
            }

            if (intensity == 0)
            {
                if (runtimeRef.m_ParticleTypes[0].particleSystem.main.startLifetime.mode == ParticleSystemCurveMode.Constant)
                    Destroy(runtimeRef.gameObject, runtimeRef.m_ParticleTypes[0].particleSystem.main.startLifetime.constant);
                else
                    Destroy(runtimeRef.gameObject, runtimeRef.m_ParticleTypes[0].particleSystem.main.startLifetime.constantMax);

                runtimeRef = null;
                return;
            }

            runtimeRef.Play(transitionTimeModifier.Evaluate(intensity));
        }

        public override void StopEffect()
        {
            if (!runtimeRef)
                return;

            runtimeRef.Stop();

            Destroy(runtimeRef.gameObject);
        }

        public override bool InitializeEffect(OMWSVFXModule VFX)
        {
            if (VFX == null)
                VFX = OMWSWeather.instance.VFX;

            VFXMod = VFX;

            if (!VFX.particleManager.isEnabled)
                return false;

            runtimeRef = Instantiate(particleSystem, VFX.particleManager.parent).GetComponent<OMWSParticles>();

            if (autoScale)
                runtimeRef.transform.localScale *= VFX.weatherSphere.transform.localScale.x;

            runtimeRef.particleManager = VFX.particleManager;
            runtimeRef.SetupTriggers();

            runtimeRef.gameObject.name = name;

            return true;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(OMWSParticleFX))]
    [CanEditMultipleObjects]
    public class OMWSParticleFXEditor : OMWSFXProfileEditor
    {
        void OnEnable() { }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("particleSystem"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("autoScale"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("transitionTimeModifier"));

            serializedObject.ApplyModifiedProperties();
        }

        public override void RenderInWindow(Rect pos)
        {
            float space = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            var propPosA = new Rect(pos.x, pos.y + space, pos.width, EditorGUIUtility.singleLineHeight);
            var propPosB = new Rect(pos.x, pos.y + space * 2, pos.width, EditorGUIUtility.singleLineHeight);
            var propPosC = new Rect(pos.x, pos.y + space * 3, pos.width, EditorGUIUtility.singleLineHeight);

            serializedObject.Update();

            EditorGUI.PropertyField(propPosA, serializedObject.FindProperty("particleSystem"));
            EditorGUI.PropertyField(propPosB, serializedObject.FindProperty("autoScale"));
            EditorGUI.PropertyField(propPosC, serializedObject.FindProperty("transitionTimeModifier"));

            serializedObject.ApplyModifiedProperties();
        }

        public override float GetLineHeight() => 3;
    }
#endif
}
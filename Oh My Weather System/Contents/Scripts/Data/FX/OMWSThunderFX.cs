using UnityEngine;
using CryingOnion.OhMy.WeatherSystem.Module;
using CryingOnion.OhMy.WeatherSystem.Core;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CryingOnion.OhMy.WeatherSystem.Data
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "CryingOnion/Oh My Weather System/FX/Thunder FX", order = 361)]
    public class OMWSThunderFX : OMWSFXProfile
    {
        public Vector2 timeBetweenStrikes;
        public float weight;
        OMWSThunderManager thunderManager;

        public override void PlayEffect()
        {
            if (!VFXMod)
                if (InitializeEffect(null) == false)
                    return;

            if (VFXMod.thunderManager.isEnabled)
                weight = 1;
            else
                weight = 0;
        }

        public override void PlayEffect(float i)
        {
            if (!VFXMod)
                if (InitializeEffect(null) == false)
                    return;

            if (VFXMod.thunderManager.isEnabled)
                weight = i;
            else
                weight = 0;
        }

        public override void StopEffect() => weight = 0;

        public override bool InitializeEffect(OMWSVFXModule VFX)
        {
            if (VFX == null)
                VFX = OMWSWeather.instance.VFX;

            VFXMod = VFX;

            if (!VFX.thunderManager.isEnabled)
                return false;

            thunderManager = VFX.thunderManager;
            thunderManager.thunderFX.Add(this);

            return true;
        }
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(OMWSThunderFX))]
    [CanEditMultipleObjects]
    public class OMWSThunderFXEditor : OMWSFXProfileEditor
    {
        void OnEnable() { }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("timeBetweenStrikes"), new GUIContent("Time Between Strikes"));

            serializedObject.ApplyModifiedProperties();
        }

        public override void RenderInWindow(Rect pos)
        {
            float space = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            var propPosA = new Rect(pos.x, pos.y + space, pos.width, EditorGUIUtility.singleLineHeight);

            serializedObject.Update();

            EditorGUI.PropertyField(propPosA, serializedObject.FindProperty("timeBetweenStrikes"));

            serializedObject.ApplyModifiedProperties();
        }

        public override float GetLineHeight() => 1;
    }
#endif
}
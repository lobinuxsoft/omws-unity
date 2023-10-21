using System.Collections.Generic;
using UnityEngine;
using CryingOnion.OhMy.WeatherSystem.Core;
using UnityEngine.Audio;
using CryingOnion.OhMy.WeatherSystem.Utility;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CryingOnion.OhMy.WeatherSystem.Module
{
    [ExecuteAlways]
    public class OMWSVFXModule : OMWSModule
    {
        #region Particles

        [SerializeField]
        [Tooltip("Set the color of these particle systems to the star color of the weather system.")]
        private List<ParticleSystem> m_Stars = new List<ParticleSystem>();
        [Tooltip("Set the color of these particle systems to the cloud color of the weather system.")]
        [SerializeField]
        private List<ParticleSystem> m_CloudParticles = new List<ParticleSystem>();

        #endregion

        public Transform parent;
        public bool hideFXInHierarchy = true;

        public OMWSParticleManager particleManager = new OMWSParticleManager();
        public OMWSThunderManager thunderManager = new OMWSThunderManager();
        public OMWSFilterManager filterManager = new OMWSFilterManager();
        public OMWSAudioManager audioManager = new OMWSAudioManager();
        public OMWSPostFXManager postFXManager = new OMWSPostFXManager();
        public OMWSPrecipitationManager precipitationManager = new OMWSPrecipitationManager();
        public OMWSWindManager windManager = new OMWSWindManager();

        void OnEnable()
        {
            if (GetComponent<OMWSWeather>())
            {
                GetComponent<OMWSWeather>().IntitializeModule(typeof(OMWSSatelliteManager));
                DestroyImmediate(this);
                Debug.LogWarning("Add modules in the settings tab in OMWS 2!");
                return;
            }

            base.SetupModule();

            particleManager.vfx = this;
            thunderManager.vfx = this;
            windManager.vfx = this;
            audioManager.vfx = this;
            filterManager.vfx = this;
            postFXManager.vfx = this;
            precipitationManager.vfx = this;

            ResetFXHandler();
        }

        void Update()
        {
            if (weatherSphere == null)
                base.SetupModule();

            if (parent == null)
                ResetFXHandler();

            parent.position = transform.position;

            SetStarColors(weatherSphere.starColor);
            SetCloudColors(weatherSphere.cloudColor);

            thunderManager.OnFXUpdate();
            windManager.OnFXUpdate();
            particleManager.OnFXUpdate();
            audioManager.OnFXUpdate();
            filterManager.OnFXUpdate();
            postFXManager.OnFXUpdate();
            precipitationManager.OnFXUpdate();
        }

        public override void DisableModule()
        {
            if (parent)
                DestroyImmediate(parent.gameObject);

            thunderManager.OnFXDisable();
            windManager.OnFXDisable();
            particleManager.OnFXDisable();
            audioManager.OnFXDisable();
            postFXManager.OnFXDisable();
            precipitationManager.OnFXDisable();
            filterManager.OnFXDisable();
        }

        /// <summary>
        /// Sets the colors of the star particle systems.
        /// </summary> 
        private void SetStarColors(Color color)
        {
            if (m_Stars.Count == 0)
                return;

            foreach (ParticleSystem i in m_Stars)
            {
                if (i == null)
                    continue;

                ParticleSystem.MainModule j = i.main;
                j.startColor = color;
            }
        }

        /// <summary>
        /// Sets the colors of the cloud particle systems.
        /// </summary> 
        private void SetCloudColors(Color color)
        {
            if (m_CloudParticles.Count == 0)
                return;

            foreach (ParticleSystem i in m_CloudParticles)
            {
                if (i == null)
                    continue;

                ParticleSystem.MainModule j = i.main;
                j.startColor = color;

                ParticleSystem.TrailModule k = i.trails;
                k.colorOverLifetime = color;
            }
        }

        public void ResetFXHandler()
        {
            GameObject i = new GameObject();

            i.name = "OMWS FX";

            i.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild;

            if (hideFXInHierarchy)
                i.hideFlags = i.hideFlags | HideFlags.HideInHierarchy;

            parent = i.transform;

            thunderManager.OnFXEnable();
            windManager.OnFXEnable();
            particleManager.OnFXEnable();
            audioManager.OnFXEnable();
            precipitationManager.OnFXEnable();
            filterManager.OnFXEnable();
            postFXManager.OnFXEnable();
        }

        void Reset()
        {
            thunderManager.thunderPrefab = Resources.Load("OMWS Prefabs/Thunder And Lightning") as GameObject;
            audioManager.weatherFXMixer = (Resources.Load("OMWS Weather Mixer") as AudioMixer).FindMatchingGroups("Weather FX")[0];
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(OMWSVFXModule))]
    [CanEditMultipleObjects]
    public class OMWSVFXModuleEditor : OMWSModuleEditor
    {
        OMWSVFXModule t;

        protected static bool thunderTab;
        protected static bool windTab;
        protected static bool ParticlesTab;
        protected static bool FXTab;

        public override GUIContent GetGUIContent() =>
            new GUIContent("    VFX", (Texture)Resources.Load("FX Module"), "Manage FX types, particles, and other VFX related options.");

        void OnEnable() => t = (OMWSVFXModule)target;

        public override void DisplayInOMWSWindow()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("particleManager"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("audioManager"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("postFXManager"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("precipitationManager"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("filterManager"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("thunderManager"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("windManager"), true);

            ParticlesTab = EditorGUILayout.BeginFoldoutHeaderGroup(ParticlesTab, new GUIContent("    Miscellaneous Options"), EditorUtilities.FoldoutStyle());
            EditorGUILayout.EndFoldoutHeaderGroup();

            if (ParticlesTab)
            {

                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("Linked Particles");
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Stars"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_CloudParticles"));
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("hideFXInHierarchy"));
                if (EditorGUI.EndChangeCheck())
                    if (t.parent)
                        DestroyImmediate(t.parent.gameObject);

                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
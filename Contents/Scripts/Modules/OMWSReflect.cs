using UnityEngine;
using CryingOnion.OhMy.WeatherSystem.Core;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CryingOnion.OhMy.WeatherSystem.Module
{
    [ExecuteAlways]
    public class OMWSReflect : OMWSModule
    {
        public Cubemap reflectionCubemap;
        public Camera reflectionCamera;

        [Tooltip("How many frames should pass before the cubemap renders again? A value of 0 renders every frame and a value of 30 renders once every 30 frames.")]
        [Range(0, 30)] public int framesBetweenRenders = 10;

        [Tooltip("What layers should be rendered into the skybox reflections?.")]
        public LayerMask layerMask = 2048;
        int framesLeft;

        void OnEnable()
        {
            base.SetupModule();
            reflectionCubemap = Resources.Load("Materials/Reflection Cubemap") as Cubemap;
            RenderSettings.customReflection = reflectionCubemap;
            RenderSettings.defaultReflectionMode = UnityEngine.Rendering.DefaultReflectionMode.Custom;
            weatherSphere.fogMesh.gameObject.layer = ToLayer(layerMask);
            weatherSphere.skyMesh.gameObject.layer = ToLayer(layerMask);
            weatherSphere.cloudMesh.gameObject.layer = ToLayer(layerMask);
        }

        void Update()
        {
            if (weatherSphere == null)
                base.SetupModule();

            if (framesLeft < 0)
            {
                RenderReflections();
                framesLeft = framesBetweenRenders;
            }

            framesLeft--;
        }

        public int ToLayer(LayerMask mask)
        {
            int value = mask.value;
            if (value == 0)
                return 0;

            for (int l = 1; l < 32; l++)
                if ((value & (1 << l)) != 0)
                    return l;

            return -1;
        }

        void OnDisable()
        {
            if (reflectionCamera)
                DestroyImmediate(reflectionCamera.gameObject);

            RenderSettings.customReflection = null;
        }

        public void RenderReflections()
        {
            if (!weatherSphere.omwsCamera)
            {
                Debug.LogError("OMWS Reflections requires the omws camera to be set in the settings tab!");
                return;
            }

            if (reflectionCamera == null)
                SetupCamera();

            reflectionCamera.enabled = true;
            reflectionCamera.transform.position = transform.position;
            reflectionCamera.nearClipPlane = weatherSphere.omwsCamera.nearClipPlane;
            reflectionCamera.farClipPlane = weatherSphere.omwsCamera.farClipPlane;
            reflectionCamera.cullingMask = layerMask;


            reflectionCamera.RenderToCubemap(reflectionCubemap);
            reflectionCamera.enabled = false;
        }

        public void SetupCamera()
        {
            GameObject i = new GameObject();
            i.name = "OMWS Reflection Camera";
            i.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild | HideFlags.HideInHierarchy;

            reflectionCamera = i.AddComponent<Camera>();
            reflectionCamera.depth = -50;
            reflectionCamera.enabled = false;
        }

    }

#if UNITY_EDITOR
    [CustomEditor(typeof(OMWSReflect))]
    [CanEditMultipleObjects]
    public class OMWSReflectEditor : OMWSModuleEditor
    {
        OMWSReflect reflect;

        public override GUIContent GetGUIContent() =>
            new GUIContent("", (Texture)Resources.Load("Reflections"), "Reflections: Sets up a cubemap for reflections with OMWS.");

        void OnEnable() { }

        public override void DisplayInOMWSWindow()
        {
            if (reflect == null)
                reflect = (OMWSReflect)target;

            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("framesBetweenRenders"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("reflectionCubemap"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("layerMask"));

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
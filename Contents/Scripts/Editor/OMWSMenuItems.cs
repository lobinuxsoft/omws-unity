using CryingOnion.OhMy.WeatherSystem.Core;
using CryingOnion.OhMy.WeatherSystem.Utility;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.EditorScript
{
    public class OMWSMenuItems : MonoBehaviour
    {
        [MenuItem("Crying Onion/Oh My Weather System/Create OMWS Volume")]
        static void OMWSVolumeCreation()
        {
            Camera view = SceneView.lastActiveSceneView.camera;

            GameObject i = new GameObject();
            i.name = "OMWS Volume";
            i.AddComponent<BoxCollider>().isTrigger = true;
            i.AddComponent<OMWSVolume>();
            i.transform.position = (view.transform.forward * 5) + view.transform.position;

            Undo.RegisterCreatedObjectUndo(i, "Create OMWS Volume");
            Selection.activeGameObject = i;
        }

        [MenuItem("Crying Onion/Oh My Weather System/Create OMWS FX Block Zone")]
        static void OMWSBlockZoneCreation()
        {
            Camera view = SceneView.lastActiveSceneView.camera;

            GameObject i = new GameObject();
            i.name = "OMWS FX Block Zone";
            i.AddComponent<BoxCollider>().isTrigger = true;
            i.tag = "FX Block Zone";
            i.transform.position = (view.transform.forward * 5) + view.transform.position;

            Undo.RegisterCreatedObjectUndo(i, "Create OMWS FX Block Zone");
            Selection.activeGameObject = i;
        }

        [MenuItem("Crying Onion/Oh My Weather System/Create OMWS Biome")]
        static void OMWSBiomeCreation()
        {
            Camera view = SceneView.lastActiveSceneView.camera;

            GameObject i = new GameObject();
            i.name = "OMWS Biome";
            i.AddComponent<OMWSBiome>();
            i.transform.position = (view.transform.forward * 5) + view.transform.position;

            Undo.RegisterCreatedObjectUndo(i, "Create OMWS Biome");
            Selection.activeGameObject = i;
        }

        [MenuItem("Crying Onion/Oh My Weather System/Toggle Tooltips")]
        static void OMWSToggleTooltips() => EditorPrefs.SetBool("OMWS_Tooltips", !EditorPrefs.GetBool("OMWS_Tooltips"));

        [MenuItem("Crying Onion/Oh My Weather System/Setup Scene (No Modules)", false, 1)]
        public static void OMWSSetupSceneSimple()
        {
            if (FindObjectOfType<OMWSWeather>())
            {
                EditorUtility.DisplayDialog("OMWS:Weather", "You already have a OMWS:Weather system in your scene!", "Ok");
                return;
            }

            if (!Camera.main)
            {
                EditorUtility.DisplayDialog("OMWS:Weather", "You need a main camera in your scene to setup for OMWS:Weather!", "Ok");
                return;
            }

            if (FindObjectsOfType<Light>().Length != 0)
                foreach (Light i in FindObjectsOfType<Light>())
                {
                    if (i.type == LightType.Directional)
                        if (EditorUtility.DisplayDialog("You already have a directional light!", "Do you want to delete " + i.gameObject.name + "? OMWS:Weather will properly light your scene", "Delete", "Keep this light"))
                            DestroyImmediate(i.gameObject);

                }

            if (!Camera.main.GetComponent<FlareLayer>())
                Camera.main.gameObject.AddComponent<FlareLayer>();

#if UNITY_POST_PROCESSING_STACK_V2

            if (!FindObjectOfType<UnityEngine.Rendering.PostProcessing.PostProcessVolume>())
            {
                List<string> path = new List<string>();
                path.Add("Assets/Distant Lands/OMWS Weather/Post FX/");

                GameObject i = new GameObject();

                i.name = "Post FX Volume";
                i.AddComponent<UnityEngine.Rendering.PostProcessing.PostProcessVolume>().profile = GetAssets<UnityEngine.Rendering.PostProcessing.PostProcessProfile>(path.ToArray(), "Post FX")[0];
                i.GetComponent<UnityEngine.Rendering.PostProcessing.PostProcessVolume>().isGlobal = true;
                i.layer = 1;

                if (!Camera.main.GetComponent<UnityEngine.Rendering.PostProcessing.PostProcessLayer>())
                    Camera.main.gameObject.AddComponent<UnityEngine.Rendering.PostProcessing.PostProcessLayer>().volumeLayer = 2;
            }
#endif

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;

            GameObject weatherSphere = Instantiate(Resources.Load("OMWS Prefabs/Empty Weather Sphere Reference") as GameObject);

            weatherSphere.name = "OMWS Weather Sphere";
        }
        
        [MenuItem("Crying Onion/Oh My Weather System/Setup Scene", false, 1)]
        public static void OMWSSetupScene()
        {
            if (FindObjectOfType<OMWSWeather>())
            {
                EditorUtility.DisplayDialog("OMWS: Weather", "You already have a OMWS: Weather system in your scene!", "Ok");
                return;
            }

            if (!Camera.main)
            {
                EditorUtility.DisplayDialog("OMWS: Weather", "You need a main camera in your scene to setup for OMWS: Weather!", "Ok");
                return;
            }

            if (FindObjectsOfType<Light>().Length != 0)
            {
                foreach (Light i in FindObjectsOfType<Light>())
                {
                    if (i.type == LightType.Directional)
                        if (EditorUtility.DisplayDialog("You already have a directional light!", "Do you want to delete " + i.gameObject.name + "? OMWS:Weather will properly light your scene", "Delete", "Keep this light"))
                            DestroyImmediate(i.gameObject);
                }
            }

            if (!Camera.main.GetComponent<FlareLayer>())
                Camera.main.gameObject.AddComponent<FlareLayer>();


#if UNITY_POST_PROCESSING_STACK_V2

            if (!FindObjectOfType<UnityEngine.Rendering.PostProcessing.PostProcessVolume>())
            {
                List<string> path = new List<string>();
                path.Add("Assets/Distant Lands/OMWS Weather/Post FX/");

                GameObject i = new GameObject();

                i.name = "Post FX Volume";
                i.AddComponent<UnityEngine.Rendering.PostProcessing.PostProcessVolume>().profile = GetAssets<UnityEngine.Rendering.PostProcessing.PostProcessProfile>(path.ToArray(), "Post FX")[0];
                i.GetComponent<UnityEngine.Rendering.PostProcessing.PostProcessVolume>().isGlobal = true;
                i.layer = 1;

                if (!Camera.main.GetComponent<UnityEngine.Rendering.PostProcessing.PostProcessLayer>())
                    Camera.main.gameObject.AddComponent<UnityEngine.Rendering.PostProcessing.PostProcessLayer>().volumeLayer = 2;
            }
#endif

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.defaultReflectionMode = UnityEngine.Rendering.DefaultReflectionMode.Custom;

            GameObject weatherSphere = Instantiate(Resources.Load("OMWS Prefabs/OMWS Weather Sphere") as GameObject);

            weatherSphere.name = "OMWS Weather Sphere";
        }

        public static List<T> GetAssets<T>(string[] _foldersToSearch, string _filter) where T : UnityEngine.Object
        {
            string[] guids = AssetDatabase.FindAssets(_filter, _foldersToSearch);
            List<T> a = new List<T>();

            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                a.Add(AssetDatabase.LoadAssetAtPath<T>(path));
            }

            return a;
        }
    }
}
using UnityEngine;
using CryingOnion.OhMy.WeatherSystem.Core;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CryingOnion.OhMy.WeatherSystem.Module
{
    public class OMWSSaveLoad : OMWSModule
    {
        void OnEnable()
        {
            if (GetComponent<OMWSWeather>())
            {
                GetComponent<OMWSWeather>().IntitializeModule(typeof(OMWSSaveLoad));
                DestroyImmediate(this);
                Debug.LogWarning("Add modules in the settings tab in OMWS 2!");
                return;
            }
        }

        // Start is called before the first frame update
        void Awake()
        {
            if (!enabled)
                return;

            SetupModule();
        }


        public void Save()
        {
            if (weatherSphere == null)
                SetupModule();

            string weatherJSON = JsonUtility.ToJson(weatherSphere);
            PlayerPrefs.SetString("OMWS_Properties", weatherJSON);
            PlayerPrefs.SetString("OMWS_Perennial", JsonUtility.ToJson(weatherSphere.perennialProfile));
        }

        public void Load()
        {
            if (weatherSphere == null)
                SetupModule();

            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString("OMWS_Properties"), weatherSphere);
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString("OMWS_Perennial"), weatherSphere.perennialProfile);

            weatherSphere.Update();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(OMWSSaveLoad))]
    public class OMWSSaveLoadEditor : OMWSModuleEditor
    {
        OMWSSaveLoad saveLoad;

        void OnEnable() => saveLoad = (OMWSSaveLoad)target;

        public override GUIContent GetGUIContent() => new GUIContent("", (Texture)Resources.Load("Save"), "Save & Load: Manage save and load commands within the OMWS system.");

        public override void OnInspectorGUI() { }

        public override void DisplayInOMWSWindow()
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Save"))
                saveLoad.Save();
            if (GUILayout.Button("Load"))
                saveLoad.Load();

            EditorGUILayout.EndHorizontal();
        }
    }
#endif
}
using UnityEngine;
using CryingOnion.OhMy.WeatherSystem.Core;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CryingOnion.OhMy.WeatherSystem.Utility
{
    [ExecuteAlways]
    public class OMWSSatellite : MonoBehaviour
    {
        public float orbitOffset;
        public float satelliteRotateSpeed;
        public float satelliteDirection;
        private Transform m_Satellite;
        private OMWSWeather m_WeatherManager;

        // Start is called before the first frame update
        void Awake()
        {
            m_Satellite = transform.GetChild(0);
            m_WeatherManager = OMWSWeather.instance;
        }

        // Update is called once per frame
        void Update()
        {
            m_Satellite.localEulerAngles = m_Satellite.localEulerAngles + Vector3.up * Time.deltaTime * satelliteRotateSpeed;
            transform.localEulerAngles = new Vector3(-((m_WeatherManager.GetCurrentDayPercentage() * 360) - 90 + orbitOffset), satelliteDirection, 0);
        }
    }
#if UNITY_EDITOR

    [CustomEditor(typeof(OMWSSatellite))]
    [CanEditMultipleObjects]
    public class OMWSSatelliteEditor : Editor
    {
        public int windowNum;

        Color proCol = (Color)new Color32(50, 50, 50, 255);
        Color unityCol = (Color)new Color32(194, 194, 194, 255);

        void OnEnable()
        {
            serializedObject.Update();

            serializedObject.FindProperty("icon1").objectReferenceValue = Resources.Load<Texture>("Atmosphere");
            serializedObject.FindProperty("icon2").objectReferenceValue = Resources.Load<Texture>("OMWSCalendar");
            serializedObject.FindProperty("icon3").objectReferenceValue = Resources.Load<Texture>("Weather Profile-01");
            serializedObject.FindProperty("icon4").objectReferenceValue = Resources.Load<Texture>("OMWSTrigger");

            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.HelpBox("NOTICE: This component is now deprecated and will be removed in a future version of OMWS. Please use the new satellite profile system instead!", MessageType.Warning);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("orbitOffset"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("satelliteRotateSpeed"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("satelliteDirection"));

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
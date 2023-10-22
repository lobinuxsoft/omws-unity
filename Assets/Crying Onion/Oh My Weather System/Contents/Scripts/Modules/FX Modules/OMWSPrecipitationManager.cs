using System.Collections.Generic;
using UnityEngine;
using CryingOnion.OhMy.WeatherSystem.Data;
using CryingOnion.OhMy.WeatherSystem.Utility;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CryingOnion.OhMy.WeatherSystem.Module
{
    [System.Serializable]
    public class OMWSPrecipitationManager : OMWSFXModule
    {
        [Range(0, 2)] public float accumulationMultiplier = 1;
        public List<OMWSPrecipitationFX> precipitationFXes = new List<OMWSPrecipitationFX>();

        public override void OnFXEnable()
        {
            if (!vfx.weatherSphere.omwsMaterials)
                vfx.weatherSphere.omwsMaterials = vfx.weatherSphere.GetModule<OMWSMaterialManager>();
        }

        public override void OnFXUpdate()
        {
            if (vfx.weatherSphere.omwsMaterials == null)
                return;

            float snowSpeed = 0;
            float rainSpeed = 0;

            foreach (OMWSPrecipitationFX j in precipitationFXes)
            {
                if (j.weight > 0)
                {
                    snowSpeed += j.snowAmount * j.weight * accumulationMultiplier;
                    rainSpeed += j.rainAmount * j.weight * accumulationMultiplier;
                }
            }

            vfx.weatherSphere.omwsMaterials.rainSpeed = rainSpeed;
            vfx.weatherSphere.omwsMaterials.snowSpeed = snowSpeed;
        }

        public override void OnFXDisable() { }

        public override void SetupFXParent() { }
    }

#if UNITY_EDITOR
    [UnityEditor.CustomPropertyDrawer(typeof(OMWSPrecipitationManager))]
    public class OMWSPrecipitationManagerDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            Rect pos = position;

            Rect tabPos = new Rect(pos.x + 35, pos.y, pos.width - 41, pos.height);
            Rect togglePos = new Rect(5, pos.y, 30, pos.height);

            property.FindPropertyRelative("_OpenTab").boolValue = EditorGUI.BeginFoldoutHeaderGroup(tabPos, property.FindPropertyRelative("_OpenTab").boolValue, new GUIContent("    Precipitation FX", "Precipitation FX allow for snow and rain accumulation in your scene using the material manager."), OMWSEditorUtilities.FoldoutStyle());

            bool toggle = EditorGUI.Toggle(togglePos, GUIContent.none, property.FindPropertyRelative("_IsEnabled").boolValue);

            if (property.FindPropertyRelative("_IsEnabled").boolValue != toggle)
            {
                property.FindPropertyRelative("_IsEnabled").boolValue = toggle;

                if (toggle == true)
                    (property.serializedObject.targetObject as OMWSVFXModule).precipitationManager.OnFXEnable();
                else
                    (property.serializedObject.targetObject as OMWSVFXModule).precipitationManager.OnFXDisable();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            if (property.FindPropertyRelative("_OpenTab").boolValue)
            {
                using (new EditorGUI.DisabledScope(!property.FindPropertyRelative("_IsEnabled").boolValue))
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("accumulationMultiplier"));
                    EditorGUI.indentLevel--;
                }
            }

            EditorGUI.EndProperty();
        }
    }
#endif
}
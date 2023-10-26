using CryingOnion.OhMy.WeatherSystem.Module;
using CryingOnion.OhMy.WeatherSystem.Utility;
using UnityEditor;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.EditorScript
{
    [CustomPropertyDrawer(typeof(OMWSWindManager))]
    public class OMWSWindManagerDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            Rect pos = position;

            Rect tabPos = new Rect(pos.x + 35, pos.y, pos.width - 41, pos.height);
            Rect togglePos = new Rect(5, pos.y, 30, pos.height);

            property.FindPropertyRelative("_OpenTab").boolValue = EditorGUI.BeginFoldoutHeaderGroup(tabPos, property.FindPropertyRelative("_OpenTab").boolValue, new GUIContent("    Wind FX", "Wind FX set the speed and amount of the OMWS windzone and shader-based wind."), OMWSEditorUtilities.FoldoutStyle());

            bool toggle = EditorGUI.Toggle(togglePos, GUIContent.none, property.FindPropertyRelative("_IsEnabled").boolValue);

            if (property.FindPropertyRelative("_IsEnabled").boolValue != toggle)
            {
                property.FindPropertyRelative("_IsEnabled").boolValue = toggle;

                if (toggle == true)
                    (property.serializedObject.targetObject as OMWSVFXModule).windManager.OnFXEnable();
                else
                    (property.serializedObject.targetObject as OMWSVFXModule).windManager.OnFXDisable();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            if (property.FindPropertyRelative("_OpenTab").boolValue)
            {
                using (new EditorGUI.DisabledScope(!property.FindPropertyRelative("_IsEnabled").boolValue))
                {
                    EditorGUILayout.Space();
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("useWindzone"));

                    if (property.FindPropertyRelative("useWindzone").boolValue)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(property.FindPropertyRelative("generateWindzoneAtRuntime"));
                        EditorGUI.indentLevel--;
                    }

                    if (!property.FindPropertyRelative("generateWindzoneAtRuntime").boolValue)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(property.FindPropertyRelative("windZone"));
                        EditorGUI.indentLevel--;
                    }

                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("useShaderWind"));

                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("windMultiplier"));
                    EditorGUI.indentLevel--;

                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("Edit wind speed properties in the invdividual wind FX profiles!", MessageType.Info);
                }
            }

            EditorGUI.EndProperty();
        }
    }
}
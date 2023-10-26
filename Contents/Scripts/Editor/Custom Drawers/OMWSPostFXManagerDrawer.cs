using CryingOnion.OhMy.WeatherSystem.Module;
using CryingOnion.OhMy.WeatherSystem.Utility;
using UnityEditor;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.EditorScript
{
    [CustomPropertyDrawer(typeof(OMWSPostFXManager))]
    public class OMWSPostFXManagerDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            Rect pos = position;

            Rect tabPos = new Rect(pos.x + 35, pos.y, pos.width - 41, pos.height);
            Rect togglePos = new Rect(5, pos.y, 30, pos.height);

            property.FindPropertyRelative("_OpenTab").boolValue = EditorGUI.BeginFoldoutHeaderGroup(tabPos, property.FindPropertyRelative("_OpenTab").boolValue, new GUIContent("    Post Processing FX", "Post Processing FX overrides the current post processing settings based on a custom volume. Requires the Post Processing package in BIRP."), OMWSEditorUtilities.FoldoutStyle());

            bool toggle = EditorGUI.Toggle(togglePos, GUIContent.none, property.FindPropertyRelative("_IsEnabled").boolValue);

            if (property.FindPropertyRelative("_IsEnabled").boolValue != toggle)
            {
                property.FindPropertyRelative("_IsEnabled").boolValue = toggle;

                if (toggle == true)
                    (property.serializedObject.targetObject as OMWSVFXModule).postFXManager.OnFXEnable();
                else
                    (property.serializedObject.targetObject as OMWSVFXModule).postFXManager.OnFXDisable();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            if (property.FindPropertyRelative("_OpenTab").boolValue)
            {
                using (new EditorGUI.DisabledScope(!property.FindPropertyRelative("_IsEnabled").boolValue))
                {
                    EditorGUILayout.Space();
                    EditorGUI.indentLevel++;
                    EditorGUILayout.HelpBox("No additional values to tweak here!", MessageType.Info);
                    EditorGUI.indentLevel--;
                }
            }

            EditorGUI.EndProperty();
        }
    }
}
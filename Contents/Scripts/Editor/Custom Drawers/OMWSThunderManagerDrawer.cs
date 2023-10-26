using CryingOnion.OhMy.WeatherSystem.Module;
using CryingOnion.OhMy.WeatherSystem.Utility;
using UnityEditor;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.EditorScript
{
    [UnityEditor.CustomPropertyDrawer(typeof(OMWSThunderManager))]
    public class OMWSThunderManagerDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            Rect pos = position;

            Rect tabPos = new Rect(pos.x + 35, pos.y, pos.width - 41, pos.height);
            Rect togglePos = new Rect(5, pos.y, 30, pos.height);

            property.FindPropertyRelative("_OpenTab").boolValue = EditorGUI.BeginFoldoutHeaderGroup(tabPos, property.FindPropertyRelative("_OpenTab").boolValue, new GUIContent("    Thunder FX", "Thunder FX control the rate at which lightning strikes during your weather profiles."), OMWSEditorUtilities.FoldoutStyle());

            bool toggle = EditorGUI.Toggle(togglePos, GUIContent.none, property.FindPropertyRelative("_IsEnabled").boolValue);

            if (property.FindPropertyRelative("_IsEnabled").boolValue != toggle)
            {
                property.FindPropertyRelative("_IsEnabled").boolValue = toggle;

                if (toggle == true)
                    (property.serializedObject.targetObject as OMWSVFXModule).thunderManager.OnFXEnable();
                else
                    (property.serializedObject.targetObject as OMWSVFXModule).thunderManager.OnFXDisable();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            if (property.FindPropertyRelative("_OpenTab").boolValue)
            {
                using (new EditorGUI.DisabledScope(!property.FindPropertyRelative("_IsEnabled").boolValue))
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("thunderPrefab"));
                    EditorGUI.indentLevel++;

                    if (property.FindPropertyRelative("thunderPrefab").objectReferenceValue)
                        Editor.CreateEditor((property.FindPropertyRelative("thunderPrefab").objectReferenceValue as GameObject).GetComponent<OMWSThunder>()).OnInspectorGUI();

                    EditorGUI.indentLevel--;
                    EditorGUI.indentLevel--;
                }
            }

            EditorGUI.EndProperty();
        }
    }
}
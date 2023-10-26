using CryingOnion.OhMy.WeatherSystem.Data;
using CryingOnion.OhMy.WeatherSystem.Utility;
using UnityEditor;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.EditorScript
{
    [CustomPropertyDrawer(typeof(OMWSFXAttribute))]
    public class OMWSFXDrawer : PropertyDrawer
    {
        string title;
        OMWSFXAttribute _attribute;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _attribute = (OMWSFXAttribute)attribute;
            title = _attribute.title;

            float height = EditorGUIUtility.singleLineHeight;
            var unitARect = new Rect(position.x, position.y, position.width, height);

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.PropertyField(unitARect, property, GUIContent.none);

            position = new Rect(position.x + 30, position.y, position.width - 30, position.height);

            if (property.objectReferenceValue != null)
            {
                OMWSFXProfile profile = (OMWSFXProfile)property.objectReferenceValue;
                (Editor.CreateEditor(profile) as OMWSFXProfileEditor).RenderInWindow(position);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float lineCount = 1;

            if (property.objectReferenceValue != null)
            {
                OMWSFXProfile profile = (OMWSFXProfile)property.objectReferenceValue;
                lineCount += 0.5f + (Editor.CreateEditor(profile) as OMWSFXProfileEditor).GetLineHeight();
            }

            return EditorGUIUtility.singleLineHeight * lineCount + EditorGUIUtility.standardVerticalSpacing * (lineCount - 1);
        }

    }
}
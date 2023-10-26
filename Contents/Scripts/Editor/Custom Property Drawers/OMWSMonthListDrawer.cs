using CryingOnion.OhMy.WeatherSystem.Utility;
using UnityEditor;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.EditorScript
{
    [CustomPropertyDrawer(typeof(OMWSMonthListAttribute))]
    public class OMWSMonthListDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var rect1 = new Rect(position.x, position.y, 40, EditorGUIUtility.singleLineHeight);
            var rect2 = new Rect(position.x + 50, position.y, position.width / 2 - 54, EditorGUIUtility.singleLineHeight);
            var rect3 = new Rect(position.x + position.width / 2, position.y, position.width / 2 - 4, EditorGUIUtility.singleLineHeight);
            var rect4 = new Rect(position.x + position.width / 2 - 4, position.y, position.width / 2 - 4, EditorGUIUtility.singleLineHeight);

            var name = property.FindPropertyRelative("name");
            var days = property.FindPropertyRelative("days");

            EditorGUI.LabelField(rect1, "Month Name");
            EditorGUI.PropertyField(rect2, name, GUIContent.none);
            EditorGUI.PropertyField(rect3, days, new GUIContent(days.displayName));

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float lineCount = 1.15f;

            return EditorGUIUtility.singleLineHeight * lineCount + EditorGUIUtility.standardVerticalSpacing * 2f * (lineCount - 1);
        }
    }
}
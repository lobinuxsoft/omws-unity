using CryingOnion.OhMy.WeatherSystem.Utility;
using UnityEditor;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.EditorScript
{
    [CustomPropertyDrawer(typeof(OMWSFormatTimeAttribute))]
    public class OMWSFormattedTimeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Keyboard), label);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            float div = position.width / 3;

            var hoursRect = new Rect(position.x, position.y, div - 10, position.height);
            var minutesRect = new Rect(position.x + div, position.y, div - 10, position.height);
            var meridiemRect = new Rect(position.x + div * 2, position.y, div - 10, position.height);

            var hours = property.FindPropertyRelative("hours");
            var minutes = property.FindPropertyRelative("minutes");
            var meridiem = property.FindPropertyRelative("meridiem");

            hours.intValue = Mathf.Clamp(EditorGUI.IntField(hoursRect, GUIContent.none, hours.intValue), 0, 12);

            if (hours.intValue == 0)
            {
                hours.intValue = 12;
                meridiem.intValue = meridiem.intValue == 1 ? 0 : 1;
            }

            minutes.intValue = Mathf.Clamp(EditorGUI.IntField(minutesRect, GUIContent.none, minutes.intValue), 0, 59);
            EditorGUI.PropertyField(meridiemRect, meridiem, GUIContent.none);

            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}
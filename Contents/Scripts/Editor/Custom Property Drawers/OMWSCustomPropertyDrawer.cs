using CryingOnion.OhMy.WeatherSystem.Utility;
using UnityEditor;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.EditorScript
{
    [CustomPropertyDrawer(typeof(OMWSPropertyTypeAttribute))]
    public class OMWSCustomPropertyDrawer : PropertyDrawer
    {
        bool color;
        float min;
        float max;
        OMWSPropertyTypeAttribute _attribute;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            _attribute = (OMWSPropertyTypeAttribute)attribute;

            if (_attribute.min != _attribute.max)
            {
                min = _attribute.min;
                max = _attribute.max;
            }

            color = _attribute.color;

            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Keyboard), label);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var unitRect = new Rect(position.x, position.y, position.width - 25, position.height);
            var dropdown = new Rect(position.x + (position.width - 20), position.y, 20, position.height);

            var mode = property.FindPropertyRelative("mode");
            var floatVal = property.FindPropertyRelative("floatVal");


            if (color)
            {
                if (mode.intValue == 0)
                    EditorGUI.PropertyField(unitRect, property.FindPropertyRelative("gradientVal"), GUIContent.none);
                if (mode.intValue == 1)
                    EditorGUI.PropertyField(unitRect, property.FindPropertyRelative("colorVal"), GUIContent.none);
            }
            else
            {
                if (mode.intValue == 0)
                    EditorGUI.PropertyField(unitRect, property.FindPropertyRelative("curveVal"), GUIContent.none);
                if (mode.intValue == 1)
                    if (_attribute.min != _attribute.max)
                        EditorGUI.Slider(unitRect, floatVal, min, max, GUIContent.none);
                    else
                        EditorGUI.PropertyField(unitRect, property.FindPropertyRelative("floatVal"), GUIContent.none);
            }
            EditorGUI.PropertyField(dropdown, property.FindPropertyRelative("mode"), GUIContent.none);


            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}
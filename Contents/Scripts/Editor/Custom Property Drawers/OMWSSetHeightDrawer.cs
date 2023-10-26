using CryingOnion.OhMy.WeatherSystem.Utility;
using UnityEditor;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.EditorScript
{
    [CustomPropertyDrawer(typeof(OMWSSetHeightAttribute))]
    public class OMWSSetHeightDrawer : PropertyDrawer
    {
        int lines;
        OMWSSetHeightAttribute _attribute;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _attribute = (OMWSSetHeightAttribute)attribute;
            lines = _attribute.lines;
            position = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight * lines);
            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.PropertyField(position, property, label);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            _attribute = (OMWSSetHeightAttribute)attribute;
            lines = _attribute.lines;

            return EditorGUIUtility.singleLineHeight * lines;
        }
    }
}
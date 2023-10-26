using CryingOnion.OhMy.WeatherSystem.Utility;
using UnityEditor;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.EditorScript
{
    [CustomPropertyDrawer(typeof(OMWSWeightedWeatherAttribute))]
    public class OMWSWeightedWeatherDrawer : PropertyDrawer
    {
        OMWSWeightedWeatherAttribute _attribute;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _attribute = (OMWSWeightedWeatherAttribute)attribute;

            EditorGUI.BeginProperty(position, label, property);

            var titleRect = new Rect(position.x, position.y, 150, position.height);
            var unitRect = new Rect(position.x + 157, position.y, position.width - 155, position.height);

            EditorGUI.PropertyField(titleRect, property.FindPropertyRelative("profile"), GUIContent.none);
            EditorGUI.PropertyField(unitRect, property.FindPropertyRelative("weight"), GUIContent.none);

            EditorGUI.EndProperty();
        }
    }
}
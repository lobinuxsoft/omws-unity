using CryingOnion.OhMy.WeatherSystem.Utility;
using UnityEditor;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.EditorScript
{
    [CustomPropertyDrawer(typeof(OMWSHideTitleAttribute))]
    public class OMWSHideTitleDrawer : PropertyDrawer
    {
        string title;
        OMWSHideTitleAttribute _attribute;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _attribute = (OMWSHideTitleAttribute)attribute;
            title = _attribute.title;
            EditorGUI.BeginProperty(position, label, property);

            if (title != "")
                EditorGUI.PropertyField(position, property, GUIContent.none);
            else
                EditorGUI.PropertyField(position, property, new GUIContent(title));

            EditorGUI.EndProperty();
        }
    }
}
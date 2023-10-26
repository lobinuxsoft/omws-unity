using CryingOnion.OhMy.WeatherSystem.Utility;
using UnityEditor;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.EditorScript
{
    [CustomPropertyDrawer(typeof(OMWSDisplayHorizontallyAttribute))]
    public class OMWSDisplayHorizontallyDrawer : PropertyDrawer
    {
        OMWSDisplayHorizontallyAttribute _attribute;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _attribute = (OMWSDisplayHorizontallyAttribute)attribute;
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.LabelField(position, label);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
            (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * (property.CountInProperty() + 2);
    }
}
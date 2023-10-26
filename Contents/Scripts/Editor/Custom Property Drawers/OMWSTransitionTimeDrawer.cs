using CryingOnion.OhMy.WeatherSystem.Utility;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.EditorScript
{
    [CustomPropertyDrawer(typeof(OMWSTransitionTimeAttribute))]
    public class OMWSTransitionTimeDrawer : PropertyDrawer
    {
        OMWSTransitionTimeAttribute _attribute;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _attribute = (OMWSTransitionTimeAttribute)attribute;
            int preset = -1;

            EditorGUI.BeginProperty(position, label, property);

            var unitRect = new Rect(position.x, position.y, position.width - 25, position.height);
            var dropdown = new Rect(position.x + (position.width - 20), position.y, 20, position.height);

            List<AnimationCurve> presets = new List<AnimationCurve>()
            {
                new AnimationCurve (new Keyframe(0, 0, 1, 1), new Keyframe (1, 1, 1, 1)),
                new AnimationCurve (new Keyframe(0, 0, 0, 0), new Keyframe (1, 1, 2, -2)),
                new AnimationCurve (new Keyframe(0, 0, 2, 2), new Keyframe (1, 1, 0, 0)),
                new AnimationCurve (new Keyframe(0, 0, 0, 0), new Keyframe (1, 1, 3.25f, -3.25f)),
                new AnimationCurve (new Keyframe(0, 0, 3.25f, 3.25f), new Keyframe (1, 1, 0, 0)),
                new AnimationCurve (new Keyframe(0, 0, 0, 0), new Keyframe (1, 1, 0, 0)),
                new AnimationCurve (new Keyframe(0, 0, 3, 3), new Keyframe (1, 1, 3, 3))
            };

            List<GUIContent> presetNames = new List<GUIContent>()
            {
                new GUIContent("Linear"),
                new GUIContent("Exponential"),
                new GUIContent("Inverse Exponential"),
                new GUIContent("Steep Exponential"),
                new GUIContent("Steep Inverse Exponential"),
                new GUIContent("Smooth"),
                new GUIContent("Slerped"),
            };

            EditorGUI.PropertyField(unitRect, property, label);

            preset = EditorGUI.Popup(dropdown, GUIContent.none, -1, presetNames.ToArray());

            if (preset != -1)
                property.animationCurveValue = presets[preset];

            EditorGUI.EndProperty();
        }
    }
}
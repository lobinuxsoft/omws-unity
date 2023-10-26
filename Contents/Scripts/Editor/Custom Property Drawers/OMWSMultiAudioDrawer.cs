using CryingOnion.OhMy.WeatherSystem.Utility;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.EditorScript
{
    [CustomPropertyDrawer(typeof(OMWSMultiAudioAttribute))]
    public class OMWSMultiAudioDrawer : PropertyDrawer
    {
        OMWSMultiAudioAttribute _attribute;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _attribute = (OMWSMultiAudioAttribute)attribute;
            int preset = -1;

            EditorGUI.BeginProperty(position, label, property);

            var titleRect = new Rect(position.x, position.y, 150, position.height);
            var unitRect = new Rect(position.x + 157, position.y, position.width - 185, position.height);
            var dropdown = new Rect(position.x + (position.width - 20), position.y, 20, position.height);

            List<AnimationCurve> presets = new List<AnimationCurve>() { new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.2f, 1), new Keyframe(0.25f, 0), new Keyframe(0.75f, 0), new Keyframe(0.8f, 1), new Keyframe(1, 1)),
            new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.2f, 0), new Keyframe(0.25f, 1), new Keyframe(0.75f, 1), new Keyframe(0.8f, 0), new Keyframe(1, 0)),
            new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.18f, 0), new Keyframe(0.25f, 1), new Keyframe(0.35f, 0), new Keyframe(0.7f, 0), new Keyframe(0.75f, 1), new Keyframe(0.85f, 0), new Keyframe(1, 0)),
            new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.70f, 0), new Keyframe(0.8f, 1), new Keyframe(0.85f, 0), new Keyframe(1, 0)),
            new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.18f, 0), new Keyframe(0.22f, 1), new Keyframe(0.3f, 0), new Keyframe(1, 0))};
            List<GUIContent> presetNames = new List<GUIContent>() { new GUIContent("Plays at night"), new GUIContent("Plays during the day"),
            new GUIContent("Plays in the evening & morning"), new GUIContent("Plays in the evening"), new GUIContent("Plays in the morning")};

            EditorGUI.PropertyField(titleRect, property.FindPropertyRelative("FX"), GUIContent.none);
            EditorGUI.PropertyField(unitRect, property.FindPropertyRelative("intensityCurve"), GUIContent.none);

            preset = EditorGUI.Popup(dropdown, GUIContent.none, -1, presetNames.ToArray());

            if (preset != -1)
                property.FindPropertyRelative("intensityCurve").animationCurveValue = presets[preset];

            EditorGUI.EndProperty();
        }
    }
}
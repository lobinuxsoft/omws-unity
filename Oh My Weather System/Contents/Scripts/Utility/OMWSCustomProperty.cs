using System;
using UnityEngine;
using System.Collections.Generic;
using CryingOnion.OhMy.WeatherSystem.Data;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CryingOnion.OhMy.WeatherSystem.Utility
{
    [Serializable]
    public class OMWSCustomProperty
    {
        public enum Mode { interpolate, constant }
        public Mode mode = Mode.constant;

        [ColorUsage(true, true)] public Color colorVal = Color.white;
        [GradientUsage(true)] public Gradient gradientVal;
        public float floatVal = 1;
        public AnimationCurve curveVal = new AnimationCurve() { keys = new Keyframe[2] { new Keyframe(0, 1), new Keyframe(1, 1) } };
        public bool systemContainsProp = true;

        public void GetValue(out Color color, float time) => color = mode == Mode.constant ? colorVal : gradientVal.Evaluate(time);

        public void GetValue(out float value, float time) => value = mode == Mode.constant ? floatVal : curveVal.Evaluate(time);

        public Color GetColorValue(float time) => mode == Mode.constant ? colorVal : gradientVal.Evaluate(time);

        public float GetFloatValue(float time) => mode == Mode.constant ? floatVal : curveVal.Evaluate(time);
    }

    public class PropertyRelation
    {
        public OMWSCustomProperty property;
    }

#if UNITY_EDITOR
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

    [CustomPropertyDrawer(typeof(OMWSModulatedPropertyAttribute))]
    public class OMWSModulatedPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var indent = EditorGUI.indentLevel;

            float space = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            float height = EditorGUIUtility.singleLineHeight;

            var titleRect = new Rect(position.x + 15, position.y, position.width, height);
            var unitARect = new Rect(position.x, position.y + space, (position.width / 2) - 3, height);
            var unitBRect = new Rect((position.width / 2) + 75, position.y + space, (position.width / 2), height);
            var unitCRect = new Rect(position.x + 30, position.y + space * 2, position.width - 30, height);
            var unitDRect = new Rect(position.x + 30, position.y + space * 3, position.width - 30, height);
            var unitERect = new Rect(position.x + 30, position.y + space * 4, position.width - 30, height);
            var source = property.FindPropertyRelative("modulationSource");
            var target = property.FindPropertyRelative("modulationTarget");

            property.FindPropertyRelative("expanded").boolValue = EditorGUI.Foldout(titleRect, property.FindPropertyRelative("expanded").boolValue, GetTitle(property), true);

            if (property.FindPropertyRelative("expanded").boolValue)
            {
                SerializedProperty map = null;
                if ((OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationTarget)target.enumValueIndex == OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationTarget.globalValue ||
                (OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationTarget)target.enumValueIndex == OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationTarget.materialValue)
                    map = property.FindPropertyRelative("mappedCurve");
                else
                    map = property.FindPropertyRelative("mappedGradient");

                var targetLayer = property.FindPropertyRelative("targetLayer");
                var targetMaterial = property.FindPropertyRelative("targetMaterial");
                var targetVariableName = property.FindPropertyRelative("targetVariableName");

                EditorGUI.PropertyField(unitARect, target, GUIContent.none);
                EditorGUI.PropertyField(unitBRect, source, GUIContent.none);
                EditorGUI.PropertyField(unitCRect, map);

                List<string> names = new List<string>();
                int selected = -1;

                if ((OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationTarget)target.enumValueIndex == OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationTarget.materialValue)
                {
                    if (property.FindPropertyRelative("targetMaterial").objectReferenceValue)
                    {
                        for (int i = 0; i < (targetMaterial.objectReferenceValue as Material).shader.GetPropertyCount(); i++)
                            if ((targetMaterial.objectReferenceValue as Material).shader.GetPropertyType(i) == UnityEngine.Rendering.ShaderPropertyType.Float)
                                names.Add((targetMaterial.objectReferenceValue as Material).shader.GetPropertyName(i));

                        if (names.Contains(targetVariableName.stringValue))
                            selected = names.IndexOf(targetVariableName.stringValue);
                        else
                            selected = 0;
                    }
                }
                else if ((OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationTarget)target.enumValueIndex == OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationTarget.materialColor)
                {
                    if (property.FindPropertyRelative("targetMaterial").objectReferenceValue)
                    {
                        for (int i = 0; i < (targetMaterial.objectReferenceValue as Material).shader.GetPropertyCount(); i++)
                            if ((targetMaterial.objectReferenceValue as Material).shader.GetPropertyType(i) == UnityEngine.Rendering.ShaderPropertyType.Color)
                                names.Add((targetMaterial.objectReferenceValue as Material).shader.GetPropertyName(i));

                        if (names.Contains(targetVariableName.stringValue))
                            selected = names.IndexOf(targetVariableName.stringValue);
                        else
                            selected = 0;
                    }
                }

                switch ((OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationTarget)target.enumValueIndex)
                {
                    case (OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationTarget.globalColor):
                        EditorGUI.PropertyField(unitDRect, property.FindPropertyRelative("targetVariableName"), new GUIContent("Global Color Property Name", "The name of the global shader property to set."));
                        break;
                    case (OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationTarget.globalValue):
                        EditorGUI.PropertyField(unitDRect, property.FindPropertyRelative("targetVariableName"), new GUIContent("Global Value Property Name", "The name of the global shader property to set."));
                        break;
                    case (OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationTarget.materialColor):
                        EditorGUI.PropertyField(unitDRect, targetMaterial);
                        if (names.Count > 0)
                            property.FindPropertyRelative("targetVariableName").stringValue = names[EditorGUI.Popup(unitERect, "Material Value Property Name", selected, names.ToArray())];
                        break;
                    case (OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationTarget.materialValue):
                        EditorGUI.PropertyField(unitDRect, targetMaterial);
                        if (names.Count > 0)
                            property.FindPropertyRelative("targetVariableName").stringValue = names[EditorGUI.Popup(unitERect, "Material Value Property Name", selected, names.ToArray())];
                        break;
                    case (OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationTarget.terrainLayerColor):
                        EditorGUI.PropertyField(unitDRect, targetLayer);
                        break;
                }
            }

            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }

        string GetTitle(SerializedProperty property)
        {
            int value = property.FindPropertyRelative("modulationTarget").intValue;

            switch ((OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationTarget)value)
            {
                case (OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationTarget.globalColor):
                    if (property.FindPropertyRelative("targetVariableName").stringValue != "")
                        return $"   {property.FindPropertyRelative("targetVariableName").stringValue}";
                    else
                        return "   Global Shader Color";
                case (OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationTarget.globalValue):
                    if (property.FindPropertyRelative("targetVariableName").stringValue != "")
                        return $"   {property.FindPropertyRelative("targetVariableName").stringValue}";
                    else
                        return "   Global Shader Value";
                case (OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationTarget.materialColor):
                    if (property.FindPropertyRelative("targetMaterial").objectReferenceValue)
                        return $"   {property.FindPropertyRelative("targetMaterial").objectReferenceValue.name}";
                    else
                        return "   Local Material Color";
                case (OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationTarget.materialValue):
                    if (property.FindPropertyRelative("targetMaterial").objectReferenceValue)
                        return $"   {property.FindPropertyRelative("targetMaterial").objectReferenceValue.name}";
                    else
                        return "   Local Material Value";
                case (OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationTarget.terrainLayerColor):
                    if (property.FindPropertyRelative("targetLayer").objectReferenceValue)
                        return $"   {property.FindPropertyRelative("targetLayer").objectReferenceValue.name}";
                    else
                        return "   Terrain Layer Color";
            }

            return "";
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float lineCount = 1.25f;

            if (property.FindPropertyRelative("expanded").boolValue)
                lineCount += 3.5f;

            return EditorGUIUtility.singleLineHeight * lineCount + EditorGUIUtility.standardVerticalSpacing * 2f * (lineCount - 1);
        }
    }

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

#endif
    public class OMWSPropertyTypeAttribute : PropertyAttribute
    {
        public bool color;
        public float min;
        public float max;

        public OMWSPropertyTypeAttribute() => color = false;

        public OMWSPropertyTypeAttribute(bool isColorType) => color = isColorType;

        public OMWSPropertyTypeAttribute(bool isColorType, float min, float max)
        {
            color = isColorType;
            this.min = min;
            this.max = max;
        }
    }

    public class OMWSFXAttribute : PropertyAttribute
    {
        public string title;

        public OMWSFXAttribute() => title = "";

        public OMWSFXAttribute(string _title) => title = _title;
    }

    public class OMWSHideTitleAttribute : PropertyAttribute
    {
        public string title;
        public float lines;

        public OMWSHideTitleAttribute()
        {
            title = "";
            lines = 1;
        }

        public OMWSHideTitleAttribute(float _lines)
        {
            title = "";
            lines = _lines;
        }

        public OMWSHideTitleAttribute(string _title, float _lines)
        {
            title = _title;
            lines = _lines;
        }
    }

    public class OMWSDisplayHorizontallyAttribute : PropertyAttribute
    {
        public string key;

        public OMWSDisplayHorizontallyAttribute(string _Key) => key = _Key;
    }

    public class OMWSMonthListAttribute : PropertyAttribute
    {
        public OMWSMonthListAttribute() { }
    }

    public class OMWSSetHeightAttribute : PropertyAttribute
    {
        public int lines;

        public OMWSSetHeightAttribute() => lines = 1;

        public OMWSSetHeightAttribute(int _lines) => lines = _lines;
    }

    public class OMWSFormatTimeAttribute : PropertyAttribute
    {
        public OMWSFormatTimeAttribute() { }
    }

    public class OMWSModulatedPropertyAttribute : PropertyAttribute
    {
        public OMWSModulatedPropertyAttribute() { }
    }

    public class OMWSTransitionTimeAttribute : PropertyAttribute
    {
        public OMWSTransitionTimeAttribute() { }
    }

    public class OMWSWeightedWeatherAttribute : PropertyAttribute
    {
        public OMWSWeightedWeatherAttribute() { }
    }

    public class OMWSMultiAudioAttribute : PropertyAttribute
    {
        public OMWSMultiAudioAttribute() { }
    }
}
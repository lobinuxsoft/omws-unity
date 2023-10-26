using System;
using UnityEngine;

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
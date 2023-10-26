using CryingOnion.OhMy.WeatherSystem.Data;
using UnityEditor;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.EditorScript
{
    [CustomEditor(typeof(OMWSFXProfile))]
    [CanEditMultipleObjects]
    public abstract class OMWSFXProfileEditor : Editor
    {
        public abstract float GetLineHeight();

        public abstract void RenderInWindow(Rect pos);
    }
}
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.Utility
{
    public class OMWSUtilities
    {
        public Color DoubleGradient(Gradient start, Gradient target, float depth, float time) =>
            Color.Lerp(start.Evaluate(time), target.Evaluate(time), depth);
    }
}
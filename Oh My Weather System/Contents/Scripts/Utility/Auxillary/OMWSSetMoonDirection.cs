using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.Utility
{
    [ExecuteAlways]
    public class OMWSSetMoonDirection : MonoBehaviour
    {
        void Update() => Shader.SetGlobalVector("OMWS_MoonDirection", -transform.forward);
    }
}
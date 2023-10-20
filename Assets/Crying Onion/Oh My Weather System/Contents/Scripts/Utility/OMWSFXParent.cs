using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.Utility
{
    [ExecuteAlways]
    public class OMWSFXParent : MonoBehaviour
    {
        void OnEnable()
        {
            if (transform.parent == null)
                DestroyImmediate(gameObject);
        }
    }
}
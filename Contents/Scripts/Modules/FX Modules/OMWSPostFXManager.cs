using UnityEngine;
using CryingOnion.OhMy.WeatherSystem.Core;
using CryingOnion.OhMy.WeatherSystem.Utility;

namespace CryingOnion.OhMy.WeatherSystem.Module
{
    [System.Serializable]
    public class OMWSPostFXManager : OMWSFXModule
    {
        public override void OnFXEnable() { }

        public override void OnFXUpdate()
        {
            if (!isEnabled)
                return;

            if (vfx == null)
                vfx = OMWSWeather.instance.GetModule<OMWSVFXModule>();

            if (parent == null)
                SetupFXParent();
            else if (parent.parent == null)
                parent.parent = vfx.parent;

            parent.transform.localPosition = Vector3.zero;
        }

        public override void OnFXDisable()
        {
            if (parent)
                MonoBehaviour.DestroyImmediate(parent.gameObject);
        }

        public override void SetupFXParent()
        {
            if (vfx.parent == null)
                return;

            parent = new GameObject().transform;
            parent.parent = vfx.parent;
            parent.localPosition = Vector3.zero;
            parent.localScale = Vector3.one;
            parent.name = "Post FX";
            parent.gameObject.AddComponent<OMWSFXParent>();
        }
    }
}
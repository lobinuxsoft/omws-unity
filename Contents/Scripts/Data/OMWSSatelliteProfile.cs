using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.Data
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Crying Onion/Oh My Weather System/Satellite Profile", order = 361)]
    public class OMWSSatelliteProfile : ScriptableObject
    {
        public GameObject satelliteReference;
        public Transform orbitRef;
        public Transform moonRef;
        public Light lightRef;
        public float size = 1;
        [Range(0, 1)] public float distance = 1;
        public bool useLight = true;
        public Flare flare;
        public Color lightColorMultiplier = Color.white;
        public LightShadows castShadows;
        public float orbitOffset;
        public Vector3 initialRotation;
        public float satelliteRotateSpeed;
        public Vector3 satelliteRotateAxis;
        public float satelliteDirection;
        public float satelliteRotation;
        public float satellitePitch;
        public bool changedLastFrame;
        public bool open;
    }
}
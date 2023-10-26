using CryingOnion.OhMy.WeatherSystem.Utility;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.Data
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Crying Onion/Oh My Weather System/Atmosphere Profile", order = 361)]
    public class OMWSAtmosphereProfile : ScriptableObject
    {
        public bool win1;
        public bool win2;
        public bool win3;
        public bool win4;

        [Tooltip("Sets the color of the zenith (or top) of the skybox at a certain time. Starts and ends at midnight.")]
        [OMWSPropertyType(true)]
        public OMWSCustomProperty skyZenithColor;

        [Tooltip("Sets the color of the horizon (or middle) of the skybox at a certain time. Starts and ends at midnight.")]
        [OMWSPropertyType(true)]
        public OMWSCustomProperty skyHorizonColor;

        [Tooltip("Sets the main color of the clouds at a certain time. Starts and ends at midnight.")]
        [OMWSPropertyType(true)]
        public OMWSCustomProperty cloudColor;

        [Tooltip("Sets the highlight color of the clouds at a certain time. Starts and ends at midnight.")]
        [OMWSPropertyType(true)]
        public OMWSCustomProperty cloudHighlightColor;

        [Tooltip("Sets the color of the high altitude clouds at a certain time. Starts and ends at midnight.")]
        [OMWSPropertyType(true)]
        public OMWSCustomProperty highAltitudeCloudColor;

        [Tooltip("Sets the color of the sun light source at a certain time. Starts and ends at midnight.")]
        [OMWSPropertyType(true)]
        public OMWSCustomProperty sunlightColor;

        [Tooltip("Sets the color of the moon light source at a certain time. Starts and ends at midnight.")]
        [OMWSPropertyType(true)]
        public OMWSCustomProperty moonlightColor;

        [Tooltip("Sets the color of the star particle FX and textures at a certain time. Starts and ends at midnight.")]
        [OMWSPropertyType(true)]
        public OMWSCustomProperty starColor;

        [Tooltip("Sets the color of the zenith (or top) of the ambient scene lighting at a certain time. Starts and ends at midnight.")]
        [OMWSPropertyType(true)]
        public OMWSCustomProperty ambientLightHorizonColor;

        [Tooltip("Sets the color of the horizon (or middle) of the ambient scene lighting at a certain time. Starts and ends at midnight.")]
        [OMWSPropertyType(true)]
        public OMWSCustomProperty ambientLightZenithColor;

        [Tooltip("Multiplies the ambient light intensity.")]
        [OMWSPropertyType(false, 0, 4)]
        public OMWSCustomProperty ambientLightMultiplier;

        [Tooltip("Sets the intensity of the galaxy effects at a certain time. Starts and ends at midnight.")]
        [OMWSPropertyType(false, 0, 1)]
        public OMWSCustomProperty galaxyIntensity;


        [OMWSPropertyType(true)]
        [Tooltip("Sets the fog color from 0m away from the camera to fog start 1.")]
        public OMWSCustomProperty fogColor1;

        [OMWSPropertyType(true)]
        [Tooltip("Sets the fog color from fog start 1 to fog start 2.")]
        public OMWSCustomProperty fogColor2;

        [Tooltip("Sets the fog color from fog start 2 to fog start 3.")]
        [OMWSPropertyType(true)]
        public OMWSCustomProperty fogColor3;

        [Tooltip("Sets the fog color from fog start 3 to fog start 4.")]
        [OMWSPropertyType(true)]
        public OMWSCustomProperty fogColor4;

        [Tooltip("Sets the fog color from fog start 4 to fog start 5.")]
        [OMWSPropertyType(true)]
        public OMWSCustomProperty fogColor5;

        [OMWSPropertyType(true)]
        [Tooltip("Sets the color of the fog flare.")]
        public OMWSCustomProperty fogFlareColor;

        [OMWSPropertyType(false, 0, 1)]
        [Tooltip("Controls the exponent used to modulate from the horizon color to the zenith color of the sky.")]
        public OMWSCustomProperty gradientExponent;

        [OMWSPropertyType(false, 0, 1)]
        public OMWSCustomProperty atmosphereVariationMin;

        [OMWSPropertyType(false, 0, 1)]
        public OMWSCustomProperty atmosphereVariationMax;

        [OMWSPropertyType(false, 0, 1)]
        [Tooltip("Controls the atmospheric variation multiplier.")]
        public OMWSCustomProperty atmosphereBias;

        [OMWSPropertyType(false, 0, 5)]
        [Tooltip("Sets the size of the visual sun in the sky.")]
        public OMWSCustomProperty sunSize;

        [Tooltip("Sets the world space direction of the sun in degrees.")]
        [OMWSPropertyType(false, 0, 360)]
        public OMWSCustomProperty sunDirection;

        [Tooltip("Sets the roll value of the sun's rotation. Allows the sun to be slightly off from directly overhead at noon.")]
        [OMWSPropertyType(false, 0, 90)]
        public OMWSCustomProperty sunPitch;

        [Tooltip("Sets the color of the visual sun in the sky.")]
        [OMWSPropertyType(true)]
        public OMWSCustomProperty sunColor;

        [OMWSPropertyType(false, 0, 100)]
        [Tooltip("Sets the falloff of the halo around the visual sun.")]
        public OMWSCustomProperty sunFalloff;

        [OMWSPropertyType(true)]
        [Tooltip("Sets the color of the halo around the visual sun.")]
        public OMWSCustomProperty sunFlareColor;

        [OMWSPropertyType(false, 0, 100)]
        [Tooltip("Sets the falloff of the halo around the main moon.")]
        public OMWSCustomProperty moonFalloff;

        [OMWSPropertyType(true)]
        [Tooltip("Sets the color of the halo around the main moon.")]
        public OMWSCustomProperty moonFlareColor;

        [OMWSPropertyType(true)]
        [Tooltip("Sets the color of the first galaxy algorithm.")]
        public OMWSCustomProperty galaxy1Color;

        [OMWSPropertyType(true)]
        [Tooltip("Sets the color of the second galaxy algorithm.")]
        public OMWSCustomProperty galaxy2Color;

        [OMWSPropertyType(true)]
        [Tooltip("Sets the color of the third galaxy algorithm.")]
        public OMWSCustomProperty galaxy3Color;

        [OMWSPropertyType(true)]
        [Tooltip("Sets the color of the light columns around the horizon.")]
        public OMWSCustomProperty lightScatteringColor;

        [Tooltip("Should OMWS use a rainbow?")]
        public bool useRainbow = true;

        [Tooltip("Sets the position of the rainbow in the sky.")]
        [OMWSPropertyType(false, 0, 100)]
        public OMWSCustomProperty rainbowPosition;

        [Tooltip("Sets the width of the rainbow in the sky.")]
        [OMWSPropertyType(false, 0, 50)]
        public OMWSCustomProperty rainbowWidth;


        [OMWSPropertyType(false, 0, 5)]
        [Tooltip("Multiplies the world space distance before entering the fog algorithm. Use this for simple density changes.")]
        public OMWSCustomProperty fogDensityMultiplier;

        [Tooltip("Sets the distance at which the first fog color fades into the second fog color.")]
        public float fogStart1;
        public float fogStart2;
        public float fogStart3;
        public float fogStart4;

        [OMWSPropertyType(false, 0, 2)]
        public OMWSCustomProperty fogHeight;

        [OMWSPropertyType(false, 0, 2)]
        public OMWSCustomProperty fogLightFlareIntensity;

        [OMWSPropertyType(false, 0, 40)]
        public OMWSCustomProperty fogLightFlareFalloff;

        [OMWSPropertyType(false, 0, 10)]
        [Tooltip("Sets the height divisor for the fog flare. High values sit the flare closer to the horizon, small values extend the flare into the sky.")]

        public OMWSCustomProperty fogLightFlareSquish;

        [OMWSPropertyType(true)]
        public OMWSCustomProperty cloudMoonColor;

        [OMWSPropertyType(false, 0, 50)]
        public OMWSCustomProperty cloudSunHighlightFalloff;

        [OMWSPropertyType(false, 0, 50)]
        public OMWSCustomProperty cloudMoonHighlightFalloff;

        [OMWSPropertyType(false, 0, 10)]
        public OMWSCustomProperty cloudWindSpeed;

        [OMWSPropertyType(false, 0, 1)]
        public OMWSCustomProperty clippingThreshold;

        [OMWSPropertyType(false, 2, 60)]
        public OMWSCustomProperty cloudMainScale;

        [OMWSPropertyType(false, 0.2f, 10)]
        public OMWSCustomProperty cloudDetailScale;

        [OMWSPropertyType(false, 0, 30)]
        public OMWSCustomProperty cloudDetailAmount;

        [OMWSPropertyType(false, 0.1f, 3)]
        public OMWSCustomProperty acScale;

        [OMWSPropertyType(false, 0, 3)]
        public OMWSCustomProperty cirroMoveSpeed;

        [OMWSPropertyType(false, 0, 3)]
        public OMWSCustomProperty cirrusMoveSpeed;

        [OMWSPropertyType(false, 0, 3)]
        public OMWSCustomProperty chemtrailsMoveSpeed;

        public Texture cloudTexture;

        [OMWSPropertyType(true)]
        public OMWSCustomProperty cloudTextureColor;

        [OMWSPropertyType(false, 0, 10)]
        public OMWSCustomProperty cloudCohesion;

        [OMWSPropertyType(false, 0, 1)]
        public OMWSCustomProperty spherize;

        [OMWSPropertyType(false, 0, 10)]
        public OMWSCustomProperty shadowDistance;

        [OMWSPropertyType(false, 0, 4)]
        public OMWSCustomProperty cloudThickness;

        [OMWSPropertyType(false, 0, 3)]
        public OMWSCustomProperty textureAmount;

        public Vector3 texturePanDirection;
    }
}
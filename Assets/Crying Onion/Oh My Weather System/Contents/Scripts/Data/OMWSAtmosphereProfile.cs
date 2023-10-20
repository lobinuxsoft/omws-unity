using CryingOnion.OhMy.WeatherSystem.Utility;
using UnityEngine;
using System.Collections.Generic;
using CryingOnion.OhMy.WeatherSystem.Core;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CryingOnion.OhMy.WeatherSystem.Data
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "CryingOnion/Oh My Weather System/Atmosphere Profile", order = 361)]
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

#if UNITY_EDITOR
    [CustomEditor(typeof(OMWSAtmosphereProfile))]
    [CanEditMultipleObjects]
    public class OMWSAtmosphereProfileEditor : Editor
    {
        Vector2 scrollPos;

        public int windowNum;
        public Texture icon1;
        public Texture icon2;
        public Texture icon3;
        public Texture icon4;
        public bool tooltips;

        public OMWSWeather defaultWeather;

        Color proCol = (Color)new Color32(50, 50, 50, 255);
        Color unityCol = (Color)new Color32(194, 194, 194, 255);

        void OnEnable()
        {
            icon1 = Resources.Load<Texture>("OMWSAtmosphere");
            icon2 = Resources.Load<Texture>("OMWSCloud");
            icon3 = Resources.Load<Texture>("OMWSMoon");
            icon4 = Resources.Load<Texture>("OMWSTrigger");

            if (OMWSWeather.instance)
                defaultWeather = OMWSWeather.instance;
        }

        public override void OnInspectorGUI()
        {
            tooltips = EditorPrefs.GetBool("OMWS_Tooltips", true);

            if (defaultWeather)
                OnInspectorGUIInline(defaultWeather);
            else
                EditorGUILayout.HelpBox("To edit the atmosphere profile make sure that your scene is properly setup with a OMWS system!", MessageType.Warning);
        }

        public void OnInspectorGUIInline(OMWSWeather omwsWeather)
        {
            serializedObject.Update();
            tooltips = EditorPrefs.GetBool("OMWS_Tooltips", true);

            serializedObject.FindProperty("win1").boolValue = EditorGUILayout.BeginFoldoutHeaderGroup(serializedObject.FindProperty("win1").boolValue,
                new GUIContent("    Atmosphere & Lighting", "Skydome, fog, and lighting settings."), EditorUtilities.FoldoutStyle());

            if (serializedObject.FindProperty("win1").boolValue)
                DrawAtmosphereTab(omwsWeather);

            EditorGUILayout.EndFoldoutHeaderGroup();

            serializedObject.FindProperty("win2").boolValue = EditorGUILayout.BeginFoldoutHeaderGroup(serializedObject.FindProperty("win2").boolValue,
                            new GUIContent("    Clouds", "Cloud color, generation, and variation settings."), EditorUtilities.FoldoutStyle());

            if (serializedObject.FindProperty("win2").boolValue)
                DrawCloudsTab(omwsWeather);

            EditorGUILayout.EndFoldoutHeaderGroup();

            serializedObject.FindProperty("win3").boolValue = EditorGUILayout.BeginFoldoutHeaderGroup(serializedObject.FindProperty("win3").boolValue,
                            new GUIContent("    Celestials & VFX", "Sun, moon, and light FX settings."), EditorUtilities.FoldoutStyle());

            if (serializedObject.FindProperty("win3").boolValue)
                DrawCelestialsTab(omwsWeather);

            EditorGUILayout.EndFoldoutHeaderGroup();

            serializedObject.ApplyModifiedProperties();
        }

        void DrawAtmosphereTab(OMWSWeather omwsWeather)
        {
            GUIStyle labelStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
            labelStyle.fontStyle = FontStyle.Bold;

            bool advancedSky = omwsWeather.skyStyle == OMWSWeather.SkyStyle.desktop;

            if (tooltips)
                EditorGUILayout.HelpBox("Interpolate controls change the value depending on the time of day. These range from 00:00 to 23:59, which means that morning is about 25% through the curve, midday 50%, evening 75%, etc. \n \n Constant controls set the value to a single value that remains constant regardless of the time of day.", MessageType.Info);

            Color col = EditorGUIUtility.isProSkin ? proCol : unityCol;

            EditorGUILayout.LabelField(" Skydome Settings", labelStyle);
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("skyZenithColor"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("skyHorizonColor"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("gradientExponent"), false);

            if (advancedSky)
            {
                EditorGUILayout.Space(5);
                float min = serializedObject.FindProperty("atmosphereVariationMin").FindPropertyRelative("floatVal").floatValue;
                float max = serializedObject.FindProperty("atmosphereVariationMax").FindPropertyRelative("floatVal").floatValue;

                Rect position = EditorGUILayout.GetControlRect();
                float startPos = position.width / 2.5f;
                var titleRect = new Rect(position.x, position.y, 70, position.height);
                EditorGUI.PrefixLabel(titleRect, new GUIContent("Atmosphere Variation"));
                var label1Rect = new Rect();
                var label2Rect = new Rect();
                var sliderRect = new Rect();

                if (position.width > 359)
                {
                    label1Rect = new Rect(startPos, position.y, 64, position.height);
                    label2Rect = new Rect(position.width - 71, position.y, 64, position.height);
                    sliderRect = new Rect(startPos + 56, position.y, (position.width - startPos) - 135, position.height);
                    EditorGUI.MinMaxSlider(sliderRect, ref min, ref max, 0, 1);
                }
                else
                {
                    label1Rect = new Rect(position.width - 110, position.y, 50, position.height);
                    label2Rect = new Rect(position.width - 72, position.y, 50, position.height);
                }

                min = EditorGUI.FloatField(label1Rect, (Mathf.Round(min * 100) / 100));
                max = EditorGUI.FloatField(label2Rect, (Mathf.Round(max * 100) / 100));

                if (min > max)
                    min = max;

                serializedObject.FindProperty("atmosphereVariationMin").FindPropertyRelative("floatVal").floatValue = min;
                serializedObject.FindProperty("atmosphereVariationMax").FindPropertyRelative("floatVal").floatValue = max;

                EditorGUILayout.PropertyField(serializedObject.FindProperty("atmosphereBias"), false);
            }

            EditorGUILayout.Space(5);
            EditorGUI.indentLevel--;
            EditorGUILayout.LabelField(" Fog Settings", labelStyle);
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("fogColor1"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("fogColor2"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("fogColor3"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("fogColor4"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("fogColor5"), false);
            EditorGUILayout.Space(5);

            float fogStart1 = serializedObject.FindProperty("fogStart1").floatValue;
            float fogStart2 = serializedObject.FindProperty("fogStart2").floatValue;
            float fogStart3 = serializedObject.FindProperty("fogStart3").floatValue;
            float fogStart4 = serializedObject.FindProperty("fogStart4").floatValue;

            fogStart1 = Mathf.Clamp(EditorGUILayout.Slider("Fog Start 2", fogStart1, 0, 50), 0, fogStart2 - 0.1f);
            fogStart2 = Mathf.Clamp(EditorGUILayout.Slider("Fog Start 3", fogStart2, 0, 50), fogStart1 + 0.1f, fogStart3 - 0.1f);
            fogStart3 = Mathf.Clamp(EditorGUILayout.Slider("Fog Start 4", fogStart3, 0, 50), fogStart2 + 0.1f, fogStart4 - 0.1f);
            fogStart4 = Mathf.Clamp(EditorGUILayout.Slider("Fog Start 5", fogStart4, 0, 50), fogStart3 + 0.1f, 50);

            serializedObject.FindProperty("fogStart1").floatValue = fogStart1;
            serializedObject.FindProperty("fogStart2").floatValue = fogStart2;
            serializedObject.FindProperty("fogStart3").floatValue = fogStart3;
            serializedObject.FindProperty("fogStart4").floatValue = fogStart4;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("fogHeight"), false);

            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("fogDensityMultiplier"), false);


            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("fogFlareColor"), new GUIContent("Light Flare Color",
                "Sets the color of the fog for a false \"light flare\" around the main sun directional light."), false);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("fogLightFlareIntensity"), new GUIContent("Light Flare Intensity",
                "Modulates the brightness of the light flare."), false);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("fogLightFlareFalloff"), new GUIContent("Light Flare Falloff",
                "Sets the falloff speed for the light flare."), false);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("fogLightFlareSquish"), new GUIContent("Light Flare Squish",
                "Sets the height divisor for the fog flare. High values sit the flare closer to the horizon, small values extend the flare into the sky."), false);

            EditorGUILayout.Space(5);
            EditorGUI.indentLevel--;
            EditorGUILayout.LabelField(" Lighting Settings", labelStyle);
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("sunlightColor"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("moonlightColor"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ambientLightHorizonColor"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ambientLightZenithColor"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ambientLightMultiplier"), false);
            EditorGUI.indentLevel--;
        }


        void DrawCloudsTab(OMWSWeather omwsWeather)
        {

            Material cloudShader = omwsWeather.cloudMesh.sharedMaterial;

            Color col = EditorGUIUtility.isProSkin ? proCol : unityCol;

            if (tooltips)
                EditorGUILayout.HelpBox("Interpolate controls change the value depending on the time of day. These range from 00:00 to 23:59, which means that morning is about 25% through the curve, midday 50%, evening 75%, etc. \n \n Constant controls set the value to a single value that remains constant regardless of the time of day.", MessageType.Info);

            GUIStyle labelStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
            labelStyle.fontStyle = FontStyle.Bold;

            EditorGUILayout.LabelField(" Color Settings", labelStyle);
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudColor"), new GUIContent("Cloud Color", "The main color of the unlit clouds."), false);

            if (cloudShader.HasProperty("_AltoCloudColor"))
                EditorGUILayout.PropertyField(serializedObject.FindProperty("highAltitudeCloudColor"), new GUIContent("High Altitude Color", "The main color multiplier of the high altitude clouds. The cloud types affected are the cirrostratus and the altocumulus types."), false);

            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudHighlightColor"), new GUIContent("Sun Highlight Color", "The color multiplier for the clouds in a \"dot\" around the sun."), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudSunHighlightFalloff"), new GUIContent("Sun Highlight Falloff", "The falloff for the \"dot\" around the sun."), false);
            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudMoonColor"), new GUIContent("Moon Highlight Color", "The color multiplier for the clouds in a \"dot\" around the moon."), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudMoonHighlightFalloff"), new GUIContent("Moon Highlight Falloff", "The falloff for the \"dot\" around the moon."), false);


            EditorGUI.indentLevel--;
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField(" Generation Settings", labelStyle);
            EditorGUI.indentLevel++;

            if (cloudShader.HasProperty("_WindSpeed"))
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudWindSpeed"), new GUIContent("Wind Speed", "The speed at which the cloud generation will progress."), false);

            if (cloudShader.HasProperty("_ClippingThreshold"))
                EditorGUILayout.PropertyField(serializedObject.FindProperty("clippingThreshold"), new GUIContent("Clipping Threshold", "The alpha that the clouds will clip to full alpha at. Default is 0.5"), false);

            if (cloudShader.HasProperty("_MainCloudScale"))
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudMainScale"), new GUIContent("Main Scale", "The scale of the main perlin noise for the cumulus cloud type."), false);

            if (cloudShader.HasProperty("_DetailScale"))
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudDetailScale"), new GUIContent("Detail Scale", "The scale of the secondary voronoi noise functions for the cumulus cloud type."), false);

            if (cloudShader.HasProperty("_DetailAmount"))
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudDetailAmount"), new GUIContent("Detail Amount", "The multiplier for the secondary voronoi noise functions for the cumulus cloud type. Lower values give more cohesive cloud types."), false);
            EditorGUILayout.Space(10);

            if (cloudShader.HasProperty("_AltocumulusScale"))
                EditorGUILayout.PropertyField(serializedObject.FindProperty("acScale"), new GUIContent("Altocumulus Scale"), false);

            if (cloudShader.HasProperty("_CirrostratusMoveSpeed"))
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cirroMoveSpeed"), new GUIContent("Cirrostratus Movement Speed"), false);

            if (cloudShader.HasProperty("_CirrusMoveSpeed"))
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cirrusMoveSpeed"), new GUIContent("Cirrus Movement Speed"), false);

            if (cloudShader.HasProperty("_ChemtrailsMoveSpeed"))
                EditorGUILayout.PropertyField(serializedObject.FindProperty("chemtrailsMoveSpeed"), new GUIContent("Chemtrails Movement Speed"), false);

            if (cloudShader.HasProperty("_CloudTexture"))
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudTexture"), new GUIContent("Cloud Texture"), false);

            if (cloudShader.HasProperty("_TexturePanDirection"))
                EditorGUILayout.PropertyField(serializedObject.FindProperty("texturePanDirection"), new GUIContent("Cloud Texture Pan Direction"), false);

            if (omwsWeather.cloudStyle == OMWSWeather.CloudStyle.paintedSkies)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudTextureColor"), new GUIContent("Texture Color Multiplier"), false);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("textureAmount"), new GUIContent("Texture Amount"), false);
            }

            if (cloudShader.HasProperty("_CloudCohesion"))
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudCohesion"), new GUIContent("Cloud Cohesion"), false);

            if (cloudShader.HasProperty("_Spherize"))
                EditorGUILayout.PropertyField(serializedObject.FindProperty("spherize"), new GUIContent("Sphere Distortion"), false);

            if (cloudShader.HasProperty("_ShadowingDistance"))
                EditorGUILayout.PropertyField(serializedObject.FindProperty("shadowDistance"), new GUIContent("Shadow Distance"), false);

            if (cloudShader.HasProperty("_CloudThickness"))
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudThickness"), new GUIContent("Cloud Thickness"), false);

            EditorGUI.indentLevel--;


        }

        void DrawCelestialsTab(OMWSWeather omwsWeather)
        {
            bool advancedSky = omwsWeather.skyStyle == OMWSWeather.SkyStyle.desktop;

            Color col = EditorGUIUtility.isProSkin ? proCol : unityCol;

            if (tooltips)
                EditorGUILayout.HelpBox("Interpolate controls change the value depending on the time of day. These range from 00:00 to 23:59, which means that morning is about 25% through the curve, midday 50%, evening 75%, etc. \n \n Constant controls set the value to a single value that remains constant regardless of the time of day.", MessageType.Info);

            GUIStyle labelStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
            labelStyle.fontStyle = FontStyle.Bold;

            EditorGUILayout.LabelField(" Sun Settings", labelStyle);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("sunColor"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("sunSize"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("sunDirection"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("sunPitch"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("sunFalloff"), new GUIContent("Sun Halo Falloff"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("sunFlareColor"), new GUIContent("Sun Halo Color"), false);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space(16);

            if (advancedSky)
            {
                EditorGUILayout.LabelField(" Moon Settings", labelStyle);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("moonFalloff"), new GUIContent("Moon Halo Falloff"), false);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("moonFlareColor"), new GUIContent("Moon Halo Color"), false);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField(" VFX", labelStyle);
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("starColor"), false);

            if (advancedSky)
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("galaxyIntensity"), false);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("galaxy1Color"), false);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("galaxy2Color"), false);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("galaxy3Color"), false);
                EditorGUILayout.Space(5);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("lightScatteringColor"), false);
                EditorGUILayout.Space(5);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("useRainbow"), false);
                EditorGUI.BeginDisabledGroup(!serializedObject.FindProperty("useRainbow").boolValue);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("rainbowPosition"), false);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("rainbowWidth"), false);
                EditorGUI.EndDisabledGroup();
            }

            EditorGUI.indentLevel--;
        }
    }
#endif
}
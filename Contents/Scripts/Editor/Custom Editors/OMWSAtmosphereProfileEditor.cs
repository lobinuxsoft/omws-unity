using CryingOnion.OhMy.WeatherSystem.Core;
using CryingOnion.OhMy.WeatherSystem.Data;
using CryingOnion.OhMy.WeatherSystem.Utility;
using UnityEditor;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.EditorScript
{
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
                new GUIContent("    Atmosphere & Lighting", "Skydome, fog, and lighting settings."), OMWSEditorUtilities.FoldoutStyle());

            if (serializedObject.FindProperty("win1").boolValue)
                DrawAtmosphereTab(omwsWeather);

            EditorGUILayout.EndFoldoutHeaderGroup();

            serializedObject.FindProperty("win2").boolValue = EditorGUILayout.BeginFoldoutHeaderGroup(serializedObject.FindProperty("win2").boolValue,
                            new GUIContent("    Clouds", "Cloud color, generation, and variation settings."), OMWSEditorUtilities.FoldoutStyle());

            if (serializedObject.FindProperty("win2").boolValue)
                DrawCloudsTab(omwsWeather);

            EditorGUILayout.EndFoldoutHeaderGroup();

            serializedObject.FindProperty("win3").boolValue = EditorGUILayout.BeginFoldoutHeaderGroup(serializedObject.FindProperty("win3").boolValue,
                            new GUIContent("    Celestials & VFX", "Sun, moon, and light FX settings."), OMWSEditorUtilities.FoldoutStyle());

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
}
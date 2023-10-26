using CryingOnion.OhMy.WeatherSystem.Core;
using CryingOnion.OhMy.WeatherSystem.Data;
using CryingOnion.OhMy.WeatherSystem.Module;
using CryingOnion.OhMy.WeatherSystem.Utility;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.EditorScript
{
    [CustomEditor(typeof(OMWSWeather))]
    [CanEditMultipleObjects]
    public class OMWSWeatherEditor : Editor
    {
        public List<Type> mods;

        public int windowNum;
        protected static bool climate;
        protected static bool satWindow;
        protected static bool windWindow;
        protected static bool thunderWindow;
        protected static bool matProfileWindow;
        protected static bool matOptionsWindow;
        protected static bool modules;
        protected static bool options;

        public bool tooltips;

        Color proCol = (Color)new Color32(50, 50, 50, 255);
        Color unityCol = (Color)new Color32(194, 194, 194, 255);
        OMWSWeather t;

        Editor atmosEditor;

        void OnEnable()
        {
            serializedObject.Update();

            serializedObject.FindProperty("icon1").objectReferenceValue = Resources.Load<Texture>("Atmosphere");
            serializedObject.FindProperty("icon2").objectReferenceValue = Resources.Load<Texture>("OMWSCalendar");
            serializedObject.FindProperty("icon3").objectReferenceValue = Resources.Load<Texture>("Weather Profile-01");
            serializedObject.FindProperty("icon4").objectReferenceValue = Resources.Load<Texture>("MoreOptions");
            t = (OMWSWeather)target;

            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            tooltips = EditorPrefs.GetBool("OMWS_Tooltips", true);

            List<GUIContent> icons = new List<GUIContent>();
            Rect position = EditorGUILayout.GetControlRect(GUILayout.Height(0));

            icons.Add(new GUIContent("", (Texture)serializedObject.FindProperty("icon1").objectReferenceValue, "Atmosphere: Manage skydome, fog, and lighting settings."));
            icons.Add(new GUIContent("", (Texture)serializedObject.FindProperty("icon2").objectReferenceValue, "Time: Setup time settings, calendars, and manage current settings."));
            icons.Add(new GUIContent("", (Texture)serializedObject.FindProperty("icon3").objectReferenceValue, "Ecosystem: Manage weather, climate, and year settings."));
            icons.Add(new GUIContent("", (Texture)serializedObject.FindProperty("icon4").objectReferenceValue, "Settings & Modules: Adjust the functions of OMWS to get the most out of your system."));

            if (tooltips)
            {
                EditorGUILayout.HelpBox("Welcome to the OMWS system! This is your one-stop-shop to managing all the weather parameters for this system. " +
                "OMWS organizes parameters into various components called modules. You can add or remove modules in the settings tab. Check out the various modules on the system to edit the parameters", MessageType.Info, true);
                if (GUILayout.Button("Disable Tooltips"))
                    EditorPrefs.SetBool("OMWS_Tooltips", !EditorPrefs.GetBool("OMWS_Tooltips", true));
                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox("Atmosphere tab controls skydome, fog, and lighting settings.", MessageType.Info);
                EditorGUILayout.HelpBox("Setup time settings, calendars, and manage current settings.", MessageType.Info);
                EditorGUILayout.HelpBox("Manage weather profiles, forecast and ecosystem.", MessageType.Info);
                EditorGUILayout.HelpBox("Control options and add and remove modules.", MessageType.Info);
                EditorGUILayout.EndHorizontal();

            }

            foreach (OMWSModule module in t.modules)
            {
                if (module != null)
                    icons.Add((CreateEditor(module) as OMWSModuleEditor).GetGUIContent());
            }

            GUIStyle iconStyle = new GUIStyle(GUI.skin.GetStyle("Button"));
            iconStyle.fixedHeight = 40;
            iconStyle.fixedWidth = (position.width / 4) - 5;
            iconStyle.margin = new RectOffset(5, 5, 5, 5);
            iconStyle.fontStyle = FontStyle.Bold;

            windowNum = serializedObject.FindProperty("window").intValue;
            int j = GUILayout.SelectionGrid(windowNum, icons.ToArray(), 4, iconStyle);

            if (j != windowNum)
            {
                if (j == 3)
                    mods = OMWSEditorUtilities.ResetModuleList();

                windowNum = j;
            }

            serializedObject.FindProperty("window").intValue = windowNum;

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();

            switch (windowNum)
            {
                case 0:
                    Atmos();
                    break;
                case 1:
                    Time();
                    break;
                case 2:
                    Weather();
                    break;
                case 3:
                    Settings();
                    break;
                default:
                    CustomModule(t.modules[windowNum - 4]);
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }

        #region Atmosphere

        public void Atmos()
        {
            GUIStyle labelStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
            labelStyle.fontStyle = FontStyle.Bold;

            if (t.overrideAtmosphere)
            {
                EditorGUILayout.HelpBox($"Atmosphere control is currently being overriden by {t.overrideAtmosphere.GetType().Name}", MessageType.Warning);
                DrawNativeSettingsTab();
                return;
            }

            serializedObject.FindProperty("atmosSettingsWindow").boolValue = EditorGUILayout.BeginFoldoutHeaderGroup(serializedObject.FindProperty("atmosSettingsWindow").boolValue,
                new GUIContent("    Selection Settings"), OMWSEditorUtilities.FoldoutStyle());

            if (serializedObject.FindProperty("atmosSettingsWindow").boolValue)
            {
                if (tooltips)
                    EditorGUILayout.HelpBox("How should this weather system manage atmosphere settings? Native sets all settings locally to this system, Profile sets global settings on the atmosphere profile.", MessageType.Info);

                if (serializedObject.FindProperty("atmosphereControl").enumValueIndex == (int)OMWSWeather.AtmosphereSelection.profile)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("atmosphereControl"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("atmosphereProfile"));
                    if (serializedObject.hasModifiedProperties)
                        atmosEditor = CreateEditor(serializedObject.FindProperty("atmosphereProfile").objectReferenceValue);
                }
                else
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("atmosphereControl"));
                }

                EditorGUILayout.Space();

                if (tooltips)
                    EditorGUILayout.HelpBox("Set the shader model used for the sky, clouds, and fog here.", MessageType.Info);

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("skyStyle"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudStyle"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("fogStyle"));
                serializedObject.ApplyModifiedProperties();

                if (EditorGUI.EndChangeCheck())
                    t.ResetQuality();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            EditorGUI.indentLevel = 0;

            if (serializedObject.FindProperty("atmosphereControl").enumValueIndex == (int)OMWSWeather.AtmosphereSelection.native)
            {
                DrawNativeAtmosphere();
            }
            else if (serializedObject.FindProperty("atmosphereProfile").objectReferenceValue)
            {
                if (atmosEditor == null)
                {
                    atmosEditor = CreateEditor(serializedObject.FindProperty("atmosphereProfile").objectReferenceValue);
                    (atmosEditor as OMWSAtmosphereProfileEditor).OnInspectorGUIInline(t);
                }
                else
                    (atmosEditor as OMWSAtmosphereProfileEditor).OnInspectorGUIInline(t);
            }
            else
            {
                EditorGUILayout.HelpBox("Assign an atmosphere profile!", MessageType.Error);
            }
        }

        public void DrawNativeAtmosphere()
        {
            serializedObject.FindProperty("win1").boolValue = EditorGUILayout.BeginFoldoutHeaderGroup(serializedObject.FindProperty("win1").boolValue,
              new GUIContent("    Atmosphere & Lighting", "Skydome, fog, and lighting settings."), OMWSEditorUtilities.FoldoutStyle());

            if (serializedObject.FindProperty("win1").boolValue)
                DrawAtmosphereTab();

            EditorGUILayout.EndFoldoutHeaderGroup();

            serializedObject.FindProperty("win2").boolValue = EditorGUILayout.BeginFoldoutHeaderGroup(serializedObject.FindProperty("win2").boolValue,
                            new GUIContent("    Clouds", "Cloud color, generation, and variation settings."), OMWSEditorUtilities.FoldoutStyle());

            if (serializedObject.FindProperty("win2").boolValue)
                DrawCloudsTab();

            EditorGUILayout.EndFoldoutHeaderGroup();

            serializedObject.FindProperty("win3").boolValue = EditorGUILayout.BeginFoldoutHeaderGroup(serializedObject.FindProperty("win3").boolValue,
                            new GUIContent("    Celestials & VFX", "Sun, moon, and light FX settings."), OMWSEditorUtilities.FoldoutStyle());

            if (serializedObject.FindProperty("win3").boolValue)
                DrawCelestialsTab();

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        void DrawAtmosphereTab()
        {
            GUIStyle labelStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
            labelStyle.fontStyle = FontStyle.Bold;

            OMWSAtmosphereProfile atmos = t.atmosphereProfile;

            bool advancedSky = t.skyStyle == OMWSWeather.SkyStyle.desktop;

            EditorGUILayout.LabelField(" Skydome Settings", labelStyle);
            EditorGUI.indentLevel++;

            if (tooltips)
                EditorGUILayout.HelpBox("In native mode, all values are set to static references that must be interpolated manually. For automatic interpolation based on the time of day, switch to profile mode.", MessageType.Info);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("skyZenithColor"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("skyHorizonColor"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("gradientExponent"), false);

            if (advancedSky)
            {
                EditorGUILayout.Space(5);
                float min = serializedObject.FindProperty("atmosphereVariationMin").floatValue;
                float max = serializedObject.FindProperty("atmosphereVariationMax").floatValue;

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

                serializedObject.FindProperty("atmosphereVariationMin").floatValue = min;
                serializedObject.FindProperty("atmosphereVariationMax").floatValue = max;

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

        void DrawNativeSettingsTab()
        {
            serializedObject.FindProperty("atmosSettingsWindow").boolValue = EditorGUILayout.BeginFoldoutHeaderGroup(serializedObject.FindProperty("atmosSettingsWindow").boolValue,
                new GUIContent("    Global Settings"), OMWSEditorUtilities.FoldoutStyle());

            if (!serializedObject.FindProperty("atmosSettingsWindow").boolValue)
                return;

            Material cloudShader = t.cloudMesh.sharedMaterial;

            if (tooltips)
                EditorGUILayout.HelpBox("In native mode, all values are set to static references that must be interpolated manually. For automatic interpolation based on the time of day, switch to profile mode.", MessageType.Info);

            GUIStyle labelStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
            labelStyle.fontStyle = FontStyle.Bold;

            EditorGUILayout.LabelField(" Sun Settings", labelStyle);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("sunDirection"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("sunPitch"), false);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            EditorGUILayout.LabelField(" Atmosphere Settings", labelStyle);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ambientLightMultiplier"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("useRainbow"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("rainbowPosition"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("rainbowWidth"), false);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            EditorGUILayout.LabelField(" Cloud Settings", labelStyle);
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


            if (cloudShader.HasProperty("_CloudTextureColor"))
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudTextureColor"), new GUIContent("Texture Color Multiplier"), false);

            if (cloudShader.HasProperty("_CloudTexture"))
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudTexture"), new GUIContent("Cloud Texture"), false);

            if (cloudShader.HasProperty("_TexturePanDirection"))
                EditorGUILayout.PropertyField(serializedObject.FindProperty("texturePanDirection"), new GUIContent("Cloud Texture Pan Direction"), false);

            if (cloudShader.HasProperty("_TextureAmount"))
                EditorGUILayout.PropertyField(serializedObject.FindProperty("textureAmount"), new GUIContent("Texture Amount"), false);

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

        void DrawCloudsTab()
        {
            Material cloudShader = t.cloudMesh.sharedMaterial;

            if (tooltips)
                EditorGUILayout.HelpBox("In native mode, all values are set to static references that must be interpolated manually. For automatic interpolation based on the time of day, switch to profile mode.", MessageType.Info);

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


            if (cloudShader.HasProperty("_CloudTextureColor"))
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudTextureColor"), new GUIContent("Texture Color Multiplier"), false);

            if (cloudShader.HasProperty("_CloudTexture"))
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudTexture"), new GUIContent("Cloud Texture"), false);

            if (cloudShader.HasProperty("_TexturePanDirection"))
                EditorGUILayout.PropertyField(serializedObject.FindProperty("texturePanDirection"), new GUIContent("Cloud Texture Pan Direction"), false);

            if (cloudShader.HasProperty("_TextureAmount"))
                EditorGUILayout.PropertyField(serializedObject.FindProperty("textureAmount"), new GUIContent("Texture Amount"), false);

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

        void DrawCelestialsTab()
        {

            if (tooltips)
                EditorGUILayout.HelpBox("In native mode, all values are set to static references that must be interpolated manually. For automatic interpolation based on the time of day, switch to profile mode.", MessageType.Info);

            bool advancedSky = t.skyStyle == OMWSWeather.SkyStyle.desktop;

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
                if (t.GetModule<OMWSSatelliteManager>())
                {
                    EditorGUILayout.LabelField(" Moon Settings", labelStyle);
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("moonFalloff"), new GUIContent("Moon Halo Falloff"), false);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("moonFlareColor"), new GUIContent("Moon Halo Color"), false);
                    EditorGUI.indentLevel--;
                }
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
                EditorGUILayout.PropertyField(serializedObject.FindProperty("rainbowPosition"), false);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("rainbowWidth"), false);
            }

            EditorGUI.indentLevel--;
        }

        #endregion

        #region Time
        public void Time()
        {
            if (t.overrideTime)
            {
                EditorGUILayout.HelpBox($"Time control is currently being overriden by {t.overrideTime.GetType().Name}", MessageType.Warning);
                return;
            }

            bool timeControl = serializedObject.FindProperty("timeControl").enumValueIndex == (int)OMWSWeather.TimeControl.profile;
            SerializedProperty calendar = serializedObject.FindProperty("calendar");
            OMWSPerennialProfile perennial = serializedObject.FindProperty("perennialProfile").objectReferenceValue as OMWSPerennialProfile;
            OMWSPerennialProfileEditor timeEditor = CreateEditor(serializedObject.FindProperty("perennialProfile").objectReferenceValue) as OMWSPerennialProfileEditor;

            serializedObject.FindProperty("atmosSettingsWindow").boolValue = EditorGUILayout.BeginFoldoutHeaderGroup(serializedObject.FindProperty("atmosSettingsWindow").boolValue,
                new GUIContent("    Selection Settings"), OMWSEditorUtilities.FoldoutStyle());

            if (serializedObject.FindProperty("atmosSettingsWindow").boolValue)
            {
                if (tooltips)
                    EditorGUILayout.HelpBox("How should this weather system manage time settings? Native sets the time locally to this system, Profile mode sets global settings on the perennial profile.", MessageType.Info);

                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("timeControl"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("perennialProfile"));
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            serializedObject.FindProperty("timeCurrentWindow").boolValue = EditorGUILayout.BeginFoldoutHeaderGroup(serializedObject.FindProperty("timeCurrentWindow").boolValue,
                new GUIContent("    Current Settings"), OMWSEditorUtilities.FoldoutStyle());

            if (serializedObject.FindProperty("timeCurrentWindow").boolValue)
            {
                EditorGUI.indentLevel++;

                if (tooltips)
                {
                    EditorGUILayout.HelpBox("OMWS uses a tick system to tell time instead of a minutes/hours system. By default, a day is set to 360 ticks long (one for every degree of rotation that the sun will make). Converting the ticks to a time of day is easy! Midnight is 0 ticks, 6:00 AM is 90, noon is 180, 6:00 PM is 270, and then the cycle restarts.", MessageType.Info);
                    EditorGUILayout.HelpBox("You can also change the length of the year! The default profile uses 48 days in a year to create a shorter year to improve contrast.", MessageType.Info);
                    EditorGUILayout.HelpBox("Don't like the proportions of the current time system? Not to worry! Check out the 2400 tick perennial profile for a more realistic year!", MessageType.Info);
                }

                if (timeControl)
                {
                    timeEditor.OnRuntimeMeasureGUI();
                }
                else
                {
                    EditorGUI.BeginChangeCheck();
                    calendar.FindPropertyRelative("currentTicks").floatValue = EditorGUILayout.Slider("Current Ticks", calendar.FindPropertyRelative("currentTicks").floatValue, 0, perennial.ticksPerDay);

                    if (EditorGUI.EndChangeCheck())
                    {
                        OMWSMeridiemTime i = new OMWSMeridiemTime();
                        OMWSMeridiemTime.DayPercentToMeridiemTime(t.GetCurrentDayPercentage(), ref i);
                        calendar.FindPropertyRelative("meridiemTime").FindPropertyRelative("hours").intValue = i.hours;
                        calendar.FindPropertyRelative("meridiemTime").FindPropertyRelative("minutes").intValue = i.minutes;
                        calendar.FindPropertyRelative("meridiemTime").FindPropertyRelative("meridiem").intValue = (int)i.meridiem;
                    }

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(calendar.FindPropertyRelative("meridiemTime"));

                    if (EditorGUI.EndChangeCheck())
                    {
                        calendar.FindPropertyRelative("currentTicks").floatValue = t.perennialProfile.ticksPerDay * OMWSMeridiemTime.MeridiemTimeToDeyPercent(
                        calendar.FindPropertyRelative("meridiemTime").FindPropertyRelative("hours").intValue,
                        calendar.FindPropertyRelative("meridiemTime").FindPropertyRelative("minutes").intValue,
                        (OMWSMeridiemTime.Meridiem)calendar.FindPropertyRelative("meridiemTime").FindPropertyRelative("meridiem").intValue);
                    }

                    calendar.FindPropertyRelative("currentDay").intValue = EditorGUILayout.IntSlider("Current Day", calendar.FindPropertyRelative("currentDay").intValue, 0, perennial.daysPerYear);
                    calendar.FindPropertyRelative("currentYear").intValue = EditorGUILayout.IntField("Current Year", calendar.FindPropertyRelative("currentYear").intValue);
                }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            bool lengthWindow = serializedObject.FindProperty("tickLengthWindow").boolValue;
            bool movementWindow = serializedObject.FindProperty("tickMovementWindow").boolValue;
            bool curveWindow = serializedObject.FindProperty("curveWindow").boolValue;

            timeEditor.OnStaticMeasureGUI(OMWSEditorUtilities.FoldoutStyle(), ref lengthWindow, ref movementWindow, ref curveWindow);

            serializedObject.FindProperty("tickLengthWindow").boolValue = lengthWindow;
            serializedObject.FindProperty("tickMovementWindow").boolValue = movementWindow;
            serializedObject.FindProperty("curveWindow").boolValue = curveWindow;
        }

        #endregion

        #region Weather

        public void Weather()
        {
            if (t.overrideWeather)
            {
                EditorGUILayout.HelpBox($"Weather property control is currently being overriden by {t.overrideWeather.GetType().Name}", MessageType.Warning);
                return;
            }

            serializedObject.FindProperty("atmosSettingsWindow").boolValue = EditorGUILayout.BeginFoldoutHeaderGroup(serializedObject.FindProperty("atmosSettingsWindow").boolValue,
                new GUIContent("    Selection Settings"), OMWSEditorUtilities.FoldoutStyle());

            EditorGUILayout.EndFoldoutHeaderGroup();

            bool useSingle = (OMWSEcosystem.EcosystemStyle)serializedObject.FindProperty("weatherSelectionMode").enumValueIndex == OMWSEcosystem.EcosystemStyle.manual;
            serializedObject.ApplyModifiedProperties();

            if (serializedObject.FindProperty("atmosSettingsWindow").boolValue)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("weatherSelectionMode"));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("currentWeather"), new GUIContent("Global Weather"));
                if (t.GetCurrentWeatherProfile())
                    EditorGUILayout.LabelField(new GUIContent($"Current Weather is {t.GetCurrentWeatherProfile().name}"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("weatherTransitionTime"));


                if (serializedObject.hasModifiedProperties)
                {
                    serializedObject.ApplyModifiedProperties();
                    t.CalculateFilterColors();
                }

                EditorGUI.indentLevel--;
            }
            if (useSingle)
            {
                serializedObject.FindProperty("currentWeatherWindow").boolValue = EditorGUILayout.BeginFoldoutHeaderGroup(serializedObject.FindProperty("currentWeatherWindow").boolValue,
                    new GUIContent("    Profile Settings"), OMWSEditorUtilities.FoldoutStyle());

                EditorGUILayout.EndFoldoutHeaderGroup();

                if (serializedObject.FindProperty("currentWeatherWindow").boolValue)
                {
                    EditorGUI.indentLevel++;
                    (CreateEditor(serializedObject.FindProperty("currentWeather").objectReferenceValue) as OMWSWeatherProfileEditor).DisplayInOMWSWindow(t);
                    EditorGUI.indentLevel--;
                }
            }
            else
            {
                serializedObject.FindProperty("forecastWindow").boolValue = EditorGUILayout.BeginFoldoutHeaderGroup(serializedObject.FindProperty("forecastWindow").boolValue,
                    new GUIContent("    Forecasting Behaviors"), OMWSEditorUtilities.FoldoutStyle());

                EditorGUILayout.EndFoldoutHeaderGroup();

                if (serializedObject.FindProperty("forecastWindow").boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("forecastProfile"));
                    EditorGUILayout.Space();
                    EditorGUI.indentLevel++;

                    if (serializedObject.FindProperty("forecastProfile").objectReferenceValue)
                        CreateEditor(serializedObject.FindProperty("forecastProfile").objectReferenceValue).OnInspectorGUI();

                    EditorGUI.indentLevel--;
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("weatherTransitionTime"));
                    EditorGUI.indentLevel--;
                }

                climate = EditorGUILayout.BeginFoldoutHeaderGroup(climate, new GUIContent("    Climate Settings"), OMWSEditorUtilities.FoldoutStyle());

                EditorGUILayout.EndFoldoutHeaderGroup();

                if (climate)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("climateProfile"));
                    EditorGUILayout.Space();
                    EditorGUI.indentLevel++;
                    CreateEditor(serializedObject.FindProperty("climateProfile").objectReferenceValue).OnInspectorGUI();
                    EditorGUI.indentLevel--;
                    EditorGUI.indentLevel--;
                }
            }
        }

        #endregion

        public void Settings()
        {
            if (tooltips)
                EditorGUILayout.HelpBox("Add modules using this foldout! Use the reset module list buttone to search for any scripts that derive from OMWSModule and add them to the dropdown.", MessageType.Info, true);

            modules = EditorGUILayout.BeginFoldoutHeaderGroup(modules, "    Modules", OMWSEditorUtilities.FoldoutStyle());
            EditorGUILayout.EndFoldoutHeaderGroup();

            if (modules)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.BeginHorizontal();

                List<string> modNames = new List<string>() { "Select..." };

                if (mods == null)
                    mods = OMWSEditorUtilities.ResetModuleList();

                if (mods.Contains(typeof(OMWSModule)))
                    mods.Remove(typeof(OMWSModule));

                if (mods.Contains(typeof(OMWSExampleModuleEditor)))
                    mods.Remove(typeof(OMWSExampleModuleEditor));

                foreach (OMWSModule a in t.modules)
                    if (mods.Contains(a.GetType()))
                        mods.Remove(a.GetType());

                foreach (Type a in mods)
                    modNames.Add(a.Name);

                int moduleNumber = 0;
                moduleNumber = EditorGUILayout.Popup(new GUIContent("Add New Module"), 0, modNames.ToArray());

                if (GUILayout.Button("Reset Module List"))
                    OMWSEditorUtilities.ResetModuleList();

                EditorGUILayout.EndHorizontal();

                if (moduleNumber != 0)
                    t.IntitializeModule(mods[moduleNumber - 1]);

                EditorGUI.indentLevel++;

                OMWSModule j = null;

                foreach (OMWSModule i in t.modules)
                {
                    if (i == null)
                        continue;

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.PrefixLabel((CreateEditor(i) as OMWSModuleEditor).GetGUIContent());
                    if (GUILayout.Button("Remove"))
                    {
                        j = i;
                        mods = OMWSEditorUtilities.ResetModuleList();
                    }

                    EditorGUILayout.EndHorizontal();
                }

                t.DeintitializeModule(j);

                EditorGUI.indentLevel--;

                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }

            options = EditorGUILayout.BeginFoldoutHeaderGroup(options, "    Options", OMWSEditorUtilities.FoldoutStyle());
            EditorGUILayout.EndFoldoutHeaderGroup();

            if (options)
            {
                if (tooltips)
                {
                    EditorGUILayout.HelpBox("OMWS automatically aligns itself to a camera. By default the main camera is used, however you can set it up to use a different camera or to not lock to a camera by using the enumerator below.", MessageType.Info, true);
                    EditorGUILayout.Space();
                }

                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("lockToCamera"));

                if (t.lockToCamera == OMWSWeather.LockToCameraStyle.useCustomCamera)
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("omwsCamera"));

                EditorGUILayout.Space();

                if (tooltips)
                {
                    EditorGUILayout.HelpBox("Should the properties and colors of the system be set via a procedural profile that applies to all systems (default) or via a native direct setting that only applies to this system.", MessageType.Info, true);
                    EditorGUILayout.Space();
                }

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("atmosphereControl"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("timeControl"));
                EditorGUILayout.Space();

                if (tooltips)
                {
                    EditorGUILayout.HelpBox("Change the shader used for the sky, clouds and fog. You can also disable all the features individually here!", MessageType.Info, true);
                    EditorGUILayout.Space();
                }

                EditorGUILayout.PropertyField(serializedObject.FindProperty("skyStyle"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudStyle"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("fogStyle"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("usePhysicalSunHeight"));

                EditorGUI.indentLevel--;

                serializedObject.ApplyModifiedProperties();

                if (EditorGUI.EndChangeCheck())
                    t.ResetQuality();
            }

            if (GUILayout.Button("Toggle Tooltips"))
                EditorPrefs.SetBool("OMWS_Tooltips", !EditorPrefs.GetBool("OMWS_Tooltips", true));
        }

        public void CustomModule(OMWSModule module)
        {
            if (t.gameObject.scene == UnityEngine.SceneManagement.SceneManager.GetActiveScene())
            {
                if (module != null && module.isActiveAndEnabled)
                {
                    EditorGUI.indentLevel++;
                    (CreateEditor(module) as OMWSModuleEditor).DisplayInOMWSWindow();
                    EditorGUI.indentLevel--;
                }
                else
                    EditorGUILayout.HelpBox("Something went wrong! Try removing and readding the module!", MessageType.Error);
            }
            else
                EditorGUILayout.HelpBox("Modules may only be edited in the scene!", MessageType.Info);
        }
    }
}
﻿using UnityEngine;
using System.Collections.Generic;
using CryingOnion.OhMy.WeatherSystem.Utility;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CryingOnion.OhMy.WeatherSystem.Data
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "CryingOnion/Oh My Weather System/Material Manager Profile", order = 361)]
    public class OMWSMaterialManagerProfile : ScriptableObject
    {
        public Texture snowTexture;
        public float snowNoiseSize = 10;
        public Color snowColor = Color.white;
        public float puddleScale = 2;

        [System.Serializable]
        public class OMWSModulatedValue
        {
            public bool expanded = false;
            public enum ModulationSource { dayPercent, yearPercent, precipitation, temperature, snowAmount, rainAmount }
            public enum ModulationTarget { terrainLayerColor, materialColor, materialValue, globalColor, globalValue }

            [Tooltip("The source that will modulate the target.")]
            public ModulationSource modulationSource;

            [Tooltip("The target type that will be modulated.")]
            public ModulationTarget modulationTarget;

            [Tooltip("The gradient that will pass a color to the modulation target based on the modulation source.")]
            public Gradient mappedGradient;

            [Tooltip("The curve that will pass a float value to the modulation target based on the modulation source.")]
            public AnimationCurve mappedCurve;

            [Tooltip("The terrain layer that this profile impacts.")]
            public TerrainLayer targetLayer;

            [Tooltip("The material that this profile impacts.")]
            public Material targetMaterial;

            public string targetVariableName;
        }

        [OMWSModulatedProperty]
        public List<OMWSModulatedValue> modulatedValues;
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(OMWSMaterialManagerProfile))]
    [CanEditMultipleObjects]
    public class OMWSMaterialProfileEditor : Editor
    {
        SerializedProperty terrainLayers;
        SerializedProperty seasonalMaterials;
        SerializedProperty seasonalValueMaterials;
        SerializedProperty snowTexture;
        SerializedProperty snowNoiseSize;
        SerializedProperty snowColor;
        SerializedProperty puddleScale;
        OMWSMaterialManagerProfile prof;

        public static bool modulatedValuesOpen;
        public static bool globalOpen;

        void OnEnable()
        {
            snowTexture = serializedObject.FindProperty("snowTexture");
            snowNoiseSize = serializedObject.FindProperty("snowNoiseSize");
            snowColor = serializedObject.FindProperty("snowColor");
            puddleScale = serializedObject.FindProperty("puddleScale");
            prof = (OMWSMaterialManagerProfile)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("modulatedValues"));

            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Global Snow Settings", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(snowTexture);
            EditorGUILayout.PropertyField(snowNoiseSize);
            EditorGUILayout.PropertyField(snowColor);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Global Rain Settings", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(puddleScale);
            EditorGUI.indentLevel--;

            serializedObject.ApplyModifiedProperties();
        }


        public void DisplayInCozyWindow()
        {
            serializedObject.Update();

            modulatedValuesOpen = EditorGUILayout.BeginFoldoutHeaderGroup(modulatedValuesOpen, "    Modulated Values", OMWSEditorUtilities.FoldoutStyle());
            EditorGUILayout.EndFoldoutHeaderGroup();

            if (modulatedValuesOpen)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(serializedObject.FindProperty("modulatedValues"));

                EditorGUI.indentLevel--;
            }

            globalOpen = EditorGUILayout.BeginFoldoutHeaderGroup(globalOpen, "    Global Values", OMWSEditorUtilities.FoldoutStyle());
            EditorGUILayout.EndFoldoutHeaderGroup();

            if (globalOpen)
            {
                EditorGUILayout.Space();

                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("Global Snow Settings", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(snowTexture);
                EditorGUILayout.PropertyField(snowNoiseSize);
                EditorGUILayout.PropertyField(snowColor);
                EditorGUI.indentLevel--;

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Global Rain Settings", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(puddleScale);
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
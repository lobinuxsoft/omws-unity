using CryingOnion.OhMy.WeatherSystem.Data;
using CryingOnion.OhMy.WeatherSystem.Utility;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.EditorScript
{
    [CustomPropertyDrawer(typeof(OMWSModulatedPropertyAttribute))]
    public class OMWSModulatedPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var indent = EditorGUI.indentLevel;

            float space = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            float height = EditorGUIUtility.singleLineHeight;

            var titleRect = new Rect(position.x + 15, position.y, position.width, height);
            var unitARect = new Rect(position.x, position.y + space, (position.width / 2) - 3, height);
            var unitBRect = new Rect((position.width / 2) + 75, position.y + space, (position.width / 2), height);
            var unitCRect = new Rect(position.x + 30, position.y + space * 2, position.width - 30, height);
            var unitDRect = new Rect(position.x + 30, position.y + space * 3, position.width - 30, height);
            var unitERect = new Rect(position.x + 30, position.y + space * 4, position.width - 30, height);
            var source = property.FindPropertyRelative("modulationSource");
            var target = property.FindPropertyRelative("modulationTarget");

            property.FindPropertyRelative("expanded").boolValue = EditorGUI.Foldout(titleRect, property.FindPropertyRelative("expanded").boolValue, GetTitle(property), true);

            if (property.FindPropertyRelative("expanded").boolValue)
            {
                SerializedProperty map = null;
                if ((OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationTarget)target.enumValueIndex == OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationTarget.globalValue ||
                (OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationTarget)target.enumValueIndex == OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationTarget.materialValue)
                    map = property.FindPropertyRelative("mappedCurve");
                else
                    map = property.FindPropertyRelative("mappedGradient");

                var targetLayer = property.FindPropertyRelative("targetLayer");
                var targetMaterial = property.FindPropertyRelative("targetMaterial");
                var targetVariableName = property.FindPropertyRelative("targetVariableName");

                EditorGUI.PropertyField(unitARect, target, GUIContent.none);
                EditorGUI.PropertyField(unitBRect, source, GUIContent.none);
                EditorGUI.PropertyField(unitCRect, map);

                List<string> names = new List<string>();
                int selected = -1;

                if ((OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationTarget)target.enumValueIndex == OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationTarget.materialValue)
                {
                    if (property.FindPropertyRelative("targetMaterial").objectReferenceValue)
                    {
                        for (int i = 0; i < (targetMaterial.objectReferenceValue as Material).shader.GetPropertyCount(); i++)
                            if ((targetMaterial.objectReferenceValue as Material).shader.GetPropertyType(i) == UnityEngine.Rendering.ShaderPropertyType.Float)
                                names.Add((targetMaterial.objectReferenceValue as Material).shader.GetPropertyName(i));

                        if (names.Contains(targetVariableName.stringValue))
                            selected = names.IndexOf(targetVariableName.stringValue);
                        else
                            selected = 0;
                    }
                }
                else if ((OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationTarget)target.enumValueIndex == OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationTarget.materialColor)
                {
                    if (property.FindPropertyRelative("targetMaterial").objectReferenceValue)
                    {
                        for (int i = 0; i < (targetMaterial.objectReferenceValue as Material).shader.GetPropertyCount(); i++)
                            if ((targetMaterial.objectReferenceValue as Material).shader.GetPropertyType(i) == UnityEngine.Rendering.ShaderPropertyType.Color)
                                names.Add((targetMaterial.objectReferenceValue as Material).shader.GetPropertyName(i));

                        if (names.Contains(targetVariableName.stringValue))
                            selected = names.IndexOf(targetVariableName.stringValue);
                        else
                            selected = 0;
                    }
                }

                switch ((OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationTarget)target.enumValueIndex)
                {
                    case (OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationTarget.globalColor):
                        EditorGUI.PropertyField(unitDRect, property.FindPropertyRelative("targetVariableName"), new GUIContent("Global Color Property Name", "The name of the global shader property to set."));
                        break;
                    case (OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationTarget.globalValue):
                        EditorGUI.PropertyField(unitDRect, property.FindPropertyRelative("targetVariableName"), new GUIContent("Global Value Property Name", "The name of the global shader property to set."));
                        break;
                    case (OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationTarget.materialColor):
                        EditorGUI.PropertyField(unitDRect, targetMaterial);
                        if (names.Count > 0)
                            property.FindPropertyRelative("targetVariableName").stringValue = names[EditorGUI.Popup(unitERect, "Material Value Property Name", selected, names.ToArray())];
                        break;
                    case (OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationTarget.materialValue):
                        EditorGUI.PropertyField(unitDRect, targetMaterial);
                        if (names.Count > 0)
                            property.FindPropertyRelative("targetVariableName").stringValue = names[EditorGUI.Popup(unitERect, "Material Value Property Name", selected, names.ToArray())];
                        break;
                    case (OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationTarget.terrainLayerColor):
                        EditorGUI.PropertyField(unitDRect, targetLayer);
                        break;
                }
            }

            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }

        string GetTitle(SerializedProperty property)
        {
            int value = property.FindPropertyRelative("modulationTarget").intValue;

            switch ((OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationTarget)value)
            {
                case (OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationTarget.globalColor):
                    if (property.FindPropertyRelative("targetVariableName").stringValue != "")
                        return $"   {property.FindPropertyRelative("targetVariableName").stringValue}";
                    else
                        return "   Global Shader Color";
                case (OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationTarget.globalValue):
                    if (property.FindPropertyRelative("targetVariableName").stringValue != "")
                        return $"   {property.FindPropertyRelative("targetVariableName").stringValue}";
                    else
                        return "   Global Shader Value";
                case (OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationTarget.materialColor):
                    if (property.FindPropertyRelative("targetMaterial").objectReferenceValue)
                        return $"   {property.FindPropertyRelative("targetMaterial").objectReferenceValue.name}";
                    else
                        return "   Local Material Color";
                case (OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationTarget.materialValue):
                    if (property.FindPropertyRelative("targetMaterial").objectReferenceValue)
                        return $"   {property.FindPropertyRelative("targetMaterial").objectReferenceValue.name}";
                    else
                        return "   Local Material Value";
                case (OMWSMaterialManagerProfile.OMWSModulatedValue.ModulationTarget.terrainLayerColor):
                    if (property.FindPropertyRelative("targetLayer").objectReferenceValue)
                        return $"   {property.FindPropertyRelative("targetLayer").objectReferenceValue.name}";
                    else
                        return "   Terrain Layer Color";
            }

            return "";
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float lineCount = 1.25f;

            if (property.FindPropertyRelative("expanded").boolValue)
                lineCount += 3.5f;

            return EditorGUIUtility.singleLineHeight * lineCount + EditorGUIUtility.standardVerticalSpacing * 2f * (lineCount - 1);
        }
    }
}
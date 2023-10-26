using CryingOnion.OhMy.WeatherSystem.Data;
using CryingOnion.OhMy.WeatherSystem.Utility;
using UnityEditor;

namespace CryingOnion.OhMy.WeatherSystem.EditorScript
{
    [CustomEditor(typeof(OMWSMaterialManagerProfile))]
    [CanEditMultipleObjects]
    public class OMWSMaterialManagerProfileEditor : Editor
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


        public void DisplayInOMWSWindow()
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
}
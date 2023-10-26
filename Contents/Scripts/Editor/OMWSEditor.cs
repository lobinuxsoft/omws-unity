using CryingOnion.OhMy.WeatherSystem.Core;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.EditorScript
{
    public class OMWSEditor : EditorWindow
    {
        public Texture titleWindow;
        public Vector2 scrollPos;

        public OMWSWeather headUnit;
        public Editor editor;

        [MenuItem("Crying Onion/Oh My Weather System/Open OMWS Editor", false, 0)]
        static void Init()
        {
            OMWSEditor window = (OMWSEditor)EditorWindow.GetWindow(typeof(OMWSEditor), false, "OMWS: Weather");
            window.minSize = new Vector2(400, 500);
            window.Show();
        }

        private void OnGUI()
        {
            GUI.DrawTexture(new Rect(0, 0, position.width, position.width * 1 / 3), titleWindow);
            EditorGUILayout.Space(position.width * 1 / 3);
            EditorGUILayout.Space(10);
            EditorGUILayout.Separator();
            EditorGUI.indentLevel = 1;

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            if (headUnit == null)
            {
                if (OMWSWeather.instance)
                {
                    headUnit = OMWSWeather.instance;
                    editor = Editor.CreateEditor(headUnit);
                }
                else
                {
                    if (GUILayout.Button("Setup OMWS"))
                        OMWSMenuItems.OMWSSetupScene();

                    EditorGUILayout.EndScrollView();
                    return;
                }
            }

            if (editor)
                editor.OnInspectorGUI();
            else if (headUnit)
                editor = Editor.CreateEditor(headUnit);


            EditorGUILayout.EndScrollView();
        }

        public static List<T> GetAssets<T>(string[] _foldersToSearch, string _filter) where T : UnityEngine.Object
        {
            string[] guids = AssetDatabase.FindAssets(_filter, _foldersToSearch);
            List<T> a = new List<T>();

            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                a.Add(AssetDatabase.LoadAssetAtPath<T>(path));
            }

            return a;
        }
    }
}
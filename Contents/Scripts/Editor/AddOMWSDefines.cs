using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace CryingOnion.OhMy.WeatherSystem.EditorScript
{
    [InitializeOnLoad]
    public class AddOMWSDefines : Editor
    {
        const string OMWS_WEATHER_URP = "OMWS_WEATHER_URP";
        const string OMWS_WEATHER_FMOD = "OMWS_WEATHER_FMOD";

        /// <summary>
        /// Symbols that will be added to the editor
        /// </summary>
        public static readonly string[] Symbols = new string[] { "CRYING_ONION", "OMWS" };

        /// <summary>
        /// Add define symbols as soon as Unity gets done compiling.
        /// </summary>
        static AddOMWSDefines()
        {
            string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            List<string> allDefines = definesString.Split(';').ToList();

            allDefines.AddRange(Symbols.Except(allDefines));

            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(";", allDefines.ToArray()));
        }

        [MenuItem("Crying Onion/Oh My Weather System/Enable FMOD Support", true)]
        static bool ValidateOMWSEnableFMODSupport()
        {
            string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            List<string> allDefines = definesString.Split(';').ToList();

            return !allDefines.Contains(OMWS_WEATHER_FMOD);
        }

        [MenuItem("Crying Onion/Oh My Weather System/Enable FMOD Support")]
        static void OMWSEnableFMODSupport()
        {
            string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            List<string> allDefines = definesString.Split(';').ToList();

            allDefines.Add(OMWS_WEATHER_FMOD);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(";", allDefines.ToArray()));
        }

        [MenuItem("Crying Onion/Oh My Weather System/Disable FMOD Support", true)]
        static bool ValidateOMWSDisableFMODSupport()
        {
            string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            List<string> allDefines = definesString.Split(';').ToList();

            return allDefines.Contains(OMWS_WEATHER_FMOD);
        }

        [MenuItem("Crying Onion/Oh My Weather System/Disable FMOD Support")]
        static void OMWSDisableFMODSupport()
        {
            string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            List<string> allDefines = definesString.Split(';').ToList();

            allDefines.Remove(OMWS_WEATHER_FMOD);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(";", allDefines.ToArray()));
        }
    }
}
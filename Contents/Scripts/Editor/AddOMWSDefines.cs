using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace CryingOnion.OhMy.WeatherSystem.EditorScript
{
    [InitializeOnLoad]
    public class AddOMWSDefines : Editor
    {
        const string OMWS_WEATHER_URP = "OMWS_WEATHER_URP";

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
    }
}
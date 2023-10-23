using CryingOnion.OhMy.WeatherSystem.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace CryingOnion.OhMy.WeatherSystem.EditorScript
{
    [InitializeOnLoad]
    public class AddOMWSModules : Editor
    {
        public AddOMWSModules()
        {
            List<Type> listOfMods = (
                from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in domainAssembly.GetTypes()
                where typeof(OMWSModule).IsAssignableFrom(type) && type != typeof(OMWSModule)
                select type).ToList();

            listOfMods.Remove(typeof(OMWSModule));
        }
    }
}
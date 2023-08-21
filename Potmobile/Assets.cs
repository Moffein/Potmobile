using System;
using System.Reflection;
using UnityEngine;

namespace Potmobile
{
    public class Assets
    {
        public static AssetBundle assetBundle;

        internal static string assemblyDir
        {
            get
            {
                return System.IO.Path.GetDirectoryName(PotmobilePlugin.pluginInfo.Location);
            }
        }

        public static void Init()
        {
            if (assetBundle) return;

            assetBundle = AssetBundle.LoadFromFile(Files.GetPathToFile("potmobilebundle"));
        }
    }
}

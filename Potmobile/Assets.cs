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
                return System.IO.Path.GetDirectoryName(Potmobile.pluginInfo.Location);
            }
        }

        public static void Init()
        {
            if (assetBundle) return;
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Potmobile.potmobilebundle"))
            {
                assetBundle = AssetBundle.LoadFromStream(stream);
            }
        }
    }
}

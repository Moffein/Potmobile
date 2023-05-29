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

            using (var bankStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Potmobile.MoffeinPotmobileSoundbank.bnk"))
            {
                var bytes = new byte[bankStream.Length];
                bankStream.Read(bytes, 0, bytes.Length);
                R2API.SoundAPI.SoundBanks.Add(bytes);
            }
        }
    }
}

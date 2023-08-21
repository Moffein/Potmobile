using System.IO;
using BepInEx;

namespace Potmobile
{
    internal static class Files
    {

        //public static SubFileSystem fileSystem;

        public static PluginInfo PluginInfo;

        internal static string assemblyDir
        {
            get
            {
                return System.IO.Path.GetDirectoryName(PluginInfo.Location);
            }
        }

        internal static void Init(PluginInfo info)
        {
            PluginInfo = info;
        }

        internal static string GetPathToFile(string folderName, string fileName)
        {
            return Path.Combine(assemblyDir, folderName, fileName);
        }


        internal static string GetPathToFile(string fileName)
        {
            return Path.Combine(assemblyDir, fileName);
        }
    }
}

using R2API;
using System.IO;

namespace Potmobile
{
    internal static class SoundBanks
    {
        private static bool initialized = false;

        public static void Init()
        {
            if (initialized) return;
            initialized = true;

            using (Stream manifestResourceStream = new FileStream(Files.assemblyDir + "\\MoffeinPotmobileSoundbank.bnk", FileMode.Open))
            {

                byte[] array = new byte[manifestResourceStream.Length];
                manifestResourceStream.Read(array, 0, array.Length);
                SoundAPI.SoundBanks.Add(array);
            }
        }
    }
}

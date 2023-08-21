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
            AKRESULT akResult = AkSoundEngine.AddBasePath(Files.assemblyDir);

            AkSoundEngine.LoadBank("MoffeinPotmobileSoundbank.bnk", out _);
        }
    }
}

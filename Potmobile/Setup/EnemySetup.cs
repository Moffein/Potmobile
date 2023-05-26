using R2API;
using RoR2;

namespace Potmobile
{
    public static class EnemySetup
    {
        public static bool nerfEnemy = true;

        private static bool initialized = true;

        public static void Init()
        {
            if (initialized) return;
            initialized = true;

            if (nerfEnemy)
            {
                RecalculateStatsAPI.GetStatCoefficients += (sender, args) =>
                {
                    if (sender.bodyIndex == PotmobileContent.PotmobileBodyIndex)
                    {
                        if (!sender.isPlayerControlled && sender.teamComponent && sender.teamComponent.teamIndex != TeamIndex.Player)
                        {
                            float levelBonus = sender.level - 1f;
                            args.baseRegenAdd -= sender.baseRegen + sender.levelRegen * levelBonus;
                            args.baseDamageAdd -= 0.7f * (sender.baseDamage + sender.levelDamage * levelBonus);
                        }
                    }
                };
            }
        }
    }
}

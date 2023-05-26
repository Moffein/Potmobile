using R2API;
using RoR2;
using RoR2.Navigation;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Potmobile
{
    public static class EnemySetup
    {
        public static bool nerfEnemy = true;

        private static bool initialized = false;
        private static bool setSpawns = false;

        public static bool enableEnemy = false;
        public static bool enableDissonance = true;

        public static void Init()
        {
            if (initialized) return;
            initialized = true;

            BuildCards();
            NerfEnemy();
        }

        private static void NerfEnemy()
        {
            RecalculateStatsAPI.GetStatCoefficients += (sender, args) =>
            {
                if (sender.bodyIndex == PotmobileContent.PotmobileBodyIndex && nerfEnemy)
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

        private static void BuildCards()
        {
            CharacterSpawnCard potCSC = ScriptableObject.CreateInstance<CharacterSpawnCard>();
            potCSC.name = "cscPotmobile";
            potCSC.prefab = PotmobileContent.PotmobileMasterObject;
            potCSC.sendOverNetwork = true;
            potCSC.hullSize = HullClassification.Golem;
            potCSC.nodeGraphType = MapNodeGroup.GraphType.Ground;
            potCSC.requiredFlags = NodeFlags.None;
            potCSC.forbiddenFlags = NodeFlags.NoCharacterSpawn;
            potCSC.directorCreditCost = 80;
            potCSC.occupyPosition = false;
            potCSC.loadout = new SerializableLoadout();
            potCSC.noElites = false;
            potCSC.forbiddenAsBoss = false;

            DirectorCard potDC = new DirectorCard
            {
                spawnCard = potCSC,
                selectionWeight = 1,
                preventOverhead = false,
                minimumStageCompletions = 0,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };
            DirectorAPI.DirectorCardHolder potCard = new DirectorAPI.DirectorCardHolder
            {
                Card = potDC,
                MonsterCategory = DirectorAPI.MonsterCategory.Minibosses
            };

            DirectorCard potLoopDC = new DirectorCard
            {
                spawnCard = potCSC,
                selectionWeight = 1,
                preventOverhead = false,
                minimumStageCompletions = 5,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };
            DirectorAPI.DirectorCardHolder potLoopCard = new DirectorAPI.DirectorCardHolder
            {
                Card = potLoopDC,
                MonsterCategory = DirectorAPI.MonsterCategory.Minibosses
            };

            PotmobileContent.PotmobileCard = potCard;
            PotmobileContent.PotmobileLoopCard = potLoopCard;
        }

        public static void SetSpawns()
        {
            if (!enableEnemy) return;
            if (setSpawns) return;
            setSpawns = true;

            if (enableDissonance)
            {
                DirectorCardCategorySelection dissonanceSpawns = Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/Base/MixEnemy/dccsMixEnemy.asset").WaitForCompletion();
                int categoryIndex = FindCategoryIndexByName(dissonanceSpawns, "Minibosses");
                if (categoryIndex >= 0)
                {
                    dissonanceSpawns.AddCard(categoryIndex, PotmobileContent.PotmobileCard.Card);
                }
            }

            foreach (Potmobile.StageSpawnInfo ssi in Potmobile.StageList)
            {
                DirectorAPI.DirectorCardHolder toAdd = ssi.GetMinStages() == 0 ? PotmobileContent.PotmobileCard : PotmobileContent.PotmobileLoopCard;

                SceneDef sd = ScriptableObject.CreateInstance<SceneDef>();
                sd.baseSceneNameOverride = ssi.GetStageName();

                DirectorAPI.Helpers.AddNewMonsterToStage(toAdd, false, DirectorAPI.GetStageEnumFromSceneDef(sd), ssi.GetStageName());
            }
        }

        //Minibosses
        //Basic Monsters
        //Champions
        public static int FindCategoryIndexByName(DirectorCardCategorySelection dcs, string categoryName)
        {
            for (int i = 0; i < dcs.categories.Length; i++)
            {
                if (string.CompareOrdinal(dcs.categories[i].name, categoryName) == 0)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}

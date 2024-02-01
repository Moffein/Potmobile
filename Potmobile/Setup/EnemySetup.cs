using R2API;
using RoR2;
using RoR2.Navigation;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Potmobile
{
    public static class EnemySetup
    {
        public static bool nerfPotmobile = true;
        public static bool nerfHauler = true;

        private static bool initialized = false;
        private static bool setSpawns = false;

        public static bool enableEnemy = false;
        public static bool enableDissonance = true;

        public static int potmobileCost = 80;
        public static int haulerCost = 80;

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
                if ((sender.bodyIndex == PotmobileContent.PotmobileBodyIndex && nerfPotmobile) || (sender.bodyIndex == PotmobileContent.HaulerBodyIndex && nerfHauler))
                {
                    if (!sender.isPlayerControlled && sender.teamComponent && sender.teamComponent.teamIndex != TeamIndex.Player)
                    {
                        float levelBonus = sender.level - 1f;
                        args.baseRegenAdd -= sender.baseRegen + sender.levelRegen * levelBonus;
                        args.baseDamageAdd -= 0.8f * (sender.baseDamage + sender.levelDamage * levelBonus);
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
            potCSC.directorCreditCost = potmobileCost;
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

            CharacterSpawnCard haulCSC = ScriptableObject.CreateInstance<CharacterSpawnCard>();
            haulCSC.name = "cscHauler";
            haulCSC.prefab = PotmobileContent.HaulerMasterObject;
            haulCSC.sendOverNetwork = true;
            haulCSC.hullSize = HullClassification.Golem;
            haulCSC.nodeGraphType = MapNodeGroup.GraphType.Ground;
            haulCSC.requiredFlags = NodeFlags.None;
            haulCSC.forbiddenFlags = NodeFlags.NoCharacterSpawn;
            haulCSC.directorCreditCost = potmobileCost;
            haulCSC.occupyPosition = false;
            haulCSC.loadout = new SerializableLoadout();
            haulCSC.noElites = false;
            haulCSC.forbiddenAsBoss = false;

            DirectorCard haulDC = new DirectorCard
            {
                spawnCard = haulCSC,
                selectionWeight = 1,
                preventOverhead = false,
                minimumStageCompletions = 0,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };
            DirectorAPI.DirectorCardHolder haulCard = new DirectorAPI.DirectorCardHolder
            {
                Card = haulDC,
                MonsterCategory = DirectorAPI.MonsterCategory.Minibosses
            };

            DirectorCard haulLoopDC = new DirectorCard
            {
                spawnCard = haulCSC,
                selectionWeight = 1,
                preventOverhead = false,
                minimumStageCompletions = 5,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };
            DirectorAPI.DirectorCardHolder haulLoopCard = new DirectorAPI.DirectorCardHolder
            {
                Card = haulLoopDC,
                MonsterCategory = DirectorAPI.MonsterCategory.Minibosses
            };

            PotmobileContent.HaulerCard = haulCard;
            PotmobileContent.HaulerLoopCard = haulLoopCard;
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

            foreach (PotmobilePlugin.StageSpawnInfo ssi in PotmobilePlugin.StageListPotmobile)
            {
                DirectorAPI.DirectorCardHolder toAdd = ssi.GetMinStages() == 0 ? PotmobileContent.PotmobileCard : PotmobileContent.PotmobileLoopCard;

                SceneDef sd = ScriptableObject.CreateInstance<SceneDef>();
                sd.baseSceneNameOverride = ssi.GetStageName();

                DirectorAPI.Helpers.AddNewMonsterToStage(toAdd, false, DirectorAPI.GetStageEnumFromSceneDef(sd), ssi.GetStageName());
            }

            foreach (PotmobilePlugin.StageSpawnInfo ssi in PotmobilePlugin.StageListHauler)
            {
                DirectorAPI.DirectorCardHolder toAdd = ssi.GetMinStages() == 0 ? PotmobileContent.HaulerCard : PotmobileContent.HaulerLoopCard;

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

using BepInEx;
using BepInEx.Configuration;
using EntityStates;
using Potmobile.Components;
using R2API;
using R2API.Utils;
using RoR2;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace Potmobile
{
    [BepInDependency("com.bepis.r2api")]
    [BepInDependency("com.bepis.r2api.prefab")]
    [BepInDependency("com.bepis.r2api.director")]
    [BepInDependency("com.bepis.r2api.damagetype")]
    [BepInDependency("com.bepis.r2api.recalculatestats")]
    [BepInPlugin("com.Moffein.Potmobile", "Potmobile", "1.0.0")]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    public class Potmobile : BaseUnityPlugin
    {
        public static PluginInfo pluginInfo;
        public static float sortPosition = 9999f;
        public static string stages = string.Empty;
        public static List<StageSpawnInfo> StageList = new List<StageSpawnInfo>();
        public static bool fixJumpPad = true;

        public void Awake()
        {
            pluginInfo = Info;
            ReadConfig();
            Tokens.Init();
            DamageTypeSetup.Init();
            BuildBodyObject();
            CreateSurvivorDef();
            SkillSetup.Init();
            MasterSetup.Init();
            EnemySetup.Init();
            FixJumpPad();
            RoR2.ContentManagement.ContentManager.collectContentPackProviders += ContentManager_collectContentPackProviders;
            RoR2.RoR2Application.onLoad += LateSetup;
        }

        private void ReadConfig()
        {
            fixJumpPad = base.Config.Bind<bool>(new ConfigDefinition("General", "Fix Jump Pads"), true, new ConfigDescription("Fixes Potmobiles ignoring jump pads.")).Value;

            sortPosition = base.Config.Bind<float>(new ConfigDefinition("Survivor", "Sort Position"), 9999f, new ConfigDescription("Position in the Survivor Select menu.")).Value;
            GiveItemsOnSpawn.giveVase = base.Config.Bind<bool>(new ConfigDefinition("Survivor", "Start with Vase"), false, new ConfigDescription("Gives an Eccentric Vase if your equipment slot is empty so that you can skip platforming sections.")).Value;

            EnemySetup.enableEnemy = base.Config.Bind<bool>(new ConfigDefinition("Enemy", "Enable"), false, new ConfigDescription("Adds Potmobiles to the enemy spawn pool.")).Value;
            EnemySetup.enableDissonance = base.Config.Bind<bool>(new ConfigDefinition("Enemy", "Dissonance"), true, new ConfigDescription("Adds Potmobiles to the Dissonance spawn pool if the enemy is enabled.")).Value;
            EnemySetup.nerfEnemy = base.Config.Bind<bool>(new ConfigDefinition("Enemy", "Nerf Enemy"), true, new ConfigDescription("Nerfs NPC Potmobiles so they don't instakill you.")).Value;
            stages = base.Config.Bind<string>(new ConfigDefinition("Enemy", "Stage List"), "golemplains - loop, itgolemplains, goolake, itgoolake, frozenwall, itfrozenwall, snowyforest - loop, goldshores, drybasin, forgottenhaven", new ConfigDescription("What stages the monster will show up on. Add a '- loop' after the stagename to make it only spawn after looping. List of stage names can be found at https://github.com/risk-of-thunder/R2Wiki/wiki/List-of-scene-names")).Value;

            //parse stage
            stages = new string(stages.ToCharArray().Where(c => !System.Char.IsWhiteSpace(c)).ToArray());
            string[] splitStages = stages.Split(',');
            foreach (string str in splitStages)
            {
                string[] current = str.Split('-');

                string name = current[0];
                int minStages = 0;
                if (current.Length > 1)
                {
                    minStages = 5;
                }

                StageList.Add(new StageSpawnInfo(name, minStages));
            }
        }

        private void LateSetup()
        {
            PotmobileContent.PotmobileBodyIndex = BodyCatalog.FindBodyIndex("MoffeinPotmobileBody");
            EnemySetup.SetSpawns(); //Run this here after all the custom stages have been loaded.
        }

        private void BuildBodyObject()
        {
            if (PotmobileContent.PotmobileBodyObject) return;

            GameObject bodyObject = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/PotMobile/PotMobileBody.prefab").WaitForCompletion(), "MoffeinPotmobileBody", true);

            bodyObject.AddComponent<SpeedController>(); //Allows it to benefit from move speed
            bodyObject.AddComponent<EquipmentSlot>();   //Fixes Equipment not working.
            bodyObject.AddComponent<GiveItemsOnSpawn>();   //Prevents AI Potmobiles from spawning in the ground and instantly dying

            //Fix interactor
            Interactor interactor = bodyObject.AddComponent<Interactor>();
            interactor.maxInteractionDistance = 6f;

            InteractionDriver id = bodyObject.AddComponent<InteractionDriver>();
            id.highlightInteractor = true;

            //Fix Out of Bounds teleport
            bodyObject.layer = 0;

            CharacterBody cb = bodyObject.GetComponent<CharacterBody>();
            cb.bodyColor = new Color32(220, 220, 200, 255);
            cb.name = "MoffeinPotmobileBody";
            cb.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
            cb.bodyFlags |= CharacterBody.BodyFlags.Mechanical;
            cb._defaultCrosshairPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Toolbot/ToolbotGrenadeLauncherCrosshair.prefab").WaitForCompletion();

            cb.baseNameToken = "MOFFEINPOTMOBILEBODY_NAME";
            cb.subtitleNameToken = "MOFFEINPOTMOBILEBODY_SUBTITLE";
            cb.baseMaxHealth = 480f;
            cb.levelMaxHealth = 144f;
            cb.baseArmor =  0;
            cb.levelArmor = 0f;
            cb.baseRegen = 1f;
            cb.levelRegen = 0.2f;
            cb.baseDamage = 12f;
            cb.levelDamage = 2.4f;

            #region hurtbox
            HurtBox[] existingHurtboxes = bodyObject.GetComponentsInChildren<HurtBox>();
            for (int i = 0; i < existingHurtboxes.Length; i++)
            {
                Destroy(existingHurtboxes[i]);
            }
            HurtBoxGroup existingHBG = bodyObject.GetComponentInChildren<HurtBoxGroup>();
            if (existingHBG) Destroy(existingHBG);

            ModelLocator modelLocator = bodyObject.GetComponent<ModelLocator>();
            modelLocator.modelTransform.gameObject.layer = LayerIndex.entityPrecise.intVal;

            GameObject hbObject = modelLocator.modelTransform.gameObject;
            BoxCollider bc = hbObject.AddComponent<BoxCollider>();
            bc.center = new Vector3(0f, 0f, 0f);
            bc.size = new Vector3(3.8f, 1.1f, 3.8f);
            HurtBoxGroup goHurtBoxGroup = hbObject.AddComponent<HurtBoxGroup>();

            HurtBox goHurtBox = hbObject.AddComponent<HurtBox>();
            goHurtBox.isBullseye = true;
            goHurtBox.isSniperTarget = true;
            goHurtBox.healthComponent = bodyObject.GetComponent<HealthComponent>();
            goHurtBox.damageModifier = HurtBox.DamageModifier.Normal;
            goHurtBox.hurtBoxGroup = goHurtBoxGroup;
            goHurtBox.indexInGroup = 0;

            HurtBox[] goHurtBoxArray = new HurtBox[]
            {
                goHurtBox
            };

            goHurtBoxGroup.bullseyeCount = 1;
            goHurtBoxGroup.hurtBoxes = goHurtBoxArray;
            goHurtBoxGroup.mainHurtBox = goHurtBox;
            #endregion

            #region statemachines
            NetworkStateMachine nsm = bodyObject.GetComponent<NetworkStateMachine>();
            if (!nsm)
            {
                nsm = bodyObject.AddComponent<NetworkStateMachine>();
                nsm.stateMachines = bodyObject.GetComponents<EntityStateMachine>();
            }

            EntityStateMachine boostMachine = bodyObject.AddComponent<EntityStateMachine>();
            boostMachine.customName = "Boost";
            boostMachine.initialStateType = new SerializableEntityStateType(typeof(EntityStates.Idle));
            boostMachine.mainStateType = new SerializableEntityStateType(typeof(EntityStates.Idle));
            nsm.stateMachines.Append(boostMachine).ToArray();
            #endregion

            //Fix freeze nullref spam
            SetStateOnHurt ssoh = bodyObject.AddComponent<SetStateOnHurt>();
            ssoh.canBeFrozen = true;
            ssoh.canBeHitStunned = false;
            ssoh.canBeStunned = false;
            ssoh.idleStateMachine = new EntityStateMachine[] { EntityStateMachine.FindByCustomName(bodyObject, "Weapon"), boostMachine };
            ssoh.targetStateMachine = EntityStateMachine.FindByCustomName(bodyObject, "Body");

            //Fix Potmobiles living after death
            PotmobileContent.entityStates.Add(typeof(EntityStates.MoffeinPotmobile.PotmobileDeath));
            CharacterDeathBehavior cdb = bodyObject.AddComponent<CharacterDeathBehavior>();
            cdb.deathState = new SerializableEntityStateType(typeof(EntityStates.MoffeinPotmobile.PotmobileDeath));
            cdb.deathStateMachine = ssoh.targetStateMachine;
            cdb.idleStateMachine = ssoh.idleStateMachine;

            PotmobileContent.PotmobileBodyObject = bodyObject;
        }

        private void CreateSurvivorDef()
        {
            GameObject displayObject = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/PotMobile/PotMobileBody.prefab").WaitForCompletion(), "MoffeinPotmobileDisplay", false);
            displayObject = displayObject.GetComponent<ModelLocator>().modelTransform.gameObject;
            displayObject.transform.localScale *= 0.5f;

            SurvivorDef sd = ScriptableObject.CreateInstance<SurvivorDef>();
            sd.cachedName = "MoffeinPotmobile";
            sd.bodyPrefab = PotmobileContent.PotmobileBodyObject;
            sd.hidden = false;
            sd.desiredSortPosition = sortPosition;
            sd.descriptionToken = "MOFFEINPOTMOBILEBODY_DESCRIPTION";
            sd.displayPrefab = displayObject;
            sd.mainEndingEscapeFailureFlavorToken = "MOFFEINPOTMOBILEBODY_MAIN_ENDING_ESCAPE_FAILURE_FLAVOR";
            sd.outroFlavorToken = "MOFFEINPOTMOBILEBODY_OUTRO_FLAVOR";
            (sd as ScriptableObject).name = sd.cachedName;
            PotmobileContent.PotmobileSurvivorDef = sd;
        }

        private void FixJumpPad()
        {
            if (!fixJumpPad) return;

            On.RoR2.JumpVolume.OnTriggerStay += JumpPadFix;
        }

        private void JumpPadFix(On.RoR2.JumpVolume.orig_OnTriggerStay orig, JumpVolume self, Collider other)
        {
            orig(self, other);

            if (other.gameObject)
            {
                SpeedController sc = other.gameObject.GetComponent<SpeedController>();
                if (sc)
                {
                    if (sc.rigidbody && RoR2.Util.HasEffectiveAuthority(other.gameObject))
                    {
                        Util.PlaySound(self.jumpSoundString, self.gameObject);
                        sc.rigidbody.velocity = 1.15f * self.jumpVelocity;   //undershoots moon jump pad without multiplier
                    }
                }
            }
        }

        private void ContentManager_collectContentPackProviders(RoR2.ContentManagement.ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(new PotmobileContent());
        }

        public class StageSpawnInfo
        {
            private string stageName;
            private int minStages;

            public StageSpawnInfo(string stageName, int minStages)
            {
                this.stageName = stageName;
                this.minStages = minStages;
            }

            public string GetStageName() { return stageName; }
            public int GetMinStages() { return minStages; }
        }
    }
}

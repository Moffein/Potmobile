using BepInEx;
using BepInEx.Configuration;
using EntityStates;
using Potmobile.Components;
using R2API;
using R2API.Utils;
using RiskOfOptions;
using RoR2;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Potmobile
{
    [BepInDependency("com.bepis.r2api")]
    [BepInDependency("com.bepis.r2api.prefab")]
    [BepInDependency("com.bepis.r2api.sound")]
    [BepInDependency("com.bepis.r2api.director")]
    [BepInDependency("com.bepis.r2api.damagetype")]
    [BepInDependency("com.bepis.r2api.loadout")]
    [BepInDependency("com.bepis.r2api.recalculatestats")]
    [BepInDependency("com.DestroyedClone.AncientScepter", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.ThinkInvisible.ClassicItems", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin("com.Moffein.Potmobile", "Potmobile", "1.2.0")]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    public class Potmobile : BaseUnityPlugin
    {
        public static bool classicItemsLoaded = false;
        public static bool scepterPluginLoaded = false;
        public static bool riskOfOptionsLoaded = false;

        public static PluginInfo pluginInfo;
        public static float potSortPosition = 9999f;
        public static float haulSortPosition = 10000f;
        public static string stagesPotmobile = string.Empty;
        public static string stagesHauler = string.Empty;
        public static List<StageSpawnInfo> StageListPotmobile = new List<StageSpawnInfo>();
        public static List<StageSpawnInfo> StageListHauler = new List<StageSpawnInfo>();
        public static bool fixJumpPad = true;

        public static float potmobileImpactMult, haulerImpactMult;

        public static bool ramEnabled = true;
        public static bool ramDisableOnEnemies = true;
        public static bool ramDisableAgainstPlayerPotmobiles = true;

        public static int secondaryStocks, utilityStocks, specialStocks;
        public static float secondaryCooldown, utilityCooldown, specialCooldown, primaryRadius;

        public static ConfigEntry<KeyboardShortcut> honkButton;

        public void Awake()
        {
            scepterPluginLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.DestroyedClone.AncientScepter");
            riskOfOptionsLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions");
            classicItemsLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.ThinkInvisible.ClassicItems");

            pluginInfo = Info;
            Assets.Init();
            ReadConfig();
            Tokens.Init();
            DamageTypeSetup.Init();

            //Potmobile
            BuildPotmobileBodyObject();
            Skins.InitSkins(PotmobileContent.PotmobileBodyObject);
            CreatePotmobileSurvivorDef();
            SkillSetup.Init();
            MasterSetup.CreatePotmobileMaster();

            //Hauler
            BuildHaulerBodyObject();
            Skins.InitSkins(PotmobileContent.HaulerBodyObject); //Has no CharacterModel, likely incompatible with skins.
            CreateHaulerSurvivorDef();
            HaulerSkillSetup();
            MasterSetup.CreateHaulerMaster();

            FixJumpPad();
            EnemySetup.Init();

            RoR2.ContentManagement.ContentManager.collectContentPackProviders += ContentManager_collectContentPackProviders;
            RoR2.RoR2Application.onLoad += LateSetup;
        }

        private void ReadConfig()
        {
            fixJumpPad = base.Config.Bind<bool>(new ConfigDefinition("General", "Fix Jump Pads"), true, new ConfigDescription("Fixes Potmobiles ignoring jump pads.")).Value;
            SpeedController.allowReverse = base.Config.Bind<bool>(new ConfigDefinition("General", "Allow Reverse"), true, new ConfigDescription("Allow Potmobiles to reverse.")).Value;
            honkButton = base.Config.Bind<KeyboardShortcut>(new ConfigDefinition("General", "Honk Button"), KeyboardShortcut.Empty, new ConfigDescription("Button to honk."));
            potmobileImpactMult = base.Config.Bind<float>(new ConfigDefinition("General", "Impact Force Multiplier (Potmobile)"), 1f, new ConfigDescription("Affects knockback force when ramming things.")).Value;
            haulerImpactMult = base.Config.Bind<float>(new ConfigDefinition("General", "Impact Force Multiplier (Hauler)"), 1f, new ConfigDescription("Affects knockback force when ramming things.")).Value;

            potSortPosition = base.Config.Bind<float>(new ConfigDefinition("Survivor", "Sort Position (Potmobile)"), 9999f, new ConfigDescription("Position of Potmobile in the Survivor Select menu.")).Value;
            haulSortPosition = base.Config.Bind<float>(new ConfigDefinition("Survivor", "Sort Position (Hauler)"), 10000f, new ConfigDescription("Position of Hauler in the Survivor Select menu.")).Value;
            GiveItemsOnSpawn.giveVase = base.Config.Bind<bool>(new ConfigDefinition("Survivor", "Start with Vase"), false, new ConfigDescription("Gives an Eccentric Vase if your equipment slot is empty so that you can skip platforming sections.")).Value;

            ramEnabled = base.Config.Bind<bool>(new ConfigDefinition("Ramming", "Enabled"),true, new ConfigDescription("Ramming enemies deals damage.")).Value;
            ramDisableOnEnemies = base.Config.Bind<bool>(new ConfigDefinition("Ramming", "Disable on Enemies"), true, new ConfigDescription("Enemy Potmobiles and Haulers dont deal ram damage.")).Value;
            ramDisableAgainstPlayerPotmobiles = base.Config.Bind<bool>(new ConfigDefinition("Ramming", "Disable against Player Vehicles"), true, new ConfigDescription("Player-controlled Potmobiles and Haulers don't deal ram damage to each other.")).Value;

            EntityStates.MoffeinPotmobile.Weapon.FirePotCannon.enableICBMSynergy = base.Config.Bind<bool>(new ConfigDefinition("Stats", "Primary - ICBM Synergy"), true, new ConfigDescription("Primary is affected by ICBM.")).Value;
            EntityStates.MoffeinPotmobile.Weapon.FirePotCannon.damageCoefficient = base.Config.Bind<float>(new ConfigDefinition("Stats", "Primary - Damage Coefficient"), 10f, new ConfigDescription("How much damage this attack deals. (changes do not show up in skill description)")).Value;
            EntityStates.MoffeinPotmobile.Weapon.FirePotCannon.force = base.Config.Bind<float>(new ConfigDefinition("Stats", "Primary - Force"), 2500f, new ConfigDescription("Knockback power against enemies.")).Value;
            EntityStates.MoffeinPotmobile.Weapon.FirePotCannon.baseDuration = base.Config.Bind<float>(new ConfigDefinition("Stats", "Primary - Duration"), 1.5f, new ConfigDescription("How long it takes to fire each shot.")).Value;
            primaryRadius = base.Config.Bind<float>(new ConfigDefinition("Stats", "Primary - Blast Radius"), 8f, new ConfigDescription("Blast radius of the primary attack.")).Value;

            EntityStates.MoffeinPotmobile.Weapon.PushAll.jumpVelocity = base.Config.Bind<float>(new ConfigDefinition("Stats", "Secondary - Bounce Velocity"), 32f, new ConfigDescription("Jump power of vertical bounce.")).Value;
            EntityStates.MoffeinPotmobile.Weapon.PushAll.radius = base.Config.Bind<float>(new ConfigDefinition("Stats", "Secondary - Push Radius"), 12f, new ConfigDescription("Radius of push effect against enemies.")).Value;
            EntityStates.MoffeinPotmobile.Weapon.PushAll.force = base.Config.Bind<float>(new ConfigDefinition("Stats", "Secondary - Push Force"), 4500f, new ConfigDescription("Power of push effect.")).Value;
            secondaryStocks = base.Config.Bind<int>(new ConfigDefinition("Stats", "Secondary Stocks"), 1, new ConfigDescription("How many charges this skill has.")).Value;
            secondaryCooldown = base.Config.Bind<float>(new ConfigDefinition("Stats", "Secondary Cooldown"), 5f, new ConfigDescription("How long this skill takes to recharge.")).Value;

            EntityStates.MoffeinPotmobile.Boost.Boost.baseDuration = base.Config.Bind<float>(new ConfigDefinition("Stats", "Utility - Boost Duration"), 2.5f, new ConfigDescription("Length of Nitro Rush.")).Value;
            EntityStates.MoffeinPotmobile.Boost.Boost.forceMultiplier = base.Config.Bind<float>(new ConfigDefinition("Stats", "Utility - Speed Multiplier"), 2f, new ConfigDescription("Speed multiplier while using Nitro Rush.")).Value;
            utilityStocks = base.Config.Bind<int>(new ConfigDefinition("Stats", "Utility Stocks"), 1, new ConfigDescription("How many charges this skill has.")).Value;
            utilityCooldown = base.Config.Bind<float>(new ConfigDefinition("Stats", "Utility Cooldown"), 10f, new ConfigDescription("How long this skill takes to recharge.")).Value;

            specialStocks = base.Config.Bind<int>(new ConfigDefinition("Stats", "Special Stocks"), 1, new ConfigDescription("How many charges this skill has.")).Value;
            specialCooldown = base.Config.Bind<float>(new ConfigDefinition("Stats", "Special Cooldown"), 5f, new ConfigDescription("How long this skill takes to recharge.")).Value;

            EnemySetup.enableEnemy = base.Config.Bind<bool>(new ConfigDefinition("Enemy", "Enable"), false, new ConfigDescription("Adds Potmobiles and Haulers to the enemy spawn pool.")).Value;
            EnemySetup.enableDissonance = base.Config.Bind<bool>(new ConfigDefinition("Enemy", "Dissonance"), true, new ConfigDescription("Adds Potmobiles and Haulers to the Dissonance spawn pool if the enemy is enabled.")).Value;
            EnemySetup.potmobileCost = base.Config.Bind<int>(new ConfigDefinition("Enemy", "Director Cost (Potmobile)"), 80, new ConfigDescription("Cost of spawning a Potmobile.")).Value;
            EnemySetup.haulerCost = base.Config.Bind<int>(new ConfigDefinition("Enemy", "Director Cost (Hauler)"), 120, new ConfigDescription("Cost of spawning a Hauler.")).Value;
            EnemySetup.nerfPotmobile = base.Config.Bind<bool>(new ConfigDefinition("Enemy", "Nerf Potmobile"), true, new ConfigDescription("Nerfs NPC Potmobiles and Haulers so they don't instakill you.")).Value;
            EnemySetup.nerfHauler = base.Config.Bind<bool>(new ConfigDefinition("Enemy", "Nerf Hauler"), true, new ConfigDescription("Nerfs NPC Haulers so they don't instakill you.")).Value;
            stagesPotmobile = base.Config.Bind<string>(new ConfigDefinition("Enemy", "Stage List (Potmobile)"), "golemplains - loop, itgolemplains, goolake, itgoolake, frozenwall, itfrozenwall, snowyforest - loop, drybasin, forgottenhaven, goldshores", new ConfigDescription("What stages Potmobiles will show up on. Add a '- loop' after the stagename to make it only spawn after looping. List of stage names can be found at https://github.com/risk-of-thunder/R2Wiki/wiki/List-of-scene-names")).Value;
            stagesHauler = base.Config.Bind<string>(new ConfigDefinition("Enemy", "Stage List (Hauler)"), "golemplains - loop, itgolemplains, goolake, itgoolake, frozenwall, itfrozenwall, snowyforest - loop, drybasin, forgottenhaven, goldshores", new ConfigDescription("What stages Haulers will show up on. Add a '- loop' after the stagename to make it only spawn after looping. List of stage names can be found at https://github.com/risk-of-thunder/R2Wiki/wiki/List-of-scene-names")).Value;

            //parse stage
            stagesPotmobile = new string(stagesPotmobile.ToCharArray().Where(c => !System.Char.IsWhiteSpace(c)).ToArray());
            string[] splitStages = stagesPotmobile.Split(',');
            foreach (string str in splitStages)
            {
                string[] current = str.Split('-');

                string name = current[0];
                int minStages = 0;
                if (current.Length > 1)
                {
                    minStages = 5;
                }

                StageListPotmobile.Add(new StageSpawnInfo(name, minStages));
            }

            stagesHauler = new string(stagesHauler.ToCharArray().Where(c => !System.Char.IsWhiteSpace(c)).ToArray());
            string[] splitStages2 = stagesHauler.Split(',');
            foreach (string str in splitStages2)
            {
                string[] current = str.Split('-');

                string name = current[0];
                int minStages = 0;
                if (current.Length > 1)
                {
                    minStages = 5;
                }

                StageListHauler.Add(new StageSpawnInfo(name, minStages));
            }

            if (riskOfOptionsLoaded)
            {
                RiskOfOptionsCompat();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void RiskOfOptionsCompat()
        {
            ModSettingsManager.SetModIcon(Assets.assetBundle.LoadAsset<Sprite>("texIconPotmobile.png"));
            ModSettingsManager.AddOption(new RiskOfOptions.Options.KeyBindOption(honkButton));
        }

        private void LateSetup()
        {
            PotmobileContent.PotmobileBodyIndex = BodyCatalog.FindBodyIndex("MoffeinPotmobileBody");
            PotmobileContent.HaulerBodyIndex = BodyCatalog.FindBodyIndex("MoffeinHaulerBody");
            EnemySetup.SetSpawns(); //Run this here after all the custom stages have been loaded.
        }

        private void BuildPotmobileBodyObject()
        {
            if (PotmobileContent.PotmobileBodyObject) return;

            GameObject bodyObject = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/PotMobile/PotMobileBody.prefab").WaitForCompletion(), "MoffeinPotmobileBody", true);

            SpeedController sc = bodyObject.AddComponent<SpeedController>(); //Allows it to benefit from move speed
            sc.minOverlapDamageCoefficient = 2.5f;
            sc.minOverlapSpeed = 10f;
            sc.doubleDamageOverlapSpeed = 20f;

            bodyObject.AddComponent<EquipmentSlot>();   //Fixes Equipment not working.
            bodyObject.AddComponent<GiveItemsOnSpawn>();   //Prevents AI Potmobiles from spawning in the ground and instantly dying
            bodyObject.AddComponent<PotmobileNetworkComponent>();   //Used to squash things

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
            cb.portraitIcon = Assets.assetBundle.LoadAsset<Texture2D>("texIconPotmobile.png");

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
            cb.sprintingSpeedMultiplier = 1f;

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
            bc.size = new Vector3(3.5f, 1f, 4f);
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

            //Doesn't work, build hitbox at runtime instead.
            //Would be simpler if this was done on the prefab in Unity, but that's not an option here.
            /*GameObject ramHitbox = new GameObject();
            ramHitbox.transform.parent = bodyObject.transform;
            ramHitbox.transform.localScale = bc.size * 1.5f;
            ramHitbox.name = "RamHitbox";
            SetupHitbox(bodyObject, "RamHitbox", new Transform[] { ramHitbox.transform });*/
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

            VehicleForceZone vfz = bodyObject.GetComponentInChildren<VehicleForceZone>();
            if (potmobileImpactMult <= 0f)
            {
                Destroy(vfz);
            }
            else
            {
                vfz.impactMultiplier = potmobileImpactMult;
            }

            PotmobileContent.PotmobileBodyObject = bodyObject;
        }
        private void CreatePotmobileSurvivorDef()
        {
            GameObject displayObject = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/PotMobile/PotMobileBody.prefab").WaitForCompletion(), "MoffeinPotmobileDisplay", false);
            displayObject = displayObject.GetComponent<ModelLocator>().modelTransform.gameObject;
            displayObject.transform.localScale *= 0.5f;

            SurvivorDef sd = ScriptableObject.CreateInstance<SurvivorDef>();
            sd.cachedName = "MoffeinPotmobile";
            sd.bodyPrefab = PotmobileContent.PotmobileBodyObject;
            sd.hidden = false;
            sd.desiredSortPosition = potSortPosition;
            sd.descriptionToken = "MOFFEINPOTMOBILEBODY_DESCRIPTION";
            sd.displayPrefab = displayObject;
            sd.mainEndingEscapeFailureFlavorToken = "MOFFEINPOTMOBILEBODY_MAIN_ENDING_ESCAPE_FAILURE_FLAVOR";
            sd.outroFlavorToken = "MOFFEINPOTMOBILEBODY_OUTRO_FLAVOR";
            (sd as ScriptableObject).name = sd.cachedName;
            PotmobileContent.PotmobileSurvivorDef = sd;
        }

        private void BuildHaulerBodyObject()
        {
            if (PotmobileContent.HaulerBodyObject) return;
            GameObject bodyObject = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/Hauler/HaulerBody.prefab").WaitForCompletion(), "MoffeinHaulerBody", true);

            SpeedController sc = bodyObject.AddComponent<SpeedController>(); //Allows it to benefit from move speed
            sc.minOverlapDamageCoefficient = 4.5f;
            sc.minOverlapSpeed = 10f;
            sc.doubleDamageOverlapSpeed = 20f;

            bodyObject.AddComponent<EquipmentSlot>();   //Fixes Equipment not working.
            bodyObject.AddComponent<GiveItemsOnSpawn>();   //Prevents AI Potmobiles from spawning in the ground and instantly dying
            bodyObject.AddComponent<PotmobileNetworkComponent>();   //Used to squash things

            //Fix interactor
            Interactor interactor = bodyObject.AddComponent<Interactor>();
            interactor.maxInteractionDistance = 10f;

            InteractionDriver id = bodyObject.AddComponent<InteractionDriver>();
            id.highlightInteractor = true;

            //Fix Out of Bounds teleport
            bodyObject.layer = 0;

            //Add charactermodel. Does this do anything?
            CharacterModel characterModel = bodyObject.AddComponent<CharacterModel>();
            characterModel.baseRendererInfos = bodyObject.GetComponentInChildren<CharacterModel>().baseRendererInfos;

            CharacterBody cb = bodyObject.GetComponent<CharacterBody>();
            cb.bodyColor = new Color32(34, 71, 224, 255);
            cb.name = "MoffeinHaulerBody";
            cb.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
            cb.bodyFlags |= CharacterBody.BodyFlags.Mechanical;
            cb._defaultCrosshairPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Toolbot/ToolbotGrenadeLauncherCrosshair.prefab").WaitForCompletion();
            cb.portraitIcon = Assets.assetBundle.LoadAsset<Texture2D>("texIconHauler.png");

            cb.baseNameToken = "MOFFEINHAULERBODY_NAME";
            cb.subtitleNameToken = "MOFFEINHAULERBODY_SUBTITLE";
            cb.baseMaxHealth = 600f;
            cb.levelMaxHealth = 185f;
            cb.baseArmor = 20;
            cb.levelArmor = 0f;
            cb.baseRegen = 1f;
            cb.levelRegen = 0.2f;
            cb.baseDamage = 12f;
            cb.levelDamage = 2.4f;
            cb.sprintingSpeedMultiplier = 1f;

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
            bc.center = new Vector3(0f, 1f, 0f);
            bc.size = new Vector3(5.5f, 3f, 10f);
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

            //Doesn't work, build hitbox at runtime instead.
            //Would be simpler if this was done on the prefab in Unity, but that's not an option here.
            /*GameObject ramHitbox = new GameObject();
            ramHitbox.transform.parent = bodyObject.transform;
            ramHitbox.transform.localScale = bc.size * 1.5f;
            ramHitbox.name = "RamHitbox";
            SetupHitbox(bodyObject, "RamHitbox", new Transform[] { ramHitbox.transform });*/

            #region statemachines
            NetworkStateMachine nsm = bodyObject.GetComponent<NetworkStateMachine>();
            if (!nsm)
            {
                nsm = bodyObject.AddComponent<NetworkStateMachine>();
                nsm.stateMachines = bodyObject.GetComponents<EntityStateMachine>();
            }

            EntityStateMachine weaponMachine = bodyObject.AddComponent<EntityStateMachine>();
            weaponMachine.customName = "Weapon";
            weaponMachine.initialStateType = new SerializableEntityStateType(typeof(EntityStates.Idle));
            weaponMachine.mainStateType = new SerializableEntityStateType(typeof(EntityStates.Idle));
            nsm.stateMachines.Append(weaponMachine).ToArray();

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
            ssoh.idleStateMachine = new EntityStateMachine[] { weaponMachine, boostMachine };
            ssoh.targetStateMachine = EntityStateMachine.FindByCustomName(bodyObject, "Body");

            //Fix Potmobiles living after death
            CharacterDeathBehavior cdb = bodyObject.AddComponent<CharacterDeathBehavior>();
            cdb.deathState = new SerializableEntityStateType(typeof(EntityStates.MoffeinPotmobile.PotmobileDeath));
            cdb.deathStateMachine = ssoh.targetStateMachine;
            cdb.idleStateMachine = ssoh.idleStateMachine;

            VehicleForceZone vfz = bodyObject.GetComponentInChildren<VehicleForceZone>();
            if (haulerImpactMult <= 0f)
            {
                Destroy(vfz);
            }
            else
            {
                vfz.impactMultiplier = haulerImpactMult;
            }

            PotmobileContent.HaulerBodyObject = bodyObject;
        }

        private void CreateHaulerSurvivorDef()
        {
            GameObject displayObject = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/Hauler/HaulerBody.prefab").WaitForCompletion(), "MoffeinHaulerDisplay", false);
            displayObject = displayObject.GetComponent<ModelLocator>().modelTransform.gameObject;
            displayObject.transform.localScale *= 0.25f;

            SurvivorDef sd = ScriptableObject.CreateInstance<SurvivorDef>();
            sd.cachedName = "MoffeinHauler";
            sd.bodyPrefab = PotmobileContent.HaulerBodyObject;
            sd.hidden = false;
            sd.desiredSortPosition = haulSortPosition;
            sd.descriptionToken = "MOFFEINHAULERBODY_DESCRIPTION";
            sd.displayPrefab = displayObject;
            sd.mainEndingEscapeFailureFlavorToken = "MOFFEINHAULERBODY_MAIN_ENDING_ESCAPE_FAILURE_FLAVOR";
            sd.outroFlavorToken = "MOFFEINHAULERBODY_OUTRO_FLAVOR";
            (sd as ScriptableObject).name = sd.cachedName;
            PotmobileContent.HaulerSurvivorDef = sd;
        }

        private void HaulerSkillSetup()
        {
            SkillLocator skillLocator = PotmobileContent.HaulerBodyObject.GetComponent<SkillLocator>();
            SkillSetup.CreateSkillFamilies(PotmobileContent.HaulerBodyObject, true);
            SkillSetup.AddSkillToFamily(skillLocator.primary.skillFamily, PotmobileContent.SkillDefs.FirePotCannon);
            SkillSetup.AddSkillToFamily(skillLocator.secondary.skillFamily, PotmobileContent.SkillDefs.Push);
            SkillSetup.AddSkillToFamily(skillLocator.utility.skillFamily, PotmobileContent.SkillDefs.Boost);
            SkillSetup.AddSkillToFamily(skillLocator.special.skillFamily, PotmobileContent.SkillDefs.Reset);
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
                    if (sc.rigidbody && sc.HasEffectiveAuthority())
                    {
                        //Util.PlaySound(self.jumpSoundString, self.gameObject);    //Sound gets spammed since there's no DisableAirControl flag to tell when a jump pad has been used.
                        sc.rigidbody.velocity = 1.15f * self.jumpVelocity;   //undershoots moon jump pad without multiplier
                    }
                }
            }
        }

        private void ContentManager_collectContentPackProviders(RoR2.ContentManagement.ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(new PotmobileContent());
        }

        public static void SetupHitbox(GameObject prefab, string hitboxName, params Transform[] hitboxTransforms)
        {
            HitBoxGroup hitBoxGroup = prefab.AddComponent<HitBoxGroup>();
            List<HitBox> hitBoxes = new List<HitBox>();

            foreach (Transform i in hitboxTransforms)
            {
                HitBox hitBox = i.gameObject.AddComponent<HitBox>();
                i.gameObject.layer = LayerIndex.projectile.intVal;
                hitBoxes.Add(hitBox);
            }

            hitBoxGroup.hitBoxes = hitBoxes.ToArray();

            hitBoxGroup.groupName = hitboxName;
        }

        //Taken from https://github.com/ToastedOven/CustomEmotesAPI/blob/main/CustomEmotesAPI/CustomEmotesAPI/CustomEmotesAPI.cs
        public static bool GetKeyPressed(ConfigEntry<KeyboardShortcut> entry)
        {
            foreach (var item in entry.Value.Modifiers)
            {
                if (!Input.GetKey(item))
                {
                    return false;
                }
            }
            return Input.GetKeyDown(entry.Value.MainKey);
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

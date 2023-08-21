using BepInEx;
using BepInEx.Configuration;
using EntityStates;
using Potmobile.Components;
using Potmobile.Cores;
using Potmobile.Survivors.Hauler;
using Potmobile.Survivors.Potmobile;
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
    [BepInDependency("com.DestroyedClone.AncientScepter", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.ThinkInvisible.ClassicItems", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Mico27.RideMeExtended", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin("com.Moffein.Potmobile", "Potmobile", "1.4.0")]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [R2API.Utils.R2APISubmoduleDependency(nameof(RecalculateStatsAPI), nameof(PrefabAPI), nameof(DamageAPI), nameof(SoundAPI), nameof(LoadoutAPI), nameof(DirectorAPI))]
    public class PotmobilePlugin : BaseUnityPlugin
    {
        public static bool classicItemsLoaded = false;
        public static bool scepterPluginLoaded = false;
        public static bool riskOfOptionsLoaded = false;
        public static bool rideMeExtendedLoaded = false;

        public static PluginInfo pluginInfo;
        public static float potSortPosition = 9999f;
        public static float haulSortPosition = 10000f;
        public static string stagesPotmobile = string.Empty;
        public static string stagesHauler = string.Empty;
        public static List<StageSpawnInfo> StageListPotmobile = new List<StageSpawnInfo>();
        public static List<StageSpawnInfo> StageListHauler = new List<StageSpawnInfo>();
        public static bool fixJumpPad = true;
        public static bool impactFriendlyFireEnabled = false;
        public static bool impactFriendlyFirePotmobileEnabled = true;
        public static float haulerForce, potmobileForce;

        public static float potmobileImpactMult, haulerImpactMult, potmobileMinDamageSpeed, haulerMinDamageSpeed, potmobileDoubleDamageSpeed, haulerDoubleDamageSpeed, potmobileMinRamDamage, haulerMinRamDamage, potmobileReverseCoefficient, haulerReverseCoefficient;

        public static bool ramEnabled = true;
        public static bool ramDisableOnEnemies = true;
        public static bool ramDisableAgainstPlayerPotmobiles = true;

        public static int secondaryStocks, utilityStocks, specialStocks;
        public static float secondaryCooldown, utilityCooldown, specialCooldown, primaryRadius;

        public static bool stridesHeresyAdjustment;

        public static ConfigEntry<KeyboardShortcut> honkButton;

        public void Awake()
        {
            Files.PluginInfo = this.Info;

            scepterPluginLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.DestroyedClone.AncientScepter");
            riskOfOptionsLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions");
            classicItemsLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.ThinkInvisible.ClassicItems");
            rideMeExtendedLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Mico27.RideMeExtended");

            pluginInfo = Info;
            Assets.Init();
            ReadConfig();
            Tokens.Init();
            DamageTypeSetup.Init();

            //Potmobile
            new PotmobileCore();

            //Hauler
            new HaulerCore();

            FixJumpPad();
            EnemySetup.Init();

            FixVehicleForceZoneFriendlyFire();

            new ItemDisplayCore();

            RoR2.ContentManagement.ContentManager.collectContentPackProviders += ContentManager_collectContentPackProviders;
            RoR2.RoR2Application.onLoad += LateSetup;
        }

        public void Start()
        {
            SoundBanks.Init();
        }

        private void ReadConfig()
        {
            fixJumpPad = base.Config.Bind<bool>(new ConfigDefinition("General", "Fix Jump Pads"), true, new ConfigDescription("Fixes Potmobiles ignoring jump pads.")).Value;
            SpeedController.allowReverse = base.Config.Bind<bool>(new ConfigDefinition("General", "Allow Reverse"), true, new ConfigDescription("Allow Potmobiles to reverse.")).Value;
            honkButton = base.Config.Bind<KeyboardShortcut>(new ConfigDefinition("General", "Honk Button"), KeyboardShortcut.Empty, new ConfigDescription("Button to honk."));
            potmobileImpactMult = base.Config.Bind<float>(new ConfigDefinition("General", "Impact Force Multiplier (Potmobile)"), 1f, new ConfigDescription("Affects knockback force when colliding with things.")).Value;
            haulerImpactMult = base.Config.Bind<float>(new ConfigDefinition("General", "Impact Force Multiplier (Hauler)"), 1f, new ConfigDescription("Affects knockback force when colliding with things.")).Value;
            impactFriendlyFireEnabled = base.Config.Bind<bool>(new ConfigDefinition("General", "Impact Friendly Fire"), false, new ConfigDescription("Colliding with teammates sends them flying.")).Value;
            impactFriendlyFirePotmobileEnabled = base.Config.Bind<bool>(new ConfigDefinition("General", "Impact Friendly Fire (Vehicles)"), true, new ConfigDescription("Colliding with teammate Potmobiles and Haulers sends them flying.")).Value;
            stridesHeresyAdjustment = base.Config.Bind(new ConfigDefinition("General", "Strides of Heresy Adjustment"), true, new ConfigDescription("Using Strides of Heresy auto aligns the body.")).Value;

            //Place these in General so config is only 1 page.
            EntityStates.MoffeinPotmobile.Boost.Reset.resetVelocity = base.Config.Bind<bool>(new ConfigDefinition("General", "Special Reset Velocity"), false, new ConfigDescription("Reset velocity to 0 when using the Special."));

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

            potmobileForce = base.Config.Bind<float>(new ConfigDefinition("Stats - Potmobile", "Motor Force"), 15000f, new ConfigDescription("Affects the speed of this vehicle.")).Value;
            potmobileMinDamageSpeed = base.Config.Bind<float>(new ConfigDefinition("Stats - Potmobile", "Min Ram Speed"), 10f, new ConfigDescription("Minimum speed to deal ram damage with this vehicle.")).Value;
            potmobileDoubleDamageSpeed = base.Config.Bind<float>(new ConfigDefinition("Stats - Potmobile", "Double Damage Ram Speed"), 20f, new ConfigDescription("Speed at which ramming damage is doubled.")).Value;
            potmobileMinRamDamage = base.Config.Bind<float>(new ConfigDefinition("Stats - Potmobile", "Min Ram Damage Coefficient"), 4.5f, new ConfigDescription("Damage dealt when ramming at minimum speed.")).Value;
            potmobileReverseCoefficient = base.Config.Bind<float>(new ConfigDefinition("Stats - Potmobile", "Reverse Speed Coefficient"), 0.8f, new ConfigDescription("Motor force is multiplied by this when reversing.")).Value;

            haulerForce = base.Config.Bind<float>(new ConfigDefinition("Stats - Hauler", "Motor Force"), 3000f, new ConfigDescription("Affects the speed of this vehicle.")).Value;
            haulerMinDamageSpeed = base.Config.Bind<float>(new ConfigDefinition("Stats - Hauler", "Min Ram Speed"), 10f, new ConfigDescription("Minimum speed to deal ram damage with this vehicle.")).Value;
            haulerDoubleDamageSpeed = base.Config.Bind<float>(new ConfigDefinition("Stats - Hauler", "Double Damage Ram Speed"), 20f, new ConfigDescription("Speed at which ramming damage is doubled.")).Value;
            haulerMinRamDamage = base.Config.Bind<float>(new ConfigDefinition("Stats - Hauler", "Min Ram Damage Coefficient"), 4.5f, new ConfigDescription("Damage dealt when ramming at minimum speed.")).Value;
            haulerReverseCoefficient = base.Config.Bind<float>(new ConfigDefinition("Stats - Hauler", "Reverse Speed Coefficient"), 0.8f, new ConfigDescription("Motor force is multiplied by this when reversing.")).Value;

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
            ModSettingsManager.AddOption(new RiskOfOptions.Options.CheckBoxOption(EntityStates.MoffeinPotmobile.Boost.Reset.resetVelocity));
        }

        private void LateSetup()
        {
            PotmobileContent.PotmobileBodyIndex = BodyCatalog.FindBodyIndex("MoffeinPotmobileBody");
            PotmobileContent.HaulerBodyIndex = BodyCatalog.FindBodyIndex("MoffeinHaulerBody");
            EnemySetup.SetSpawns(); //Run this here after all the custom stages have been loaded.
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

        private void FixVehicleForceZoneFriendlyFire()
        {
            //If both are true, just use Vanilla behavior
            if (impactFriendlyFireEnabled && impactFriendlyFirePotmobileEnabled) return;

            On.RoR2.VehicleForceZone.OnCollisionEnter += (orig, self, collision) =>
            {
                if (collision.collider)
                {
                    CharacterBody victimBody = collision.collider.GetComponent<CharacterBody>();
                    if (victimBody)
                    {
                        CharacterBody attackerBody = self.GetComponentInParent<CharacterBody>();
                        if (attackerBody)
                        {
                            bool isFriendly = attackerBody.teamComponent && victimBody.teamComponent && attackerBody.teamComponent.teamIndex == victimBody.teamComponent.teamIndex;
                            bool isVehicle = victimBody.bodyIndex == PotmobileContent.PotmobileBodyIndex || victimBody.bodyIndex == PotmobileContent.HaulerBodyIndex;

                            if (isFriendly && ((isVehicle && !impactFriendlyFirePotmobileEnabled) || (!isVehicle && !impactFriendlyFireEnabled)))
                            {
                                return;
                            }
                        }
                    }
                }

                orig(self, collision);
            };

            On.RoR2.VehicleForceZone.OnTriggerEnter += (orig, self, other) =>
            {
                if (other)
                {
                    CharacterBody victimBody = other.GetComponent<CharacterBody>();
                    if (victimBody)
                    {
                        CharacterBody attackerBody = self.GetComponentInParent<CharacterBody>();
                        if (attackerBody)
                        {
                            bool isFriendly = attackerBody.teamComponent && victimBody.teamComponent && attackerBody.teamComponent.teamIndex == victimBody.teamComponent.teamIndex;
                            bool isVehicle = victimBody.bodyIndex == PotmobileContent.PotmobileBodyIndex || victimBody.bodyIndex == PotmobileContent.HaulerBodyIndex;

                            if (isFriendly && ((isVehicle && !impactFriendlyFirePotmobileEnabled) || (!isVehicle && !impactFriendlyFireEnabled)))
                            {
                                return;
                            }
                        }
                    }
                }

                orig(self, other);
            };
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

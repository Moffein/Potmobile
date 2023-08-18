using UnityEngine;
using RoR2;
using System.Collections.Generic;
using Potmobile.Cores;
using System.Runtime.CompilerServices;

namespace Potmobile.Survivors.Hauler
{
    internal class HaulerItemDisplays
    {
        public static ItemDisplayRuleSet itemDisplayRuleSet;

        public static List<ItemDisplayRuleSet.KeyAssetRuleGroup> itemRules;

        public static void RegisterDisplays()
        {
            GameObject bodyPrefab = PotmobileContent.HaulerBodyObject;
            GameObject model = bodyPrefab.GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
            CharacterModel characterModel = model.GetComponentInChildren<CharacterModel>();

            itemDisplayRuleSet = ScriptableObject.CreateInstance<ItemDisplayRuleSet>();
            itemRules = new List<ItemDisplayRuleSet.KeyAssetRuleGroup>();

            #region DisplayRules
            #region Vanilla
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Syringe, "DisplaySyringe", "armatureRoot", new Vector3(-1.65544F, 1.61826F, 0.47913F), new Vector3(0F, 19.70516F, 0F), new Vector3(0.20678F, 0.20678F, 0.20678F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Bear, "DisplayBear", "armatureRoot", new Vector3(2.22056F, 0F, -5.24749F), new Vector3(15.12841F, 186.6021F, 1.73018F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Behemoth, "DisplayBehemoth", "armatureRoot", new Vector3(1.18153F, -0.30867F, -5.02094F), new Vector3(88.88548F, 22.79844F, 202.8023F), new Vector3(0.12653F, 0.12653F, 0.12653F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Missile, "DisplayMissileLauncher", "armatureRoot", new Vector3(2.72169F, 3.51991F, 2.05472F), new Vector3(0F, 0F, 328.1266F), new Vector3(0.17004F, 0.17004F, 0.17004F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ExplodeOnDeath, "DisplayWilloWisp", "armatureRoot", new Vector3(-2.48619F, -0.39048F, -5.07656F), new Vector3(0F, 0F, 0F), new Vector3(0.13154F, 0.13154F, 0.13154F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Dagger, "DisplayDagger", "armatureRoot", new Vector3(1.851F, 3.24806F, 0.966F), new Vector3(0F, 0F, 0.9131F), new Vector3(3.25566F, 3.25566F, 3.25566F)));

            //IDRS NOTE: [TOOTH] = Decal is the 'string' of the necklace and its a projected decal.
            //IDRS NOTE: [TOOTH] = Order is: Small1, Small1, Large, Small2, Small2 on the character
            itemRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Tooth,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayToothNecklaceDecal"),
                            childName = "armatureRoot",
                            localPos = Vector3.zero,
                            localAngles = Vector3.zero,
                            localScale = Vector3.one,
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayToothMeshLarge"),
                            childName = "armatureRoot",
                            localPos = Vector3.zero,
                            localAngles = Vector3.zero,
                            localScale = Vector3.one,
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayToothMeshSmall1"),
                            childName = "armatureRoot",
                            localPos = Vector3.zero,
                            localAngles = Vector3.zero,
                            localScale = Vector3.one,
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayToothMeshSmall1"),
                            childName = "armatureRoot",
                            localPos = Vector3.zero,
                            localAngles = Vector3.zero,
                            localScale = Vector3.one,
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayToothMeshSmall2"),
                            childName = "armatureRoot",
                            localPos = Vector3.zero,
                            localAngles = Vector3.zero,
                            localScale = Vector3.one,
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayToothMeshSmall2"),
                            childName = "armatureRoot",
                            localPos = Vector3.zero,
                            localAngles = Vector3.zero,
                            localScale = Vector3.one,
                            limbMask = LimbFlags.None
                        },
                    }
                }
            });
            //might also be able to just do this but thats not the spirit of it
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Tooth, RoR2/Base/Tooth/mdlToothNecklaceDisplay.fbx
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.CritGlasses, "DisplayGlasses", "armatureRoot", new Vector3(0F, 0.882F, 4.93111F), new Vector3(0F, 0F, 0F), new Vector3(4.48361F, 4.48361F, 0.64829F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Hoof, "DisplayHoof", "armatureRoot", new Vector3(-1.47365F, -0.89497F, 4.11411F), new Vector3(274.3279F, 180F, 180F), new Vector3(0.28675F, 0.28675F, 0.28675F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Feather, "DisplayFeather", "armatureRoot", new Vector3(0F, 2.3279F, -2.57481F), new Vector3(271.9536F, 180F, 180F), new Vector3(0.17274F, 0.17274F, 0.03091F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ChainLightning, "DisplayUkulele", "armatureRoot", new Vector3(1.54447F, 1.84132F, -1.84835F), new Vector3(0F, 350.0657F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Seed, "DisplaySeed", "armatureRoot", new Vector3(-1.38058F, 1.48966F, -1.32882F), new Vector3(0F, 0F, 113.6799F), new Vector3(0.15863F, 0.15863F, 0.15863F)));
            itemRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(RoR2Content.Items.Icicle, "DisplayFrostRelic", Vector3.zero, Vector3.zero, Vector3.one));
            //this seems used for the ice elite? then why that asset path? 
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Icicle, "DisplayIcicle", "armatureRoot", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.GhostOnKill, "DisplayMask", "armatureRoot", new Vector3(0F, 2.80105F, 2.57173F), new Vector3(0F, 0F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Mushroom, "DisplayMushroom", "armatureRoot", new Vector3(2.083F, 3.06064F, -2.29928F), new Vector3(0F, 0F, 0F), new Vector3(0.19576F, 0.19576F, 0.19576F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Crowbar, "DisplayCrowbar", "armatureRoot", new Vector3(0F, 0.17556F, -5.22331F), new Vector3(87.32951F, 334.1944F, 64.17004F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.AttackSpeedOnCrit, "DisplayWolfPelt", "armatureRoot", new Vector3(0F, 1.57103F, 5.06986F), new Vector3(0F, 0F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BleedOnHit, "DisplayTriTip", "armatureRoot", new Vector3(2.0349F, 0F, 5.1745F), new Vector3(0F, 270.9555F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SprintOutOfCombat, "DisplayWhip", "armatureRoot", new Vector3(-2.73249F, 0.99339F, 0F), new Vector3(0F, 0F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.FallBoots, "DisplayGravBoots", "armatureRoot", new Vector3(0F, -0.41076F, 0F), new Vector3(0F, 0F, 0F), new Vector3(5.20044F, 5.88793F, 8.70718F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.WardOnLevel, "DisplayWarbanner", "armatureRoot", new Vector3(0F, 1.99027F, -5.2294F), new Vector3(270F, 270F, 0F), new Vector3(0.89045F, 2.73078F, 0.89045F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Phasing, "DisplayStealthkit", "armatureRoot", new Vector3(-1.17575F, 1.72287F, 1.55187F), new Vector3(69.74921F, 180F, 180F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.HealOnCrit, "DisplayScythe", "armatureRoot", new Vector3(-0.84072F, 1.83539F, 4.17108F), new Vector3(15.12057F, 309.139F, 354.8051F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.HealWhileSafe, "DisplaySnail", "armatureRoot", new Vector3(1.43455F, 3.02326F, 2.20732F), new Vector3(13.50017F, 0F, 0F), new Vector3(0.30245F, 0.30245F, 0.30245F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.PersonalShield, "DisplayShieldGenerator", "armatureRoot", new Vector3(0F, -0.27431F, 5.14626F), new Vector3(290.8771F, 0F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.EquipmentMagazine, "DisplayBattery", "armatureRoot", new Vector3(-2.8898F, 0F, -2.35959F), new Vector3(0F, 0F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateMirroredDisplayRule(RoR2Content.Items.NovaOnHeal, "DisplayDevilHorns", "armatureRoot", new Vector3(2.50123F, 1.45344F, 4.14543F), new Vector3(3.52188F, 85.80187F, 180.0117F), new Vector3(3.17042F, 3.17042F, 3.17042F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ShockNearby, "DisplayTeslaCoil", "armatureRoot", new Vector3(2.02542F, 3.14668F, -1.31613F), new Vector3(0F, 0F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Infusion, "DisplayInfusion", "armatureRoot", new Vector3(0.92559F, 1.65802F, -1.87595F), new Vector3(0F, 0F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Clover, "DisplayClover", "armatureRoot", new Vector3(-0.84866F, 2.0579F, 2.61868F), new Vector3(0F, 0F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Medkit, "DisplayMedkit", "armatureRoot", new Vector3(0F, 2.27075F, -1.89192F), new Vector3(270F, 0F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Bandolier, "DisplayBandolier", "armatureRoot", new Vector3(2.01435F, 2.43731F, 0F), new Vector3(0F, 0F, 271.872F), new Vector3(1.58099F, 1.58099F, 1.58099F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BounceNearby, "DisplayHook", "armatureRoot", new Vector3(-2.00177F, 3.20577F, 2.23162F), new Vector3(0F, 0F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.IgniteOnKill, "DisplayGasoline", "armatureRoot", new Vector3(-2.71051F, 1.7329F, -2.5243F), new Vector3(0F, 0F, 0F), new Vector3(2.13548F, 2.13548F, 2.13548F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.StunChanceOnHit, "DisplayStunGrenade", "armatureRoot", new Vector3(1.64048F, 1.60734F, -0.4977F), new Vector3(0F, 0F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Firework, "DisplayFirework", "armatureRoot", new Vector3(0F, 3.13214F, -1.89523F), new Vector3(270.8209F, 180F, 180F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LunarDagger, "DisplayLunarDagger", "armatureRoot", new Vector3(-1.74605F, 3.12007F, -2.36839F), new Vector3(0F, 0F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.GoldOnHit, "DisplayBoneCrown", "armatureRoot", new Vector3(-1.47893F, 1.49868F, 0F), new Vector3(0F, 0F, 0F), new Vector3(1.58871F, 1.58871F, 1.58871F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.WarCryOnMultiKill, "DisplayPauldron", "armatureRoot", new Vector3(-1.27527F, 2.14549F, -5.00375F), new Vector3(326.8751F, 0F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateZMirroredDisplayRule(RoR2Content.Items.ShieldOnly, "DisplayShieldBug", "armatureRoot", new Vector3(0.48731F, 3.21024F, 2.51923F), new Vector3(31.24605F, 0F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.AlienHead, "DisplayAlienHead", "armatureRoot", new Vector3(-1.6391F, 3.04133F, 1.87018F), new Vector3(285.0858F, 274.4774F, 203.0919F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(RoR2Content.Items.Talisman, "DisplayTalisman", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Knurl, "DisplayKnurl", "armatureRoot", new Vector3(-2.44825F, 0.72256F, 1.98715F), new Vector3(0F, 0F, 59.78923F), new Vector3(0.6506F, 0.6506F, 0.6506F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BeetleGland, "DisplayBeetleGland", "armatureRoot", new Vector3(0F, 2.57782F, -2.74076F), new Vector3(0F, 0F, 49.82682F), new Vector3(0.3817F, 0.3817F, 0.3817F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SprintBonus, "DisplaySoda", "armatureRoot", new Vector3(-1.59176F, 1.65494F, -0.68669F), new Vector3(0F, 0F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SecondarySkillMagazine, "DisplayDoubleMag", "armatureRoot", new Vector3(0F, 1.61468F, 1.47179F), new Vector3(0F, 0F, 73.89843F), new Vector3(0.24635F, 0.24635F, 0.24635F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.StickyBomb, "DisplayStickyBomb", "armatureRoot", new Vector3(0F, 2.0902F, 1.08989F), new Vector3(345.2114F, 0F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.TreasureCache, "DisplayKey", "armatureRoot", new Vector3(-0.853F, 2.39764F, 1.78961F), new Vector3(274.7848F, 302.7948F, 147.2962F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BossDamageBonus, "DisplayAPRound", "armatureRoot", new Vector3(1.65241F, 1.56601F, 0F), new Vector3(0F, 0F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SprintArmor, "DisplayBuckler", "armatureRoot", new Vector3(0F, 1.55442F, 0F), new Vector3(270F, 0F, 0F), new Vector3(1.51607F, 1.75524F, 0.03487F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.IceRing, "DisplayIceRing", "armatureRoot", new Vector3(1.023F, 2.61029F, 1.88038F), new Vector3(0F, 0F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.FireRing, "DisplayFireRing", "armatureRoot", new Vector3(0F, 2.61029F, 1.88038F), new Vector3(0F, 0F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SlowOnHit, "DisplayBauble", "armatureRoot", new Vector3(-0.77148F, 0.76729F, 0.83394F), new Vector3(0F, 0F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ExtraLife, "DisplayHippo", "armatureRoot", new Vector3(1.71864F, 0.05812F, -5.04995F), new Vector3(0F, 180F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.UtilitySkillMagazine, "DisplayAfterburner", "armatureRoot", new Vector3(1.13252F, 0.39693F, -5.32351F), new Vector3(-0.00001F, 180F, 180F), new Vector3(3.62095F, 3.62095F, 3.62095F)));

            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.HeadHunter, "DisplaySkullcrown", "armatureRoot", new Vector3(0F, 1.17559F, 4.67365F), new Vector3(0F, 0F, 0F), new Vector3(0.77046F, 0.77046F, 0.77046F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.KillEliteFrenzy, "DisplayBrainstalk", "armatureRoot", new Vector3(1.12963F, 1.34918F, -1.40009F), new Vector3(0F, 0F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.RepeatHeal, "DisplayCorpseflower", "armatureRoot", new Vector3(2.1422F, 2.2567F, -3.7506F), new Vector3(347.2389F, 0F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.AutoCastEquipment, "DisplayFossil", "armatureRoot", new Vector3(-1.66896F, 2.86735F, 0F), new Vector3(0F, 0F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateMirroredDisplayRule(RoR2Content.Items.IncreaseHealing, "DisplayAntler", "armatureRoot", new Vector3(0.22614F, 1.52272F, 5.013F), new Vector3(0F, 90F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.JumpBoost, "DisplayWaxBird", "armatureRoot", new Vector3(1.04863F, 2.49328F, 2.20322F), new Vector3(0F, 0F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ExecuteLowHealthElite, "DisplayGuillotine", "armatureRoot", new Vector3(0.2005F, 0.37122F, 5.15505F), new Vector3(0F, 90F, 90F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.EnergizedOnEquipmentUse, "DisplayWarHorn", "armatureRoot", new Vector3(-0.63121F, 3.34712F, 2.1999F), new Vector3(0F, 266.5145F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BarrierOnOverHeal, "DisplayAegis", "armatureRoot", new Vector3(0F, 1.55993F, -5.21732F), new Vector3(272.3226F, 292.5519F, 247.2942F), new Vector3(0.3756F, 0.3756F, 0.3756F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.TitanGoldDuringTP, "DisplayGoldHeart", "armatureRoot", new Vector3(0.62153F, 2.2716F, -1.86493F), new Vector3(0F, 0F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SprintWisp, "DisplayBrokenMask", "armatureRoot", new Vector3(-0.65045F, 2.78287F, -1.85148F), new Vector3(0F, 357.6176F, 0F), new Vector3(0.3988F, 0.3988F, 0.3988F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BarrierOnKill, "DisplayBrooch", "armatureRoot", new Vector3(0F, 0.68444F, 5.19105F), new Vector3(88.74961F, 180F, 180F), new Vector3(4.21656F, 4.21656F, 4.21656F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ArmorReductionOnHit, "DisplayWarhammer", "armatureRoot", new Vector3(1.07952F, 2.08991F, 3.60535F), new Vector3(359.991F, 90.03133F, 16.03757F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.TPHealingNova, "DisplayGlowFlower", "armatureRoot", new Vector3(1.70953F, 2.38037F, -0.5923F), new Vector3(0F, 272.8649F, 32.11892F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.NearbyDamageBonus, "DisplayDiamond", "armatureRoot", new Vector3(1.6929F, 2.78604F, -1.9202F), new Vector3(0F, 0F, 0F), new Vector3(0.32502F, 0.32502F, 0.32502F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LunarUtilityReplacement, "DisplayBirdFoot", "armatureRoot", new Vector3(0F, 3.28475F, -2.39251F), new Vector3(357.6339F, 267.9203F, 311.3337F), new Vector3(1.65571F, 1.65571F, 1.65571F)));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.MonsoonPlayerHelper, 
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Thorns, "DisplayRazorwireCoiled", "armatureRoot", new Vector3(-1.26547F, 2.29449F, -1.90475F), new Vector3(0F, 0F, 0F), new Vector3(0.39437F, 0.39437F, 0.39437F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.FlatHealth, "DisplaySteakFlat", "armatureRoot", new Vector3(2.28633F, 1.10511F, 5.21638F), new Vector3(0F, 0F, 0F), new Vector3(0.23157F, 0.23157F, 0.23157F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Pearl, "DisplayPearl", "armatureRoot", new Vector3(0F, 0F, 0F), new Vector3(0F, 0F, 0F), new Vector3(1.65637F, 1.65637F, 1.65637F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ShinyPearl, "DisplayShinyPearl", "armatureRoot", new Vector3(0F, 0F, 0F), new Vector3(0F, 0F, 0F), new Vector3(2.02357F, 2.02357F, 2.02357F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BonusGoldPackOnKill, "DisplayTome", "armatureRoot", new Vector3(1.43183F, 1.83783F, 1.76488F), new Vector3(335.8132F, 180.1042F, 359.9573F), new Vector3(0.19975F, 0.19975F, 0.19975F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LaserTurbine, "DisplayLaserTurbine", "armatureRoot", new Vector3(0F, 2.17405F, -4.85408F), new Vector3(78.87201F, 0F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LunarPrimaryReplacement, "DisplayBirdEye", "armatureRoot", new Vector3(0F, 3.15131F, 2.36139F), new Vector3(350.6103F, 180F, 180F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.NovaOnLowHealth, "DisplayJellyGuts", "armatureRoot", new Vector3(-1.98519F, 3.07921F, -0.51692F), new Vector3(0F, 0F, 305.3978F), new Vector3(0.29692F, 0.29692F, 0.29692F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LunarTrinket, "DisplayBeads", "armatureRoot", new Vector3(-1.37129F, 3.16799F, 2.40142F), new Vector3(348.3794F, 358.296F, 8.40125F), new Vector3(1F, 1F, 1F)));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.InvadingDoppelganger, 
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ArmorPlate, "DisplayRepulsionArmorPlate", "armatureRoot", new Vector3(-2.75493F, 0.23523F, 5.13278F), new Vector3(271.035F, 0F, 0F), new Vector3(1F, 0.3034F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Squid, "DisplaySquidTurret", "armatureRoot", new Vector3(-0.9459F, 1.49687F, -1.73751F), new Vector3(0F, 0F, 0F), new Vector3(0.12167F, 0.12167F, 0.12167F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.DeathMark, "DisplayDeathMark", "armatureRoot", new Vector3(-0.49962F, 1.68125F, -1.84827F), new Vector3(272.2963F, 180F, 180F), new Vector3(0.07907F, 0.07907F, 0.07907F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Plant, "DisplayInterstellarDeskPlant", "armatureRoot", new Vector3(1.47218F, 1.828F, 0.83669F), new Vector3(270F, 0F, 0F), new Vector3(0.2763F, 0.2763F, 0.2763F)));
            itemRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(RoR2Content.Items.FocusConvergence, "DisplayFocusedConvergence", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.FireballsOnHit, "DisplayFireballsOnHit", "armatureRoot", new Vector3(2.70022F, 1.27159F, 0F), new Vector3(0F, 0F, 0F), new Vector3(0.12369F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LightningStrikeOnHit, "DisplayChargedPerforator", "armatureRoot", new Vector3(-2.74067F, 1.51728F, -0.4536F), new Vector3(270F, 0F, 0F), new Vector3(1.35862F, 12.39644F, 12.39644F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BleedOnHitAndExplode, "DisplayBleedOnHitAndExplode", "armatureRoot", new Vector3(0F, 1.67309F, -1.81445F), new Vector3(0F, 0F, 0F), new Vector3(0.13153F, 0.13153F, 0.13153F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SiphonOnLowHealth, "DisplaySiphonOnLowHealth", "armatureRoot", new Vector3(-2.5527F, 2.35968F, -1.67867F), new Vector3(348.7201F, -0.00003F, 70.90158F), new Vector3(0.63073F, 0.22338F, 0.63073F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.MonstersOnShrineUse, "DisplayMonstersOnShrineUse", "armatureRoot", new Vector3(-1.16184F, 2.4554F, -3.22789F), new Vector3(0F, 0F, 89.72976F), new Vector3(0.31843F, 0.31843F, 0.31843F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.RandomDamageZone, "DisplayRandomDamageZone", "armatureRoot", new Vector3(0F, 0.1421F, -5.86588F), new Vector3(55.65305F, 0F, 0F), new Vector3(0.46588F, 0.46588F, 0.46588F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LunarSecondaryReplacement, "DisplayBirdClaw", "armatureRoot", new Vector3(-2.40323F, 2.94709F, 0F), new Vector3(0F, 0F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LunarSpecialReplacement, "DisplayBirdHeart", "armatureRoot", new Vector3(-3.01158F, 4.46906F, 0F), new Vector3(0F, 0F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateMirroredDisplayRule(RoR2Content.Items.RoboBallBuddy, "DisplayEmpathyChip", "armatureRoot", new Vector3(2.17376F, 0.80626F, 5.09474F), new Vector3(270F, 0F, 0F), new Vector3(1F, 0.15377F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ParentEgg, "DisplayParentEgg", "armatureRoot", new Vector3(-1.54012F, 1.75477F, -1.72937F), new Vector3(0F, 0F, 0F), new Vector3(0.17116F, 0.17116F, 0.17116F)));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SummonedEcho, 

            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.CommandMissile, "DisplayMissileRack", "armatureRoot", new Vector3(0.83524F, 3.25594F, -1.98162F), new Vector3(0F, 180F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Fruit, "DisplayFruit", "armatureRoot", new Vector3(-2.33682F, 1.43466F, 0F), new Vector3(347.6046F, 211.1074F, 19.5824F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(RoR2Content.Equipment.Meteor, "DisplayMeteor", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateZMirroredDisplayRule(RoR2Content.Equipment.AffixRed, "DisplayEliteHorn", "armatureRoot", new Vector3(2.29177F, 2.80128F, 2.35831F), new Vector3(75.20471F, 0F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.AffixBlue, "DisplayEliteRhinoHorn", "armatureRoot", new Vector3(0F, 2.09986F, 2.7822F), new Vector3(335.1285F, 0F, 0F), new Vector3(1.80561F, 1.80561F, 1.80561F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.AffixWhite, "DisplayEliteIceCrown", "armatureRoot", new Vector3(0F, 4.34657F, 0F), new Vector3(274.7242F, 0F, 0F), new Vector3(0.22042F, 0.22042F, 0.22042F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.AffixPoison, "DisplayEliteUrchinCrown", "armatureRoot", new Vector3(0F, 4.31127F, 0F), new Vector3(270F, 0F, 0F), new Vector3(0.4752F, 0.4752F, 0.3159F)));
            itemRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(RoR2Content.Equipment.Blackhole, "DisplayGravCube", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.CritOnUse, "DisplayNeuralImplant", "armatureRoot", new Vector3(0F, 1.05135F, 5.91328F), new Vector3(0F, 0F, 0F), new Vector3(2.65287F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.DroneBackup, "DisplayRadio", "armatureRoot", new Vector3(0.81401F, 1.83701F, 1.84535F), new Vector3(0F, 176.7103F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.BFG, "DisplayBFG", "armatureRoot", new Vector3(2.35776F, 2.85516F, 0F), new Vector3(0F, 0F, 334.2025F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Jetpack, "DisplayBugWings", "armatureRoot", new Vector3(0F, 1.82763F, -5.16831F), new Vector3(310.2771F, 0F, 0F), new Vector3(0.26944F, 0.26944F, 0.26944F)));

            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Lightning, "DisplayLightningArmLeft", "armatureRoot", new Vector3(0.09498F, 1.84666F, 3.8602F), new Vector3(0F, 0F, 48.6577F), new Vector3(2.1286F, 2.1286F, 2.1286F)));

            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.GoldGat, "DisplayGoldGat", "armatureRoot", new Vector3(2.15488F, 3.42716F, 0F), new Vector3(344.8477F, 78.91943F, 306.842F), new Vector3(0.33741F, 0.33741F, 0.33741F)));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.LunarPotion, 
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.BurnNearby, "DisplayPotion", "armatureRoot", new Vector3(0.82251F, 1.68673F, 1.82549F), new Vector3(0F, 0F, 346.1176F), new Vector3(0.0771F, 0.0771F, 0.0771F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Scanner, "DisplayScanner", "armatureRoot", new Vector3(-2.23142F, 2.11811F, 5.04656F), new Vector3(283.6222F, 0F, 0F), new Vector3(0.47859F, 0.07292F, 1.69041F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.CrippleWard, "DisplayEffigy", "armatureRoot", new Vector3(0F, 1.59461F, 0F), new Vector3(0F, 173.1952F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Gateway, "DisplayVase", "armatureRoot", new Vector3(0F, 1.92872F, 3.14162F), new Vector3(14.73813F, 0F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Tonic, "DisplayTonic", "armatureRoot", new Vector3(-0.61991F, 2.09404F, 0F), new Vector3(68.75536F, 7.8942F, 51.59753F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.QuestVolatileBattery, "DisplayBatteryArray", "armatureRoot", new Vector3(0F, 1.97107F, 3.8962F), new Vector3(280.1454F, 0F, 0F), new Vector3(1.16881F, 1.16881F, 1.16881F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Cleanse, "DisplayWaterPack", "armatureRoot", new Vector3(-0.5568F, 2.11861F, -1.93958F), new Vector3(0F, 0F, 0F), new Vector3(0.2647F, 0.2647F, 0.2647F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.FireBallDash, "DisplayEgg", "armatureRoot", new Vector3(0.48075F, 1.58824F, -1.8715F), new Vector3(295.9519F, 180F, 180F), new Vector3(0.46438F, 0.46438F, 0.46438F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.AffixHaunted, "DisplayEliteStealthCrown", "armatureRoot", new Vector3(-1.69504F, 1.64676F, 4.26145F), new Vector3(270F, 0.00001F, 0F), new Vector3(0.28553F, 0.28553F, 0.28553F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.GainArmor, "DisplayElephantFigure", "armatureRoot", new Vector3(0F, 1.66056F, 0F), new Vector3(0F, 0F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(RoR2Content.Equipment.Saw, "DisplaySawmerang", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Recycle, "DisplayRecycler", "armatureRoot", new Vector3(-2.27554F, 2.30015F, 2.74449F), new Vector3(0F, 266.7743F, 0F), new Vector3(0.22312F, 0.22312F, 0.22312F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.LifestealOnHit, "DisplayLifestealOnHit", "armatureRoot", new Vector3(-2.15624F, 2.63208F, -3.93053F), new Vector3(79.04874F, 57.30419F, 0.00001F), new Vector3(0.37302F, 0.37302F, 0.37302F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.TeamWarCry, "DisplayTeamWarCry", "armatureRoot", new Vector3(0F, 1.51424F, -0.46866F), new Vector3(277.9734F, 0F, 0F), new Vector3(0.333F, 0.333F, 0.333F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.DeathProjectile, "DisplayDeathProjectile", "armatureRoot", Vector3.zero, Vector3.zero, Vector3.one));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.AffixEcho, 
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.AffixLunar, "DisplayEliteLunar, Fire", "armatureRoot", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.AffixLunar, "DisplayEliteLunar,Eye", "armatureRoot", new Vector3(0F, 4.5335F, 0F), new Vector3(270F, 0F, 0F), new Vector3(2.95876F, 2.95876F, 1.02455F)));
            //IDRS NOTE: [AFFIXLUNAR] = I don't think EliteLunar, Fire is used?

            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.MoveSpeedOnKill, "DisplayGrappleHook", "armatureRoot", new Vector3(-1.7017F, 0.23874F, -5.22081F), new Vector3(0F, 0F, 0F), new Vector3(1F, 0.57988F, 0.46889F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.HealingPotion, "DisplayHealingPotion", "armatureRoot", new Vector3(0.89229F, 1.91584F, 1.69437F), new Vector3(0F, 0F, 0F), new Vector3(0.20344F, 0.20344F, 0.20344F)));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.HealingPotionConsumed, 
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.PermanentDebuffOnHit, "DisplayScorpion", "armatureRoot", new Vector3(0F, 0F, -5.41748F), new Vector3(60.25602F, 0F, 0F), new Vector3(12.31363F, 12.31363F, 12.31363F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.AttackSpeedAndMoveSpeed, "DisplayCoffee", "armatureRoot", new Vector3(-1.52705F, 1.92118F, 1.74906F), new Vector3(0F, 0F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.CritDamage, "DisplayLaserSight", "armatureRoot", new Vector3(-1.54373F, 3.06194F, 2.3903F), new Vector3(0F, 90F, 0F), new Vector3(0.25157F, 0.25157F, 0.25157F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.BearVoid, "DisplayBearVoid", "armatureRoot", new Vector3(2.22056F, 0F, -5.24749F), new Vector3(15.12841F, 186.6021F, 1.73018F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.MushroomVoid, "DisplayMushroomVoid", "armatureRoot", new Vector3(2.083F, 3.06064F, -2.29928F), new Vector3(0F, 0F, 0F), new Vector3(0.19576F, 0.19576F, 0.19576F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.CloverVoid, "DisplayCloverVoid", "armatureRoot", new Vector3(-0.84866F, 2.0579F, 2.61868F), new Vector3(0F, 0F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.StrengthenBurn, "DisplayGasTank", "armatureRoot", new Vector3(0F, -0.41474F, -4.58943F), new Vector3(89.82087F, 269.3694F, -0.00218F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.RegeneratingScrap, "DisplayRegeneratingScrap", "armatureRoot", new Vector3(0.30478F, 1.96326F, -0.5808F), new Vector3(0F, 0F, 0F), new Vector3(0.49226F, 0.49226F, 0.49226F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.BleedOnHitVoid, "DisplayTriTipVoid", "armatureRoot", new Vector3(0F, 0F, 5.20804F), new Vector3(0F, 0F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.CritGlassesVoid, "DisplayGlassesVoid", "armatureRoot", new Vector3(0F, 0.882F, 4.93111F), new Vector3(0F, 0F, 0F), new Vector3(4.48361F, 4.48361F, 0.64829F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.TreasureCacheVoid, "DisplayKeyVoid", "armatureRoot", new Vector3(-0.853F, 2.39764F, 1.78961F), new Vector3(274.7848F, 302.7948F, 147.2962F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.SlowOnHitVoid, "DisplayBaubleVoid", "armatureRoot", new Vector3(-0.77148F, 0.76729F, 0.83394F), new Vector3(0F, 0F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.MissileVoid, "DisplayMissileLauncherVoid", "armatureRoot", new Vector3(2.72169F, 3.51991F, 2.05472F), new Vector3(0F, 0F, 328.1266F), new Vector3(0.17004F, 0.17004F, 0.17004F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.ChainLightningVoid, "DisplayUkuleleVoid", "armatureRoot", new Vector3(1.54447F, 1.84132F, -1.84835F), new Vector3(0F, 350.0657F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.ExtraLifeVoid, "DisplayHippoVoid", "armatureRoot", new Vector3(1.71864F, 0.05812F, -5.04995F), new Vector3(0F, 180F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.EquipmentMagazineVoid, "DisplayFuelCellVoid", "armatureRoot", new Vector3(-2.8898F, 0F, -2.35959F), new Vector3(0F, 0F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.ExplodeOnDeathVoid, "DisplayWillowWispVoid", "armatureRoot", new Vector3(-2.48619F, -0.39048F, -5.07656F), new Vector3(0F, 0F, 0F), new Vector3(0.13154F, 0.13154F, 0.13154F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.FragileDamageBonus, "DisplayDelicateWatch", "armatureRoot", new Vector3(0F, 2.4595F, 2.07553F), new Vector3(0F, 90F, 270F), new Vector3(3.52113F, 3.52113F, 3.52113F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.OutOfCombatArmor, "DisplayOddlyShapedOpal", "armatureRoot", new Vector3(1.88464F, 3.10628F, 0.66852F), new Vector3(89.87489F, 180F, 180F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.MoreMissile, "DisplayICBM", "armatureRoot", new Vector3(1.92378F, 2.42406F, -2.67514F), new Vector3(0F, 0F, 93.37376F), new Vector3(0.43233F, 0.43233F, 0.43233F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.ImmuneToDebuff, "DisplayRainCoatBelt", "armatureRoot", new Vector3(1.16436F, 2.424F, -4.81634F), new Vector3(0F, 0F, 0F), new Vector3(2.58814F, 2.58814F, 2.58814F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.RandomEquipmentTrigger, "DisplayBottledChaos", "armatureRoot", new Vector3(0F, 1.57364F, 1.90599F), new Vector3(0F, 0F, 0F), new Vector3(0.42254F, 0.42254F, 0.42254F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.PrimarySkillShuriken, "DisplayShuriken", "armatureRoot", new Vector3(0.57463F, 1.87714F, 1.88133F), new Vector3(0F, 0F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(DLC1Content.Items.RandomlyLunar, "DisplayDomino", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.GoldOnHurt, "DisplayRollOfPennies", "armatureRoot", new Vector3(0F, 1.73304F, 1.86226F), new Vector3(0F, 0F, 79.38425F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.HalfAttackSpeedHalfCooldowns, "DisplayLunarShoulderNature", "armatureRoot", new Vector3(-2.50981F, 2.84735F, 2.08225F), new Vector3(0F, 0F, 25.77266F), new Vector3(2.26115F, 2.26115F, 2.26115F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.HalfSpeedDoubleHealth, "DisplayLunarShoulderStone", "armatureRoot", new Vector3(2.47021F, 2.93055F, 2.37046F), new Vector3(0F, 178.5694F, 0F), new Vector3(1.79997F, 1.79997F, 1.79997F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.FreeChest, "DisplayShippingRequestForm", "armatureRoot", new Vector3(0.97871F, 1.56102F, 1.51623F), new Vector3(0F, 155.0735F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.ElementalRingVoid, "DisplayVoidRing", "armatureRoot", new Vector3(-1F, 2.60718F, 1.8375F), new Vector3(0F, 0F, 0F), new Vector3(1F, 1F, 1F)));

            itemRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = DLC1Content.Items.LunarSun,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplaySunHead"),
                            childName = "armatureRoot",
localPos = new Vector3(2.23349F, 3.25037F, -0.62005F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(1.92663F, 1.92663F, 1.92663F),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplaySunHeadNeck"),
                            childName = "armatureRoot",
localPos = new Vector3(2.18605F, 3.02837F, -0.58111F),
localAngles = new Vector3(0F, 0F, 347.5511F),
localScale = new Vector3(3.21668F, 3.21668F, 3.21668F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });
            //Not an item display
            ////itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.DroneWeapons, 
            // Unless you're a drone, you don't need these.
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.DroneWeaponsBoost, 
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.DroneWeaponsDisplay1, 
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.DroneWeaponsDisplay2, 
            itemRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(DLC1Content.Items.MinorConstructOnKill, "DisplayDefenseNucleus", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.VoidMegaCrabItem, "DisplayMegaCrabItem", "armatureRoot", new Vector3(0F, 1.58423F, 0.59716F), new Vector3(275.6215F, 0F, 0F), new Vector3(0.39579F, 0.39579F, 0.39579F)));

            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Equipment.Molotov, "DisplayMolotov", "armatureRoot", new Vector3(-0.28762F, 1.6434F, -1.90862F), new Vector3(0F, 0F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Equipment.VendingMachine, "DisplayVendingMachine2", "armatureRoot", new Vector3(-1.13131F, 1.45258F, 0F), new Vector3(311.65F, 79.27425F, 99.23319F), new Vector3(0.56553F, 0.56553F, 0.56553F)));
            //IDRS NOTE: [BOSSHUNTER] = Keep both. DisplayBlunderbuss is the hat, and Dis 
            itemRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = DLC1Content.Equipment.BossHunter,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayBlunderbuss"),
                            childName = "armatureRoot",
localPos = new Vector3(3.38288F, 3.5433F, 0F),
localAngles = new Vector3(86.89291F, 180F, 180F),
localScale = new Vector3(5.08676F, 5.08676F, 5.08676F),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayTricornGhost"),
                            childName = "armatureRoot",
localPos = new Vector3(0F, 4.17061F, -0.50977F),
localAngles = new Vector3(29.23001F, 0F, 0F),
localScale = new Vector3(7.39143F, 7.39143F, 7.39143F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Equipment.BossHunterConsumed, "DisplayTricornUsed", "armatureRoot", new Vector3(-1.03709F, 3.16114F, 2.1418F), new Vector3(0F, 0F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Equipment.GummyClone, "DisplayGummyClone", "armatureRoot", new Vector3(0F, 2.43042F, 0F), new Vector3(0F, 0F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Equipment.MultiShopCard, "DisplayExecutiveCard", "armatureRoot", new Vector3(-1.9667F, 1.17342F, -5.21842F), new Vector3(90F, 180F, 0F), new Vector3(6.50947F, 6.50947F, 6.50947F)));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Equipment.LunarPortalOnUse, "DisplayLunarPortalOnUse", "armatureRoot", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Equipment.EliteVoidEquipment, "DisplayAffixVoid", "armatureRoot", new Vector3(0F, 0.64587F, 5.07928F), new Vector3(77.35911F, 0F, 0F), new Vector3(1.47539F, 1.47539F, 1.47539F)));
            //from claymen git
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Elites.Earth.eliteEquipmentDef, "DisplayEliteMendingAntlers", "armatureRoot", new Vector3(0F, 3.03717F, 2.52891F), new Vector3(0F, 0F, 0F), new Vector3(2.27016F, 2.27016F, 2.27016F)));
            #endregion
            #region Modded
            if (PotmobilePlugin.scepterPluginLoaded)
            {
                AddScepterItemDisplay();
            }
            #endregion
            #endregion

            ItemDisplayRuleSet.KeyAssetRuleGroup[] item = itemRules.ToArray();
            itemDisplayRuleSet.keyAssetRuleGroups = item;
            //itemDisplayRuleSet.GenerateRuntimeValues();

            characterModel.itemDisplayRuleSet = itemDisplayRuleSet;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static void AddScepterItemDisplay()
        {
            var scepterDef = ItemDisplayCore.LoadScepterObject();
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(scepterDef, AncientScepter.ItemBase.displayPrefab,
                "armatureRoot", Vector3.zero, Vector3.zero, Vector3.one));
        }
    }
}

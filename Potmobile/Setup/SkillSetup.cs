using System;
using System.Collections.Generic;
using RoR2;
using UnityEngine;
using RoR2.Skills;
using EntityStates;
using UnityEngine.AddressableAssets;
using R2API;
using RoR2.Projectile;
using System.Runtime.CompilerServices;

namespace Potmobile
{
    public static class SkillSetup
    {
        private static bool initialized = false;
        public static void Init()
        {
            if (initialized) return;
            initialized = true;

            SkillLocator skillLocator = PotmobileContent.PotmobileBodyObject.GetComponent<SkillLocator>();
            CreateSkillFamilies(PotmobileContent.PotmobileBodyObject, true);
            BuildPrimary(skillLocator);
            BuildPrimaryScepter();
            BuildSecondary(skillLocator);
            BuildUtility(skillLocator);
            BuildSpecial(skillLocator);
        }


        private static void BuildPrimary(SkillLocator skillLocator)
        {

            SkillDef primaryDef = SkillDef.CreateInstance<SkillDef>();
            primaryDef.activationState = new SerializableEntityStateType(typeof(EntityStates.MoffeinPotmobile.Weapon.FirePotCannon));
            primaryDef.baseRechargeInterval = 0f;
            primaryDef.skillNameToken = "MOFFEINPOTMOBILEBODY_PRIMARY_NAME";
            primaryDef.skillDescriptionToken = "MOFFEINPOTMOBILEBODY_PRIMARY_DESCRIPTION";
            primaryDef.skillName = "FireCannon";
            primaryDef.icon = Addressables.LoadAssetAsync<ToolbotWeaponSkillDef>("RoR2/Base/Toolbot/ToolbotBodyFireGrenadeLauncher.asset").WaitForCompletion().icon;
            primaryDef.baseMaxStock = 1;
            primaryDef.rechargeStock = 1;
            primaryDef.beginSkillCooldownOnSkillEnd = false;
            primaryDef.activationStateMachineName = "Weapon";
            primaryDef.interruptPriority = InterruptPriority.Any;
            primaryDef.isCombatSkill = true;
            primaryDef.cancelSprintingOnActivation = false;
            primaryDef.canceledFromSprinting = false;
            primaryDef.mustKeyPress = false;
            primaryDef.requiredStock = 1;
            primaryDef.stockToConsume = 1;
            primaryDef.keywordTokens = new string[] { };
            (primaryDef as ScriptableObject).name = primaryDef.skillName;

            PotmobileContent.skillDefs.Add(primaryDef);
            PotmobileContent.entityStates.Add(typeof(EntityStates.MoffeinPotmobile.Weapon.FirePotCannon));
            AddSkillToFamily(skillLocator.primary.skillFamily, primaryDef);
            PotmobileContent.SkillDefs.FirePotCannon = primaryDef;

            GameObject projectilePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Toolbot/ToolbotGrenadeLauncherProjectile.prefab").WaitForCompletion().InstantiateClone("MoffeinPotmobileRocket", true);
            ProjectileImpactExplosion pie = projectilePrefab.GetComponent<ProjectileImpactExplosion>();
            pie.blastRadius = PotmobilePlugin.primaryRadius;
            pie.falloffModel = BlastAttack.FalloffModel.None;
            pie.bonusBlastForce = Vector3.zero;

            ProjectileController pc = projectilePrefab.GetComponent<ProjectileController>();
            pc.ghostPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ClayBoss/ClayPotProjectileGhost.prefab").WaitForCompletion();

            ProjectileDamage pd = projectilePrefab.GetComponent<ProjectileDamage>();
            pd.damageType = DamageType.ClayGoo;

            PotmobileContent.projectilePrefabs.Add(projectilePrefab);
            EntityStates.MoffeinPotmobile.Weapon.FirePotCannon.projectilePrefab = projectilePrefab;
        }

        private static void BuildPrimaryScepter()
        {
            SkillDef primaryScepterDef = SkillDef.CreateInstance<SkillDef>();
            primaryScepterDef.activationState = new SerializableEntityStateType(typeof(EntityStates.MoffeinPotmobile.Weapon.FirePotCannonScepter));
            primaryScepterDef.baseRechargeInterval = 0f;
            primaryScepterDef.skillNameToken = "MOFFEINPOTMOBILEBODY_PRIMARY_SCEPTER_NAME";
            primaryScepterDef.skillDescriptionToken = "MOFFEINPOTMOBILEBODY_PRIMARY_SCEPTER_DESCRIPTION";
            primaryScepterDef.skillName = "FireCannonScepter";
            primaryScepterDef.icon = Assets.assetBundle.LoadAsset<Sprite>("texIconPrimaryScepter.png");
            primaryScepterDef.baseMaxStock = 1;
            primaryScepterDef.rechargeStock = 1;
            primaryScepterDef.beginSkillCooldownOnSkillEnd = false;
            primaryScepterDef.activationStateMachineName = "Weapon";
            primaryScepterDef.interruptPriority = InterruptPriority.Any;
            primaryScepterDef.isCombatSkill = true;
            primaryScepterDef.cancelSprintingOnActivation = false;
            primaryScepterDef.canceledFromSprinting = false;
            primaryScepterDef.mustKeyPress = false;
            primaryScepterDef.requiredStock = 1;
            primaryScepterDef.stockToConsume = 1;
            primaryScepterDef.keywordTokens = new string[] { };
            (primaryScepterDef as ScriptableObject).name = primaryScepterDef.skillName;

            PotmobileContent.skillDefs.Add(primaryScepterDef);
            PotmobileContent.entityStates.Add(typeof(EntityStates.MoffeinPotmobile.Weapon.FirePotCannonScepter));
            PotmobileContent.SkillDefs.FirePotCannonScepter = primaryScepterDef;

            GameObject projectilePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Toolbot/ToolbotGrenadeLauncherProjectile.prefab").WaitForCompletion().InstantiateClone("MoffeinPotmobileRocketScepter", true);
            ProjectileImpactExplosion pie = projectilePrefab.GetComponent<ProjectileImpactExplosion>();
            pie.blastRadius = PotmobilePlugin.primaryRadius * 2f;
            pie.falloffModel = BlastAttack.FalloffModel.None;
            pie.bonusBlastForce = Vector3.zero;

            ProjectileController pc = projectilePrefab.GetComponent<ProjectileController>();
            pc.ghostPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ClayBoss/ClayPotProjectileGhost.prefab").WaitForCompletion();

            ProjectileDamage pd = projectilePrefab.GetComponent<ProjectileDamage>();
            pd.damageType = DamageType.ClayGoo;

            PotmobileContent.projectilePrefabs.Add(projectilePrefab);
            EntityStates.MoffeinPotmobile.Weapon.FirePotCannonScepter.scepterProjectilePrefab = projectilePrefab;

            if (PotmobilePlugin.scepterPluginLoaded) AssignScepter();
            if (PotmobilePlugin.classicItemsLoaded) AssignScepterClassic();
        }


        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void AssignScepter()
        {
            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(PotmobileContent.SkillDefs.FirePotCannonScepter, "MoffeinPotmobileBody", SkillSlot.Primary, 0);
            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(PotmobileContent.SkillDefs.FirePotCannonScepter, "MoffeinHaulerBody", SkillSlot.Primary, 0);
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void AssignScepterClassic()
        {
            ThinkInvisible.ClassicItems.Scepter.instance.RegisterScepterSkill(PotmobileContent.SkillDefs.FirePotCannonScepter, "MoffeinPotmobileBody", SkillSlot.Primary, PotmobileContent.SkillDefs.FirePotCannon);
            ThinkInvisible.ClassicItems.Scepter.instance.RegisterScepterSkill(PotmobileContent.SkillDefs.FirePotCannonScepter, "MoffeinHaulerBody", SkillSlot.Primary, PotmobileContent.SkillDefs.FirePotCannon);
        }


        private static void BuildSecondary(SkillLocator skillLocator)
        {
            SkillDef secondaryDef = SkillDef.CreateInstance<SkillDef>();
            secondaryDef.activationState = new SerializableEntityStateType(typeof(EntityStates.MoffeinPotmobile.Weapon.PushAll));
            secondaryDef.baseRechargeInterval = PotmobilePlugin.secondaryCooldown;
            secondaryDef.skillNameToken = "MOFFEINPOTMOBILEBODY_SECONDARY_NAME";
            secondaryDef.skillDescriptionToken = "MOFFEINPOTMOBILEBODY_SECONDARY_DESCRIPTION";
            secondaryDef.skillName = "PushAll";
            secondaryDef.icon = Addressables.LoadAssetAsync<SkillDef>("RoR2/DLC1/Railgunner/RailgunnerBodyFireMineConcussive.asset").WaitForCompletion().icon;
            secondaryDef.baseMaxStock = PotmobilePlugin.secondaryStocks;
            secondaryDef.rechargeStock = 1;
            secondaryDef.beginSkillCooldownOnSkillEnd = false;
            secondaryDef.activationStateMachineName = "Weapon";
            secondaryDef.interruptPriority = InterruptPriority.Skill;
            secondaryDef.isCombatSkill = false;
            secondaryDef.cancelSprintingOnActivation = false;
            secondaryDef.canceledFromSprinting = false;
            secondaryDef.mustKeyPress = true;
            secondaryDef.requiredStock = 1;
            secondaryDef.stockToConsume = 1;
            secondaryDef.keywordTokens = new string[] { };
            (secondaryDef as ScriptableObject).name = secondaryDef.skillName;

            PotmobileContent.skillDefs.Add(secondaryDef);
            PotmobileContent.entityStates.Add(typeof(EntityStates.MoffeinPotmobile.Weapon.PushAll));
            AddSkillToFamily(skillLocator.secondary.skillFamily, secondaryDef);
            PotmobileContent.SkillDefs.Push = secondaryDef;
        }

        private static void BuildUtility(SkillLocator skillLocator)
        {
            SkillDef utilityDef = SkillDef.CreateInstance<SkillDef>();
            utilityDef.activationState = new SerializableEntityStateType(typeof(EntityStates.MoffeinPotmobile.Boost.Boost));
            utilityDef.baseRechargeInterval = PotmobilePlugin.utilityCooldown;
            utilityDef.skillNameToken = "MOFFEINPOTMOBILEBODY_UTILITY_NAME";
            utilityDef.skillDescriptionToken = "MOFFEINPOTMOBILEBODY_UTILITY_DESCRIPTION";
            utilityDef.skillName = "Boost";
            utilityDef.icon = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Toolbot/ToolbotBodyToolbotDash.asset").WaitForCompletion().icon;
            utilityDef.baseMaxStock = PotmobilePlugin.utilityStocks;
            utilityDef.rechargeStock = 1;
            utilityDef.beginSkillCooldownOnSkillEnd = true;
            utilityDef.activationStateMachineName = "Boost";
            utilityDef.interruptPriority = InterruptPriority.Any;
            utilityDef.isCombatSkill = false;
            utilityDef.cancelSprintingOnActivation = false;
            utilityDef.canceledFromSprinting = false;
            utilityDef.mustKeyPress = false;
            utilityDef.requiredStock = 1;
            utilityDef.stockToConsume = 1;
            utilityDef.keywordTokens = new string[] { };
            (utilityDef as ScriptableObject).name = utilityDef.skillName;

            PotmobileContent.skillDefs.Add(utilityDef);
            PotmobileContent.entityStates.Add(typeof(EntityStates.MoffeinPotmobile.Boost.Boost));
            AddSkillToFamily(skillLocator.utility.skillFamily, utilityDef);
            PotmobileContent.SkillDefs.Boost = utilityDef;
        }

        private static void BuildSpecial(SkillLocator skillLocator)
        {
            SkillDef specialDef = SkillDef.CreateInstance<SkillDef>();
            specialDef.activationState = new SerializableEntityStateType(typeof(EntityStates.MoffeinPotmobile.Boost.Reset));
            specialDef.baseRechargeInterval = PotmobilePlugin.specialCooldown;
            specialDef.skillNameToken = "MOFFEINPOTMOBILEBODY_SPECIAL_NAME";
            specialDef.skillDescriptionToken = "MOFFEINPOTMOBILEBODY_SPECIAL_DESCRIPTION";
            specialDef.skillName = "Boost";
            specialDef.icon = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Toolbot/ToolbotBodySwap.asset").WaitForCompletion().icon;
            specialDef.baseMaxStock = PotmobilePlugin.specialStocks;
            specialDef.rechargeStock = 1;
            specialDef.beginSkillCooldownOnSkillEnd = true;
            specialDef.activationStateMachineName = "Boost";
            specialDef.interruptPriority = InterruptPriority.Skill;
            specialDef.isCombatSkill = false;
            specialDef.cancelSprintingOnActivation = false;
            specialDef.canceledFromSprinting = false;
            specialDef.mustKeyPress = true;
            specialDef.requiredStock = 1;
            specialDef.stockToConsume = 1;
            specialDef.keywordTokens = new string[] { };
            (specialDef as ScriptableObject).name = specialDef.skillName;

            PotmobileContent.skillDefs.Add(specialDef);
            PotmobileContent.entityStates.Add(typeof(EntityStates.MoffeinPotmobile.Boost.Reset));
            AddSkillToFamily(skillLocator.special.skillFamily, specialDef);
            PotmobileContent.SkillDefs.Reset = specialDef;
        }

        public static void CreateSkillFamilies(GameObject targetPrefab, bool destroyExisting = true)
        {
            if (destroyExisting)
            {
                foreach (GenericSkill obj in targetPrefab.GetComponentsInChildren<GenericSkill>())
                {
                    UnityEngine.Object.DestroyImmediate(obj);
                }
            }

            SkillLocator skillLocator = targetPrefab.GetComponent<SkillLocator>();

            skillLocator.primary = CreateGenericSkillWithSkillFamily(targetPrefab, "Primary");
            skillLocator.secondary = CreateGenericSkillWithSkillFamily(targetPrefab, "Secondary");
            skillLocator.utility = CreateGenericSkillWithSkillFamily(targetPrefab, "Utility");
            skillLocator.special = CreateGenericSkillWithSkillFamily(targetPrefab, "Special");
        }

        public static GenericSkill CreateGenericSkillWithSkillFamily(GameObject targetPrefab, string familyName, bool hidden = false)
        {
            GenericSkill skill = targetPrefab.AddComponent<GenericSkill>();
            skill.skillName = familyName;
            skill.hideInCharacterSelect = hidden;

            SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
            (newFamily as ScriptableObject).name = targetPrefab.name + familyName + "Family";
            newFamily.variants = new SkillFamily.Variant[0];

            skill._skillFamily = newFamily;

            PotmobileContent.skillFamilies.Add(newFamily);
            return skill;
        }
        public static void AddSkillToFamily(SkillFamily skillFamily, SkillDef skillDef, UnlockableDef unlockableDef = null)
        {
            Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);

            skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = skillDef,
                unlockableDef = unlockableDef,
                viewableNode = new ViewablesCatalog.Node(skillDef.skillNameToken, false, null)
            };
        }
    }
}

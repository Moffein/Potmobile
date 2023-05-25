using System;
using System.Collections.Generic;
using RoR2;
using UnityEngine;
using RoR2.Skills;
using EntityStates;
using UnityEngine.AddressableAssets;
using R2API;
using RoR2.Projectile;

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
            BuildPrimary(skillLocator);
            BuildUtility(skillLocator);

            SkillDef filler = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Heretic/HereticDefaultAbility.asset").WaitForCompletion();
            AddSkillToFamily(skillLocator.secondary.skillFamily, filler);
            AddSkillToFamily(skillLocator.special.skillFamily, filler);
        }


        private static void BuildPrimary(SkillLocator skillLocator)
        {
            CreateSkillFamilies(PotmobileContent.PotmobileBodyObject, true);

            SkillDef primaryDef = SkillDef.CreateInstance<SkillDef>();
            primaryDef.activationState = new SerializableEntityStateType(typeof(EntityStates.MoffeinPotmobile.Weapon.FireCannon));
            primaryDef.baseRechargeInterval = 0f;
            primaryDef.skillNameToken = "MOFFEINPOTMOBILE_PRIMARY_NAME";
            primaryDef.skillDescriptionToken = "MOFFEINPOTMOBILE_PRIMARY_DESC";
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
            PotmobileContent.entityStates.Add(typeof(EntityStates.MoffeinPotmobile.Weapon.FireCannon));
            AddSkillToFamily(skillLocator.primary.skillFamily, primaryDef);
            PotmobileContent.SkillDefs.FireCannon = primaryDef;

            GameObject projectilePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Toolbot/ToolbotGrenadeLauncherProjectile.prefab").WaitForCompletion().InstantiateClone("MoffeinPotmobileRocket", true);
            ProjectileImpactExplosion pie = projectilePrefab.GetComponent<ProjectileImpactExplosion>();
            pie.blastRadius = 8f;
            pie.falloffModel = BlastAttack.FalloffModel.None;
            pie.bonusBlastForce = Vector3.zero;
            PotmobileContent.projectilePrefabs.Add(projectilePrefab);
            EntityStates.MoffeinPotmobile.Weapon.FireCannon.projectilePrefab = projectilePrefab;
        }

        private static void BuildUtility(SkillLocator skillLocator)
        {
            SkillDef utilityDef = SkillDef.CreateInstance<SkillDef>();
            utilityDef.activationState = new SerializableEntityStateType(typeof(EntityStates.MoffeinPotmobile.Boost.Boost));
            utilityDef.baseRechargeInterval = 8f;
            utilityDef.skillNameToken = "MOFFEINPOTMOBILE_UTILITY_NAME";
            utilityDef.skillDescriptionToken = "MOFFEINPOTMOBILE_UTILITY_DESC";
            utilityDef.skillName = "Boost";
            utilityDef.icon = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Toolbot/ToolbotBodyToolbotDash.asset").WaitForCompletion().icon;
            utilityDef.baseMaxStock = 1;
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
            utilityDef.keywordTokens = new string[] { "KEYWORD_STUNNING" };
            (utilityDef as ScriptableObject).name = utilityDef.skillName;

            PotmobileContent.skillDefs.Add(utilityDef);
            PotmobileContent.entityStates.Add(typeof(EntityStates.MoffeinPotmobile.Boost.Boost));
            AddSkillToFamily(skillLocator.utility.skillFamily, utilityDef);
            PotmobileContent.SkillDefs.Boost = utilityDef;
        }

        private static void CreateSkillFamilies(GameObject targetPrefab, bool destroyExisting = true)
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
        private static void AddSkillToFamily(SkillFamily skillFamily, SkillDef skillDef, UnlockableDef unlockableDef = null)
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

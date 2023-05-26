using System;
using System.Collections.Generic;
using R2API;
using RoR2;
using RoR2.CharacterAI;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Potmobile
{
    public static class MasterSetup
    {
        private static bool initialized = false;
        public static void Init()
        {
            if (initialized) return;
            initialized = true;

            GameObject masterObject = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/CommandoMonsterMaster.prefab").WaitForCompletion().InstantiateClone("MoffeinPotmobileMaster", true);
            CharacterMaster cm = masterObject.GetComponent<CharacterMaster>();
            cm.bodyPrefab = PotmobileContent.PotmobileBodyObject;

            RemoveAISkillDrivers(masterObject);

            AddAISkillDriver(masterObject, "Reset", SkillSlot.Special, null,
                false, false,
                Mathf.NegativeInfinity, Mathf.Infinity,
                Mathf.NegativeInfinity, Mathf.Infinity,
                0f, Mathf.Infinity,
                false, false, false, -1,
                AISkillDriver.TargetType.CurrentEnemy,
                false, false, false,
                AISkillDriver.MovementType.ChaseMoveTarget, 1f,
                AISkillDriver.AimType.AtCurrentEnemy,
                false,
                true,
                false,
                AISkillDriver.ButtonPressType.Hold,
                0.25f,
                false,
                true,
                null);

            AddAISkillDriver(masterObject, "Boost", SkillSlot.Utility, null,
                false, false,
                Mathf.NegativeInfinity, Mathf.Infinity,
                Mathf.NegativeInfinity, Mathf.Infinity,
                20f, Mathf.Infinity,
                false, false, false, -1,
                AISkillDriver.TargetType.CurrentEnemy,
                false, false, false,
                AISkillDriver.MovementType.ChaseMoveTarget, 1f,
                AISkillDriver.AimType.AtCurrentEnemy,
                false,
                true,
                false,
                AISkillDriver.ButtonPressType.Hold,
                2f,
                false,
                true,
                null);

            AddAISkillDriver(masterObject, "Bounce", SkillSlot.Secondary, null,
                true, false,
                Mathf.NegativeInfinity, Mathf.Infinity,
                Mathf.NegativeInfinity, Mathf.Infinity,
                0f, 20f,
                false, false, false, -1,
                AISkillDriver.TargetType.CurrentEnemy,
                false, false, false,
                AISkillDriver.MovementType.ChaseMoveTarget, 1f,
                AISkillDriver.AimType.AtCurrentEnemy,
                false,
                false,
                false,
                AISkillDriver.ButtonPressType.Hold,
                0.5f,
                false,
                true,
                null);

            AddAISkillDriver(masterObject, "Primary", SkillSlot.Primary, null,
                true, false,
                Mathf.NegativeInfinity, Mathf.Infinity,
                Mathf.NegativeInfinity, Mathf.Infinity,
                0f, 90f,
                false, false, false, -1,
                AISkillDriver.TargetType.CurrentEnemy,
                false, false, false,
                AISkillDriver.MovementType.ChaseMoveTarget, 1f,
                AISkillDriver.AimType.AtCurrentEnemy,
                false,
                false,
                false,
                AISkillDriver.ButtonPressType.Hold,
                1.5f,
                false,
                false,
                null);

            AddAISkillDriver(masterObject, "Chase", SkillSlot.None, null,
                false, false,
                Mathf.NegativeInfinity, Mathf.Infinity,
                Mathf.NegativeInfinity, Mathf.Infinity,
                0f, Mathf.Infinity,
                false, false, false, -1,
                AISkillDriver.TargetType.CurrentEnemy,
                false, false, false,
                AISkillDriver.MovementType.ChaseMoveTarget, 1f,
                AISkillDriver.AimType.AtCurrentEnemy,
                false,
                true,
                false,
                AISkillDriver.ButtonPressType.Abstain,
                -1,
                false,
                false,
                null);

            PotmobileContent.PotmobileMasterObject = masterObject;
        }

        private static void RemoveAISkillDrivers(GameObject masterObject)
        {
            AISkillDriver[] skillDrivers = masterObject.GetComponents<AISkillDriver>();
            for (int i = 0; i < skillDrivers.Length; i++)
            {
                UnityEngine.Object.Destroy(skillDrivers[i]);
            }
        }

        private static AISkillDriver AddAISkillDriver(GameObject masterObject,
            string customName,
            SkillSlot skillSlot,
            RoR2.Skills.SkillDef requiredSkill,
            bool requireSkillReady,
            bool requireEquipmentReady,
            float minUserHealthFraction,
            float maxUserHealthFraction,
            float minTargetHealthFraction,
            float maxTargetHealthFraction,
            float minDistance,
            float maxDistance,
            bool selectionRequiresTargetLoS,
            bool selectionRequiresOnGround,
            bool selectionRequiresAimTarget,
            int maxTimesSelected,
            AISkillDriver.TargetType moveTargetType,
            bool activationRequiresTargetLoS,
            bool activationRequiresAimTargetLoS,
            bool activationRequiresAimConfirmation,
            AISkillDriver.MovementType movementType,
            float moveInputScale,
            AISkillDriver.AimType aimType,
            bool ignoreNodeGraph,
            bool shouldSprint,
            bool shouldFireEquipment,
            AISkillDriver.ButtonPressType buttonPressType,
            float driverUpdateTimerOverride,
            bool resetCurrentEnemyOnNextDriverSelection,
            bool noRepeat,
            AISkillDriver nextHighPriorityOverride)
        {
            //Make sure you're not accidentally adding them to something that's not a charactermaster
            CharacterMaster cm = masterObject.GetComponent<CharacterMaster>();
            if (!cm) return null;

            AISkillDriver newSkillDriver = masterObject.AddComponent<AISkillDriver>();
            newSkillDriver.customName = customName;
            newSkillDriver.skillSlot = skillSlot;
            newSkillDriver.requiredSkill = requiredSkill;
            newSkillDriver.requireSkillReady = requireSkillReady;
            newSkillDriver.requireEquipmentReady = requireEquipmentReady;
            newSkillDriver.minUserHealthFraction = minUserHealthFraction;
            newSkillDriver.maxUserHealthFraction = maxUserHealthFraction;
            newSkillDriver.minTargetHealthFraction = minTargetHealthFraction;
            newSkillDriver.maxTargetHealthFraction = maxTargetHealthFraction;
            newSkillDriver.minDistance = minDistance;
            newSkillDriver.maxDistance = maxDistance;
            newSkillDriver.selectionRequiresTargetLoS = selectionRequiresTargetLoS;
            newSkillDriver.selectionRequiresOnGround = selectionRequiresOnGround;
            newSkillDriver.selectionRequiresAimTarget = selectionRequiresAimTarget;
            newSkillDriver.maxTimesSelected = maxTimesSelected;
            newSkillDriver.moveTargetType = moveTargetType;
            newSkillDriver.activationRequiresTargetLoS = activationRequiresTargetLoS;
            newSkillDriver.activationRequiresAimConfirmation = activationRequiresAimConfirmation;
            newSkillDriver.activationRequiresAimTargetLoS = activationRequiresAimTargetLoS;
            newSkillDriver.movementType = movementType;
            newSkillDriver.moveInputScale = moveInputScale;
            newSkillDriver.aimType = aimType;
            newSkillDriver.ignoreNodeGraph = ignoreNodeGraph;
            newSkillDriver.shouldSprint = shouldSprint;
            newSkillDriver.shouldFireEquipment = shouldFireEquipment;
            newSkillDriver.buttonPressType = buttonPressType;
            newSkillDriver.driverUpdateTimerOverride = driverUpdateTimerOverride;
            newSkillDriver.resetCurrentEnemyOnNextDriverSelection = resetCurrentEnemyOnNextDriverSelection;
            newSkillDriver.noRepeat = noRepeat;
            newSkillDriver.nextHighPriorityOverride = nextHighPriorityOverride;

            return newSkillDriver;
        }
    }
}

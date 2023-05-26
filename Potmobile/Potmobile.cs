using BepInEx;
using EntityStates;
using Potmobile.Components;
using R2API;
using R2API.Utils;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Potmobile
{
    [BepInDependency("com.bepis.r2api")]
    [BepInDependency("com.bepis.r2api.prefab")]
    [BepInDependency("com.bepis.r2api.recalculatestats")]
    [BepInPlugin("com.Moffein.Potmobile", "Potmobile", "1.0.0")]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    public class Potmobile : BaseUnityPlugin
    {
        public void Awake()
        {
            BuildBodyObject();
            SkillSetup.Init();
            RoR2.ContentManagement.ContentManager.collectContentPackProviders += ContentManager_collectContentPackProviders;
        }

        private void BuildBodyObject()
        {
            if (PotmobileContent.PotmobileBodyObject) return;

            GameObject bodyObject = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/PotMobile/PotMobileBody.prefab").WaitForCompletion(), "MoffeinPotmobileBody", true);

            bodyObject.AddComponent<SpeedController>(); //Allows it to benefit from move speed
            bodyObject.AddComponent<EquipmentSlot>();   //Fixes Equipment not working.

            //Fix Out of Bounds teleport
            bodyObject.layer = 0;

            CharacterBody cb = bodyObject.GetComponent<CharacterBody>();
            cb.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
            cb.bodyFlags |= CharacterBody.BodyFlags.Mechanical;
            cb._defaultCrosshairPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Toolbot/ToolbotGrenadeLauncherCrosshair.prefab").WaitForCompletion();

            cb.baseMaxHealth = 480;
            cb.levelMaxHealth = 144f;
            cb.baseArmor = 25f;
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

            PotmobileContent.PotmobileBodyObject = bodyObject;
        }

        private void ContentManager_collectContentPackProviders(RoR2.ContentManagement.ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(new PotmobileContent());
        }
    }
}

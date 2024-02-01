using Potmobile.Components;
using R2API;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.AddressableAssets;
using UnityEngine;
using BepInEx;
using BepInEx.Configuration;
using EntityStates;
using Potmobile.Components;
using Potmobile.Cores;
using Potmobile.Survivors.Hauler;
using R2API;
using R2API.Utils;
using RiskOfOptions;
using RoR2;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static Potmobile.PotmobilePlugin;

namespace Potmobile.Survivors.Potmobile
{
    internal class PotmobileCore
    {
        public PotmobileCore()
        {
            BuildPotmobileBodyObject();
            PotmobileSkins.InitSkins();
            CreatePotmobileSurvivorDef();
            SkillSetup.Init();
            MasterSetup.CreatePotmobileMaster();
        }


        private void BuildPotmobileBodyObject()
        {
            if (PotmobileContent.PotmobileBodyObject) return;

            GameObject bodyObject = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/PotMobile/PotMobileBody.prefab").WaitForCompletion(), "MoffeinPotmobileBody", true);

            SpeedController sc = bodyObject.AddComponent<SpeedController>(); //Allows it to benefit from move speed
            sc.minOverlapDamageCoefficient = potmobileMinRamDamage;
            sc.minOverlapSpeed = potmobileMinDamageSpeed;
            sc.doubleDamageOverlapSpeed = potmobileDoubleDamageSpeed;
            sc.ramHitboxSize = new Vector3(6f, 3f, 6f);
            sc.reverseSpeedCoefficient = potmobileReverseCoefficient;

            bodyObject.AddComponent<EquipmentSlot>();   //Fixes Equipment not working.
            bodyObject.AddComponent<GiveItemsOnSpawn>();   //Prevents AI Potmobiles from spawning in the ground and instantly dying
            bodyObject.AddComponent<PotmobileNetworkComponent>();   //Used to squash things

            DeathRewards dr = bodyObject.GetComponent<DeathRewards>();
            if (!dr) dr = bodyObject.AddComponent<DeathRewards>();
            dr.logUnlockableDef = null;
            dr.bossPickup = new SerializablePickupIndex { pickupName = "" };

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
            cb.bodyFlags |= CharacterBody.BodyFlags.ImmuneToExecutes;
            cb._defaultCrosshairPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Toolbot/ToolbotGrenadeLauncherCrosshair.prefab").WaitForCompletion();
            cb.portraitIcon = Assets.assetBundle.LoadAsset<Texture2D>("texIconPotmobile.png");

            cb.baseNameToken = "MOFFEINPOTMOBILEBODY_NAME";
            cb.subtitleNameToken = "MOFFEINPOTMOBILEBODY_SUBTITLE";
            cb.baseMaxHealth = 480f;
            cb.levelMaxHealth = 144f;
            cb.baseArmor = 0;
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
                UnityEngine.Object.Destroy(existingHurtboxes[i]);
            }
            HurtBoxGroup existingHBG = bodyObject.GetComponentInChildren<HurtBoxGroup>();
            if (existingHBG) UnityEngine.Object.Destroy(existingHBG);

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
                UnityEngine.Object.Destroy(vfz);
            }
            else
            {
                vfz.impactMultiplier = potmobileImpactMult;
            }

            HoverVehicleMotor hvm = bodyObject.GetComponent<HoverVehicleMotor>();
            hvm.motorForce = potmobileForce;

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
    }
}

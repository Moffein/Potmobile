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

namespace Potmobile.Survivors.Hauler
{
    internal class HaulerCore
    {
        public HaulerCore() => Setup();

        private void Setup()
        {
            BuildHaulerBodyObject();
            HaulerSkins.InitSkins();
            CreateHaulerSurvivorDef();
            HaulerSkillSetup();
            MasterSetup.CreateHaulerMaster();

            RoR2.ContentManagement.ContentManager.onContentPacksAssigned += ContentManager_onContentPacksAssigned;
        }

        private void ContentManager_onContentPacksAssigned(HG.ReadOnlyArray<RoR2.ContentManagement.ReadOnlyContentPack> obj)
        {
            HaulerItemDisplays.RegisterDisplays();
        }

        private void BuildHaulerBodyObject()
        {
            if (PotmobileContent.HaulerBodyObject) return;
            GameObject bodyObject = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/Hauler/HaulerBody.prefab").WaitForCompletion(), "MoffeinHaulerBody", true);

            SpeedController sc = bodyObject.AddComponent<SpeedController>(); //Allows it to benefit from move speed
            sc.minOverlapDamageCoefficient = haulerMinRamDamage;
            sc.minOverlapSpeed = haulerMinDamageSpeed;
            sc.doubleDamageOverlapSpeed = haulerDoubleDamageSpeed;
            sc.reverseSpeedCoefficient = haulerReverseCoefficient;
            sc.ramHitboxSize = new Vector3(7.7f, 4.2f, 14f);

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

            //Syncing with Jellyfish (closest example)
            //SfxLocator sfxLocator = bodyObject.AddComponent<SfxLocator>(); atypical system so skip
            Transform modelBaseTransform = bodyObject.transform.Find("Model Base");
            Transform mdlHaulerTransform = bodyObject.transform.Find("Model Base/mdlHauler");
            Transform haulerMeshTransform = bodyObject.transform.Find("Model Base/mdlHauler/HaulerMesh");

            //maybe this is stupid
            //var cacheMesh = haulerMeshTransform.GetComponent<MeshFilter>().sharedMesh;
            //var cacheRenderer = haulerMeshTransform.GetComponent<MeshRenderer>().sharedMaterials;
            //var smr = haulerMeshTransform.gameObject.AddComponent<SkinnedMeshRenderer>();
            //smr.sharedMesh = cacheMesh;
            //smr.sharedMaterials = cacheRenderer;
            //Destroy(haulerMeshTransform.GetComponent<MeshFilter>());
            //Destroy(haulerMeshTransform.GetComponent<MeshRenderer>());


            Transform AddChild(Transform parent, string name)
            {
                var go = new GameObject(name);
                var got = go.transform;
                got.SetParent(parent);
                got.localPosition = Vector3.zero;
                got.localRotation = Quaternion.identity;
                return got;
            }
            Transform armatureTransform = AddChild(mdlHaulerTransform, "HaulerArmature");
            Transform seatDriver = AddChild(armatureTransform, "Seat.Driver");
            Transform seatShotgun = AddChild(armatureTransform, "Seat.Shotgun");
            Transform seatBackDriver = AddChild(armatureTransform, "Seat.BackDriver");
            Transform seatBackShotgun = AddChild(armatureTransform, "Seat.BackShotgun");
            Transform exhaustTransform = AddChild(armatureTransform, "ExhaustPoint");

            Transform wheelFrontL = mdlHaulerTransform.Find("Front.L");
            // this / WheelContactPoint / HaulerWheelMesh
            //meshfilter/meshrenderer
            Transform wheelFrontR = mdlHaulerTransform.Find("Front.R");
            Transform wheelBackL = mdlHaulerTransform.Find("Back.L");
            Transform wheelBackR = mdlHaulerTransform.Find("Back.R");
            void GetWheelComponents(Transform wheelTransform, out MeshFilter meshFilter, out MeshRenderer meshRenderer)
            {
                var target = wheelTransform.Find("WheelContactPoint/HaulerWheelMesh");
                meshFilter = target.GetComponent<MeshFilter>();
                meshRenderer = target.GetComponent<MeshRenderer>();
            }

            if (modelBaseTransform.GetComponent<ChildLocator>())
            {
                Debug.Log($"Base Transform has ChildLocator, deleting.");
                UnityEngine.Object.Destroy(modelBaseTransform.GetComponent<ChildLocator>());
            }
            ChildLocator childLocator = mdlHaulerTransform.GetComponent<ChildLocator>();
            if (!mdlHaulerTransform.GetComponent<ChildLocator>())
            {
                Debug.Log("mdlHauler missing childlocator, adding.");
                childLocator = mdlHaulerTransform.gameObject.AddComponent<ChildLocator>();
            }
            childLocator.transformPairs = new ChildLocator.NameTransformPair[]
            {
                new ChildLocator.NameTransformPair
                {
                    name = "armatureRoot",
                    transform = armatureTransform,
                },
                new ChildLocator.NameTransformPair
                {
                    name = "SeatDriver",
                    transform = seatDriver,
                },
                new ChildLocator.NameTransformPair
                {
                    name = "SeatShotgun",
                    transform = seatShotgun,
                },
                new ChildLocator.NameTransformPair
                {
                    name = "SeatBackDriver",
                    transform = seatBackDriver,
                },
                new ChildLocator.NameTransformPair
                {
                    name = "SeatBackShotgun",
                    transform = seatBackShotgun,
                },
                new ChildLocator.NameTransformPair
                {
                    name = "WheelFrontL",
                    transform = wheelFrontL,
                },
                new ChildLocator.NameTransformPair
                {
                    name = "WheelFrontR",
                    transform = wheelFrontR,
                },
                new ChildLocator.NameTransformPair
                {
                    name = "WheelBackL",
                    transform = wheelBackL,
                },
                new ChildLocator.NameTransformPair
                {
                    name = "WheelBackR",
                    transform = wheelBackR,
                },
                new ChildLocator.NameTransformPair
                {
                    name = "ExhautPoint",
                    transform = exhaustTransform
                }
            };
            CharacterModel characterModel = mdlHaulerTransform.GetComponent<CharacterModel>();
            if (!characterModel) Debug.Log($"{mdlHaulerTransform.name} does not have a CharacterModel. adding.");
            characterModel = mdlHaulerTransform.gameObject.AddComponent<CharacterModel>();
            characterModel.body = bodyObject.GetComponent<CharacterBody>();

            //set in awake to first in baserendererinfos
            //characterModel.mainSkinnedMeshRenderer = smr;

            //FUCK
            //EVERYTHING
            //
            // ☆*: .｡. o(≧▽≦)o .｡.:*☆
            //
            var jeepPrefab = UnityEngine.Object.Instantiate(Assets.assetBundle.LoadAsset<GameObject>("mdlHaulerJeep.prefab"), mdlHaulerTransform);
            jeepPrefab.transform.localPosition = Vector3.zero;
            jeepPrefab.SetActive(false);

            var irohaPrefab = UnityEngine.Object.Instantiate(Assets.assetBundle.LoadAsset<GameObject>("TankGirlPrefab.prefab"), mdlHaulerTransform);
            irohaPrefab.transform.localPosition = Vector3.zero;
            irohaPrefab.SetActive(false);

            //ModelPanelParameters modelPanelParameters = mdlHaulerTransform.gameObject.AddComponent<ModelPanelParameters>();
            //needs to be configured. or else it throws something

            //no point in doing auto if we're doing gameobjectactivation skindef jank
            GetWheelComponents(wheelFrontL, out MeshFilter _, out MeshRenderer wheelFrontLMR);
            GetWheelComponents(wheelFrontR, out MeshFilter _, out MeshRenderer wheelFrontRMR);
            GetWheelComponents(wheelBackL, out MeshFilter _, out MeshRenderer wheelBackLMR);
            GetWheelComponents(wheelBackR, out MeshFilter _, out MeshRenderer wheelBackRMR);

            List<CharacterModel.RendererInfo> rendererInfos = new List<CharacterModel.RendererInfo>();
            MeshRenderer[] meshRenderers = new MeshRenderer[] { wheelFrontLMR, wheelFrontRMR, wheelBackLMR, wheelBackRMR };

            foreach (var renderer in meshRenderers)
            {
                CharacterModel.RendererInfo rendererInfo = new CharacterModel.RendererInfo
                {
                    renderer = renderer,
                    defaultMaterial = renderer.sharedMaterial,
                    defaultShadowCastingMode = renderer.shadowCastingMode
                };
                rendererInfos.Add(rendererInfo);
            }

            /*
            MeshRenderer[] meshRenderers = mdlHaulerTransform.GetComponentsInChildren<MeshRenderer>();
            List<CharacterModel.RendererInfo> rendererInfos = new List<CharacterModel.RendererInfo>();
            for (int i = meshRenderers.Length - 1; i >= 0; i--)
            {
                MeshRenderer mesh = meshRenderers[i];
                CharacterModel.RendererInfo rendererInfo = new CharacterModel.RendererInfo
                {
                    renderer = mesh,
                    defaultMaterial = mesh.sharedMaterial,
                    defaultShadowCastingMode = mesh.shadowCastingMode
                };
                if (rendererInfo.renderer.name == "HaulerMesh") continue;
                //Skipping because the first SMR seemingly vanishes afterwards ???, but doesnt get set... so.. why???
                //rendererInfo.renderer = smr;
                rendererInfos.Add(rendererInfo);
                Debug.Log($"Adding base renderer info for renderer {rendererInfo.renderer.name}");
            }*/

            characterModel.baseRendererInfos = rendererInfos.ToArray();

            CharacterBody cb = bodyObject.GetComponent<CharacterBody>();
            cb.bodyColor = new Color32(34, 71, 224, 255);
            cb.name = "MoffeinHaulerBody";
            cb.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
            cb.bodyFlags |= CharacterBody.BodyFlags.Mechanical;
            cb.bodyFlags |= CharacterBody.BodyFlags.ImmuneToExecutes;
            cb._defaultCrosshairPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Toolbot/ToolbotGrenadeLauncherCrosshair.prefab").WaitForCompletion();
            cb.portraitIcon = Assets.assetBundle.LoadAsset<Texture2D>("texIconHauler.png");

            cb.baseNameToken = "MOFFEINHAULERBODY_NAME";
            cb.subtitleNameToken = "MOFFEINHAULERBODY_SUBTITLE";
            cb.baseMaxHealth = 600f;
            cb.levelMaxHealth = 180f;
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
                UnityEngine.Object.Destroy(existingHurtboxes[i]);
            }
            HurtBoxGroup existingHBG = bodyObject.GetComponentInChildren<HurtBoxGroup>();
            if (existingHBG) UnityEngine.Object.Destroy(existingHBG);

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
                UnityEngine.Object.Destroy(vfz);
            }
            else
            {
                vfz.impactMultiplier = haulerImpactMult;
            }

            HoverVehicleMotor hvm = bodyObject.GetComponent<HoverVehicleMotor>();
            hvm.motorForce = haulerForce;

            PotmobileContent.HaulerBodyObject = bodyObject;
        }

        private void CreateHaulerSurvivorDef()
        {
            //GameObject displayObject = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/Hauler/HaulerBody.prefab").WaitForCompletion(), "MoffeinHaulerDisplay", false);
            //displayObject = displayObject.GetComponent<ModelLocator>().modelTransform.gameObject;
            //displayObject.transform.localScale *= 0.25f;
            GameObject displayObject = PrefabAPI.InstantiateClone(PotmobileContent.HaulerBodyObject.transform.Find("Model Base").gameObject, "MoffeinHaulerDisplay");
            displayObject.transform.localScale *= 0.25f;
            var mdlHauler = displayObject.transform.Find("mdlHauler");
            mdlHauler.transform.localPosition = Vector3.up;
            mdlHauler.GetComponent<HurtBoxGroup>().enabled = false;
            mdlHauler.GetComponent<HurtBox>().enabled = false;
            //mdlHauler.transform.Find("Front.L").GetComponent<HoverEngine>().enabled = false;
            mdlHauler.transform.Find("ForceBox").gameObject.SetActive(false);


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
    }
}

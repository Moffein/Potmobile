using BepInEx;
using EntityStates;
using Potmobile.Components;
using R2API;
using R2API.Utils;
using RoR2;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace Potmobile
{
    [BepInDependency("com.bepis.r2api")]
    [BepInDependency("com.bepis.r2api.prefab")]
    [BepInDependency("com.bepis.r2api.damagetype")]
    [BepInDependency("com.bepis.r2api.recalculatestats")]
    [BepInPlugin("com.Moffein.Potmobile", "Potmobile", "1.0.0")]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    public class Potmobile : BaseUnityPlugin
    {
        public static PluginInfo pluginInfo;
        private static float sortPosition = 9999f;
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
            RoR2.ContentManagement.ContentManager.collectContentPackProviders += ContentManager_collectContentPackProviders;
            RoR2.RoR2Application.onLoad += LateSetup;
        }

        private void ReadConfig()
        {

        }

        private void LateSetup()
        {
            PotmobileContent.PotmobileBodyIndex = BodyCatalog.FindBodyIndex("MoffeinPotmobileBody");
        }

        private void BuildBodyObject()
        {
            if (PotmobileContent.PotmobileBodyObject) return;

            GameObject bodyObject = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/PotMobile/PotMobileBody.prefab").WaitForCompletion(), "MoffeinPotmobileBody", true);

            bodyObject.AddComponent<SpeedController>(); //Allows it to benefit from move speed
            bodyObject.AddComponent<EquipmentSlot>();   //Fixes Equipment not working.
            bodyObject.AddComponent<GiveOOBItem>();   //Prevents AI Potmobiles from spawning in the ground and instantly dying

            //Fix interactor
            Interactor interactor = bodyObject.AddComponent<Interactor>();
            interactor.maxInteractionDistance = 6f;

            InteractionDriver id = bodyObject.AddComponent<InteractionDriver>();
            id.highlightInteractor = true;

            //Fix Out of Bounds teleport
            bodyObject.layer = 0;

            CharacterBody cb = bodyObject.GetComponent<CharacterBody>();
            cb.bodyColor = new Color32(200, 200, 200, 255);
            cb.name = "MoffeinPotmobileBody";
            cb.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
            cb.bodyFlags |= CharacterBody.BodyFlags.Mechanical;
            cb._defaultCrosshairPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Toolbot/ToolbotGrenadeLauncherCrosshair.prefab").WaitForCompletion();

            cb.baseNameToken = "MOFFEINPOTMOBILE_BODY_NAME";
            cb.subtitleNameToken = "MOFFEINPOTMOBILE_BODY_SUBTITLE";
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
            if (!bodyObject.GetComponent<SetStateOnHurt>())
            {
                SetStateOnHurt ssoh = bodyObject.AddComponent<SetStateOnHurt>();
                ssoh.canBeFrozen = false;   //setting this to true doesnt seem to enable freezing, so I'll just leave it off
                ssoh.canBeHitStunned = false;
                ssoh.canBeStunned = false;
                ssoh.idleStateMachine = new EntityStateMachine[] { EntityStateMachine.FindByCustomName(bodyObject, "Body"), EntityStateMachine.FindByCustomName(bodyObject, "Weapon"), boostMachine };
            }

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
            sd.descriptionToken = "MOFFEINPOTMOBILE_BODY_DESCRIPTION";
            sd.displayPrefab = displayObject;
            sd.mainEndingEscapeFailureFlavorToken = "MOFFEINPOTMOBILE_BODY_MAIN_ENDING_ESCAPE_FAILURE_FLAVOR";
            (sd as ScriptableObject).name = sd.cachedName;
            PotmobileContent.PotmobileSurvivorDef = sd;
        }

        private void ContentManager_collectContentPackProviders(RoR2.ContentManagement.ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(new PotmobileContent());
        }
    }
}

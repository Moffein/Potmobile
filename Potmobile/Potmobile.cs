using BepInEx;
using EntityStates;
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

            //Mapzone teleports gameobjects on layer 0
            /*On.RoR2.MapZone.TryZoneStart += (orig, self, other) =>
            {
                Debug.Log(other.gameObject.layer);
                orig(self, other);
            };*/

            GameObject bodyObject = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/PotMobile/PotMobileBody.prefab").WaitForCompletion(), "MoffeinPotmobileBody", true);

            //Fix Out of Bounds teleport
            /*Debug.Log("Commando Layer: " + Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/CommandoBody.prefab").WaitForCompletion().layer);    //0
            Debug.Log("Potmobile Layer: " + PotmobileBodyObject.layer); //17
            Debug.Log("Player Layer: " + LayerIndex.playerBody.intVal); //22
            */
            PotmobileContent.PotmobileBodyObject.layer = 0;

            CharacterBody cb = PotmobileContent.PotmobileBodyObject.GetComponent<CharacterBody>();
            cb.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
            cb.bodyFlags |= CharacterBody.BodyFlags.Mechanical;
            cb._defaultCrosshairPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Toolbot/ToolbotGrenadeLauncherCrosshair.prefab").WaitForCompletion();

            //Lower max health and add armor, keeps EHP the same but makes flat healing items a bit better.
            cb.baseMaxHealth = 480f;
            cb.levelMaxHealth = 144f;
            cb.baseArmor = 25f;


            NetworkStateMachine nsm = bodyObject.GetComponent<NetworkStateMachine>();

            EntityStateMachine boostMachine = bodyObject.AddComponent<EntityStateMachine>();
            boostMachine.customName = "Boost";
            boostMachine.initialStateType = new SerializableEntityStateType(typeof(EntityStates.Idle));
            boostMachine.mainStateType = new SerializableEntityStateType(typeof(EntityStates.Idle));
            nsm.stateMachines.Append(boostMachine).ToArray();

            PotmobileContent.PotmobileBodyObject = bodyObject;
        }

        private void ContentManager_collectContentPackProviders(RoR2.ContentManagement.ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(new PotmobileContent());
        }
    }
}

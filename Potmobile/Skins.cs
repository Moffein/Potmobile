using System;
using System.Collections.Generic;
using RoR2;
using UnityEngine;
using R2API;
using UnityEngine.AddressableAssets;

namespace Potmobile
{
    //Based off of HenryMod code.
    public class Skins
    {
        public static SkinDef jeepSkinDef;

        public enum SkinGOSelection
        {
            Base,
            Jeep
        }

        public static void SetupJankGOToggles(SkinDef skinDef, SkinGOSelection index)
        {
            skinDef.gameObjectActivations = new SkinDef.GameObjectActivation[]
            {
                new SkinDef.GameObjectActivation
                {
                    gameObject = PotmobileContent.HaulerBodyObject.transform.Find("Model Base/mdlHauler/HaulerMesh").gameObject,
                    shouldActivate = index == SkinGOSelection.Base
                },
                new SkinDef.GameObjectActivation
                {
                    gameObject = PotmobileContent.HaulerBodyObject.transform.Find("Model Base/mdlHauler/mdlHaulerJeep(Clone)").gameObject,
                    shouldActivate = index == SkinGOSelection.Jeep
                }
            };
        }

        public static void InitSkins(GameObject bodyPrefab)
        {
            if (!bodyPrefab) return;

            GameObject model = bodyPrefab.GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
            CharacterModel characterModel = model.GetComponent<CharacterModel>();

            ModelSkinController skinController = model.GetComponent<ModelSkinController>();
            if (!skinController) skinController = model.AddComponent<ModelSkinController>();

            ChildLocator childLocator = model.GetComponent<ChildLocator>();

            SkinnedMeshRenderer mainRenderer = null;
            CharacterModel.RendererInfo[] defaultRenderers = null;
            if (characterModel)
            {
                //mainRenderer = characterModel.mainSkinnedMeshRenderer;
                /*if (!mainRenderer && bodyPrefab == PotmobileContent.HaulerBodyObject)
                {
                    Debug.Log($"wee woo wee woo");
                    mainRenderer = bodyPrefab.GetComponentInChildren<SkinnedMeshRenderer>();
                    if (!mainRenderer) Debug.Log($"uh captain? i do say shit");
                }*/
                defaultRenderers = characterModel.baseRendererInfos;
            }

            List<SkinDef> skins = new List<SkinDef>();

            #region DefaultSkin
            SkinDef defaultSkin = CreateSkinDef("DEFAULT_SKIN",
                (bodyPrefab == PotmobileContent.HaulerBodyObject ? Assets.assetBundle.LoadAsset<Sprite>("texIconHauler.png") : Assets.assetBundle.LoadAsset<Sprite>("texIconPotmobile.png")),
                defaultRenderers,
                mainRenderer,
                model);

            defaultSkin.meshReplacements = new SkinDef.MeshReplacement[] { };
            if (bodyPrefab == PotmobileContent.HaulerBodyObject)
            {
                SetupJankGOToggles(defaultSkin, SkinGOSelection.Base);
                MeshRenderer haulerMR = PotmobileContent.HaulerBodyObject.transform.Find("Model Base/mdlHauler/HaulerMesh").GetComponent<MeshRenderer>();
                var haulerRendererInfo = new CharacterModel.RendererInfo()
                {
                    defaultMaterial = haulerMR.sharedMaterial,
                    defaultShadowCastingMode = haulerMR.shadowCastingMode,
                    hideOnDeath = false,
                    ignoreOverlays = false,
                    renderer = haulerMR
                };
                HG.ArrayUtils.ArrayInsert<CharacterModel.RendererInfo>(ref defaultSkin.rendererInfos, 0, in haulerRendererInfo);
            }
            skins.Add(defaultSkin);
            #endregion

            #region Hauler Jeep Skin

            if (bodyPrefab == PotmobileContent.HaulerBodyObject)
            {
                jeepSkinDef = CreateSkinDef("MOFFEINHAULERBODY_SKIN_GEEP_NAME",
                Assets.assetBundle.LoadAsset<Sprite>("Assets/Resources/texIconJeep.png"),
                defaultRenderers,
                mainRenderer,
                model);
                //body + wheels

                var jeepMeshMR = PotmobileContent.HaulerBodyObject.transform.Find("Model Base/mdlHauler/mdlHaulerJeep(Clone)").GetComponent<MeshRenderer>();
                SetupJankGOToggles(jeepSkinDef, SkinGOSelection.Jeep);
                var jeepRendererInfo = new CharacterModel.RendererInfo()
                {
                    defaultMaterial = jeepMeshMR.sharedMaterial,
                    defaultShadowCastingMode = jeepMeshMR.shadowCastingMode,
                    hideOnDeath = false,
                    ignoreOverlays = false,
                    renderer = jeepMeshMR
                };
                HG.ArrayUtils.ArrayInsert<CharacterModel.RendererInfo>(ref jeepSkinDef.rendererInfos, 0, in jeepRendererInfo);
                /*jeepSkinDef.meshReplacements = new SkinDef.MeshReplacement[]
                {
                    new SkinDef.MeshReplacement
                    {
                        mesh = Assets.jeepMesh,
                        renderer = mainRenderer
                    }
                };*/
                skins.Add(jeepSkinDef);
            }

            #endregion

            skinController.skins = skins.ToArray();
        }

        public static SkinDef CreateSkinDef(string skinName, Sprite skinIcon, CharacterModel.RendererInfo[] rendererInfos, SkinnedMeshRenderer mainRenderer, GameObject root)
        {
            return CreateSkinDef(skinName, skinIcon, rendererInfos, mainRenderer, root, null);
        }

        public static SkinDef CreateSkinDef(string skinName, Sprite skinIcon, CharacterModel.RendererInfo[] rendererInfos, SkinnedMeshRenderer mainRenderer, GameObject root, UnlockableDef unlockableDef)
        {
            LoadoutAPI.SkinDefInfo skinDefInfo = new LoadoutAPI.SkinDefInfo
            {
                BaseSkins = Array.Empty<SkinDef>(),
                GameObjectActivations = new SkinDef.GameObjectActivation[0],
                Icon = skinIcon,
                MeshReplacements = new SkinDef.MeshReplacement[0],
                MinionSkinReplacements = new SkinDef.MinionSkinReplacement[0],
                Name = skinName,
                NameToken = skinName,
                ProjectileGhostReplacements = new SkinDef.ProjectileGhostReplacement[0],
                RendererInfos = rendererInfos,
                RootObject = root,
                UnlockableDef = unlockableDef
            };

            On.RoR2.SkinDef.Awake += DoNothing;

            SkinDef skinDef = ScriptableObject.CreateInstance<RoR2.SkinDef>();
            skinDef.baseSkins = skinDefInfo.BaseSkins;
            skinDef.icon = skinDefInfo.Icon;
            skinDef.unlockableDef = skinDefInfo.UnlockableDef;
            skinDef.rootObject = skinDefInfo.RootObject;
            skinDef.rendererInfos = skinDefInfo.RendererInfos;
            skinDef.gameObjectActivations = skinDefInfo.GameObjectActivations;
            skinDef.meshReplacements = skinDefInfo.MeshReplacements;
            skinDef.projectileGhostReplacements = skinDefInfo.ProjectileGhostReplacements;
            skinDef.minionSkinReplacements = skinDefInfo.MinionSkinReplacements;
            skinDef.nameToken = skinDefInfo.NameToken;
            skinDef.name = skinDefInfo.Name;

            On.RoR2.SkinDef.Awake -= DoNothing;

            return skinDef;
        }


        private static void DoNothing(On.RoR2.SkinDef.orig_Awake orig, RoR2.SkinDef self)
        {
        }
    }
}

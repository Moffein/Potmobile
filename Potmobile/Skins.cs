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
                mainRenderer = characterModel.mainSkinnedMeshRenderer;
                defaultRenderers = characterModel.baseRendererInfos;
            }

            List<SkinDef> skins = new List<SkinDef>();

            #region DefaultSkin
            SkinDef defaultSkin = CreateSkinDef("DEFAULT_SKIN",
                Assets.assetBundle.LoadAsset<Sprite>("texModIcon.png"),
                defaultRenderers,
                mainRenderer,
                model);

            defaultSkin.meshReplacements = new SkinDef.MeshReplacement[] { };

            skins.Add(defaultSkin);
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

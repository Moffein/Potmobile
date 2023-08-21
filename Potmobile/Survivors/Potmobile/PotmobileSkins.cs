using System;
using System.Collections.Generic;
using RoR2;
using UnityEngine;
using R2API;
using UnityEngine.AddressableAssets;

namespace Potmobile.Survivors.Potmobile
{
    internal class PotmobileSkins
    {
        public static List<SkinDef> skins = new List<SkinDef>();

        public static void InitSkins()
        {
            GameObject model = PotmobileContent.PotmobileBodyObject.GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
            CharacterModel characterModel = model.GetComponent<CharacterModel>();

            ModelSkinController skinController = model.GetComponent<ModelSkinController>();
            if (!skinController) skinController = model.AddComponent<ModelSkinController>();

            ChildLocator childLocator = model.GetComponent<ChildLocator>();

            SkinnedMeshRenderer mainRenderer = null;
            CharacterModel.RendererInfo[] defaultRenderers = null;
            if (characterModel)
            {
                defaultRenderers = characterModel.baseRendererInfos;
            }

            List<SkinDef> skins = new List<SkinDef>();

            SkinDef defaultSkin = Skins.CreateSkinDef("DEFAULT_SKIN",
                Assets.assetBundle.LoadAsset<Sprite>("texIconPotmobile.png"),
                defaultRenderers,
                mainRenderer,
                model);
            skins.Add(defaultSkin);
            skinController.skins = skins.ToArray();
        }
    }
}

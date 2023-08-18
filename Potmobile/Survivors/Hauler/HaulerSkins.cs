using System;
using System.Collections.Generic;
using RoR2;
using UnityEngine;
using R2API;
using UnityEngine.AddressableAssets;
using R2API.Utils;
using System.Text;

namespace Potmobile.Survivors.Hauler
{
    public static class HaulerSkins
    {
        public static List<SkinDef> skins = new List<SkinDef>();
        public static SkinDef jeepSkinDef;

        public static Dictionary<string, GameObject> skinGOSelection = new Dictionary<string, GameObject>();
        public static Dictionary<string, SkinDef> skinIdentifiers = new Dictionary<string, SkinDef>();

        public static GameObject AutoAddGameObjectActivations(SkinDef skinDef, string identifier, GameObject originalPrefab, bool shouldCopy = true)
        {
            var mdlHaulerTransform = PotmobileContent.HaulerBodyObject.transform.Find("Model Base/mdlHauler");

            GameObject copyPrefab = shouldCopy ?
                UnityEngine.Object.Instantiate(originalPrefab) : originalPrefab;
            copyPrefab.transform.SetParent(mdlHaulerTransform);
            copyPrefab.transform.localPosition = Vector3.zero;
            copyPrefab.SetActive(!shouldCopy);

            skinGOSelection[identifier] = copyPrefab;
            skinIdentifiers[identifier] = skinDef;
            skins.Add(skinDef);

            foreach (var skin in skins)
            {
                List<SkinDef.GameObjectActivation> gameObjectActivations = new List<SkinDef.GameObjectActivation>();
                foreach (var selection in skinGOSelection)
                {
                    var goIdentifier = selection.Key;
                    var go = selection.Value;
                    var goSkin = skinIdentifiers[goIdentifier];

                    var activation = new SkinDef.GameObjectActivation()
                    {
                        gameObject = go,
                        shouldActivate = skin == goSkin,
                    };
                    gameObjectActivations.Add(activation);
                }
                skin.gameObjectActivations = gameObjectActivations.ToArray();
            }

            GameObject model = PotmobileContent.HaulerBodyObject.GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
            ModelSkinController skinController = model.GetComponent<ModelSkinController>();
            skinController.skins = skins.ToArray();
            return copyPrefab;
        }

        public static void InitSkins()
        {
            #region Setup
            GameObject model = PotmobileContent.HaulerBodyObject.GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
            CharacterModel characterModel = model.GetComponent<CharacterModel>();

            ModelSkinController skinController = model.GetComponent<ModelSkinController>();
            if (!skinController) skinController = model.AddComponent<ModelSkinController>();

            //ChildLocator childLocator = model.GetComponent<ChildLocator>();

            SkinnedMeshRenderer mainRenderer = null;
            CharacterModel.RendererInfo[] defaultRenderers = null;
            if (characterModel)
            {
                defaultRenderers = characterModel.baseRendererInfos;
            }
            #endregion
            #region Default

            //skinGOSelection.Add("Hauler", PotmobileContent.HaulerBodyObject.transform.Find("Model Base/mdlHauler/HaulerMesh").gameObject);
            SkinDef defaultSkin = Skins.CreateSkinDef("DEFAULT_SKIN",
                Assets.assetBundle.LoadAsset<Sprite>("texIconHauler.png"),
                defaultRenderers,
                mainRenderer,
                model);

            AutoAddGameObjectActivations(defaultSkin, "Hauler", PotmobileContent.HaulerBodyObject.transform.Find("Model Base/mdlHauler/HaulerMesh").gameObject, false);
            MeshRenderer haulerMR = skinGOSelection["Hauler"].GetComponent<MeshRenderer>();
            var haulerRendererInfo = new CharacterModel.RendererInfo()
            {
                defaultMaterial = haulerMR.sharedMaterial,
                defaultShadowCastingMode = haulerMR.shadowCastingMode,
                hideOnDeath = false,
                ignoreOverlays = false,
                renderer = haulerMR
            };
            HG.ArrayUtils.ArrayInsert(ref defaultSkin.rendererInfos, 0, in haulerRendererInfo);
            #endregion

            #region jeep
            jeepSkinDef = Skins.CreateSkinDef("MOFFEINHAULERBODY_SKIN_GEEP_NAME",
                Assets.assetBundle.LoadAsset<Sprite>("Assets/Resources/texIconJeep.png"),
                defaultRenderers,
                mainRenderer,
                model);
            //body + wheels

            var haulerJeepGO = AutoAddGameObjectActivations(jeepSkinDef, "HaulerJeep", Assets.assetBundle.LoadAsset<GameObject>("mdlHaulerJeep.prefab"));
            var jeepMeshMR = haulerJeepGO.GetComponent<MeshRenderer>();
            var jeepRendererInfo = new CharacterModel.RendererInfo()
            {
                defaultMaterial = jeepMeshMR.sharedMaterial,
                defaultShadowCastingMode = jeepMeshMR.shadowCastingMode,
                hideOnDeath = false,
                ignoreOverlays = false,
                renderer = jeepMeshMR
            };
            HG.ArrayUtils.ArrayInsert(ref jeepSkinDef.rendererInfos, 0, in jeepRendererInfo);
            #endregion
        }
    }
}

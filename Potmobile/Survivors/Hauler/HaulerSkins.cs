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

        public static Transform mdlHaulerTransform
        {
            get
            {
                return PotmobileContent.HaulerBodyObject.transform.Find("Model Base/mdlHauler");
            }
        }

        public static GameObject ParentToHauler(GameObject gameObject, bool copy, Vector3 localScale, Vector3 localPosition)
        {
            if (copy) gameObject = UnityEngine.Object.Instantiate(gameObject);
            gameObject.transform.SetParent(mdlHaulerTransform);
            gameObject.transform.localScale = localScale;
            gameObject.transform.localPosition = localPosition;
            return gameObject;
        }

        public static void AutoAddGameObjectActivations(SkinDef skinDef, string identifier, GameObject skinGameObject)
        {
            skinGOSelection[identifier] = skinGameObject;
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
            var skinGO = PotmobileContent.HaulerBodyObject.transform.Find("Model Base/mdlHauler/HaulerMesh").gameObject;
            AutoAddGameObjectActivations(defaultSkin, "Hauler", skinGO);
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

            var haulerJeepGO = ParentToHauler(Assets.assetBundle.LoadAsset<GameObject>("mdlHaulerJeep.prefab"), true, Vector3.one, Vector3.zero);
            AutoAddGameObjectActivations(jeepSkinDef, "HaulerJeep", haulerJeepGO);
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

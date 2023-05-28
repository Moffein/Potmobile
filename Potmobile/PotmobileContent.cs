using RoR2.ContentManagement;
using System;
using System.Collections.Generic;
using UnityEngine;
using RoR2.Skills;
using System.Collections;
using RoR2;
using R2API;

namespace Potmobile
{
    public class PotmobileContent : IContentPackProvider
    {
        public static ContentPack content = new ContentPack();

        public static SurvivorDef PotmobileSurvivorDef;
        public static GameObject PotmobileBodyObject;
        public static GameObject PotmobileMasterObject;
        public static BodyIndex PotmobileBodyIndex = BodyIndex.None;
        public static DirectorAPI.DirectorCardHolder PotmobileCard;
        public static DirectorAPI.DirectorCardHolder PotmobileLoopCard;

        public static SurvivorDef HaulerSurvivorDef;
        public static GameObject HaulerBodyObject;
        public static GameObject HaulerMasterObject;
        public static BodyIndex HaulerBodyIndex = BodyIndex.None;
        public static DirectorAPI.DirectorCardHolder HaulerCard;
        public static DirectorAPI.DirectorCardHolder HaulerLoopCard;

        public static List<SkillFamily> skillFamilies = new List<SkillFamily>();
        public static List<SkillDef> skillDefs = new List<SkillDef>();
        public static List<Type> entityStates = new List<Type>();
        public static List<GameObject> projectilePrefabs = new List<GameObject>();

        public static class ModdedDamageTypes
        {
            public static DamageAPI.ModdedDamageType BonusForceToPotmobile;
            public static DamageAPI.ModdedDamageType SquashOnKill;
        }

        public static class SkillDefs
        {
            public static SkillDef FirePotCannon;
            public static SkillDef FirePotCannonScepter;
            public static SkillDef Push;
            public static SkillDef Boost;
            public static SkillDef Reset;
        }

        public string identifier => "MoffeinPotmobile.content";

        public IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
        {
            args.ReportProgress(1f);
            yield break;
        }

        public IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
        {
            ContentPack.Copy(content, args.output);
            yield break;
        }

        public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
        {
            if (PotmobileBodyObject) content.bodyPrefabs.Add(new GameObject[] { PotmobileBodyObject });
            if (PotmobileMasterObject) content.masterPrefabs.Add(new GameObject[] { PotmobileMasterObject });
            if (PotmobileSurvivorDef) content.survivorDefs.Add(new SurvivorDef[] { PotmobileSurvivorDef });

            if (HaulerBodyObject) content.bodyPrefabs.Add(new GameObject[] { HaulerBodyObject });
            if (HaulerMasterObject) content.masterPrefabs.Add(new GameObject[] { HaulerMasterObject });
            if (HaulerSurvivorDef) content.survivorDefs.Add(new SurvivorDef[] { HaulerSurvivorDef });

            content.skillDefs.Add(skillDefs.ToArray());
            content.entityStateTypes.Add(entityStates.ToArray());
            content.skillFamilies.Add(skillFamilies.ToArray());
            content.projectilePrefabs.Add(projectilePrefabs.ToArray());

            yield break;
        }
    }
}

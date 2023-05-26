﻿using UnityEngine;
using RoR2;
using R2API;
using UnityEngine.Networking;
using Potmobile.Components;

namespace Potmobile
{
    public static class DamageTypeSetup
    {
        private static bool initialized = false;
        public static void Init()
        {
            if (initialized) return;
            initialized = true;
            PotmobileContent.ModdedDamageTypes.BonusForceToPotmobile = DamageAPI.ReserveDamageType();

            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
        }

        private static void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (NetworkServer.active)
            {
                if (damageInfo.HasModdedDamageType(PotmobileContent.ModdedDamageTypes.BonusForceToPotmobile))
                {
                    if (self.body.bodyIndex == PotmobileContent.PotmobileBodyIndex)
                    {
                        damageInfo.force *= 4f;
                    }
                }
            }
            orig(self, damageInfo);
        }
    }
}

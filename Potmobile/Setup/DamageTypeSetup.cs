using UnityEngine;
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
            PotmobileContent.ModdedDamageTypes.SquashOnKill = DamageAPI.ReserveDamageType();
            PotmobileContent.ModdedDamageTypes.PotmobileRam = DamageAPI.ReserveDamageType();

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

                if (damageInfo.attacker)
                {
                    if (damageInfo.HasModdedDamageType(PotmobileContent.ModdedDamageTypes.SquashOnKill))
                    {
                        PotmobileNetworkComponent pnc = damageInfo.attacker.GetComponent<PotmobileNetworkComponent>();
                        if (pnc)
                        {
                            if (self.body.master)
                            {
                                NetworkIdentity ni = self.body.master.GetComponent<NetworkIdentity>();
                                if (ni)
                                {
                                    pnc.SquashEnemy(ni.netId.Value);
                                }
                            }
                        }
                    }
                }
            }
            orig(self, damageInfo);
        }
    }
}

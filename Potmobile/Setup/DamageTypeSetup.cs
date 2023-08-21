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
                    if (self.body.bodyIndex == PotmobileContent.PotmobileBodyIndex || self.body.bodyIndex == PotmobileContent.HaulerBodyIndex)
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

                    CharacterBody attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                    if (attackerBody)
                    {
                        if (damageInfo.HasModdedDamageType(PotmobileContent.ModdedDamageTypes.PotmobileRam))
                        {
                            if (PotmobilePlugin.ramDisableAgainstPlayerPotmobiles && self.body.isPlayerControlled && attackerBody.isPlayerControlled)
                            {
                                bool attackerIsVehicle = attackerBody.bodyIndex == PotmobileContent.PotmobileBodyIndex || attackerBody.bodyIndex == PotmobileContent.HaulerBodyIndex;
                                bool victimIsVehicle = self.body.bodyIndex == PotmobileContent.PotmobileBodyIndex || self.body.bodyIndex == PotmobileContent.HaulerBodyIndex;

                                if (attackerIsVehicle && victimIsVehicle)
                                {
                                    damageInfo.damage = 0f;
                                    damageInfo.damageType |= DamageType.Silent;
                                    damageInfo.rejected = true;
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

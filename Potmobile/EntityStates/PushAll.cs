﻿using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.MoffeinPotmobile.Weapon
{
    public class PushAll : BaseState
    {
        public static GameObject effectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Railgunner/RailgunnerMineExplosion.prefab").WaitForCompletion();
        public static float force = 4500f;
        public static float baseDuration = 2f;
        public static float jumpVelocity = 32f;
        public static float radius = 12f;
        private float duration;
        private bool buttonReleased;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / this.attackSpeedStat;
            buttonReleased = false;

            if (base.isAuthority)
            {

                EffectManager.SpawnEffect(effectPrefab, new EffectData { origin = base.transform.position, scale = radius }, true);
                BlastAttack ba = new BlastAttack
                {
                    damageType = DamageType.NonLethal | DamageType.Silent | DamageType.BypassArmor | DamageType.BypassBlock,
                    baseDamage = 0f,
                    radius = radius,
                    procCoefficient = 0f,
                    procChainMask = default,
                    attacker = base.gameObject,
                    inflictor = base.gameObject,
                    canRejectForce = false,
                    attackerFiltering = AttackerFiltering.NeverHitSelf,
                    baseForce = force,
                    bonusForce = Mathf.Min(1000f, force) * Vector3.up,
                    falloffModel = BlastAttack.FalloffModel.None,
                    crit = false,
                    losType = BlastAttack.LoSType.None,
                    position = base.transform.position
                };
                ba.AddModdedDamageType(Potmobile.PotmobileContent.ModdedDamageTypes.BonusForceToPotmobile);
                ba.Fire();

                if (base.rigidbody)
                {
                    Vector3 newVelocity = base.rigidbody.velocity;
                    if (newVelocity.y < 0f) newVelocity.y = 0f;
                    newVelocity.y += jumpVelocity;
                    base.rigidbody.velocity = newVelocity;
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority)
            {
                if (base.inputBank && !base.inputBank.skill2.down) buttonReleased = true;

                if (base.fixedAge >= duration)
                {
                    this.outer.SetNextStateToMain();
                    return;
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (buttonReleased)
            {
                return InterruptPriority.Any;
            }
            return InterruptPriority.PrioritySkill;
        }
    }
}

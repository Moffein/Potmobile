﻿using UnityEngine;
using RoR2;
using UnityEngine.AddressableAssets;
using BepInEx.Configuration;

namespace EntityStates.MoffeinPotmobile.Boost
{
    public class Reset : BaseState
    {
        public static GameObject effectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/HuntressBlinkEffect.prefab").WaitForCompletion();
        public static ConfigEntry<bool> resetVelocity;
        public static float baseDuration = 2f;
        private bool buttonReleased;

        public override void OnEnter()
        {
            base.OnEnter();

            Util.PlaySound("Play_huntress_shift_mini_blink", base.gameObject);
            buttonReleased = false;
            if (base.transform) EffectManager.SimpleEffect(effectPrefab, base.transform.position, base.transform.rotation, false);

            if (base.isAuthority)
            {
                if (base.transform)
                {
                    Ray aimRay = base.GetAimRay();
                    base.transform.forward = aimRay.direction;
                    base.transform.eulerAngles = new Vector3(0f , base.transform.eulerAngles.y, 0f);
                }

                if (resetVelocity.Value)
                {
                    if (base.rigidbody)
                    {
                        base.rigidbody.velocity = Vector3.zero;
                    }
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority)
            {
                if (base.inputBank && !base.inputBank.skill4.down) buttonReleased = true;

                if (base.fixedAge >= baseDuration)
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

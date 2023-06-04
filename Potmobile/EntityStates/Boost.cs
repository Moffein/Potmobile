using UnityEngine;
using RoR2;
using UnityEngine.Networking;
using Potmobile.Components;
using BepInEx.Configuration;

namespace EntityStates.MoffeinPotmobile.Boost
{
    public class Boost : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound("Play_MULT_shift_start", base.gameObject);
            trailStopwatch = 0f;

            speedController = base.GetComponent<SpeedController>();
            if (speedController)
            {
                speedController.speedMult *= forceMultiplier;   //Kind of an unsafe way to do this
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            trailStopwatch += Time.fixedDeltaTime;
            if (this.trailStopwatch >= trailTime)
            {
                EffectManager.SpawnEffect(trailPrefab, new EffectData
                {
                    origin = base.transform.position - base.transform.forward
                }, false);
                this.trailStopwatch -= trailTime;
            }
            if (base.isAuthority)
            {
                if (base.fixedAge >= baseDuration)
                {
                    this.outer.SetNextStateToMain();
                    return;
                }
            }
        }

        public override void OnExit()
        {
            if (speedController)
            {
                speedController.speedMult /= forceMultiplier;
            }
            Util.PlaySound("Play_MULT_shift_end", base.gameObject);
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        private SpeedController speedController;
        public static float baseDuration = 2.5f;
        public static float forceMultiplier = 2f;

        public static GameObject trailPrefab = Resources.Load<GameObject>("prefabs/effects/impacteffects/missileexplosionvfx");
        public static float trailFrequency = 8f;
        private float trailStopwatch;
        private float trailTime;
    }
}

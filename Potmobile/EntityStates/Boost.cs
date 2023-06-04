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

            maxSpeedDuringState = 10f;  //Set it to 10f minimum so that its possible to break out of straight vertical fall loops.
            trailTime = 1f / trailFrequency;

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
                if (base.rigidbody)
                {
                    Vector3 velocityFlat = new Vector3(base.rigidbody.velocity.x, 0f, base.rigidbody.velocity.z);
                    float currentSpeed = velocityFlat.magnitude;

                    maxSpeedDuringState = Mathf.Max(maxSpeedDuringState, currentSpeed);

                    Ray aimRay = base.GetAimRay();
                    Vector3 aimDirectionFlat = aimRay.direction;
                    aimDirectionFlat.y = 0f;
                    aimDirectionFlat.Normalize();

                    Vector3 desiredVelocity = maxSpeedDuringState * aimDirectionFlat;

                    Vector3 newVelocity = Vector3.RotateTowards(velocityFlat, desiredVelocity, Mathf.PI * Time.fixedDeltaTime, Mathf.Infinity);   //1pi because it should take 2s to do a full 360
                    newVelocity.y = base.rigidbody.velocity.y;

                    base.rigidbody.velocity = newVelocity;
                }
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
        public static float trailFrequency = 10f;
        private float trailStopwatch;
        private float trailTime;
        private float maxSpeedDuringState;
    }
}

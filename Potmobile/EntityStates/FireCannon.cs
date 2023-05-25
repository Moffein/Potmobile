using UnityEngine;
using RoR2;
using RoR2.Projectile;

namespace EntityStates.MoffeinPotmobile.Weapon
{
    public class FireCannon : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = baseDuration / this.attackSpeedStat;
            Ray aimRay = base.GetAimRay();
            //base.StartAimMode(aimRay, 2f, false);
            Util.PlaySound("Play_MULT_m1_grenade_launcher_shoot", base.gameObject);
            if (base.isAuthority)
            {
                ProjectileManager.instance.FireProjectile(projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * damageCoefficient, 0f, base.RollCrit(), DamageColorIndex.Default, null, -1f);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        public static GameObject projectilePrefab;
        public static float damageCoefficient = 10f;
        public static float force = 2500f;
        public static float baseDuration = 1.5f;
        private float duration;
    }
}

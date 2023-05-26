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
                int icbmCount = 0;
                if (base.characterBody && base.characterBody.inventory)
                {
                    icbmCount = base.characterBody.inventory.GetItemCount(DLC1Content.Items.MoreMissile);
                }

                if (enableICBMSynergy && icbmCount > 0)
                {
                    float damageMult = 1f;
                    int stack = icbmCount - 1;
                    if (stack > 0) damageMult += stack * 0.5f;

                    bool isCrit = base.RollCrit();
                    Vector3 rhs = Vector3.Cross(Vector3.up, aimRay.direction);
                    Vector3 axis = Vector3.Cross(aimRay.direction, rhs);

                    float currentSpread = 0f;
                    float angle = 0f;
                    float num2 = 0f;
                    num2 = UnityEngine.Random.Range(1f + currentSpread, 1f + currentSpread) * 3f;   //Bandit is x2
                    angle = num2 / 2f;  //3 - 1 rockets

                    Vector3 direction = Quaternion.AngleAxis(-num2 * 0.5f, axis) * aimRay.direction;
                    Quaternion rotation = Quaternion.AngleAxis(angle, axis);
                    Ray aimRay2 = new Ray(aimRay.origin, direction);
                    for (int i = 0; i < 3; i++)
                    {
                        ProjectileManager.instance.FireProjectile(projectilePrefab, aimRay2.origin, Util.QuaternionSafeLookRotation(aimRay2.direction), base.gameObject, damageMult * this.damageStat * damageCoefficient, (i != 1 ? 0f : force), isCrit, DamageColorIndex.Default, null, -1f);
                        aimRay2.direction = rotation * aimRay2.direction;
                    }
                }
                else
                {
                    ProjectileManager.instance.FireProjectile(projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * damageCoefficient, 0f, base.RollCrit(), DamageColorIndex.Default, null, -1f);
                }
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

        public static bool enableICBMSynergy = true;
        public static GameObject projectilePrefab;
        public static float damageCoefficient = 10f;
        public static float force = 2500f;
        public static float baseDuration = 1.5f;
        private float duration;
    }
}

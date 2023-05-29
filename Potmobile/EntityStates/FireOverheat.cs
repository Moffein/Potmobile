using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2;

namespace EntityStates.MoffeinPotmobile.Weapon
{
    public class FireOverheat : BaseState
    {
        public static int baseShotCount = 6;
        public static float fireFrequency = 6f;
        public static float damageCoefficient = 2f;
        public static float radius = 14f;
        public static GameObject effectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/IgniteOnKill/IgniteExplosionVFX.prefab").WaitForCompletion();

        protected int _shotCountInternal;
        protected float _radiusInternal;
        protected float _fireFrequencyInternal;
        private float fireDuration;
        private float fireStopwatch;
        private int shotsFired;

        public override void OnEnter()
        {
            base.OnEnter();
            LoadStats();
            fireDuration = (1f / _fireFrequencyInternal) / this.attackSpeedStat;
            fireStopwatch = 0f;
            shotsFired = 0;
            FireAttack();
        }

        public void FireAttack()
        {
            shotsFired++;
            if (effectPrefab) EffectManager.SpawnEffect(effectPrefab, new EffectData { origin = base.transform.position, scale = _radiusInternal }, false);
            if (base.isAuthority)
            {
                BlastAttack ba = new BlastAttack
                {
                    attacker = base.gameObject,
                    inflictor = base.gameObject,
                    crit = base.RollCrit(),
                    position = base.transform.position,
                    radius = _radiusInternal,
                    baseDamage = this.damageStat * damageCoefficient,
                    baseForce = 0f,
                    bonusForce = Vector3.zero,
                    attackerFiltering = AttackerFiltering.NeverHitSelf,
                    damageColorIndex = DamageColorIndex.Default,
                    damageType = DamageType.IgniteOnHit,
                    falloffModel = BlastAttack.FalloffModel.None,
                    procChainMask = default,
                    procCoefficient = 1f,
                    teamIndex = base.GetTeam()
                };
                ba.Fire();
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (shotsFired >= _shotCountInternal)
            {
                if (base.isAuthority)
                {
                    this.outer.SetNextState(new ReloadOverheat());
                    return;
                }
            }
            else
            {
                fireStopwatch += Time.fixedDeltaTime;
                if (fireStopwatch >= fireDuration)
                {
                    fireStopwatch -= fireDuration;
                    FireAttack();
                }
            }
        }

        public virtual void LoadStats()
        {
            _radiusInternal = radius;
            _fireFrequencyInternal = fireFrequency;
            _shotCountInternal = baseShotCount;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}

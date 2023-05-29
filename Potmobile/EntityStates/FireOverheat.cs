using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2;

namespace EntityStates.MoffeinPotmobile.Weapon
{
    public class FireOverheat : BaseState
    {
        public static float fireFrequency = 6f;
        public static float baseDPS = 6f;
        public static float radius = 12f;
        public static GameObject effectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/IgniteOnKill/IgniteExplosionVFX.prefab").WaitForCompletion();

        protected float _radiusInternal;
        protected float _DPSInternal;
        protected float _fireFrequencyInternal;
        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            LoadStats();
            duration = (1f / _fireFrequencyInternal) / this.attackSpeedStat;

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
                    baseDamage = this.damageStat * _DPSInternal / _fireFrequencyInternal,
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
            if (base.fixedAge >= duration)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public virtual void LoadStats()
        {
            _radiusInternal = radius;
            _DPSInternal = baseDPS;
            _fireFrequencyInternal = fireFrequency;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}

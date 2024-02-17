using UnityEngine;
using RoR2;
using RoR2.Projectile;
using Potmobile;

namespace EntityStates.MoffeinPotmobile.Weapon
{
    public class FirePotCannonScepter : FirePotCannon
    {
        public static GameObject scepterProjectilePrefab;
        public static GameObject scepterProjectilePrefabEnemy;
        public static float scepterDamageMultiplier = 2f;

        public override void ModifyStats()
        {
            _damageCoefficientInternal = FirePotCannon.damageCoefficient * scepterDamageMultiplier;
        }

        public override GameObject GetProjectilePrefab()
        {
            return EnemySetup.ShouldApplyEnemyDebuff(base.characterBody) ? FirePotCannonScepter.scepterProjectilePrefabEnemy : FirePotCannonScepter.scepterProjectilePrefab;
        }
    }
}

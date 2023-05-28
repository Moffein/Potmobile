using UnityEngine;
using RoR2;
using RoR2.Projectile;

namespace EntityStates.MoffeinPotmobile.Weapon
{
    public class FirePotCannonScepter : FirePotCannon
    {
        public static GameObject scepterProjectilePrefab;
        public static float scepterDamageMultiplier = 1.5f;

        public override void ModifyStats()
        {
            base.ModifyStats();
            _damageCoefficientInternal *= scepterDamageMultiplier;
            _projectilePrefabInternal = scepterProjectilePrefab;
        }
    }
}

using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2;

namespace EntityStates.MoffeinPotmobile.Weapon
{
    public class FireOverheatScepter : FireOverheat
    {
        public static float scepterMult = 2f;

        public override void LoadStats()
        {
            base.LoadStats();
            _radiusInternal *= scepterMult;
            _fireFrequencyInternal *= scepterMult;
        }
    }
}

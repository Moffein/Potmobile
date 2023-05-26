using UnityEngine;
using RoR2;
using UnityEngine.AddressableAssets;

namespace EntityStates.MoffeinPotmobile.Boost
{
    public class Reset : BaseState
    {
        public static GameObject effectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/HuntressBlinkEffect.prefab").WaitForCompletion();

        public override void OnEnter()
        {
            base.OnEnter();

            Util.PlaySound("Play_huntress_shift_mini_blink", base.gameObject);
            if (base.transform) EffectManager.SimpleEffect(effectPrefab, base.transform.position, base.transform.rotation, false);

            if (base.isAuthority)
            {
                //base.transform.Rotate(new Vector3(1f, 0f, 0f), 180f);
                //Debug.Log("New: " + base.transform.rotation.eulerAngles + "\n");

                if (base.transform)
                {
                    base.transform.eulerAngles = new Vector3(0f, base.transform.eulerAngles.y, 0f);
                }

                this.outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}

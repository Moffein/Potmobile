using System;
using System.Collections.Generic;
using System.Text;

namespace EntityStates.MoffeinPotmobile.Weapon
{
    public class ReloadOverheat : BaseState
    {
        public static float baseDuration = 1.5f;
        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / this.attackSpeedStat;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority && base.fixedAge >= duration)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}

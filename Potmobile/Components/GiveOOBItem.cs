using RoR2;
using UnityEngine;

namespace Potmobile.Components
{
    public class GiveOOBItem : MonoBehaviour
    {
        public void Start()
        {
            CharacterBody cb = base.GetComponent<CharacterBody>();
            if (cb && cb.inventory)
            {
                if (cb.inventory.GetItemCount(RoR2Content.Items.TeleportWhenOob) <= 0)
                {
                    cb.inventory.GiveItem(RoR2Content.Items.TeleportWhenOob);
                }
            }
        }
    }
}

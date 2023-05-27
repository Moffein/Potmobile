using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace Potmobile.Components
{
    public class GiveItemsOnSpawn : MonoBehaviour
    {
        public static bool giveVase = true;
        public void Start()
        {
            if (NetworkServer.active)
            {
                CharacterBody cb = base.GetComponent<CharacterBody>();
                if (cb && cb.inventory)
                {
                    if (cb.inventory.GetItemCount(RoR2Content.Items.TeleportWhenOob) <= 0)
                    {
                        cb.inventory.GiveItem(RoR2Content.Items.TeleportWhenOob);
                    }

                    if (giveVase && cb.inventory.currentEquipmentIndex == EquipmentIndex.None)
                    {
                        cb.inventory.SetEquipmentIndex(RoR2Content.Equipment.Gateway.equipmentIndex);
                    }
                }
            }
        }
    }
}

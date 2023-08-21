using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace Potmobile.Components
{
    public class PotmobileNetworkComponent : NetworkBehaviour
    {
        private bool wasPressed;
        private float honkCooldownStopwatch;
        private CharacterBody characterBody;
        public void Awake()
        {
            wasPressed = false;
            honkCooldownStopwatch = 0f;
            characterBody = base.GetComponent<CharacterBody>();
        }

        public void Update()
        {
            if (this.hasAuthority)
            {
                if (honkCooldownStopwatch <= 0f)
                {
                    if (PotmobilePlugin.GetKeyPressed(PotmobilePlugin.honkButton))
                    {
                        if (!wasPressed)
                        {
                            //EffectManager.SimpleSoundEffect(SquawkController.squawk.index, base.transform.position, true);
                            Util.PlaySound("Play_MoffeinPotmobile_Horn", base.gameObject);
                            CmdHonk();
                            honkCooldownStopwatch = 0.15f;
                        }
                        wasPressed = true;
                    }
                    else
                    {
                        wasPressed = false;
                    }
                }
                else
                {
                    honkCooldownStopwatch -= Time.deltaTime;
                }
            }
        }

        [Server]
        public void SquashEnemy(uint networkID)
        {
            if (NetworkServer.active)
            {
                RpcAddSquash(networkID);
            }
        }

        [ClientRpc]
        private void RpcAddSquash(uint networkID)
        {
            GameObject go = ClientScene.FindLocalObject(new NetworkInstanceId(networkID));
            if (go)
            {
                CharacterMaster cm = go.GetComponent<CharacterMaster>();
                if (cm)
                {
                    GameObject bodyObject = cm.GetBodyObject();
                    if (bodyObject)
                    {
                        SquashedComponent sq = bodyObject.GetComponent<SquashedComponent>();
                        if (sq)
                        {
                            sq.ResetGraceTimer();
                        }
                        else
                        {
                            sq = bodyObject.AddComponent<SquashedComponent>();
                        }
                        sq.triggerer = base.gameObject;
                    }
                }
            }
        }

        [Command]
        public void CmdHonk()
        {
            RpcHonk();
        }

        [ClientRpc]
        private void RpcHonk()
        {
            if (!this.hasAuthority)
            {
                Util.PlaySound("Play_MoffeinPotmobile_Horn", base.gameObject);
            }
        }
    }
}

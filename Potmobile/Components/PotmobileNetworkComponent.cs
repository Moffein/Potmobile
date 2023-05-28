using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace Potmobile.Components
{
    public class PotmobileNetworkComponent : NetworkBehaviour
    {
        private CharacterBody characterBody;
        public void Awake()
        {
            characterBody = base.GetComponent<CharacterBody>();
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
    }
}

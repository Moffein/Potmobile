using UnityEngine;
using RoR2;
using UnityEngine.AddressableAssets;

namespace EntityStates.MoffeinPotmobile
{
    public class PotmobileDeath : GenericCharacterDeath
    {
        public static GameObject effectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniExplosionVFXQuick.prefab").WaitForCompletion();
        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound("Play_golem_death", base.gameObject);
            EffectManager.SpawnEffect(effectPrefab, new EffectData { scale = 8f, origin = base.transform.position }, false);

            if (base.modelLocator && base.modelLocator.modelTransform && base.modelLocator.modelTransform.gameObject)
            {
                DestroyOnTimer d = base.modelLocator.modelTransform.gameObject.AddComponent<DestroyOnTimer>();
                d.duration = 0.1f;
                d.enabled = true;
            }
            DestroyOnTimer d2 = base.gameObject.AddComponent<DestroyOnTimer>();
            d2.duration = 0.1f;
            d2.enabled = true;
        }
    }
}

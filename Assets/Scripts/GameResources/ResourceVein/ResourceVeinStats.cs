using Core;
using GameResources.Core;
using Mirror;
using UnityEngine;

namespace GameResources.ResourceVein
{
    public class ResourceVeinStats : EntityStats
    {
        private ResourceSpawner _resourceSpawner;
        
        public override int TeamId => 100;

        public override void Initialize(params object[] objects) => 
            _resourceSpawner = objects[0] as ResourceSpawner;

        public override void TakeDamage(int teamId, float amount)
        {
            if (teamId == TeamId)
            {
                return;
            }

            if (_health - amount > 0)
            {
                TakeDamageCmd(amount);
                return;
            }
            
            DestroyEntity();
        }

        [Command(requiresAuthority = false)]
        private void TakeDamageCmd(float amount) => 
            TakeDamageRpc(amount);

        [ClientRpc]
        private void TakeDamageRpc(float amount) => 
            _health -= Mathf.Clamp(_health - amount, 0, float.MaxValue);

        public override void DestroyEntity() => 
            DestroyEntityCmd();

        [Command(requiresAuthority = false)]
        private void DestroyEntityCmd() => 
            DestroyEntityRpc();

        [ServerCallback]
        private void DestroyEntityRpc()
        {
            if (_resourceSpawner != null)
            {
                _resourceSpawner.InstantiateResource();
            }

            NetworkServer.Destroy(gameObject);
        }
    }
}
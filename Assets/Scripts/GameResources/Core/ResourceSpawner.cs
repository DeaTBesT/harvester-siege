using Interfaces;
using Mirror;
using UnityEngine;

namespace GameResources.Core
{
    public class ResourceSpawner : NetworkBehaviour, IInstantiateResource
    {
        [SerializeField] private ResourceData _resourceData;
        
        public void InstantiateResource(string resourceName = default, int amount = default)
        {
            if (isServer)
            {
                InstantiateResourceServer(resourceName, amount);
            }
            else
            {
                InstantiateResourceCmd(resourceName, amount);
            }
        }

        [Command(requiresAuthority = false)]
        public void InstantiateResourceCmd(string resourceName, int amount) => 
            InstantiateResourceServer(resourceName, amount);

        [Server]
        public void InstantiateResourceServer(string resourceName, int amount) => 
            InstantiateResourceInternal(resourceName, amount);

        protected virtual void InstantiateResourceInternal(string resourceName, int amount) => 
            ResourceData.InstantiateResource(_resourceData, transform.position);
    }
}
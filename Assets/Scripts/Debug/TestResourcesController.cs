using GameResources.Core;
using Managers;
using Mirror;
using UnityEngine;

namespace DebugServices
{
    public class TestResourcesController : NetworkBehaviour
    {
        [SerializeField] private ResourceConfig[] _testResourceConfigs;

        private PrefabPool _prefabPool;

        private void Update()
        {
            if (!isServer)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SpawnResource(0);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SpawnResource(1);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SpawnResource(2);
            }
        }

        [Server]
        private void SpawnResource(int resourceId)
        {
            var resourceData = new ResourceData(_testResourceConfigs[resourceId], 1);
            ResourceData.InstantiateResource(resourceData, transform.position);
        }
    }
}
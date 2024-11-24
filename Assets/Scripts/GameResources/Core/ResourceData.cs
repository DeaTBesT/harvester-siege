using Mirror;
using UnityEngine;
using Utils.Networking;

namespace GameResources.Core
{
    [System.Serializable]
    public class ResourceData
    {
        [SerializeField] private ResourceConfig _resourceConfig;
        [SerializeField] private int _amount;

        public ResourceConfig ResourceConfig => _resourceConfig;
        public int AmountResource => _amount;

        public ResourceData(ResourceConfig resourceConfig, int amount)
        {
            _resourceConfig = resourceConfig;
            _amount = amount;
        }

        public void SetAmount(int amount) =>
            SetAmountCmd(amount);

        [Command(requiresAuthority = false)]
        public void SetAmountCmd(int amount) =>
            SetAmountRpc(amount);

        [ClientRpc]
        public void SetAmountRpc(int amount) =>
            _amount = amount;

        public void AddResource(int amount) =>
            AddResourceCmd(amount);

        [Command]
        private void AddResourceCmd(int amount) =>
            AddResourceRpc(amount);

        [ClientRpc]
        private void AddResourceRpc(int amount) =>
            _amount += amount;

        public void RemoveResource(int amount) =>
            RemoveResourceCmd(amount);

        [Command]
        private void RemoveResourceCmd(int amount) =>
            RemoveResourceRpc(amount);

        [ClientRpc]
        private void RemoveResourceRpc(int amount) => 
            _amount = Mathf.Clamp(_amount - amount, 0, int.MaxValue);

        public static void InstantiateResource(ResourceData resourceData, Vector3 spawnPosition = default,
            Quaternion spawnRotation = default)
        {
            if (resourceData == null)
            {
#if UNITY_EDITOR
                Debug.LogError("Resource data isn't exist");
#endif
                return;
            }
            
            var newResource = Object.Instantiate(resourceData.ResourceConfig.ResourcePrefab, spawnPosition,
                spawnRotation);
            NetworkServer.Spawn(newResource);

            if (newResource.TryGetComponent(out GameResource gameResource))
            {
                gameResource.SetAmount(resourceData.AmountResource);
            }
        }

        public static void InstantiateResource(string resourceName, int amount, Vector3 spawnPosition = default,
            Quaternion spawnRotation = default)
        {
            if (string.IsNullOrEmpty(resourceName))
            {
#if UNITY_EDITOR
                Debug.LogError("Name is null");
#endif
                return;
            }

            var resourceConfig =
                (ResourceConfig)NetworkScriptableObjectSerializer.DeserializeScriptableObject(resourceName);

            if (resourceConfig == null)
            {
#if UNITY_EDITOR
                Debug.LogError("Config isn't exist");
#endif
                return;
            }

            var newResource = Object.Instantiate(resourceConfig.ResourcePrefab, spawnPosition,
                spawnRotation);
            NetworkServer.Spawn(newResource);

            if (newResource.TryGetComponent(out GameResource gameResource))
            {
                gameResource.SetAmount(amount);
            }
        }
    }
}
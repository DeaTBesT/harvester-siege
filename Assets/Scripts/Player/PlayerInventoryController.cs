using System.Collections.Generic;
using System.Linq;
using Core;
using GameResources.Core;
using Managers;
using Mirror;
using UnityEngine;
using Utils.Networking;

namespace Player
{
    public class PlayerInventoryController : EntityInventoryController
    {
        private GameResourcesManager _gameResourcesManager;

        private List<ResourceData> _resourcesData = new();

        public override void Initialize(params object[] objects) =>
            _gameResourcesManager = objects[0] as GameResourcesManager;

        public override void Deinitialize() =>
            DropAllResources();

        public override void AddResource(ResourceData addResourceData) =>
            AddResourceCmd(NetworkScriptableObjectSerializer.SerializeScriptableObject(addResourceData.ResourceConfig),
                addResourceData.AmountResource);

        [Command]
        private void AddResourceCmd(string gameResourceName, int amount) =>
            AddResourceRpc(gameResourceName, amount);

        [ClientRpc]
        private void AddResourceRpc(string gameResourceName, int amount)
        {
            var gameResource =
                (ResourceConfig)NetworkScriptableObjectSerializer.DeserializeScriptableObject(gameResourceName);
            var resourceData = _resourcesData.FirstOrDefault(x =>
                x.ResourceConfig.TypeResource == gameResource.TypeResource);

            if (resourceData != null)
            {
                resourceData.AddResource(amount);
            }
            else
            {
                _resourcesData.Add(new ResourceData(gameResource, amount));
            }

            _gameResourcesManager.AddResource(gameResource, amount);
        }

        public override void DropResource(ResourceData resourceData)
        {
            var resourceName = NetworkScriptableObjectSerializer.SerializeScriptableObject(resourceData.ResourceConfig);
            var resourceAmount = resourceData.AmountResource;

            InstanceResource(resourceName, resourceAmount);

            _gameResourcesManager.RemoveResource(resourceData.ResourceConfig, resourceAmount);
            RemoveResourceDataCmd(resourceName);
        }

        private void InstanceResource(string resourceName, int amount)
        {
            if (isServer)
            {
                InstanceResourceServer(resourceName, amount);
            }
            else
            {
                InstanceResourceCmd(resourceName, amount);
            }
        }
        
        [Command]
        private void InstanceResourceCmd(string resourceName, int amount) =>
            InstanceResourceServer(resourceName, amount);

        [Server]
        private void InstanceResourceServer(string resourceName, int amount) =>
            ResourceData.InstantiateResource(resourceName, amount, transform.position);

        [Command]
        private void RemoveResourceDataCmd(string resourceName) =>
            RemoveResourceDataRpc(resourceName);

        [ClientRpc]
        private void RemoveResourceDataRpc(string resourceName)
        {
            var resourceConfig =
                (ResourceConfig)NetworkScriptableObjectSerializer.DeserializeScriptableObject(resourceName);
            var resourceData = _resourcesData.FirstOrDefault(x =>
                x.ResourceConfig.TypeResource == resourceConfig.TypeResource);

            if (resourceData == null)
            {
#if UNITY_EDITOR
                Debug.LogError("Resource data isn't exist");
#endif
                return;
            }

            _resourcesData.Remove(resourceData);
        }

        public override void DropAllResources()
        {
            for (int i = _resourcesData.Count - 1; i >= 0; i--)
            {
                DropResource(_resourcesData[i]);
            }
        }

        private void Update()
        {
            if (!isLocalPlayer)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                DropAllResources();
            }
        }
    }
}
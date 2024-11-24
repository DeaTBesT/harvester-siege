using System.Collections.Generic;
using System.Linq;
using Core;
using GameResources.Core;
using Interfaces;
using Managers;
using Mirror;
using UnityEngine;
using Utils.Networking;

namespace Player
{
    public class PlayerInventoryController : EntityInventoryController, IInstantiateResource
    {
        private GameResourcesManager _gameResourcesManager;

        private List<ResourceData> _resourcesData = new();

        public List<ResourceData> ResourcesData => _resourcesData;
        
        public override void Initialize(params object[] objects) =>
            _gameResourcesManager = objects[0] as GameResourcesManager;

        public override void Deinitialize() =>
            DropAllResources();

        public override void AddResource(ResourceData resourceData)
        {
            var resourceName = NetworkScriptableObjectSerializer.SerializeScriptableObject(resourceData.ResourceConfig);
            var resourceAmount = resourceData.AmountResource;
            
            _gameResourcesManager.AddResource(resourceData.ResourceConfig, resourceAmount);
            AddResourceCmd(resourceName, resourceAmount);
        }

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
        }

        public override void RemoveResource(ResourceData resourceData)
        {
            var resourceName = NetworkScriptableObjectSerializer.SerializeScriptableObject(resourceData.ResourceConfig);
            var resourceAmount = resourceData.AmountResource;

            _gameResourcesManager.RemoveResource(resourceData.ResourceConfig, resourceAmount);
            RemoveResourceCmd(resourceName, resourceAmount);
        }

        [Command]
        private void RemoveResourceCmd(string resourceName, int amount) => 
            RemoveResourceRpc(resourceName, amount);

        [ClientRpc]
        private void RemoveResourceRpc(string resourceName, int amount)
        {
            var resourceConfig =
                (ResourceConfig)NetworkScriptableObjectSerializer.DeserializeScriptableObject(resourceName);
            var resourceData = _resourcesData.FirstOrDefault(x =>
                x.ResourceConfig.TypeResource == resourceConfig.TypeResource);
            
            if (resourceData.AmountResource - amount > 0)
            {
                resourceData.RemoveResource(amount);
            }
            else
            {
                _resourcesData.Remove(resourceData);
            }
        }
        
        public override void DropResource(ResourceData resourceData)
        {
            var resourceName = NetworkScriptableObjectSerializer.SerializeScriptableObject(resourceData.ResourceConfig);
            var resourceAmount = resourceData.AmountResource;

            InstantiateResource(resourceName, resourceAmount);

            _gameResourcesManager.RemoveResource(resourceData.ResourceConfig, resourceAmount);
            DropResourceDataCmd(resourceName);
        }

        public void InstantiateResource(string resourceName, int amount)
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
        
        [Command]
        public void InstantiateResourceCmd(string resourceName, int amount) =>
            InstantiateResourceServer(resourceName, amount);

        [Server]
        public void InstantiateResourceServer(string resourceName, int amount) =>
            ResourceData.InstantiateResource(resourceName, amount, transform.position);

        [Command]
        private void DropResourceDataCmd(string resourceName) =>
            DropResourceDataRpc(resourceName);

        [ClientRpc]
        private void DropResourceDataRpc(string resourceName)
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
            for (var i = _resourcesData.Count - 1; i >= 0; i--)
            {
                DropResource(_resourcesData[i]);
            }
        }

        //TODO:поменять
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
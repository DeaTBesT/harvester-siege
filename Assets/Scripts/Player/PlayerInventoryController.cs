using System.Collections.Generic;
using Core;
using GameResources;
using Managers;

namespace Player
{
    public class PlayerInventoryController : EntityInventoryController
    {
        private GameResourcesManager _gameResourcesManager;

        private List<ResourceData> _resourcesData = new();
        
        public override void Initialize(params object[] objects) => 
            _gameResourcesManager = objects[0] as GameResourcesManager;

        public override void AddResource(ResourceData resourceData)
        {
            _gameResourcesManager.AddResource(resourceData.gameResourceDataConfig, resourceData.AmountResource);
            _resourcesData.Add(resourceData);
        }

        public override void DropResource(ResourceData resourceData) => 
            ResourceData.InstantiateResource(resourceData, transform.position);

        public override void DropAllResources()
        {
            foreach (var resourceData in _resourcesData)
            {
                DropResource(resourceData);
            }
        }
    }
}
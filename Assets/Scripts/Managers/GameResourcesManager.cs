using System;
using System.Collections.Generic;
using System.Linq;
using GameResources;
using Mirror;
using Utils;

namespace Managers
{
    public class GameResourcesManager : Singleton<GameResourcesManager>
    {
        private List<ResourceData> _resourcesData = new();

        public Action<ResourceData> OnAddResource { get; set; }
        public Action<ResourceData> OnRemoveResource { get; set; }
        public Action<List<ResourceData>> OnChangeResourcesData { get; set; }

        public void AddResource(GameResourceData gameResourceData, int amount = 1) => 
            AddResourceCmd(NetworkSerializer.SerializeScriptableObject(gameResourceData), amount);

        [Command(requiresAuthority = false)]
        private void AddResourceCmd(string gameResourceName, int amount) => 
            AddResourceRpc(gameResourceName, amount);

        [ClientRpc]
        private void AddResourceRpc(string gameResourceName, int amount)
        {
            var gameResource = (GameResourceData)NetworkSerializer.DeserializeScriptableObject(gameResourceName);
            var resourceData = _resourcesData.FirstOrDefault(x => x.gameResourceDataConfig.TypeGameResource == gameResource.TypeGameResource);

            if (resourceData != null)
            {
                resourceData.AddResource(amount);
                OnAddResource?.Invoke(resourceData);
            }
            else
            {
                _resourcesData.Add(new ResourceData(gameResource, amount));
                OnChangeResourcesData?.Invoke(_resourcesData);
            }
        }
        
        public void RemoveResource(GameResourceType gameResource, int amount = 1)
        {
            
        }
    }
}
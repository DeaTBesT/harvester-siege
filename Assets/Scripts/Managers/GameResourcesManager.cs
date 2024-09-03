using System;
using System.Collections.Generic;
using System.Linq;
using GameResources;
using Mirror;
using UnityEngine;
using Utils;

namespace Managers
{
    public class GameResourcesManager : Singleton<GameResourcesManager>
    {
        private List<ResourceData> _resourcesData = new();

        public Action<ResourceData> OnAddResource { get; set; }
        public Action<ResourceData> OnRemoveResource { get; set; }
        public Action<List<ResourceData>> OnChangeResourcesData { get; set; }

        private void Start()
        {
            if (isServer)
            {
                return;
            }
            
            LoadResources(NetworkClient.localPlayer.connectionToClient);
        }

        [Command(requiresAuthority = false)]
        private void LoadResources(NetworkConnectionToClient conn) => 
            LoadResourceServer(conn);

        [Server]
        private void LoadResourceServer(NetworkConnectionToClient conn)
        {
            foreach (var resourceData in _resourcesData)
            {
                LoadResourceRpc(conn,
                    NetworkSerializer.SerializeScriptableObject(resourceData.gameResourceDataConfig),
                    resourceData.AmountResource);
            }
        }

        [TargetRpc]
        private void LoadResourceRpc(NetworkConnectionToClient target, string gameResourceName, int amount)
        {
            var gameResource = (GameResourceData)NetworkSerializer.DeserializeScriptableObject(gameResourceName);
            _resourcesData.Add(new ResourceData(gameResource, amount));
            OnChangeResourcesData?.Invoke(_resourcesData);
            Debug.Log("Load resource");
        }

        public void AddResource(GameResourceData gameResourceData, int amount = 1) =>
            AddResourceCmd(NetworkSerializer.SerializeScriptableObject(gameResourceData), amount);

        [Command(requiresAuthority = false)]
        private void AddResourceCmd(string gameResourceName, int amount) =>
            AddResourceRpc(gameResourceName, amount);

        [ClientRpc]
        private void AddResourceRpc(string gameResourceName, int amount)
        {
            var gameResource = (GameResourceData)NetworkSerializer.DeserializeScriptableObject(gameResourceName);
            var resourceData = _resourcesData.FirstOrDefault(x =>
                x.gameResourceDataConfig.TypeGameResource == gameResource.TypeGameResource);

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

        public void RemoveResource(GameResourceData gameResourceData, int amount = 1)
        {
            RemoveResourceServer(NetworkSerializer.SerializeScriptableObject(gameResourceData), amount);
        }

        [Server]
        private void RemoveResourceServer(string gameResourceName, int amount) =>
            RemoveResourceRpc(gameResourceName, amount);

        [ClientRpc]
        private void RemoveResourceRpc(string gameResourceName, int amount)
        {
        }
    }
}